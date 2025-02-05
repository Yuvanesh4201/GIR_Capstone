import { AfterViewInit, Component, ElementRef, ViewChild } from '@angular/core';
import cytoscape from 'cytoscape';

@Component({
  selector: 'app-gir-graph-cyto',
  templateUrl: './gir-graph-cyto.component.html',
  styleUrls: ['./gir-graph-cyto.component.css']
})
export class GirGraphCytoComponent implements AfterViewInit {
  @ViewChild('graph', { static: false }) cyContainer!: ElementRef;

  ngAfterViewInit(): void {
    
    const cy = cytoscape({
      container: this.cyContainer.nativeElement,
      elements: [
        { data: { id: 'A', label: 'Node A' } },
        { data: { id: 'B', label: 'Node B' } },
        { data: { id: 'C', label: 'Node C' } },
        { data: { source: 'A', target: 'B' } },
        { data: { source: 'A', target: 'C' } }
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

    cy.resize();
    cy.fit();
    }
 
}
