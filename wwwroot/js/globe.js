// Placeholder for globe visualization
class GlobeVisualization {
    constructor(canvasId) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) return;
        
        this.ctx = this.canvas.getContext('2d');
        this.rotation = 0;
    }

    update() {
        this.rotation += 0.5;
    }
}

window.GlobeVisualization = GlobeVisualization;
