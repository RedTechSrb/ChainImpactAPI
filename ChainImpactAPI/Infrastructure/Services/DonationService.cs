﻿using ChainImpactAPI.Application.RepositoryInterfaces;
using ChainImpactAPI.Application.ServiceInterfaces;
using ChainImpactAPI.Dtos;
using ChainImpactAPI.Dtos.BiggestDonations;
using ChainImpactAPI.Dtos.ImpactorsWithDonations;
using ChainImpactAPI.Dtos.ImpactorsWithProjects;
using ChainImpactAPI.Dtos.NFT;
using ChainImpactAPI.Dtos.RecentDonations;
using ChainImpactAPI.Dtos.SaveDonation;
using ChainImpactAPI.Dtos.SearchDtos;
using ChainImpactAPI.Infrastructure.Repositories;
using ChainImpactAPI.Models;

namespace ChainImpactAPI.Infrastructure.Services
{
    public class DonationService : IDonationService
    {
        private readonly IConfiguration configuration;
        private readonly IDonationRepository donationRepository;
        private readonly IImpactorRepository impactorRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IProjectRepository projectRepository;
        private readonly ICharityRepository charityRepository;

        public DonationService(
            IConfiguration configuration,
            IDonationRepository donationRepository,
            IImpactorRepository impactorRepository,
            ITransactionRepository transactionRepository,
            IProjectRepository projectRepository,
            ICharityRepository charityRepository)
        {
            this.configuration = configuration;
            this.donationRepository = donationRepository;
            this.impactorRepository = impactorRepository;
            this.transactionRepository = transactionRepository;
            this.projectRepository = projectRepository;
            this.charityRepository = charityRepository;
        }

        public List<ImpactorsWithDonationsResponseDto> GetImpactorsWithDonations(GenericDto<ImpactorsWithDonationsRequestDto>? impactorsWithDonationsDto)
        {
            int? skip = null; 
            int? take = null;
            DonationSearchDto donationSearchDto = new DonationSearchDto();

            if (impactorsWithDonationsDto != null)
            {
                if (impactorsWithDonationsDto.PageSize != null && impactorsWithDonationsDto.PageNumber != null)
                {
                    skip = impactorsWithDonationsDto.PageSize.Value * (impactorsWithDonationsDto.PageNumber.Value - 1);
                    take = impactorsWithDonationsDto.PageSize.Value;
                }

                donationSearchDto.projectType = impactorsWithDonationsDto.Dto?.projectType;
            }

            List<ImpactorsWithDonationsResponseDto> impactorsWithDonationsDtoList = donationRepository.SearchDonationsGroupedByImpactorsAsync(donationSearchDto).Result;

            if(skip != null && take != null)
            {
                impactorsWithDonationsDtoList = impactorsWithDonationsDtoList.Skip(skip.Value).Take(take.Value).ToList();
            }

            return impactorsWithDonationsDtoList;

        }


        public List<BiggestDonationsResponseDto> GetBiggestDonations(GenericDto<BiggestDonationsRequestDto>? recentDonationsDto)
        {
            int? skip = null;
            int? take = null;
            List<Donation> donations = null;

            if (recentDonationsDto != null)
            {
                if (recentDonationsDto.PageSize != null && recentDonationsDto.PageNumber != null)
                {
                    skip = recentDonationsDto.PageSize.Value * (recentDonationsDto.PageNumber.Value - 1);
                    take = recentDonationsDto.PageSize.Value;
                }

                if (recentDonationsDto.Dto != null)
                {
                    donations = donationRepository.SearchAsync(new GenericDto<DonationDto>(null, null, new DonationDto { project = new ProjectDto { id = recentDonationsDto.Dto.projectid } })).Result;
                }
            }



            var mostContributedImapctors = donations.GroupBy(d => new
            {
                d.donator.id,
            }).Select(gpb => new BiggestDonationsResponseDto
            (
                new ImpactorDto(gpb.Key.id),
                gpb.Sum(d => d.amount)
            )).OrderByDescending(bd => bd.amount).ToList();


            if (skip != null && take != null)
            {
                mostContributedImapctors = mostContributedImapctors.Skip(skip.Value).Take(take.Value).ToList();
            }

            mostContributedImapctors.ForEach(d =>
            {
                var impactor = impactorRepository.SearchAsync(new GenericDto<ImpactorDto>(null, null, d.impactor)).Result.FirstOrDefault();
                d.impactor = new ImpactorDto(impactor.id, impactor.wallet, impactor.name, impactor.description, impactor.website, impactor.facebook, impactor.discord, impactor.twitter, impactor.instagram, impactor.imageurl, impactor.role, impactor.type);
            });

            return mostContributedImapctors;

        }


