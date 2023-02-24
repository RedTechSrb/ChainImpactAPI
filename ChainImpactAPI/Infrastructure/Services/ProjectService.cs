﻿using ChainImpactAPI.Application.RepositoryInterfaces;
using ChainImpactAPI.Application.ServiceInterfaces;
using ChainImpactAPI.Dtos;

namespace ChainImpactAPI.Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IConfiguration configuration;
        private readonly IProjectRepository projectRepository;

        public ProjectService(
            IConfiguration configuration,
            IProjectRepository projectRepository)
        {
            this.configuration = configuration;
            this.projectRepository = projectRepository;
        }

        public List<ProjectDto> GetProjects()
        {
            var projects = projectRepository.ListAllAsync().Result;

            var projectsDto = new List<ProjectDto>();
            foreach (var project in projects)
            {
                projectsDto.Add(new ProjectDto(project.Id, project.Name));
            }

            return projectsDto;
        }
    }
}