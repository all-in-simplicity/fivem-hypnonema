import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Select, Store } from '@ngxs/store';
import { AppState, SetControlledScreen } from '../../app-state';
import { Observable, Subscription, timer as ObservableTimer } from 'rxjs';
import { ScreenStatus } from '../../screen-model';
import { distinctUntilChanged } from 'rxjs/operators';
import { MatSliderChange } from '@angular/material/slider';
import { NuiService } from '../../modules/core/nui.service';

@Component({
  selector: 'app-current-track',
  templateUrl: './current-track.component.html',
  styleUrls: ['./current-track.component.scss']
})
export class CurrentTrackComponent implements OnInit {
  @Select(AppState.getStatuses)
  screens$: Observable<ScreenStatus[]>;

  @Select(AppState.getControlledScreen)
  screen$: Observable<ScreenStatus>;

  selectedScreenName: string;
  isPaused: boolean;
  sliderCap = 0;
  startCurrentTime = 0;
  ticks = 0;
  timer;
  sub: Subscription;

  constructor(private nuiService: NuiService, private store: Store) { }
  stop(screenName) {
    this.nuiService.stopVideo(screenName);
  }
  onSelectionChange(newValue) {
    this.selectedScreenName = newValue;
    this.store.dispatch(new SetControlledScreen(newValue));
    const refThis = this;
    setTimeout(() => {
      refThis.nuiService.requestDuiState(this.selectedScreenName);
    }, 500);
  }
  ngOnInit() {
    this.screen$
      .pipe(
      distinctUntilChanged()
      )
      .subscribe(screen => {
      if (screen) {
        if (this.sub) {
          this.ticks = 0;
          this.sub.unsubscribe();
        }

        this.sliderCap = Math.floor(screen.duration);
        this.isPaused = screen.isPaused;

        if (!screen.isPaused) {
          this.startTimer();
        }
        this.startCurrentTime = screen.ended ? this.sliderCap : Math.floor(screen.currentTime);
      }
    });
  }

  onSliderChange(sliderVal: MatSliderChange) {
    this.startCurrentTime = sliderVal.value;
    this.nuiService.setVideoTime(this.selectedScreenName, sliderVal.value);

    const refThis = this;
    setTimeout(() => {
      refThis.nuiService.requestDuiState(this.selectedScreenName);
    }, 500);
  }

  repeat(screenName: string) {
    this.nuiService.repeatVideo(screenName);

    const refThis = this;
    setTimeout(() => {
      refThis.nuiService.requestDuiState(this.selectedScreenName);
    }, 250);
  }

  resumeOrPause(paused) {
    if (paused) {
      this.nuiService.resumeVideo(this.selectedScreenName);
    } else {
      this.nuiService.pauseVideo(this.selectedScreenName);
    }

    const refThis = this;
    setTimeout(() => {
      refThis.nuiService.requestDuiState(this.selectedScreenName);
    }, 1000);
  }

  formatSliderLabel(value: number) {
    const hours = (Math.floor((value / 60) / 60));
    const hoursPadded = hours <= 9 ? '0' + hours : hours;
    const minutes = (Math.floor(value / 60)) % 60;
    const minutesPadded = minutes <= 9 ? '0' + minutes : minutes;
    const seconds = Math.floor(value % 60);
    const secondsPadded = seconds <= 9 ? '0' + seconds : seconds;
    return `${hoursPadded}:${minutesPadded}:${secondsPadded}`;
  }

  getSeconds(ticks: number) {
    return this.pad(ticks % 60);
  }

 getMinutes(ticks: number) {
    return this.pad((Math.floor(ticks / 60)) % 60);
  }

  getHours(ticks: number) {
    return this.pad(Math.floor((ticks / 60) / 60));
  }

  private pad(digit: any) {
    return digit <= 9 ? '0' + digit : digit;
  }

  startTimer() {
    this.timer = ObservableTimer(1, 1000);
    this.sub = this.timer.subscribe(t => {
        this.ticks = t;
        if (t + this.startCurrentTime >= this.sliderCap) {
          this.sub.unsubscribe();
        }
      });
  }
}
