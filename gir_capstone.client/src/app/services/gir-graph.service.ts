import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { catchError, Observable, tap, throwError } from "rxjs";
import { BatchCorporateRequestDto, Corporate } from "../models/corporate.model";
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

  batchCorporateXml(corporateId: string) {

    const body: BatchCorporateRequestDto = { corporateId: corporateId };
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    const url = `/api/GIR/BatchCorporateStructure`;

   this.http.post(url, body, { headers }).subscribe(
      response => console.log('Success:', response),
      error => console.error('Error:', error)
    );

  }

}
