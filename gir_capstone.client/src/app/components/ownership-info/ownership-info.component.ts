import { Component, Input, OnInit } from '@angular/core';
import { Ownership } from '../../models/company-structure.model';
import { GIRService } from '../../services/gir-graph.service';
import { Observable } from 'rxjs';
import { OwnershipEdge } from '../../models/ownership-edge.model';

@Component({
  selector: 'app-ownership-info',
  templateUrl: './ownership-info.component.html',
  styleUrl: './ownership-info.component.css'
})
export class OwnershipInfoComponent implements OnInit {

  ownershipInfo$: Observable<OwnershipEdge | null> | undefined;
  constructor(private girService: GIRService) { }

  ngOnInit() {
    this.ownershipInfo$ = this.girService.selectedOwnershipInfo$;
    }
}
