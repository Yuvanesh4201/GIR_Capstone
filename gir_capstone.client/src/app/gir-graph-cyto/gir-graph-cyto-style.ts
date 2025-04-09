import { Stylesheet } from 'cytoscape';

export const girCytoGraphStyle: Stylesheet[] = [
  {
    selector: 'node',
    style: {
      'background-color': '#b7d1e9',
      'border-width': 1,
      'border-color': '#3A6FB0',
      'width': '52.5px',
      'height': '20px'
    }
  },

  {
    selector: 'node[type="CE"]',
    style: {
      'background-color': '#b7d1e9',
      'label': 'data(label)',
      'color': '#ffffff',
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '10px',
      'font-weight': 400,
      'text-outline-width': 1.5,
      'text-outline-color': '#3A6FB0',
      'width': '52.5px',
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0',
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="POPE"]',
    style: {
      'background-color': '#f4a261',
      'label': 'data(label)',
      'color': '#ffffff',
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '10px',
      'font-weight': 400,
      'text-outline-width': 1.5,
      'text-outline-color': '#3A6FB0',
      'width': '52.5px',
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0',
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="IPE"]',
    style: {
      'background-color': '#2a9d8f',
      'label': 'data(label)',
      'color': '#ffffff',
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '10px',
      'font-weight': 400,
      'text-outline-width': 1.5,
      'text-outline-color': '#3A6FB0',
      'width': '52.5px',
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0',
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="UPE"]',
    style: {
      'background-color': '#e9919c',
      'color': '#ffffff',
      'label': 'data(label)',
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '12px',
      'width': '52.5px',
      'height': '22px',
      'border-width': 1,
      'border-color': '#A01725',
      'shape': 'roundrectangle',
      'text-outline-width': 1.25,
      'text-outline-color': '#A01725'
    }
  },

  {
    selector: 'node:selected',
    style: {
      'border-width': 3,
      'border-color': 'rgba(0, 0, 0, 100)',
      'width': '60',
      'height': '30',
    }
  },

  {
    selector: 'edge',
    style: {
      'width': 1.25,
      'curve-style': 'bezier',
      'line-color': '#8e8e8e',
      'target-arrow-shape': 'vee',
      'target-arrow-color': '#A0A0A0',
      'label': 'data(label)',
      'font-size': '11px',
      'font-weight': 100,
      'text-margin-y': -10,
      'opacity': 0.85,
      'text-rotation': 'autorotate',
    }
  },

  {
    selector: 'edge:selected',
    style: {
      'line-color': '#FFA500',
      'width': 2.5,
      'target-arrow-color': '#FFA500',
      'source-arrow-color': '#FFA500',
      'opacity': 1,
    }
  }
];
