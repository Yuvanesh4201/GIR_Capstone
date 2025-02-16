export interface CorporateEntity {
  id: string;
  name: string;
  jusridiction: string;
  tin: string;
  parentId: string;
  is_Excluded: boolean;
  statuses: string[];
  ownerships: Ownership[];
  qiir_Status: string;
}

export interface Ownership {
  ownerEntityId: string;
  ownerName: string;
  ownershipType: string;
  ownershipPercentage: number;
}
