import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import cytoscape, { ElementDefinition } from 'cytoscape';
import { girCytoGraphStyle } from '../../gir-graph-cyto/gir-graph-cyto-style';
import { GIRService } from '../../services/gir-graph.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-corporate-subtree',
  templateUrl: './corporate-subtree.component.html',
  styleUrl: './corporate-subtree.component.css'
})
export class CorporateSubtreeComponent implements AfterViewInit {
  @ViewChild('subGraph', { static: false }) cyContainer!: ElementRef;
  cy: any;
  corporateStructure: any;
  constructor(private girService: GIRService) {}

  ngAfterViewInit() {
    this.girService.subTreeData$.pipe(take(1)).subscribe(data => {
      console.log("subTreeData received:", data);

      if(data)
        this.renderSubTree(data);
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
      const clickedNode = event.target;
      //this.showSelectedOwnershipList = false;
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

      this.renderSubTree(validSubTreeData);
      
      clickedNode.unselect();
    });
  }

  onWrapperClick(event: MouseEvent) {
    const wrapper = event.currentTarget as HTMLElement;
    if (event.target === wrapper) {
      this.girService.clearSelectedCorporateEntity();
      this.girService.clearSelectedOwnershipInfo();
      this.girService.clearSubTreeData();
      console.log('Clicked directly on the wrapper!');
    } else {
      console.log('Clicked on a child element, ignoring wrapper click.');
    }
  }


  ngOnDestroy() {
    if (this.cy) {
      this.cy.destroy(); // Cleanup Cytoscape instance on component destroy
    }
  }
}

