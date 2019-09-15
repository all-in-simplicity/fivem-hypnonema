import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  public volume = 100;
  public soundAttenuation = 5;
  public soundMinDistance = 15;

  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  volumeChange() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnVolumeChange`, {volume: this.volume})
      .subscribe(() => {}, error => console.log(error));
  }

  soundAttenuationChange() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnSoundAttenuationChange`, {soundAttenuation: this.soundAttenuation})
      .subscribe(() => {}, error => console.log(error));
  }

  soundMinDistanceChange() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnSoundMinDistanceChange`, {minDistance: this.soundMinDistance})
      .subscribe(() => {}, error => console.log(error));
  }
}
