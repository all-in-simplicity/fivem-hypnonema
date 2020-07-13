export interface RenderTargetSettings {
  modelName: string;
  renderTargetName?: string;
}

export interface DuiBrowserSettings {
  globalVolume: number;
  soundAttenuation: number;
  soundMinDistance: number;
  soundMaxDistance: number;
  is3DAudioEnabled: boolean;
}

export interface PositionalSettings {
  positionX: number;
  positionY: number;
  positionZ: number;
  rotationX: number;
  rotationY: number;
  rotationZ: number;
  scaleX: number;
  scaleY: number;
  scaleZ: number;
}

export interface ScreenModel {
  id: number;
  name: string;
  targetSettings: RenderTargetSettings;
  browserSettings: DuiBrowserSettings;
  positionalSettings: PositionalSettings;
  alwaysOn: boolean;
  is3DRendered: boolean;
}

export interface ScreenStatus {
  screenName: string;
  isPaused: boolean;
  currentTime: number;
  duration: number;
  currentSource: string;
  ended: boolean;
  repeat: boolean;
}