        public List<RecentDonationsResponseDto> GetRecentDonations(GenericDto<RecentDonationsRequestDto>? recentDonationsDto)
        {
            int? skip = null;
            int? take = null;
            List<Donation> donations = null;

            if (recentDonationsDto != null)
            {
                if (recentDonationsDto.PageSize != null && recentDonationsDto.PageNumber != null)
                {
                    skip = recentDonationsDto.PageSize.Value * (recentDonationsDto.PageNumber.Value - 1);
                    take = recentDonationsDto.PageSize.Value;
                }

                if (recentDonationsDto.Dto != null)
                {
                    donations = donationRepository.SearchAsync(new GenericDto<DonationDto>(null, null, new DonationDto { project = new ProjectDto { id = recentDonationsDto.Dto.projectid } })).Result;
                }
            }

            // TODO: Recent donations are returned, if someone donated twice in the recent period, he will be in the retunred list twice. 
            // TODO: Probably, this should be fixed to group recent users

            donations = donations.OrderByDescending(d => d.creationdate).ToList();

            if (skip != null && take != null)
            {
                donations = donations.Skip(skip.Value).Take(take.Value).ToList();
            }


            var recentImpactors= new List<RecentDonationsResponseDto>();
            foreach (var donation in donations)
            {
                var tranactions = transactionRepository.SearchAsync(new GenericDto<TransactionDto>(null, null, new TransactionDto { donation = new DonationDto { id = donation.id } })).Result;
                List<TransactionDto> transactionsDto = new List<TransactionDto>();
                foreach (var tranaction in tranactions)
                {
                    transactionsDto.Add(new TransactionDto(
                                            tranaction.id,
                                            tranaction.blockchainaddress,
                                            tranaction.sender,
                                            tranaction.receiver,
                                            tranaction.amount,
                                            tranaction.type,
                                            tranaction.creationdate,
                                            tranaction.donation == null ? null : new DonationDto(
                                                tranaction.donation.id,
                                                tranaction.donation.amount,
                                                tranaction.donation.creationdate,
                                                new ProjectDto(
                                                    tranaction.donation.project.id,
                                                    new CharityDto(
                                                        tranaction.donation.project.charity.id,
                                                        tranaction.donation.project.charity.name,
                                                        tranaction.donation.project.charity.wallet,
                                                        tranaction.donation.project.charity.website,
                                                        tranaction.donation.project.charity.facebook,
                                                        tranaction.donation.project.charity.discord,
                                                        tranaction.donation.project.charity.twitter,
                                                        tranaction.donation.project.charity.imageurl,
                                                        tranaction.donation.project.charity.description
                                                    ),
                                                    tranaction.donation.project.wallet,
                                                    tranaction.donation.project.name,
                                                    tranaction.donation.project.description,
                                                    tranaction.donation.project.financialgoal,
                                                    tranaction.donation.project.totaldonated,
                                                    tranaction.donation.project.totalbackers,
                                                    tranaction.donation.project.website,
                                                    tranaction.donation.project.facebook,
                                                    tranaction.donation.project.discord,
                                                    tranaction.donation.project.twitter,
                                                    tranaction.donation.project.instagram,
                                                    tranaction.donation.project.imageurl,
                                                    tranaction.donation.project.impactor == null ? null : new ImpactorDto(
                                                        tranaction.donation.project.impactor.id,
                                                        tranaction.donation.project.impactor.wallet,
                                                        tranaction.donation.project.impactor.name,
                                                        tranaction.donation.project.impactor.description,
                                                        tranaction.donation.project.impactor.website,
                                                        tranaction.donation.project.impactor.facebook,
                                                        tranaction.donation.project.impactor.discord,
                                                        tranaction.donation.project.impactor.twitter,
                                                        tranaction.donation.project.impactor.instagram,
                                                        tranaction.donation.project.impactor.imageurl,
                                                        tranaction.donation.project.impactor.role,
                                                        tranaction.donation.project.impactor.type
                                                    ),
                                                    new CauseTypeDto(
                                                        tranaction.donation.project.primarycausetype.id,
                                                        tranaction.donation.project.primarycausetype.name
                                                    ),
                                                    new CauseTypeDto(
                                                        tranaction.donation.project.secondarycausetype.id,
                                                        tranaction.donation.project.secondarycausetype.name
                                                    )
                                                ),
                                                new ImpactorDto(
                                                    tranaction.donation.donator.id,
                                                    tranaction.donation.donator.wallet,
                                                    tranaction.donation.donator.name,
                                                    tranaction.donation.donator.description,
                                                    tranaction.donation.donator.website,
                                                    tranaction.donation.donator.facebook,
                                                    tranaction.donation.donator.discord,
                                                    tranaction.donation.donator.twitter,
                                                    tranaction.donation.donator.instagram,
                                                    tranaction.donation.donator.imageurl,
                                                    tranaction.donation.donator.role,
                                                    tranaction.donation.donator.type
                                                )
                                            ),
                                            tranaction.milestone == null ? null : new MilestoneDto(
                                                tranaction.milestone.id,
                                                tranaction.milestone.name,
                                                tranaction.milestone.ordernumber,
                                                tranaction.milestone.description,
                                                tranaction.milestone.complete,
                                                new ProjectDto(
                                                    tranaction.milestone.project.id,
                                                    new CharityDto(
                                                        tranaction.milestone.project.charity.id,
                                                        tranaction.milestone.project.charity.name,
                                                        tranaction.milestone.project.charity.wallet,
                                                        tranaction.milestone.project.charity.website,
                                                        tranaction.milestone.project.charity.facebook,
                                                        tranaction.milestone.project.charity.discord,
                                                        tranaction.milestone.project.charity.twitter,
                                                        tranaction.milestone.project.charity.imageurl,
                                                        tranaction.milestone.project.charity.description
                                                    ),
                                                    tranaction.milestone.project.wallet,
                                                    tranaction.milestone.project.name,
                                                    tranaction.milestone.project.description,
                                                    tranaction.milestone.project.financialgoal,
                                                    tranaction.milestone.project.totaldonated,
                                                    tranaction.milestone.project.totalbackers,
                                                    tranaction.milestone.project.website,
                                                    tranaction.milestone.project.facebook,
                                                    tranaction.milestone.project.discord,
                                                    tranaction.milestone.project.twitter,
                                                    tranaction.milestone.project.instagram,
                                                    tranaction.milestone.project.imageurl,
                                                    tranaction.milestone.project.impactor == null ? null : new ImpactorDto(
                                                        tranaction.milestone.project.impactor.id,
                                                        tranaction.milestone.project.impactor.wallet,
                                                        tranaction.milestone.project.impactor.name,
                                                        tranaction.milestone.project.impactor.description,
                                                        tranaction.milestone.project.impactor.website,
                                                        tranaction.milestone.project.impactor.facebook,
                                                        tranaction.milestone.project.impactor.discord,
                                                        tranaction.milestone.project.impactor.twitter,
                                                        tranaction.milestone.project.impactor.instagram,
                                                        tranaction.milestone.project.impactor.imageurl,
                                                        tranaction.milestone.project.impactor.role,
                                                        tranaction.milestone.project.impactor.type
                                                    ),
                                                    new CauseTypeDto(
                                                        tranaction.milestone.project.primarycausetype.id,
                                                        tranaction.milestone.project.primarycausetype.name
                                                    ),
                                                    new CauseTypeDto(
                                                        tranaction.milestone.project.secondarycausetype.id,
                                                        tranaction.milestone.project.secondarycausetype.name
                                                    )
                                                )
                                            )
                                    ));
                }

                recentImpactors.Add(new RecentDonationsResponseDto
                {
                    impactor = new ImpactorDto(
                                            donation.donator.id, 
                                            donation.donator.wallet,
                                            donation.donator.name,
                                            donation.donator.description,
                                            donation.donator.website,
                                            donation.donator.facebook,
                                            donation.donator.discord,
                                            donation.donator.twitter,
                                            donation.donator.instagram,
                                            donation.donator.imageurl,
                                            donation.donator.role,
                                            donation.donator.type
                                           ),
                    amount = donation.amount,
                    transactions = transactionsDto
                });
            }

            return recentImpactors;

        }

