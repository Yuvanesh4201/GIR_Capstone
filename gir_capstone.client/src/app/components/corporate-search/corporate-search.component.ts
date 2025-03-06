import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { GIRService } from '../../services/gir-graph.service';

@Component({
  selector: 'app-corporate-search',
  templateUrl: './corporate-search.component.html',
  styleUrl: './corporate-search.component.css'
})
export class CorporateSearchComponent implements OnInit {
  @Input() corporatesList: string[] | undefined;
  @Output() searchedCorporate = new EventEmitter<string>();
  searchQuery: string = '';
  isDropdownOpen = false;

  constructor(private girService: GIRService) { }

  ngOnInit() {
    this.girService.isDropdownListOpen$.subscribe(data => this.isDropdownOpen = data);
  }

  get filteredItems(): string[] {
    if (!this.corporatesList || this.corporatesList.length === 0) {
      return [];
    }
    return this.corporatesList
      .filter((mne: string) => {
          return mne.toLowerCase().startsWith(this.searchQuery.toLowerCase());
      });
  }

  selectItem(item: string) {
    this.searchQuery = item;
    this.isDropdownOpen = false;
  }

  searchEntity() {
    this.searchedCorporate?.emit(this.searchQuery);
  }

  clearSearch() {
    this.searchQuery = '';
  }

}
