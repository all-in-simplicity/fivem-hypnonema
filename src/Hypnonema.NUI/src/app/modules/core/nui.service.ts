import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NuiService {
  constructor(private http: HttpClient) {
  }

  public repeatVideo(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnToggleRepeat`, {
      screenName
    }).subscribe(() => {
    }, error => console.log(error.toString()));
  }

  public editScreen(screenName: string, screenId: number, is3DRendered: boolean, alwaysOn: boolean, modelName: string,
                    renderTargetName: string, globalVolume: number, soundAttenuation: number, soundMinDistance: number,
                    soundMaxDistance: number, positionX: number, positionY: number, positionZ: number, rotationX: number,
                    rotationY: number, rotationZ: number, scaleX: number, scaleY: number, scaleZ: number, is3DAudioEnabled: boolean) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnEditScreen`, {
      id: screenId,
      is3DRendered,
      screenName,
      alwaysOn,
      modelName,
      renderTargetName,
      globalVolume,
      soundAttenuation,
      soundMinDistance,
      soundMaxDistance,
      positionX,
      positionY,
      positionZ,
      rotationX,
      rotationY,
      rotationZ,
      scaleX,
      scaleY,
      scaleZ,
      is3DAudioEnabled,
    }).subscribe(() => {
    }, error => console.log(error));
  }

  public closeScreen(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnCloseScreen`, {screenName})
      .subscribe(() => {
      }, error => console.log(JSON.stringify(error)));
  }

  public stopVideo(screenName) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnStopVideo`, {
      screenName
    })
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public hideNUI() {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnHideNUI`, {})
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public resumeVideo(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnResume`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public pauseVideo(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnPause`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public requestDuiState(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnRequestState`, {})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public deleteScreen(screenName: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnDeleteScreen`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public createScreen(screenName: string, alwaysOn: boolean, globalVolume: number, soundAttenuation: number,
                      soundMinDistance: number, soundMaxDistance: number, is3DRendered: boolean, is3DAudioEnabled: boolean, modelName?: string,
                      renderTargetName?: string, positionX?: number, positionY?: number, positionZ?: number,
                      rotationX?: number, rotationY?: number, rotationZ?: number, scaleX?: number, scaleY?: number, scaleZ?: number,
                     ) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnCreateScreen`, {
      screenName,
      alwaysOn,
      modelName,
      renderTargetName,
      globalVolume,
      soundAttenuation,
      soundMinDistance,
      soundMaxDistance,
      is3DRendered,
      positionX,
      positionY,
      positionZ,
      rotationX,
      rotationY,
      rotationZ,
      scaleX,
      scaleY,
      scaleZ,
      is3DAudioEnabled,
    })
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public playVideo(screenName: string, videoUrl: string) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnPlay`, {
      videoUrl, screen: screenName
    }).subscribe(() => {
    }, error => {
      console.log(error);
    });
  }

  public setVideoTime(screenName: string, time: number) {
    this.http.post(`http://${window.location.hostname}/Hypnonema.OnSeek`, {screenName, time})
      .subscribe(() => {
      }, error => console.log(error));
  }
}
