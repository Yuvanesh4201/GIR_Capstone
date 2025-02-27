import { AfterViewInit, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import cytoscape from 'cytoscape';
import { girCytoGraphStyle } from '../../gir-graph-cyto/gir-graph-cyto-style';
import { BehaviorSubject, Observable } from 'rxjs';
import { GIRService } from '../../services/gir-graph.service';

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
    this.girService.subTreeData$.subscribe(data => {
      if (data)
        this.renderSubTree(data);
    })
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
  }

  ngOnDestroy() {
    if (this.cy) {
      this.cy.destroy(); // Cleanup Cytoscape instance on component destroy
    }
  }
}

