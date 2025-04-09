import { Component, EventEmitter, Input, input, OnInit, Output } from '@angular/core';
import { CorporateEntity, Ownership } from '../../models/company-structure.model';
import { GIRService } from '../../services/gir-graph.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-corporate-entity-info',
  templateUrl: './corporate-entity-info.component.html',
  styleUrl: './corporate-entity-info.component.css'
})
export class CorporateEntityInfoComponent implements OnInit {
  @Output() showOwnershipList = new EventEmitter<Ownership[]>();
  corporateEntityInfo$: Observable<CorporateEntity | null> | undefined;

  constructor(private girService: GIRService) { }

  ngOnInit() {
    this.corporateEntityInfo$ = this.girService.selectedCorporateEntity$;
  }

  onLinkClick(event: Event, ownerships: Ownership[]): void {
    event.preventDefault();
    if (ownerships?.length) {
      this.showOwnershipList.emit(ownerships);
    } else {
      console.warn('No ownership data available');
      this.showOwnershipList.emit([]);
    }
  }
}
