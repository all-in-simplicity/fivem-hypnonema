import {Component, HostListener, OnInit} from '@angular/core';
import {environment} from '../environments/environment';
import {Router} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {Store} from '@ngxs/store';
import {ClearControlledScreen, CreateScreen, DeleteScreen, SetScreens, UpdateScreen, UpdateState} from './app-state';
import {ScreenModel, ScreenStatus} from './screen-model';
import {MatSnackBar} from '@angular/material/snack-bar';
import {NuiService} from './modules/core/nui.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
  appVisible = !environment.production;

  constructor(private router: Router, private http: HttpClient, private store: Store, private snackBar: MatSnackBar,
              private nuiService: NuiService) {
    if (!environment.production) {

      let screens: ScreenModel[];
      let screenStatus: ScreenStatus[];

      const testScreen = {
        name: 'testScreen',
        id: 1,
        is3DRendered: false,
        alwaysOn: false,
        targetSettings: {
          modelName: 'blabla',
          renderTargetName: 'tvscreen',
        },
        browserSettings: {
          globalVolume: 100,
          soundAttenuation: 5,
          soundMinDistance: 3,
          soundMaxDistance: 10,
          is3DAudioEnabled: true,
        },
        positionalSettings: null,
      };
      const now = new Date();
      now.setMinutes(now.getMinutes() - 5);

      const testStatus = {
        screenName: 'blabla',
        // tslint:disable-next-line:max-line-length
        startedAt: now.toDateString(),
        duration: 3500,
        isPaused: false,
        currentSource: 'https://youtube.com/blablabla',
        repeat: false,
      };

      screenStatus = [testStatus];
      screens = [testScreen];
      this.store.dispatch(new SetScreens(screens));
      this.store.dispatch(new UpdateState(screenStatus));
    }
  }

  @HostListener('window:message', ['$event'])
  handleNUIMessage(event: any) {
    if (!event) {
      return;
    }
    switch (event.data.type) {
      case 'showUI':
        this.appVisible = event.data.payload;

        event.data.payload ? this.router.navigateByUrl('quick-play') : this.store.dispatch(new ClearControlledScreen());
        break;
      case 'createdScreen':
        this.store.dispatch(new CreateScreen(event.data.payload));

        if (this.appVisible) {
          this.router.navigateByUrl('screens');
          this.snackBar.open('Screen successfully created!', 'Dismiss', {
            duration: 2500,
          });
        }

        break;
      case 'getScreenList':
        this.store.dispatch(new SetScreens(event.data.payload));
        break;
      case 'editScreen':
        this.store.dispatch(new UpdateScreen(event.data.payload));

        if (this.appVisible) {
          this.snackBar.open('Screen successfully updated!', 'Dismiss', {
            duration: 2500,
          });
        }
        break;
      case 'deleteScreen':
        this.store.dispatch(new DeleteScreen(event.data.payload));

        if (this.appVisible) {
          this.snackBar.open('Screen successfully deleted!', 'Dismiss', {
            duration: 2500,
          });
        }
        break;
    }
  }

  ngOnInit(): void {
    this.nuiService.requestDuiState('');
  }
}


