import { Component } from '@angular/core';
import { XMLParser } from 'fast-xml-parser';
import { CorporateEntity, Ownership } from '../models/company-structure.model';
import { v4 as uuidv4 } from 'uuid';
import { Router } from '@angular/router';

@Component({
  selector: 'app-gir-parse-file',
  templateUrl: './gir-parse-file.component.html',
  styleUrl: './gir-parse-file.component.css'
})
export class GirParseFileComponent {

  selectedFile: File | null = null;
  parsedData: any;

  constructor(private router: Router) {}

  onFileSelected(event: Event) {
    const inputElement = event.target as HTMLInputElement;

    // Ensure there's a file selected
    if (!inputElement.files || inputElement.files.length === 0) {
      alert("No File Selected. Please choose an XML file.");
      return;
    }

    this.selectedFile = inputElement.files[0];

    // Optional: Check if the file is an XML file
    if (this.selectedFile.type !== "text/xml") {
      alert("Invalid File Type. Please select an XML file.");
      this.selectedFile = null; // Reset selected file
    }
  }

  parseFile() {
    const reader = new FileReader();
    reader.onload = async (e) => {
      if (e.target?.result) {
        const xmlString = e.target.result as string;
        this.parsedData = this.parseXml(xmlString);
        this.router.navigate(['/gir-cyto-graph'], { queryParams: { data: JSON.stringify(this.parsedData) } });
      }
    };

    if (this.selectedFile) {
      reader.readAsText(this.selectedFile);
    } else {
      console.error("No file selected");
    }

  }
  parseXml(xmlString: string): any {
    const alwaysArray = ["UPE", "CE", "Ownership"];

     const parser = new XMLParser(
       {
         ignoreAttributes: true,
         trimValues: true,
         allowBooleanAttributes: true,
         isArray: (name: string, jpath: string, isLeafNode: boolean, isAttribute: boolean) => {
           return alwaysArray.includes(name); // ✅ If tag name is in alwaysArray, force it as an array
         },
       });
    
     const jsonObj = parser.parse(xmlString);
     const generalSection = jsonObj?.GLOBE_OECD?.GLOBEBody?.GeneralSection;
     const upeArray = this.getOtherUPE(generalSection?.CorporateStructure?.UPE);
     const ceArray = this.getCE(generalSection?.CorporateStructure?.CE, upeArray);
     const finalArray = upeArray.concat(ceArray);

     return finalArray;
  }

private getOtherUPE(upeList: any): CorporateEntity[] {
 
  if (upeList && Array.isArray(upeList)) {
    return upeList.map((entry: any) => {
      const id = entry.OtherUPE?.ID ?? {}; // Ensure `id` is always an object
      const tin = id.TIN ?? {}; // Ensure `tin` is always an object

      return {
          id: uuidv4(),
          name: id.Name ?? "N/A",
          tin: tin ?? "N/A",
          jurisdiction: id.ResCountryCode ?? "N/A",
          parentId: "",
          is_Excluded: false,
          statuses: Array.isArray(entry.GlobeStatus)
              ? entry.GlobeStatus.map((status: string) => status.slice(3))
          : [], // Ensure `statuses` is always an array
        ownerships: entry.Ownership ?? [],
          qiir_Status: "",
      } as unknown as CorporateEntity;
    });
  }
  return [];
}

private getCE(ceList: any, upeArray: CorporateEntity[]): CorporateEntity[] {
  if (!ceList || !Array.isArray(ceList)) {
    console.warn("ceList is not an array or is undefined.");
    return [];
  }

  return ceList.map((entry: any) => {
    const id = entry?.ID ?? {}; // Ensure `id` is always an object
    const tin = id.TIN ?? {}; // Ensure `tin` is always an object

    // Normalize Ownerships (ensure it's always an array)
    const ownerships = Array.isArray(entry.Ownership) ? entry.Ownership : [entry.Ownership];

    return {
      id: uuidv4(),
      name: id.Name ?? "N/A",
      tin: tin ?? "N/A", // Extract text from XML TIN object
      jurisdiction: id.ResCountryCode ?? "N/A",
      parentId: "ce",
      is_Excluded: false,
      statuses: Array.isArray(entry.GlobeStatus)
        ? entry.GlobeStatus.map((status: string) => status.slice(3))
        : [], // Ensure `statuses` is always an array
      ownerships: ownerships
        .filter((ownership: any) => ownership?.TIN) // Remove null/undefined ownerships
        .map((ownership: any) => {
          const owner = upeArray.find(entity => entity.tin === ownership.TIN);

          return {
            ownerEntityId: owner?.id ?? null,
            ownerName: owner?.name ?? "Unknown",
            ownershipType: ownership.OwnershipType?.slice(3) ?? "Unknown",
            ownershipPercentage: ownership.OwnershipPercentage ?? 0,
          } as Ownership;
        }),
      qiir_Status: "",
    } as CorporateEntity;
  });
}

}
