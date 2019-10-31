function between(value, min, max) {
    return value >= min && value <= max;
}

export default class Audio3D {
    constructor() {
        this.context = new (window.AudioContext || window.webkitAudioContext)();
        this.panner = this.context.createPanner();
        this.listener = this.context.listener;
        this.compressor = this.context.createDynamicsCompressor();
    }

    initialized = false;

    init(pannerOpts, source) {
        this.initialized = true;


        this.panner.panningModel = pannerOpts.panningModel;
        this.panner.distanceModel = pannerOpts.distanceModel;
        this.panner.refDistance = pannerOpts.refDistance;
        this.panner.maxDistance = pannerOpts.maxDistance;
        this.panner.rolloffFactor = pannerOpts.rolloffFactor;
        this.panner.coneInnerAngle = pannerOpts.coneInnerAngle;
        this.panner.coneOuterAngle = pannerOpts.coneOuterAngle;
        this.panner.coneOuterGain = pannerOpts.coneOuterGain;

        this.source = this.context.createMediaElementSource(source);

        this.source.connect(this.panner);
        this.panner.connect(this.compressor);
        this.compressor.connect(this.context.destination);
    }

    onTick(listenerObj, pannerObj) {
        if (!this.initialized) return;

        const time = this.context.currentTime;
        const positionX = Math.floor(pannerObj.positionX * 4);
        const positionY = Math.floor(pannerObj.positionY * 4);
        const positionZ = Math.floor(pannerObj.positionZ * 4);
        if (between(positionX, -1.40130e-45, 1.40130e-45)) {
            this.panner.positionX.linearRampToValueAtTime(positionX, time + 0.05)
        } else {
            this.panner.positionX.exponentialRampToValueAtTime(positionX, time + 0.05);
        }

        if (between(positionY, -1.40130e-45, 1.40130e-45)) {
            this.panner.positionY.linearRampToValueAtTime(positionY, time + 0.05);
        } else {
            this.panner.positionY.exponentialRampToValueAtTime(positionY, time + 0.05);
        }

        if (between(positionZ, -1.40130e-45, 1.40130e-45)) {
            this.panner.positionZ.linearRampToValueAtTime(positionZ, time + 0.05);
        } else {
            this.panner.positionZ.exponentialRampToValueAtTime(positionZ, time + 0.05);
        }

        this.panner.orientationX.setValueAtTime(Math.floor(pannerObj.orientationX), time);
        this.panner.orientationY.setValueAtTime(Math.floor(pannerObj.orientationY), time);
        this.panner.orientationZ.setValueAtTime(Math.floor(pannerObj.orientationZ), time);

        this.listener.forwardX.setValueAtTime(Math.floor(listenerObj.forwardX), time);
        this.listener.forwardY.setValueAtTime(Math.floor(listenerObj.forwardY), time);
        this.listener.forwardZ.setValueAtTime(Math.floor(listenerObj.forwardZ), time);

        this.listener.upX.setValueAtTime(Math.floor(listenerObj.upX), time);
        this.listener.upY.setValueAtTime(Math.floor(listenerObj.upY), time);
        this.listener.upZ.setValueAtTime(Math.floor(listenerObj.upZ), time);

    }
}