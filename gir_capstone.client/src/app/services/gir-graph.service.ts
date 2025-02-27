import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, catchError, Observable, tap, throwError } from "rxjs";
import { BatchCorporateRequestDto, Corporate } from "../models/corporate.model";
import { CorporateEntity } from "../models/company-structure.model";

@Injectable(
  {
    providedIn:'root'
  }
)
export class GIRService {

  private subTreeSubject = new BehaviorSubject<any>(null); // Holds latest graph data
  private selectedCorporateEntitySubject = new BehaviorSubject<any>(null);
  private selectedOwnershipInfoSubject = new BehaviorSubject<any>(null);

  subTreeData$ = this.subTreeSubject.asObservable(); // Observable to listen for changes
  selectedCorporateEntity$ = this.selectedCorporateEntitySubject.asObservable();
  selectedOwnershipInfo$ = this.selectedOwnershipInfoSubject.asObservable();
  constructor(private http: HttpClient) { }

  getCorporates(): Observable<Corporate[]> {
    return this.http.get<Corporate[]>(`/api/GIR/RetrieveCorporates`);
  }

  getCorporateStructure(corporateId: string, xmlParse: boolean): Observable<CorporateEntity[]> {
    return this.http.get<CorporateEntity[]>(`/api/GIR/RetrieveCorporateStructure/${corporateId}/${xmlParse}`);
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

  updateSubTreeData(newData: any) {
    this.subTreeSubject.next(newData); // Updates graph data
  }

  updateSelectedCorporateEntity(newData: any) {
    this.selectedCorporateEntitySubject.next(newData);
  }

  updateSelectedOwnershipInfo(newData: any) {
    this.selectedOwnershipInfoSubject.next(newData);
  }

}
