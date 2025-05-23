import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GIRService } from '../services/gir-graph.service';
import { Corporate } from '../models/corporate.model';

@Component({
  selector: 'app-gir-search',
  templateUrl: './gir-search.component.html',
  styleUrl: './gir-search.component.css'
})
export class GirSearchComponent implements OnInit {
  selectedCorporate!: Corporate;
  isDropdownOpen = false;
  searchQuery: string = '';
  corporates!: Corporate[];

  constructor(private router: Router, private girService: GIRService)
  {}

  ngOnInit() {
    this.girService.getCorporates().subscribe(
      (data) => {
        if (data) {
          this.corporates = data;
        }
      },
      (error) => {
        console.error("Error fetching corporates:", error);
      }
    );
  }
  get filteredItems(): Corporate[] {
    if (!this.corporates || this.corporates.length === 0) {
      return [];
    }
    return this.corporates
      .filter(mne => mne.mneName.toLowerCase().startsWith(this.searchQuery.toLowerCase()));
  }

  selectItem(item: Corporate) {
    this.selectedCorporate = item;
    this.searchQuery = item.mneName;
    this.isDropdownOpen = false;
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  runDbSearch() {
    if (this.selectedCorporate) {
      this.router.navigate(['/gir-cyto-graph'], { queryParams: { id: this.selectedCorporate.structure_Id, name: this.selectedCorporate.mneName, xmlParse: false } });
    } else {
      alert('Please enter a search term.');
    }
  }

  runXmlSearch() {
    if (this.selectedCorporate) {
      this.router.navigate(['/gir-cyto-graph'], { queryParams: { id: this.selectedCorporate.structure_Id, name: this.selectedCorporate.mneName, xmlParse: true } });
    } else {
      alert('Please enter a search term.');
    }
  }

  runBatch() {
    if (this.selectedCorporate) {
      if (this.selectedCorporate.mneName == 'Sample Corporation Ltd') //Temp Check
        this.girService.batchCorporateXml(this.selectedCorporate.structure_Id);
    }
  }
}
