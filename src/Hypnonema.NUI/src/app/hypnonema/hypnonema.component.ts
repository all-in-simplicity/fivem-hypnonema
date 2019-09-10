import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { MatSliderChange } from '@angular/material';

@Component({
  selector: 'app-hypnonema',
  templateUrl: './hypnonema.component.html',
  styleUrls: ['./hypnonema.component.scss']
})
export class HypnonemaComponent implements OnInit {
  playForm = new FormGroup({
    videoURL: new FormControl(''),
    videoType: new FormControl('auto'),
    customVideoType: new FormControl('')
  });
  public volume = 100;
  public paused = false;
  public soundAttenuation = 10;
  public soundMinDistance = 10;
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  volumeChange(event: MatSliderChange) {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnVolumeChange`, {volume: event.value})
      .subscribe(() => {}, error => console.log(error));
  }

  soundAttenuationChange(event: MatSliderChange) {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnSoundAttenuationChange`, { soundAttenuation: event.value})
      .subscribe(() => {}, error => console.log(error));
  }

  soundMinDistanceChange(event: MatSliderChange) {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnSoundMinDistanceChange`, {minDistance: event.value})
      .subscribe(() => {}, error => console.log(error));
  }

  resume() {
    this.paused = false;
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnResumeVideo`, {})
      .subscribe(() => {}, error => console.log(error));
  }

  stop() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnStopVideo`, {})
      .subscribe(() => {}, error => console.log(error));
  }

  pause() {
    this.paused = true;
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnPause`, {})
      .subscribe(() => {}, error => console.log(error));
  }

  close() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnHideNUI`, {})
      .subscribe(() => {}, error => console.log(JSON.stringify(error)));
  }
  play($event: any) {
    let videoType = $event.videoType;
    if (this.playForm.get('videoType').value === 'other') {
      videoType = $event.customVideoType;
    }
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnPlay`, {videoURL: $event.videoURL, videoType})
      .subscribe(() => {}, error => {
        console.log(`${$event.videoType}/${$event.videoURL}`);
        console.log(JSON.stringify(error));
        console.log(error);
      });
  }
}
