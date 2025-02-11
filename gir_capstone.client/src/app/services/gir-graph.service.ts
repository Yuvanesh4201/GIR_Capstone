import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Corporate } from "../models/corporate.model";
import { CorporateEntity } from "../models/company-structure.model";

@Injectable(
  {
    providedIn:'root'
  }
)
export class GIRService {

  constructor(private http: HttpClient) { }

  getCorporates(): Observable<Corporate[]> {
    return this.http.get<Corporate[]>(`/api/GIR/RetrieveCorporates`);
  }

  getCorporateStructure(corporateId: string): Observable<CorporateEntity[]> {
    return this.http.get<CorporateEntity[]>(`/api/GIR/RetrieveCorporateStructure/${corporateId}`);
  }

}
