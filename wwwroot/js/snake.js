document.addEventListener("DOMContentLoaded", () => {
    const container = document.querySelector('.snake-container');
    if (!container) return;

    const snakeSize = 20;
    const dotCount = 40;
    const speed = 2;
    const spacing = 6;

    const containerWidth = container.offsetWidth;
    const containerHeight = container.offsetHeight;

    // Dynamically create snake dots
    const snakeDots = [];
    for (let i = 0; i < dotCount; i++) {
        const dot = document.createElement('div');
        dot.classList.add('snake-dot');
        container.appendChild(dot);
        snakeDots.push(dot);
    }

    let angle = Math.random() * Math.PI * 2;
    let head = { x: containerWidth / 2, y: containerHeight / 2 };

    let snakePositions = Array.from({ length: dotCount }, (_, i) => ({
        x: head.x - i * spacing,
        y: head.y
    }));

    function getRandomBrightColor() {
        return {
            r: Math.floor(Math.random() * 156) + 100,
            g: Math.floor(Math.random() * 156) + 100,
            b: Math.floor(Math.random() * 156) + 100
        };
    }

    function lerp(start, end, t) {
        return start + (end - start) * t;
    }

    function interpolateColor(c1, c2, t) {
        return `rgb(${Math.round(lerp(c1.r, c2.r, t))}, ${Math.round(lerp(c1.g, c2.g, t))}, ${Math.round(lerp(c1.b, c2.b, t))})`;
    }

    let currentColor = getRandomBrightColor();
    let nextColor = getRandomBrightColor();
    let transitionStep = 0;
    const transitionDuration = 2000;
    const stepInterval = 50;
    const delayPerDot = 100;

    const dotTransitions = Array.from({ length: snakeDots.length }, (_, i) => ({
        startTime: i * delayPerDot,
        progress: 0
    }));

    setInterval(() => {
        transitionStep++;

        dotTransitions.forEach((dot, i) => {
            const dotEl = snakeDots[i];
            const localStep = transitionStep * stepInterval - dot.startTime;

            if (localStep >= 0 && localStep <= transitionDuration) {
                const t = localStep / transitionDuration;
                dotEl.style.backgroundColor = interpolateColor(currentColor, nextColor, t);
            } else if (localStep > transitionDuration) {
                dotEl.style.backgroundColor = `rgb(${nextColor.r}, ${nextColor.g}, ${nextColor.b})`;
            }
        });

        if (transitionStep * stepInterval >= transitionDuration + delayPerDot * snakeDots.length) {
            currentColor = nextColor;
            nextColor = getRandomBrightColor();
            transitionStep = 0;
        }
    }, stepInterval);

    function moveSnake() {
        angle += (Math.random() - 0.5) * 0.3;

        let nextX = head.x + Math.cos(angle) * speed;
        let nextY = head.y + Math.sin(angle) * speed;

        if (nextX <= 0 || nextX >= containerWidth - snakeSize) angle = Math.PI - angle;
        if (nextY <= 0 || nextY >= containerHeight - snakeSize) angle = -angle;

        head = { x: nextX, y: nextY };
        snakePositions.unshift({ ...head });
        snakePositions = snakePositions.slice(0, dotCount);

        snakeDots.forEach((dot, i) => {
            if (snakePositions[i]) {
                dot.style.left = `${snakePositions[i].x}px`;
                dot.style.top = `${snakePositions[i].y}px`;
            }
        });
    }

    setInterval(moveSnake, 20);
});