import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import cytoscape, { ElementDefinition } from 'cytoscape';
import { girCytoGraphStyle } from '../../gir-graph-cyto/gir-graph-cyto-style';
import { GIRService } from '../../services/gir-graph.service';
import { Observable, Subject, take, takeUntil } from 'rxjs';
import { OwnershipEdge } from '../../models/ownership-edge.model';

@Component({
  selector: 'app-corporate-subtree',
  templateUrl: './corporate-subtree.component.html',
  styleUrl: './corporate-subtree.component.css'
})
export class CorporateSubtreeComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('subGraph', { static: false }) cyContainer!: ElementRef;
  @Output() closeOwnershipList = new EventEmitter<void>();
  cy: any;
  corporateStructure: any;
  subTreeList: ElementDefinition[][] = [];
  subTreeList$: Observable<any> | undefined;
  private readonly destroy$ = new Subject<void>();
  constructor(private girService: GIRService) { }

  ngOnInit() {
    this.subTreeList$ = this.girService.subTreeList$;
    this.subTreeList$
      ?.pipe(takeUntil(this.destroy$)) // Automatically unsubscribe
      .subscribe(data => this.subTreeList = data || []);
  }

  ngAfterViewInit() {
    this.girService.subTreeData$.pipe(take(1)).subscribe(data => {
      console.log("subTreeData received:", data);
      if (data) {
        this.girService.updateSubTreeList(data); //dont like this
        this.renderSubTree(data);
        this.girService.updateCurrentCyGraph(this.cy);
      }
    });
  }

  renderSubTree(subTreeData: any) {
    if (this.cy) {
      this.cy.destroy();
    }

    this.cy = cytoscape({
      container: this.cyContainer.nativeElement,
      pixelRatio: 3,
      elements: subTreeData,
      style: girCytoGraphStyle,
      layout: { name: 'breadthfirst', roots: [subTreeData[0].data.id] },
    });

    this.cy.on('tap', 'node', (event: any) => {
      const target = event.target;
      this.handleNodeTap(target);
    });

    this.cy.on('tap', 'edge', (event: any) => {
      const target = event.target;
      this.handleEdgeTap(target);
    });

    this.cy.on('tap', (event: any) => {
      if(event.target === this.cy)
        this.handleBackgroundTap();
    });

  }

  private handleNodeTap(node: cytoscape.NodeSingular) {
    this.closeOwnershipPanel();
    this.girService.updateSelectedCorporateEntity(node.data().entityInfo);
    this.createAndRenderSubTree(node);
  }

  private createAndRenderSubTree(node: cytoscape.NodeSingular) {
    const subTree = this.cy.elements().bfs({
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

    const nodesOnly = validSubTreeData.filter((el: ElementDefinition) =>
      el.data?.id !== undefined && !el.data?.source && !el.data?.target
    );

    if ((nodesOnly?.length ?? 0) > 2) {
      const lastSubTree = this.subTreeList[this.subTreeList.length - 1];

      if (!lastSubTree || lastSubTree[0]?.data.id !== validSubTreeData[0]?.data.id) {
        this.girService.updateSubTreeList(validSubTreeData);
      }

      this.renderSubTree(validSubTreeData); //due to a bug in cytoscape, function is called directly here
      this.girService.updateCurrentCyGraph(this.cy);
    }
  }

  private handleEdgeTap(edge: cytoscape.EdgeSingular) {
    this.closeOwnershipPanel();
    this.girService.updateSelectedOwnershipInfo({
      ownershipInfo: edge.data().ownershipInfo,
      ownedName: edge.data().ownedName,
      ownerName: edge.data().ownerName
    } as OwnershipEdge);
  }

  private handleBackgroundTap() {
    this.closeOwnershipPanel();
  }

  onWrapperClick(event: MouseEvent) {
    const wrapper = event.currentTarget as HTMLElement;
    if (event.target === wrapper) {
      this.girService.clearSelectedCorporateEntity();
      this.girService.clearSelectedOwnershipInfo();
      this.girService.clearSubTreeData();
      this.girService.clearSubTreeList();
      this.girService.clearCurrentCyGraph();
    }
  }

  private closeOwnershipPanel() {
    this.closeOwnershipList.emit();
    this.girService.clearSelectedOwnershipInfo();
  }

  backToPreviousTree() {
    this.girService.removeLastSubTree();
    this.renderSubTree(this.subTreeList[this.subTreeList.length - 1]);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    if (this.cy) {
      this.cy.destroy();
    }
  }
}

