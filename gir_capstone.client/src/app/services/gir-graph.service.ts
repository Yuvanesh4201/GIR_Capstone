import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { BatchCorporateRequestDto, Corporate } from "../models/corporate.model";
import { CorporateEntity } from "../models/company-structure.model";
import { ElementDefinition } from "cytoscape";
import jsPDF from "jspdf";
import autoTable from "jspdf-autotable";

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
  private currentCyGraphSubject = new BehaviorSubject<any>(null);
  private mainCyGraphSubject = new BehaviorSubject<any>(null);
  private isDropdownListOpenSubject = new BehaviorSubject<boolean>(false);
  

  subTreeData$ = this.subTreeSubject.asObservable(); // Observable to listen for changes
  selectedCorporateEntity$ = this.selectedCorporateEntitySubject.asObservable();
  selectedOwnershipInfo$ = this.selectedOwnershipInfoSubject.asObservable();
  subTreeList$ = this.subTreeListSubject.asObservable();
  currentCyGraph$ = this.currentCyGraphSubject.asObservable();
  mainCyGraph$ = this.mainCyGraphSubject.asObservable();
  isDropdownListOpen$ = this.isDropdownListOpenSubject.asObservable();
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

  updateCurrentCyGraph(cy: cytoscape.Core) {
    this.currentCyGraphSubject.next(cy);
  }

  updateMainCyGraph(cy: cytoscape.Core) {
    this.mainCyGraphSubject.next(cy);
  }

  clearCurrentCyGraph() {
    this.currentCyGraphSubject.next(this.mainCyGraphSubject.value);
  }

  dropdownnListOpen() {
    this.isDropdownListOpenSubject.next(true);
  }

  dropdownnListClose() {
    this.isDropdownListOpenSubject.next(false);
  }

  exportGraphAsImage(mneName:string, cy: cytoscape.Core) {
    if(!cy) {
      console.error("Cytoscape instance is not available.");
      alert("Error: Cannot export graph, Cytoscape not initialized.");
      return;
    }

    let imageData = cy.jpg({ full: true, quality: 1 });

    // Create a download link
    const link = document.createElement('a');
    link.href = imageData;

    const currentGraphRoot = this.currentCyGraphSubject.value
      ?.nodes()
      ?.roots()[0] ?? "No Root Found";

    if (this.subTreeSubject.value === null || this.subTreeSubject.value === undefined) {
      link.download = `${mneName.replace(/\s+/g, '_')}.jpg`;
    }
    else {
      link.download = `${mneName.replace(/\s+/g, '_')}_${currentGraphRoot?.data()?.label}.jpg`;
    }

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  exportGraphAsPdf(mneName: string, cy: cytoscape.Core) {
    const imageData = cy.png({ full: true, scale: 1 });

    const pdf = new jsPDF({ orientation: "landscape", unit: "mm", format: "a4" });
    pdf.setFontSize(18);
    pdf.text("Cytoscape Graph Export", 15, 20);

    const img = new Image();
    img.src = imageData;

    img.onload = () => {

      let rowPositions: Record<string, number> = {}; // Store Table 2 row positions
      let entityTableLinks: { id: string; x: number; y: number }[] = []; // Stores Table 1 "More Info" cell positions
      // Get image's original width and height
      const imgWidth = img.width;
      const imgHeight = img.height;

      // Create a jsPDF instance
      const pdf = new jsPDF({ orientation: "landscape", unit: "mm", format: "a4" });

      // Get PDF page size
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();

      // Calculate aspect ratio
      const imgAspectRatio = imgWidth / imgHeight;
      const pageAspectRatio = pageWidth / pageHeight;

      let renderWidth, renderHeight, xPos, yPos;

      if (imgAspectRatio > pageAspectRatio) {
        // Image is wider than page, scale by width
        renderWidth = pageWidth;
        renderHeight = pageWidth / imgAspectRatio;
        xPos = 0;
        yPos = (pageHeight - renderHeight) / 2; // Center vertically
      } else {
        // Image is taller than page, scale by height
        renderHeight = pageHeight;
        renderWidth = pageHeight * imgAspectRatio;
        xPos = (pageWidth - renderWidth) / 2; // Center horizontally
        yPos = 0;
      }

      pdf.addImage(imageData, "PNG", xPos, yPos, renderWidth, renderHeight);

      const entityTableData = [["Corporate Name", "Type", "TIN" , "Juristication", "Globe Statuses" ,"QIIR Status", "Ownerships"]];
      const ownershipTableData = [["OwnedEntity", "OwnerName", "OwnershipType", "OwnershipPercentage",]];

      cy.nodes().forEach((node) => {
        const entity = node.data("entityInfo") as CorporateEntity;
        entityTableData.push([node.data("label"), node.data("type"), entity.tin, entity.jurisdiction, entity.statuses, entity.qiir_Status, "" || ""]);

        entity.ownerships.forEach((ownership) => {
          ownershipTableData.push([entity.name, ownership.ownerName, ownership.ownershipType, ownership.ownershipPercentage.toString() || ""]);
        })
        
      });

      autoTable(pdf, {
        startY: 190,
        head: [entityTableData[0]],
        body: entityTableData.slice(1),
        styles: { fontSize: 8, cellPadding: 2 },
        pageBreak: 'auto', // 'auto', 'avoid' or 'always'
        tableWidth: 'auto', // 'auto', 'wrap' or a number,
        columnStyles: {
          3: { cellWidth: 'wrap' }, // Adjust first column
          6: { cellWidth: 'auto' }
        },
        margin: { top: 10 },
        didDrawCell: (data) => {
          if (data.section === "body" && data.column.index === 6) {
            // Store cell positions for "See details" column
            const itemId = entityTableData[data.row.index + 1][0];
            entityTableLinks.push({ id: itemId, x: data.cell.x, y: data.cell.y });
          }
        },
        didDrawPage: (data) => {
          if (data.cursor?.y ?? 0 > 260) pdf.addPage(); // Handles large tables automatically
        },
      });

      autoTable(pdf, {
        startY: 20, // Start position for Table 2
        head: [ownershipTableData[0]],
        body: ownershipTableData.slice(1),
        didDrawCell: (data) => {
          if (data.section === "body") {
            const itemId = ownershipTableData[data.row.index + 1][0]; // Get item ID
            rowPositions[itemId] = data.cell.y; // Store row Y position
          }
        }
      });

      pdf.setPage(2); // Go back to Page 1

      //const defaultFontSize = pdf.getFontSize();
      const defaultFont = pdf.getFont();

      entityTableLinks.forEach((cell) => {
        if (rowPositions[cell.id]) {
          pdf.setTextColor(0, 0, 255); // 🔹 Set text color to blue (RGB)
          pdf.setFont("helvetica", "normal"); // 🔹 Make text bold
          pdf.setFontSize(10);
          pdf.text("See Ownership", cell.x + 2, cell.y + 4);
          //pdf.line(cell.x + 2, cell.y + 5, cell.x + 25, cell.y + 5);

          pdf.textWithLink("See Ownership", cell.x + 2, cell.y + 4, {
            pageNumber: 3, // Jump to Page 2 where Table 2 is
            top: rowPositions[cell.id], // Scroll to the correct row in Table 2
          });

          pdf.setTextColor(0, 0, 0); // Reset text color to black
          pdf.setFont(defaultFont.fontName, "normal"); // Reset font style
        }
      });

      const currentGraphRoot = this.currentCyGraphSubject.value
        ?.nodes()
        ?.roots()[0] ?? "No Root Found";

      let fileName = "";

      if(this.subTreeSubject.value === null || this.subTreeSubject.value === undefined) {
        fileName = `${mneName.replace(/\s+/g, '_')}.pdf`;
      }
    else {
        fileName = `${mneName.replace(/\s+/g, '_')}_${currentGraphRoot?.data()?.label}.pdf`;
      }

      pdf.save(fileName);

    };


  }
}
