/**
 * Tailwind Components - Custom JavaScript functionality for Tailwind components
 */

// Define interfaces for HTML elements with dataset properties
interface TriggerElement extends HTMLElement {
  dataset: {
    twTarget?: string;
    twToggle?: string;
  }
}

interface IconElement extends HTMLElement {
  dataset: {
    twIcon?: string;
    twIconUp?: string;
    twIconDown?: string;
  }
}

// Collapse functionality
class Collapse {
  element: HTMLElement;
  isOpen: boolean;
  
  constructor(element: HTMLElement) {
    this.element = element;
    this.isOpen = !element.classList.contains('hidden');
  }
  
  toggle(): void {
    this.isOpen ? this.hide() : this.show();
  }
  
  show(): void {
    this.element.classList.remove('hidden');
    this.isOpen = true;
    
    // Dispatch event for other components to react
    this.element.dispatchEvent(new CustomEvent('shown.tw.collapse'));
  }
  
  hide(): void {
    this.element.classList.add('hidden');
    this.isOpen = false;
    
    // Dispatch event for other components to react
    this.element.dispatchEvent(new CustomEvent('hidden.tw.collapse'));
  }
  
  // Static method to get or create instance
  static getOrCreateInstance(element: HTMLElement): Collapse {
    return new Collapse(element);
  }
}

// Extend Window interface
interface Window {
  tailwindComponents: {
    Collapse: typeof Collapse;
  }
}

// Initialize global object
window.tailwindComponents = {
  Collapse
};

// Handle collapse toggle click
function handleCollapseToggle(e: Event) {
  e.preventDefault();
  const triggerEl = e.currentTarget as TriggerElement;
  const targetId = triggerEl.dataset.twTarget;
  console.log("Clicked trigger, target:", targetId);
  
  if (!targetId) return;
  
  const target = document.getElementById(targetId.replace('#', ''));
  if (!target) {
    console.error("Target element not found:", targetId);
    return;
  }
  
  console.log("Found target:", target);
  const collapse = Collapse.getOrCreateInstance(target);
  collapse.toggle();
  
  // Toggle icon if present
  const icon = triggerEl.querySelector('i');
  if (icon && icon instanceof HTMLElement) {
    console.log("Found icon:", icon);
    icon.classList.toggle('bi-chevron-up');
    icon.classList.toggle('bi-chevron-down');
  }
}

// Initialize all collapse elements
document.addEventListener('DOMContentLoaded', () => {
  console.log("Tailwind components initialized");
  
  // Set up collapse triggers
  document.querySelectorAll('[data-tw-toggle="collapse"]').forEach(trigger => {
    console.log("Found collapse trigger:", trigger);
    
    // Remove any existing event listeners to prevent duplicates
    trigger.removeEventListener('click', handleCollapseToggle);
    
    // Add the event listener
    trigger.addEventListener('click', handleCollapseToggle);
  });
}); 