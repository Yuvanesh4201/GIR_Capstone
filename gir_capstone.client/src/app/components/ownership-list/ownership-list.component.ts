import { Component, Input, OnInit } from '@angular/core';
import { Ownership } from '../../models/company-structure.model';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-ownership-list',
  templateUrl: './ownership-list.component.html',
  styleUrl: './ownership-list.component.css'
})
export class OwnershipListComponent implements OnInit {
  @Input() ownerships!: Ownership[];
  @Input() ownedName!: string;

  displayedColumns: string[] = ['ownerName', 'ownedName', 'ownershipPercentage']; // Ensure correct column names
  dataSource: any;

  ngOnInit(): void {
    this.dataSource = new MatTableDataSource(this.ownerships);
  }
}
