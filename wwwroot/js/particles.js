// Particle Effects System
// Creates weather particle animations

class ParticleSystem {
    constructor() {
        this.particles = [];
        this.canvas = document.getElementById('globeCanvas');
        if (!this.canvas) return;
        
        this.ctx = this.canvas.getContext('2d');
        this.canvas.width = 300;
        this.canvas.height = 300;
        this.animationId = null;
        this.drawGlobe();
    }

    drawGlobe() {
        const ctx = this.ctx;
        const centerX = this.canvas.width / 2;
        const centerY = this.canvas.height / 2;
        const radius = 120;

        // Clear canvas
        ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);

        // Draw outer glow
        const glowGradient = ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, radius + 20);
        glowGradient.addColorStop(0, 'rgba(0, 212, 255, 0.3)');
        glowGradient.addColorStop(1, 'rgba(0, 212, 255, 0)');
        ctx.fillStyle = glowGradient;
        ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

        // Draw globe
        const gradient = ctx.createRadialGradient(centerX, centerY, 0, centerX, centerY, radius);
        gradient.addColorStop(0, 'rgba(0, 212, 255, 0.1)');
        gradient.addColorStop(0.5, 'rgba(0, 212, 255, 0.05)');
        gradient.addColorStop(1, 'rgba(0, 212, 255, 0.2)');

        ctx.fillStyle = gradient;
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, 0, Math.PI * 2);
        ctx.fill();

        // Draw grid lines
        ctx.strokeStyle = 'rgba(0, 212, 255, 0.3)';
        ctx.lineWidth = 1;

        // Latitude lines
        for (let i = 0; i <= 4; i++) {
            const y = centerY - radius + (radius * 2 * i) / 4;
            ctx.beginPath();
            ctx.arc(centerX, y, Math.sqrt(Math.pow(radius, 2) - Math.pow(y - centerY, 2)), 0, Math.PI * 2);
            ctx.stroke();
        }

        // Longitude lines
        for (let i = 0; i < 4; i++) {
            ctx.beginPath();
            ctx.arc(centerX, centerY, radius, (Math.PI * i) / 2, (Math.PI * (i + 1)) / 2);
            ctx.stroke();
        }

        // Draw border
        ctx.strokeStyle = 'rgba(0, 212, 255, 0.6)';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.arc(centerX, centerY, radius, 0, Math.PI * 2);
        ctx.stroke();

        // Schedule next frame
        this.animationId = requestAnimationFrame(() => this.drawGlobe());
    }

    addRainParticles(count = 20) {
        for (let i = 0; i < count; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height - this.canvas.height,
                speed: 2 + Math.random() * 3,
                size: 1 + Math.random() * 2
            });
        }
    }

    addSnowParticles(count = 15) {
        for (let i = 0; i < count; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height - this.canvas.height,
                speed: 0.5 + Math.random(),
                size: 2 + Math.random() * 3,
                wobble: Math.random() * 2
            });
        }
    }

    destroy() {
        if (this.animationId) {
            cancelAnimationFrame(this.animationId);
        }
    }
}

// Initialize particle system when page loads
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.particleSystem = new ParticleSystem();
    });
} else {
    window.particleSystem = new ParticleSystem();
}
