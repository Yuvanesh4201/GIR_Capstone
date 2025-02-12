import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import cytoscape from 'cytoscape';
import { CorporateEntity } from '../models/company-structure.model';
import { GIRService } from '../services/gir-graph.service';

@Component({
  selector: 'app-gir-graph-cyto',
  templateUrl: './gir-graph-cyto.component.html',
  styleUrls: ['./gir-graph-cyto.component.css']
})
export class GirGraphCytoComponent implements OnInit {
  @ViewChild('graph', { static: false }) cyContainer!: ElementRef;
  corporateStructure: CorporateEntity[] = [];
  constructor(private route: ActivatedRoute, private girService: GIRService) {}

  ngOnInit() {
    const corporateId = this.route.snapshot.queryParamMap.get('id');

    if (corporateId) {
      this.girService.getCorporateStructure(corporateId).subscribe(
        (data) => {
          if (data) {
            console.log('Corporate ID Works:', corporateId);
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
    const cy = cytoscape({
      container: this.cyContainer.nativeElement,
      elements: [
        ...this.corporateStructure.map(node => ({
          data: { id: node.id, label: node.name }
        })),
        ...this.corporateStructure.flatMap(corp =>
          corp.ownerships.map(edge => ({
            data: { source: edge.ownerEntityId, target: corp.id }
          }))
        )
      ], 
      style: [
        {
          selector: 'node',
          style: {
            'background-color': '#0074D9',
            'label': 'data(label)',
            'color': '#fff',
            'text-valign': 'center',
            'text-halign': 'center',
            'font-size': '12px',
            'width': 'label',
            'height': 'label',
            'shape': 'roundrectangle'
          }
        },
        {
          selector: 'edge',
          style: {
            'width': 2,
            'line-color': '#aaa',
            'target-arrow-color': '#aaa',
            'target-arrow-shape': 'triangle'
          }
        }
      ],

      layout: { name: 'breadthfirst' },
    })

    const rootNode = {
      data: { id: corporateId ?? 'Blank', label: this.route.snapshot.queryParamMap.get('name') ?? 'Blank' }
    };
    cy.add(rootNode);

    this.corporateStructure.forEach(corp => {
      if (corp.parentId == null)
        cy.add({ data: { source: corporateId, target: corp.id } })
    }
    )

    cy.layout({
      name: 'breadthfirst',
      roots: [corporateId],
      directed: true,
      spacingFactor: 1.5
    }).run();

    }
 
}
