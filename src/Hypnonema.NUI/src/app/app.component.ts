import { Component, HostListener, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Store } from '@ngxs/store';
import {
  ClearControlledScreen,
  CreateScreen,
  DeleteScreen,
  SetIsAceAllowed,
  SetIsLoading,
  SetScreens,
  UpdateScreen,
  UpdateStatuses
} from './app-state';
import { ScreenModel, ScreenStatus } from './screen-model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UpdateCheckService } from './modules/core/update-check.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  appVisible = !environment.production;
  checkedForUpdates = false;

  constructor(private router: Router, private http: HttpClient, private store: Store, private snackBar: MatSnackBar,
              private updateCheckService: UpdateCheckService) {
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
      const testStatus = {
        screenName: 'blabla',
        currentTime: 350,
        duration: 3500,
        isPaused: false,
        currentSource: 'https://youtube.com/blablabla',
        ended: false,
        repeat: false,
      };

      screenStatus = [testStatus];
      screens = [testScreen];
      this.store.dispatch(new SetScreens(screens));
      this.store.dispatch(new UpdateStatuses(screenStatus));
    }
  }

  @HostListener('window:message', ['$event'])
  handleNUIMessage(event: any) {
    if (!event) {
      return;
    }

    if (event.data.type === 'HypnonemaNUI.ShowUI') {
      if (!this.checkedForUpdates) {
        this.updateCheckService.check(event.data.hypnonemaVersion);
        this.checkedForUpdates = true;
      }
      if (event.data.isAceAllowed) {
        this.store.dispatch(new SetIsAceAllowed(event.data.isAceAllowed));
      } else {
        this.store.dispatch(new SetIsAceAllowed(false));
      }

      if (event.data.screens) {
        this.store.dispatch(new SetScreens(event.data.screens));
      } else {
        this.router.navigateByUrl('screens');
        this.appVisible = true;
        return;
      }

      this.router.navigateByUrl('quick-play');
      this.appVisible = true;
    }

    if (event.data.type === 'HypnonemaNUI.HideUI') {
      this.appVisible = false;
      this.store.dispatch(new ClearControlledScreen());
    }

    if (event.data.type === 'HypnonemaNUI.CreatedScreen') {
      this.store.dispatch(new CreateScreen(event.data.screen));
      if (this.appVisible) {
        this.snackBar.open('Screen successfully created!', 'Dismiss', {
          duration: 2500,
        });
        this.router.navigateByUrl('screens');
      }
    }

    if (event.data.type === 'HypnonemaNUI.EditedScreen') {
      this.store.dispatch(new UpdateScreen(event.data.screen));
      if (this.appVisible) {
        this.snackBar.open('Screen successfully updated!', 'Dismiss', {
          duration: 2500,
        });
        this.router.navigateByUrl('screens');
      }
    }

    if (event.data.type === 'HypnonemaNUI.DeletedScreen') {
      this.store.dispatch(new DeleteScreen(event.data.screenName));
      if (this.appVisible) {
        this.snackBar.open('Screen successfully deleted!', 'Dismiss', {
          duration: 2500,
        });
        this.router.navigateByUrl('screens');
      }
    }
    if (event.data.type === 'HypnonemaNUI.UpdateStatuses') {
      this.store.dispatch(new SetIsLoading(false));
      this.store.dispatch(new UpdateStatuses(event.data.screenStates));

      if (this.appVisible && event.data.showSnackBar) {
        this.snackBar.open('Successfully fetched status!', 'Dismiss', {
          duration: 2500,
        });
      }
    }
  }

  ngOnInit(): void {

  }
}
