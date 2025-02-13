import { Component, Input, input } from '@angular/core';
import { CorporateEntity } from '../../models/company-structure.model';

@Component({
  selector: 'app-corporate-entity-info',
  templateUrl: './corporate-entity-info.component.html',
  styleUrl: './corporate-entity-info.component.css'
})
export class CorporateEntityInfoComponent {
  @Input() corporateEntityInfo!: CorporateEntity;
}
