import { CauseType } from "./CauseType";
import { Charity } from "./Charity";
import { Impactor } from "./Impactor";

export interface Project {
    id: number,
    charity: Charity,
    name: string,
    description: string | null,
    milestones: string | null,
    finantialGoal: number,
    totaldonated: number,
    website: string | null,
    facebook: string | null,
    discord: string | null,
    twitter: string | null,
    instagram: string | null,
    imageurl: string | null,
    impactor: Impactor | null,
    primarycausetype: CauseType,
    secondarycausetype: CauseType
}