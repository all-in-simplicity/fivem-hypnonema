export default interface Screen {
  id: number;
  name: string;
  alwaysOn: boolean;
  is3DRendered: boolean;
  browserSettings?: {
    globalVolume: number;
    soundAttenuation: number;
    soundMaxDistance: number;
    soundMinDistance: number;
    is3DAudioEnabled: boolean;
  };
  positionalSettings?: {
    positionX?: number;
    positionY?: number;
    positionZ?: number;
    rotationX?: number;
    rotationY?: number;
    rotationZ?: number;
    scaleX?: number;
    scaleY?: number;
    scaleZ?: number;
  };
  targetSettings?: {
    modelName?: string;
    renderTargetName?: string;
  };
}
