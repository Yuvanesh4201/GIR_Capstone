import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import cytoscape from 'cytoscape';
import { CorporateEntity, Ownership } from '../models/company-structure.model';
import { GIRService } from '../services/gir-graph.service';
import { girCytoGraphStyle } from './gir-graph-cyto-style';

@Component({
  selector: 'app-gir-graph-cyto',
  templateUrl: './gir-graph-cyto.component.html',
  styleUrls: ['./gir-graph-cyto.component.css']
})
export class GirGraphCytoComponent implements OnInit {
  @ViewChild('graph', { static: false }) cyContainer!: ElementRef;
  corporateStructure: CorporateEntity[] = [];
  selectedCorporateEntity!: CorporateEntity;
  selectedOwnership!: Ownership;
  selectedOwnerName!: string;
  selectedOwnedName!: string;
  showSelectedCorporateEntityInfo: Boolean = false;
  showSelectedOwnershipInfo: Boolean = false;
  cy: any;
  corporateId: any;
  zoom: number = 1.5;
  constructor(private route: ActivatedRoute, private girService: GIRService) {}

  ngOnInit() {
    this.corporateId = this.route.snapshot.queryParamMap.get('id');

    if (this.corporateId) {
      this.girService.getCorporateStructure(this.corporateId).subscribe(
        (data) => {
          if (data) {
            console.log('Corporate ID Works:', this.corporateId);
            this.corporateStructure = data;
            this.renderGraph();
          }
        },
        (error) => {
          console.error("Error fetching corporates:", error);
        }
      );
    }
  }

  renderGraph(): void {
    const corporateId = this.route.snapshot.queryParamMap.get('id') ?? "";
    this.cy = cytoscape({
      container: this.cyContainer.nativeElement,
      pixelRatio: 3,
      elements: [
        ...this.corporateStructure.map(node => ({
          data: {
            id: node.id,
            label: node.name,
            entityInfo: node,
            type: "child"
          },
          grabbable: false,
        })),
        ...this.corporateStructure.flatMap(corp =>
          corp.ownerships.map(edge => ({
            data: {
              source: edge.ownerEntityId,
              target: corp.id,
              label: `Owns ${edge.ownershipPercentage}%`,
              ownershipInfo: edge,
              ownedName: corp.name,
              ownerName: this.corporateStructure.find(node => node.id === edge.ownerEntityId)?.name
            },
            grabbable: false,
          }))
        )
      ], 
      style: girCytoGraphStyle,
      layout: { name: 'breadthfirst' },
    })

    const rootNode = {
      data: {
        id: corporateId ?? 'Blank',
        label: this.route.snapshot.queryParamMap.get('name') ?? 'Blank',
        type: "root"
      }
    };

    this.zoom = this.cy.zoom();

    this.cy.add(rootNode);

    this.corporateStructure.forEach(corp => {
      if (corp.parentId == null)
        this.cy.add({ data: { source: corporateId, target: corp.id } })
    }
    )

    this.cy.on('tap', 'node[type="child"]', (event:any) => {
      const node = event.target;
      this.showSelectedCorporateEntityInfo = true;
      this.showSelectedOwnershipInfo = false;
      this.selectedCorporateEntity = node.data().entityInfo;
    });

    this.cy.on('tap', 'edge', (event: any) => {
      const edge = event.target;
      this.showSelectedCorporateEntityInfo = false;
      this.showSelectedOwnershipInfo = true;
      this.selectedOwnership = edge.data().ownershipInfo;
      this.selectedOwnedName = edge.data().ownedName;
      this.selectedOwnerName = edge.data().ownerName;
    });

    this.cy.on('tap', (event: any) => {
      if (event.target === this.cy) {
        this.showSelectedCorporateEntityInfo = false;
        this.showSelectedOwnershipInfo = false;
      }
    });

    this.cy.layout({
      name: 'breadthfirst',
      roots: [corporateId],
      directed: true,
      spacingFactor: 1.5
    }).run();

    this.cy.zoom(this.zoom);
    this.cy.centre();
    }

  resetLayout() {
    this.cy.reset();
    this.cy.zoom(this.zoom);
    this.cy.centre();
  }

  fitView() {
    this.cy.fit();
  }
}
