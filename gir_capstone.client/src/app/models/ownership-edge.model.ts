import { Ownership } from "./company-structure.model";

export interface OwnershipEdge {
  ownershipInfo: Ownership;
  ownedName: string;
  ownerName: string;
}
