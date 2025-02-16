import { Stylesheet } from 'cytoscape';

export const girCytoGraphStyle: Stylesheet[] = [
  // 🌟 Node Styling (Minimalist & Elegant)
  {
    selector: 'node[type="CE"]',
    style: {
      'background-color': '#b7d1e9', // Muted blue for a professional touch
      'label': 'data(label)',
      'color': '#ffffff', // White text for contrast
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '8px', // Subtler text size for a cleaner look
      'font-weight': 400, // Normal weight for better readability
      'text-outline-width': 0.75,
      'text-outline-color': '#3A6FB0', // Slight shadow effect for readability
      'width': '52.5px', // Scalable size but not too large
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0', // Subtle border for sleekness
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="POPE"]',
    style: {
      'background-color': '#f4a261', // Muted blue for a professional touch
      'label': 'data(label)',
      'color': '#ffffff', // White text for contrast
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '8px', // Subtler text size for a cleaner look
      'font-weight': 400, // Normal weight for better readability
      'text-outline-width': 0.75,
      'text-outline-color': '#3A6FB0', // Slight shadow effect for readability
      'width': '52.5px', // Scalable size but not too large
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0', // Subtle border for sleekness
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="IPE"]',
    style: {
      'background-color': '#2a9d8f', // Muted blue for a professional touch
      'label': 'data(label)',
      'color': '#ffffff', // White text for contrast
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '8px', // Subtler text size for a cleaner look
      'font-weight': 400, // Normal weight for better readability
      'text-outline-width': 0.75,
      'text-outline-color': '#3A6FB0', // Slight shadow effect for readability
      'width': '52.5px', // Scalable size but not too large
      'height': '20px',
      'shape': 'round-rectangle',
      'border-width': 1,
      'border-color': '#3A6FB0', // Subtle border for sleekness
      'transition-property': 'border-width, background-color',
      'transition-duration': 0.15
    }
  },

  {
    selector: 'node[type="UPE"]',
    style: {
      'background-color': '#e9919c', // Unique red color for root
      'color': '#ffffff',
      'label': 'data(label)',
      'text-valign': 'center',
      'text-halign': 'center',
      'font-size': '10px', // Slightly larger font for emphasis
      'width': '52.5px', // Larger size for root
      'height': '22px',
      'border-width': 1, // Thicker border for distinction
      'border-color': '#A01725',
      'shape': 'roundrectangle',
      'text-outline-width': 1.25,
      'text-outline-color': '#A01725'
    }
  },


  // 🎯 Node Hover Effect (Subtle Highlight)
/*  {
    selector: 'node:hover',
    style: {
      'background-color': '#3A6FB0', // Slightly darker blue on hover
      'border-color': '#315F90',
      'border-width': 2 // Subtle thickness change on hover
    }
  },*/

  // 🔗 Edge Styling (Slim & Clean)
  {
    selector: 'edge',
    style: {
      'width': 1, // Slim edge lines for a modern look
      'curve-style': 'bezier', // Smooth flow for edges
      'line-color': '#A0A0A0', // Soft gray for professional contrast
      'target-arrow-shape': 'vee',
      'target-arrow-color': '#A0A0A0',
      'label': 'data(label)', // Display relationship labels
      'font-size': '7px', // Subtle font size for readability
      'font-weight': 100,
      'text-margin-y': -6, // Moves label slightly above the edge
      'opacity': 0.85, // Slight transparency for elegance\
      'text-rotation': 'autorotate',
    }
  },

  // ✨ Edge Hover Effect (Refined Contrast)
/*  {
    selector: 'edge:hover',
    style: {
      'line-color': '#6A6A6A', // Darker gray on hover
      'target-arrow-color': '#6A6A6A',
      'width': 1.5 // Slight thickness increase for focus
    }
  }*/
];
