export interface CorporateEntity {
  id: string;
  name: string;
  jusridiction: string;
  tin: string;
  parentId: string;
  is_Excluded: boolean;
  statuses: string[];
  ownerships: Ownership[];
}

export interface Ownership {
  ownerEntityId: string;
  ownershipType: string;
  ownershipPercentage: number;
}
