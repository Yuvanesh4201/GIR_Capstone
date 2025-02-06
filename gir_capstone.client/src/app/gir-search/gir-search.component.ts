import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-gir-search',
  templateUrl: './gir-search.component.html',
  styleUrl: './gir-search.component.css'
})
export class GirSearchComponent {

  constructor(private router: Router) {

  }

  searchQuery: string = '';
  selectedItem: string | null = null;
  isDropdownOpen = false;

  mnes: string[] = [
    'Apple Inc.',
    'Microsoft Corporation',
    'Amazon',
    'Google (Alphabet Inc.)',
    'Facebook (Meta Platforms)',
    'Tesla',
    'Samsung Electronics',
    'Toyota Motor Corporation',
    'Coca-Cola',
    'Nike',
    'McDonald’s',
    'IBM',
    'General Electric',
    'Siemens',
    'Nestlé',
    'Honda Motor Co.',
    'Procter & Gamble (P&G)',
    'Unilever',
    'Pfizer',
    'Johnson & Johnson'
  ];


  get filteredItems(): string[] {
    return this.mnes.filter(mne =>
      mne.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }

  selectItem(item: string) {
    this.selectedItem = item;
    this.searchQuery = item;
    this.isDropdownOpen = false;
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  runSearch() {
    const query = this.selectedItem || this.searchQuery;
    if (query) {
      alert(`Searching for: ${query}`);
      this.router.navigateByUrl("gir-cyto-graph");
    } else {
      alert('Please enter a search term.');
    }
  }
}