        public List<DonationDto> SearchDonations(GenericDto<DonationDto>? donationDto)
        {
            var donations = donationRepository.SearchAsync(donationDto).Result;

            var donationDtoList = new List<DonationDto>();
            foreach (var donation in donations)
            {
                donationDtoList.Add(new DonationDto(
                                            donation.id, 
                                            donation.amount,
                                            donation.creationdate,
                                            new ProjectDto(
                                                donation.project.id,
                                                new CharityDto(
                                                    donation.project.charity.id,
                                                    donation.project.charity.name,
                                                    donation.project.charity.wallet,
                                                    donation.project.charity.website,
                                                    donation.project.charity.facebook,
                                                    donation.project.charity.discord,
                                                    donation.project.charity.twitter,
                                                    donation.project.charity.imageurl,
                                                    donation.project.charity.description
                                                ),
                                                donation.project.wallet,
                                                donation.project.name,
                                                donation.project.description,
                                                donation.project.financialgoal,
                                                donation.project.totaldonated,
                                                donation.project.totalbackers,
                                                donation.project.website,
                                                donation.project.facebook,
                                                donation.project.discord,
                                                donation.project.twitter,
                                                donation.project.instagram,
                                                donation.project.imageurl,
                                                donation.project.impactor == null ? null : new ImpactorDto(
                                                    donation.project.impactor.id,
                                                    donation.project.impactor.wallet,
                                                    donation.project.impactor.name,
                                                    donation.project.impactor.description,
                                                    donation.project.impactor.website,
                                                    donation.project.impactor.facebook,
                                                    donation.project.impactor.discord,
                                                    donation.project.impactor.twitter,
                                                    donation.project.impactor.instagram,
                                                    donation.project.impactor.imageurl,
                                                    donation.project.impactor.role,
                                                    donation.project.impactor.type
                                                ),
                                                new CauseTypeDto(
                                                    donation.project.primarycausetype.id,
                                                    donation.project.primarycausetype.name
                                                ),
                                                new CauseTypeDto(
                                                    donation.project.secondarycausetype.id,
                                                    donation.project.secondarycausetype.name
                                                )
                                            ),
                                            new ImpactorDto(
                                                donation.donator.id,
                                                donation.donator.wallet,
                                                donation.donator.name,
                                                donation.donator.description,
                                                donation.donator.website,
                                                donation.donator.facebook,
                                                donation.donator.discord,
                                                donation.donator.twitter,
                                                donation.donator.instagram,
                                                donation.donator.imageurl,
                                                donation.donator.role,
                                                donation.donator.type
                                            )
                                        ));
            }

            return donationDtoList;


        }

