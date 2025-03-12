import { Component } from '@angular/core';

@Component({
  selector: 'app-corporate-status-legend',
  templateUrl: './corporate-status-legend.component.html',
  styleUrls: ['./corporate-status-legend.component.css']
})
export class CorporateStatusLegendComponent {
  showLegend = false; // Default: legend is visible

  legendItems = [
    { type: 'UPE', label: 'Ultimate Parent Entity (UPE)', color: '#e9919c', border: '#A01725' },
    { type: 'POPE', label: 'Partially-Owned Parent Entity (POPE)', color: '#f4a261', border: '#c66b2b' },
    { type: 'IPE', label: 'Intermediate Parent Entity (IPE)', color: '#2a9d8f', border: '#1d7062' },
    { type: 'CE', label: 'Constituent Entity (CE)', color: '#b7d1e9', border: '#3A6FB0' }
  ];

  toggleLegend() {
    this.showLegend = !this.showLegend;
  }
}
