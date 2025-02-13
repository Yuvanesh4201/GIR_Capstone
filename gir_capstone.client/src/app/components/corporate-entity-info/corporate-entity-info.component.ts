import { Component, EventEmitter, Input, input, Output } from '@angular/core';
import { CorporateEntity, Ownership } from '../../models/company-structure.model';

@Component({
  selector: 'app-corporate-entity-info',
  templateUrl: './corporate-entity-info.component.html',
  styleUrl: './corporate-entity-info.component.css'
})
export class CorporateEntityInfoComponent {
  @Input() corporateEntityInfo!: CorporateEntity;
  @Output() showOwnershipList = new EventEmitter<Ownership[]>();

  onLinkClick(event: Event): void {
    event.preventDefault();
    if (this.corporateEntityInfo?.ownerships?.length) {
      this.showOwnershipList.emit(this.corporateEntityInfo.ownerships);
    } else {
      console.warn('No ownership data available');
      this.showOwnershipList.emit([]); // Emit empty array to prevent type errors
    }
  }
}
