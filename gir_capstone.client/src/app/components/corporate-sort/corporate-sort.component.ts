import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GIRService } from '../../services/gir-graph.service';

@Component({
  selector: 'app-corporate-sort',
  templateUrl: './corporate-sort.component.html',
  styleUrl: './corporate-sort.component.css'
})
export class CorporateSortComponent {
  @Input() jurisdictionList: string[] | undefined;
  @Output() filterOption = new EventEmitter<{ type: string; value: string }>();
  @Output() highlightOption = new EventEmitter<{ type: string; value: string }>();
  @Output() clearOption = new EventEmitter();
  isDropdownOpen = false;

  selectedSortType: string = '';
  selectedSortValue: string = '';
  dynamicSortOptions: string[] | undefined;
  percentageOptions = ['Associate', 'Subsidary'];
  showFilter: boolean = false;

  constructor(private girService: GIRService) { }

  ngOnInit() {
    this.girService.isDropdownListOpen$.subscribe(data => this.isDropdownOpen = data);
  }

  updateSortOptions() {
    if (this.selectedSortType === 'jurisdiction') {
      this.dynamicSortOptions = this.jurisdictionList;
    } else if (this.selectedSortType === 'percentage') {
      this.dynamicSortOptions = this.percentageOptions;
    } else {
      this.dynamicSortOptions = [];
    }

    this.selectedSortValue = '';
  }

  onFilter() {
    this.filterOption?.emit({ type: this.selectedSortType, value: this.selectedSortValue });
  }

  onHighlight() {
    this.highlightOption?.emit({ type: this.selectedSortType, value: this.selectedSortValue });
  }

  onClear() {
    this.clearOption?.emit({ type: this.selectedSortType, value: this.selectedSortValue });
  }

  toggleFilter() {
    this.showFilter = !this.showFilter;
  }
}
