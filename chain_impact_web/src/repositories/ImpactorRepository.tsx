import axios from "axios";
import { useEffect, useState } from "react";
import { CreateNewImpactor } from "../models/dto/request/CreateNewImpactor";
import { ImpactorTypeFilter } from "../models/dto/request/ImpactorTypeFilter";
import { ImpactorWalletSearch } from "../models/dto/request/ImpactorWalletSearch";
import { ImpactorsWithDonations } from "../models/dto/response/ImpactorsWithDonations";
import { Impactor } from "../models/Impactor";

const url = "http://167.99.246.54/";

export function useGetAllImpactors() {
  const [impactors, setImpactors] = useState<Impactor[]>([]);

  useEffect(() => {
    axios.get(url + "Impactor").then((response) => {
      const impactorData = response.data as Impactor[];
      setImpactors(impactorData);
    });
  }, []);

  return impactors;
}

export async function getSpecificImpactor(filter: ImpactorWalletSearch) {
  let impactor: Impactor | null = null;

  await axios.post(url + "Impactor/Search", filter).then((response) => {
    const impactorData = response.data as Impactor[];
    impactor = impactorData[0];
  });
  return impactor;
}

export function useGetImpactorsWithDonations(
  filter: ImpactorTypeFilter | {},
  privateUser: boolean
) {
  const [impactors, setImpactors] = useState<ImpactorsWithDonations[]>([]);

  useEffect(() => {
    axios
      .post(url + "Donation/ImpactorsWithDonations", filter)
      .then((response) => {
        const impactorData = response.data as ImpactorsWithDonations[];
        let privateUserImpactorData = [];
        let companyImpactorData = [];

        for (let i = 0; i < impactorData.length; i++) {
          if (impactorData[i].userType === 1)
            privateUserImpactorData.push(impactorData[i]);
          else companyImpactorData.push(impactorData[i]);
        }

        if (privateUser) setImpactors(privateUserImpactorData);
        else setImpactors(companyImpactorData);
      });
  }, []);

  return impactors;

  //return ImpactorData.slice(0, 5) as Impactor[];
}

export function createNewImpactor(newImpactor: CreateNewImpactor) {
  axios.post(url + "Impactor/Save", newImpactor).then((response) => {
    const impactorData = response.data as Impactor;
    return impactorData;
  });
}
