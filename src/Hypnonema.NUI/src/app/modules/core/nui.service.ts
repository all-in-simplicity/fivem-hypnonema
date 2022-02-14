import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ScreenModel} from '../../screen-model';
import {Store} from '@ngxs/store';
import {UpdateState} from '../../app-state';

@Injectable({
  providedIn: 'root'
})
export class NuiService {

  private resourceName = window.location.hostname;
  private baseUrl = `https://${this.resourceName}`;

  constructor(private http: HttpClient, private store: Store) {
  }

  public getScreenList() {
    this.http.post(`${this.baseUrl}/getScreenList`, {}).subscribe(() => {
    }, error => console.log(error.toString()));
  }

  public repeatVideo(screenName: string) {
    this.http.post(`${this.baseUrl}/repeat`, {
      screenName
    }).subscribe(() => {
    }, error => console.log(error.toString()));
  }

  public editScreen(name: string, screenId: number, is3DRendered: boolean, alwaysOn: boolean, modelName: string,
                    renderTargetName: string, globalVolume: number, soundAttenuation: number, soundMinDistance: number,
                    soundMaxDistance: number, positionX: number, positionY: number, positionZ: number, rotationX: number,
                    rotationY: number, rotationZ: number, scaleX: number, scaleY: number, scaleZ: number, is3DAudioEnabled: boolean) {
    const payload: ScreenModel = {
      id: screenId,
      name,
      browserSettings: {
        globalVolume,
        soundAttenuation,
        soundMinDistance,
        soundMaxDistance,
        is3DAudioEnabled,
      },
      positionalSettings: {
        positionX,
        positionY,
        positionZ,
        rotationX,
        rotationY,
        rotationZ,
        scaleX,
        scaleY,
        scaleZ,
      },
      targetSettings: {
        modelName,
        renderTargetName,
      },
      is3DRendered,
      alwaysOn,
    };
    this.http.post(`${this.baseUrl}/editScreen`, {
      payload: JSON.stringify(payload)
    }).subscribe(() => {
    }, error => console.log(error));
  }

  public closeScreen(screenName: string) {
    this.http.post(`${this.baseUrl}/stop`, {screenName})
      .subscribe(() => {
      }, error => console.log(JSON.stringify(error)));
  }

  public stopVideo(screenName: string) {
    this.http.post(`${this.baseUrl}/stop`, {
      screenName
    })
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public hideNUI() {
    this.http.post(`${this.baseUrl}/hideUI`, {})
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public resumeVideo(screenName: string) {
    this.http.post(`${this.baseUrl}/resume`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public pauseVideo(screenName: string) {
    this.http.post(`${this.baseUrl}/pause`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public requestDuiState(screenName: string) {
    this.http.post(`${this.baseUrl}/requestState`, {})
      .subscribe((resp: string) => {
        const status = JSON.parse(resp);
        this.store.dispatch(new UpdateState(status));
      }, error => console.log(error));
  }

  public deleteScreen(screenName: string) {
    this.http.post(`${this.baseUrl}/deleteScreen`, {screenName})
      .subscribe(() => {
      }, error => console.log(error));
  }

  public createScreen(name: string, alwaysOn: boolean, globalVolume: number, soundAttenuation: number, soundMinDistance: number,
                      soundMaxDistance: number, is3DRendered: boolean, is3DAudioEnabled: boolean, modelName?: string,
                      renderTargetName?: string, positionX?: number, positionY?: number, positionZ?: number, rotationX?: number,
                      rotationY?: number, rotationZ?: number, scaleX?: number, scaleY?: number, scaleZ?: number) {
    const screen = {
      name,
      alwaysOn,
      browserSettings: {
        globalVolume,
        soundAttenuation,
        soundMinDistance,
        soundMaxDistance,
        is3DAudioEnabled,
      },
      positionalSettings: {
        positionX,
        positionY,
        positionZ,
        rotationX,
        rotationY,
        rotationZ,
        scaleX,
        scaleY,
        scaleZ,
      },
      targetSettings: {
        modelName,
        renderTargetName,
      },
      is3DRendered,
    };

    this.http.post(`${this.baseUrl}/createScreen`, {
      payload: JSON.stringify(screen)
    })
      .subscribe(() => {
      }, error => {
        console.log(error);
      });
  }

  public playVideo(screen: ScreenModel, videoUrl: string) {
    this.http.post(`${this.baseUrl}/play`, {screen: JSON.stringify(screen), videoUrl}).subscribe(() => {
    }, error => {
      console.log(error);
    });
  }

  public setVideoTime(screenName: string, time: number) {
    this.http.post(`${this.baseUrl}/seek`, {screenName, time})
      .subscribe(() => {
      }, error => console.log(error));
  }
}
