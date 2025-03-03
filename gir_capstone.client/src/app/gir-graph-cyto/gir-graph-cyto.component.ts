import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import cytoscape, { ElementDefinition } from 'cytoscape';
import { CorporateEntity, Ownership } from '../models/company-structure.model';
import { GIRService } from '../services/gir-graph.service';
import { girCytoGraphStyle } from './gir-graph-cyto-style';
import { OwnershipEdge } from '../models/ownership-edge.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-gir-graph-cyto',
  templateUrl: './gir-graph-cyto.component.html',
  styleUrls: ['./gir-graph-cyto.component.css']
})
export class GirGraphCytoComponent implements OnInit {
  @ViewChild('graph', { static: false }) cyContainer!: ElementRef;
  corporateStructure: CorporateEntity[] = [];
  selectedOwnership!: Ownership;
  selectedOwnerships!: Ownership[];
  selectedOwnerName!: string;
  selectedOwnedName!: string;
  showSelectedCorporateEntityInfo: boolean = false;
  showSelectedOwnershipInfo: boolean = false;
  showSelectedOwnershipList: boolean = false;
  cy: any;
  corporateId: any;
  mneName: any;
  xmlParse: any;
  zoom: number = 1.5;
  corporateEntityInfo$: Observable<CorporateEntity | null> | undefined;
  ownershipInfo$: Observable<any> | undefined;
  nodesOnly: any; 

  constructor(private route: ActivatedRoute, private girService: GIRService) {}

  ngOnInit() {
    this.corporateId = this.route.snapshot.queryParamMap.get('id');
    this.mneName = this.route.snapshot.queryParamMap.get('name');
    this.xmlParse = this.route.snapshot.queryParamMap.get('xmlParse');

    if (this.corporateId) {
      this.girService.getCorporateStructure(this.corporateId, this.xmlParse).subscribe(
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

    this.corporateEntityInfo$ = this.girService.selectedCorporateEntity$;
    this.ownershipInfo$ = this.girService.selectedOwnershipInfo$;
  }

  renderGraph(): void {
    this.cy = cytoscape({
      container: this.cyContainer.nativeElement,
      pixelRatio: 3,
      elements: [
        ...this.corporateStructure.map(node => ({
          data: {
            id: node.id,
            label: node.name,
            entityInfo: node,
            type: this.setNodeStyleType(node),
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
              ownerName: edge.ownerName,
            },
            grabbable: false,
          }))
        )
      ],
      style: girCytoGraphStyle,
      layout: { name: 'breadthfirst' },
    });

    this.zoom = this.cy.zoom();

    this.cy.on('tap', 'node', (event:any) => {
      const clickedNode = event.target;
      this.girService.clearSelectedOwnershipInfo();
      this.showSelectedOwnershipList = false;
      this.girService.updateSelectedCorporateEntity(clickedNode.data().entityInfo);

      //Create SubTree
      const subTree = this.cy.elements().bfs(
        {
          roots: clickedNode,
          directed: true,
        });

      subTree.path.forEach((n: cytoscape.NodeSingular | cytoscape.EdgeSingular) => {
        n.unselect();
      });

      const validSubTreeData: ElementDefinition[] = subTree.path.map((n: cytoscape.NodeSingular | cytoscape.EdgeSingular) => ({
        data: { ...n.data() },
        grabbable: false,
      }));

      // ✅ Ensure correct typing & filtering
      this.nodesOnly = validSubTreeData.filter((el: ElementDefinition) =>
        el.data?.id !== undefined && !el.data?.source && !el.data?.target
      );

      // ✅ Update subtree only if more than 2 nodes exist
      if ((this.nodesOnly?.length ?? 0) > 2) {
        this.girService.updateSubTreeData(validSubTreeData);
      }

    });

    this.cy.on('tap', 'edge', (event: any) => {
      const edge = event.target;
      this.girService.clearSelectedCorporateEntity();
      this.showSelectedOwnershipList = false;

      this.girService.updateSelectedOwnershipInfo({
        ownershipInfo: edge.data().ownershipInfo,
        ownedName: edge.data().ownedName,
        ownerName: edge.data().ownerName
      } as OwnershipEdge);
    });

    this.cy.on('tap', (event: any) => {
      if (event.target === this.cy) {
        this.girService.clearSelectedCorporateEntity();
        this.girService.clearSelectedOwnershipInfo();
        this.showSelectedOwnershipList = false;
      }
    });

    this.cy.layout({
      name: 'breadthfirst',
      roots: this.cy.nodes('[type="UPE"]').map((node: cytoscape.NodeSingular ) => node.id()),
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

  setShowOwnershipList(ownerships: Ownership[]) {
    this.showSelectedOwnershipList = true;
    this.selectedOwnerships = ownerships;
  }

  hideOwnershipList() {
    this.showSelectedOwnershipList = false;
  }

  applyEdgeStyle(event: any) {
    this.cy.edges().forEach((edge: cytoscape.EdgeSingular) => {
      edge.style({
        'curve-style': event.target.value,
      });
    });
  }

  setNodeStyleType(node: CorporateEntity): string {
    if (node.parentId) {
      switch (node.qiir_Status) {
        case '901':
          return 'POPE';
        case '902':
          return 'IPE';
        case '903':
          return ''
        default:
          return 'CE';
      }
    }

    return 'UPE';
  }
}