        public Donation SaveDonaton(SaveDonationRequestDto saveDonationRequestDto)
        {
            var impactor = impactorRepository.SearchAsync(new GenericDto<ImpactorDto>(new ImpactorDto { wallet = saveDonationRequestDto.wallet })).Result.FirstOrDefault();
            var project = projectRepository.SearchAsync(new GenericDto<ProjectSearchDto>(new ProjectSearchDto { id = saveDonationRequestDto.projectid })).Result.FirstOrDefault();

            // Gather project's informations about backers and donations
            project.totaldonated += saveDonationRequestDto.amount;

            var thisProjectDonations = donationRepository.SearchDonationsGroupedByImpactorsAsync(new DonationSearchDto { projectid = project.id }).Result;
            if (thisProjectDonations.FindAll(d => d.wallet == saveDonationRequestDto.wallet).Count == 0)
            {
                project.totalbackers += 1;
            }

            // Donate (save donation

            var donation = new Donation
            {
                amount = saveDonationRequestDto.amount,
                projectid = project.id,
                donatorid = impactor.id,
                creationdate = saveDonationRequestDto.creationdate
            };

            var savedDonation = donationRepository.Save(donation);
            savedDonation.donator = impactor;
            savedDonation.project = project;

            // Upadte information about project
            project = projectRepository.Update(project);

            // Add trasactions 

            if (project.wallet != null)
            {
                // Project has wallet, so there will be two transactions
                // One from donator to charity (project's wallet)

                var transactionDonatorCharity = new Transaction
                {
                    amount = saveDonationRequestDto.amount * 0.98,
                    blockchainaddress = saveDonationRequestDto.blockchainaddress,
                    donationid = savedDonation.id,
                    receiver = project.charity.name,
                    type = 0,
                    creationdate = saveDonationRequestDto.creationdate
                };
                var savedTransactionDonatorCharity = transactionRepository.Save(transactionDonatorCharity);


                // Second from donator to ChainImpact

                var transactionDonatorChainImpact = new Transaction
                {
                    amount = saveDonationRequestDto.amount * 0.02,
                    blockchainaddress = saveDonationRequestDto.blockchainaddress,
                    donationid = savedDonation.id,
                    sender = impactor.name,
                    receiver = "Chain Impact",
                    type = 0
                };
                var savedTransactionDonatorChainImpact = transactionRepository.Save(transactionDonatorChainImpact);


            }
            else
            {
                // Project has no wallet, so there will be three transactions
                // TBD
            }

            // Update NFTs





            return savedDonation;

        }



    }
}
