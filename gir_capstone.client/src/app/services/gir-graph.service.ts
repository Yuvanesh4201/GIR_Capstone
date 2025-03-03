import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { BatchCorporateRequestDto, Corporate } from "../models/corporate.model";
import { CorporateEntity } from "../models/company-structure.model";
import { ElementDefinition } from "cytoscape";

@Injectable(
  {
    providedIn:'root'
  }
)
export class GIRService {

  private subTreeSubject = new BehaviorSubject<any>(null); // Holds latest graph data
  private selectedCorporateEntitySubject = new BehaviorSubject<any>(null);
  private selectedOwnershipInfoSubject = new BehaviorSubject<any>(null);
  private subTreeListSubject = new BehaviorSubject<ElementDefinition[][]>([]);
  

  subTreeData$ = this.subTreeSubject.asObservable(); // Observable to listen for changes
  selectedCorporateEntity$ = this.selectedCorporateEntitySubject.asObservable();
  selectedOwnershipInfo$ = this.selectedOwnershipInfoSubject.asObservable();
  subTreeList$ = this.subTreeListSubject.asObservable();
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
    this.subTreeSubject.next(newData);
  }

  clearSubTreeData() {
    this.subTreeSubject.next(null);
  }

  updateSelectedCorporateEntity(newData: any) {
    this.selectedCorporateEntitySubject.next(newData);
  }

  clearSelectedCorporateEntity() {
    this.selectedCorporateEntitySubject.next(null);
  }

  updateSelectedOwnershipInfo(newData: any) {
    this.selectedOwnershipInfoSubject.next(newData);
  }

  clearSelectedOwnershipInfo() {
    this.selectedOwnershipInfoSubject.next(null);
  }

  updateSubTreeList(newSubTree: ElementDefinition[]) {
    const updatedList = [...this.subTreeListSubject.getValue(), newSubTree]; // ✅ Create new array
    this.subTreeListSubject.next(updatedList); // ✅ Automatically triggers UI updates
  }

  removeLastSubTree() {
    const currentList = this.subTreeListSubject.getValue();
    if (currentList.length > 0) {
      const updatedList = currentList.slice(0, -1); // ✅ Removes last element safely
      this.subTreeListSubject.next(updatedList); // ✅ Triggers UI update
    }
  }

  clearSubTreeList() {
    this.subTreeListSubject.next([]); // ✅ Automatically triggers UI updates
  }


}
