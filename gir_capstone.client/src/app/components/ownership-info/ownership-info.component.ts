import { Component, Input } from '@angular/core';
import { Ownership } from '../../models/company-structure.model';

@Component({
  selector: 'app-ownership-info',
  templateUrl: './ownership-info.component.html',
  styleUrl: './ownership-info.component.css'
})
export class OwnershipInfoComponent {
  @Input() ownerName!: string;
  @Input() ownedName!: string;
  @Input() ownershipInfo!: Ownership;
}
