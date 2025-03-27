import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import cytoscape, { ElementDefinition } from 'cytoscape';
import { CorporateEntity, Ownership } from '../models/company-structure.model';
import { GIRService } from '../services/gir-graph.service';
import { girCytoGraphStyle } from './gir-graph-cyto-style';
import { OwnershipEdge } from '../models/ownership-edge.model';
import { Observable, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-gir-graph-cyto',
  templateUrl: './gir-graph-cyto.component.html',
  styleUrls: ['./gir-graph-cyto.component.css']
})
export class GirGraphCytoComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('graph', { static: false }) cyContainer!: ElementRef;
  corporateStructure: CorporateEntity[] = [];
  corporateList: string[] = [];
  jurisdictionList: string[] = [];
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
  private destroy$ = new Subject<void>();
  currentCy: any; 

  constructor(private route: ActivatedRoute, private girService: GIRService) { }

  ngOnInit() {
    this.corporateId = this.route.snapshot.queryParamMap.get('id');
    this.mneName = this.route.snapshot.queryParamMap.get('name');
    this.xmlParse = this.route.snapshot.queryParamMap.get('xmlParse');

    if (this.corporateId) {
      this.girService.getCorporateStructure(this.corporateId, this.xmlParse)
        .pipe(takeUntil(this.destroy$)) // 🚀 Ensure automatic cleanup
        .subscribe(
          (data) => {
            if (data) {
              console.log('Corporate ID Works:', this.corporateId);
              this.corporateStructure = data;
              this.corporateList = data.map(corporate => corporate.name);
              this.jurisdictionList = data.map(corporate => corporate.jurisdiction);
              this.renderGraph(this.corporateStructure);
              this.girService.updateMainCyGraph(this.cy);
              this.girService.updateCurrentCyGraph(this.cy);
            }
          },
          (error) => {
            console.error("Error fetching corporates:", error);
          }
        );
    }
    else {
      this.route.queryParams.subscribe(params => {
        const parsedArray = params['data'] ? JSON.parse(params['data']) : [];
        this.corporateStructure = parsedArray;
        this.corporateList = this.corporateStructure.map(corporate => corporate.name);
      });
    }

    this.corporateEntityInfo$ = this.girService.selectedCorporateEntity$;
    this.ownershipInfo$ = this.girService.selectedOwnershipInfo$;
    this.girService.currentCyGraph$.subscribe(data => this.currentCy = data);
  }

  ngAfterViewInit(): void {
    if (!this.corporateId && this.corporateStructure) {
      this.renderGraph(this.corporateStructure);
      this.girService.updateMainCyGraph(this.cy);
      this.girService.updateCurrentCyGraph(this.cy);
    }
  }

  renderGraph(corporateStructure: CorporateEntity[]): void {

    const existingNodeIds = new Set(corporateStructure.map(node => node.id));

    this.cy = cytoscape({
      container: this.cyContainer.nativeElement,
      pixelRatio: 3,
      elements: [
        ...corporateStructure.map(node => ({
          data: {
            id: node.id,
            label: node.name,
            entityInfo: node,
            type: this.setNodeStyleType(node),
          },
          grabbable: false,
        })),
        ...corporateStructure.flatMap(corp =>
          corp.ownerships
            .filter(edge => existingNodeIds.has(edge.ownerEntityId)) // ✅ Check if source exists
            .map(edge => ({
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

    this.cy.on('tap', 'node', (event: any) => {
      const target = event.target;
      this.handleNodeTap(target);
    });

    this.cy.on('tap', 'edge', (event: any) => {
      const target = event.target;
      this.handleEdgeTap(target);
    });

    this.cy.on('tap', (event: any) => {
      if (event.target === this.cy)
        this.handleBackgroundTap();
    });

    this.cy.layout({
      name: 'breadthfirst',
      roots: this.cy.nodes('[type="UPE"]').map((node: cytoscape.NodeSingular ) => node.id()),
      directed: true,
      spacingFactor: 1.5,
      padding: 50,               // Padding around layout
      avoidOverlap: true,        // Prevent node collisions
      animate: true,
      animationDuration: 500,
      orientation: 'horizontal'    // or 'horizontal' for left-to-right flow
    }).run();

    this.cy.zoom(this.zoom);
    this.cy.centre();
  }


  handleNodeTap(node: cytoscape.NodeSingular) {
    this.girService.clearSelectedOwnershipInfo();
    this.showSelectedOwnershipList = false;
    this.girService.updateSelectedCorporateEntity(node.data().entityInfo);
    this.createAndRenderSubTree(node);
  }

  createAndRenderSubTree(node: cytoscape.NodeSingular) {
    const subTree = this.cy.elements().bfs(
      {
        roots: node,
        directed: true,
      });

    subTree.path.forEach((n: cytoscape.NodeSingular | cytoscape.EdgeSingular) => {
      n.unselect();
    });

    const validSubTreeData: ElementDefinition[] = subTree.path.map((n: cytoscape.NodeSingular | cytoscape.EdgeSingular) => ({
      data: { ...n.data() },
      grabbable: false,
    }));

    // Ensure correct typing & filtering
    this.nodesOnly = validSubTreeData.filter((el: ElementDefinition) =>
      el.data?.id !== undefined && !el.data?.source && !el.data?.target
    );

    // Update subtree only if more than 2 nodes exist
    if ((this.nodesOnly?.length ?? 0) > 2) {
      this.girService.updateSubTreeData(validSubTreeData);
    }
  }

  handleEdgeTap(edge: cytoscape.EdgeSingular) {
    this.girService.clearSelectedCorporateEntity();
    this.showSelectedOwnershipList = false;
    this.girService.updateSelectedOwnershipInfo({
      ownershipInfo: edge.data().ownershipInfo,
      ownedName: edge.data().ownedName,
      ownerName: edge.data().ownerName
    } as OwnershipEdge);
  }

  handleBackgroundTap() {
    this.girService.clearSelectedCorporateEntity();
    this.girService.clearSelectedOwnershipInfo();
    this.girService.dropdownnListClose();
    this.showSelectedOwnershipList = false;
  }

  resetLayout() {
    this.renderGraph(this.corporateStructure);
    this.girService.updateCurrentCyGraph(this.cy);
    this.girService.currentCyGraph$.subscribe(graph => {
      graph.reset();
      graph.zoom(this.zoom);
      graph.centre();
    });

  }

  fitView() {
    this.girService.currentCyGraph$.subscribe(graph => {
      graph.fit();
    });
  }

  setShowOwnershipList(ownerships: Ownership[]) {
    this.showSelectedOwnershipList = true;
    this.selectedOwnerships = ownerships;
  }

  hideOwnershipList() {
    this.showSelectedOwnershipList = false;
  }

  applyEdgeStyle(event: any) {
    this.girService.currentCyGraph$.subscribe(graph => {
      graph.edges().forEach((edge: cytoscape.EdgeSingular) => {
        edge.style({
          'curve-style': event.target.value,
        });
      });
    })
  }

  //this will change later (use decoded values)
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

  exportToJpg() {
    this.girService.exportGraphAsImage(this.mneName, this.currentCy);
  }

  exportToPdf() {
    this.girService.exportGraphAsPdf(this.mneName, this.currentCy);
  }

  searchedCorporateEntity(item: string) {
    const matchingNode = this.cy.nodes(`[label= "${item}"]`);

    if (matchingNode.length > 0) {
      this.cy.elements().unselect(); // Deselect all elements first
      this.cy.center(matchingNode); // Move the canvas to the selected node

      this.cy.animate({
        center: { eles: matchingNode }, // Move camera to node
        zoom: 2, // Adjust zoom level
        duration: 1000 // Smooth animation over 1 second
      });
    } else {
      console.log("No node found with label:", item);
    }
  }

  sort(item:any) {

    let sortedStructure: CorporateEntity[] = [];

  if (item.type === 'jurisdiction') {
    sortedStructure = this.corporateStructure.filter(
      corporate => corporate.jurisdiction === item.value
    );
  } else if (item.type === 'percentage') {
    const corporateIds = new Set<string>;
    const filteredOwnerships: Ownership[] = [];
    this.corporateStructure.forEach(corp => corp.ownerships.forEach(
      ownership => {
        const percent = ownership.ownershipPercentage;
        if ((item.value === 'Associate' && percent >= 20 && percent < 50) ||
          (item.value === 'Subsidary' && percent >= 50)) {

          filteredOwnerships.push(ownership);
          corporateIds.add(corp.id);
          corporateIds.add(ownership.ownerEntityId);

        }
      }
    ));

    const filteredNodes = this.corporateStructure.filter(corp =>
      corporateIds.has(corp.id)
    );

    sortedStructure = filteredNodes.map(corp => ({
      ...corp,
      ownerships: corp.ownerships.filter(o => filteredOwnerships.includes(o))
    }));

    }
    this.renderGraph(sortedStructure);
    this.girService.updateCurrentCyGraph(this.cy);
    
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.cy) this.cy.destroy();
  }

}
