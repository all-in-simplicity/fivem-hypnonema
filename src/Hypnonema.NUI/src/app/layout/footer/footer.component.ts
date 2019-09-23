import { Component, OnInit } from '@angular/core';
import { faPlay } from '@fortawesome/free-solid-svg-icons/faPlay';
import { faPause } from '@fortawesome/free-solid-svg-icons/faPause';
import { faStop } from '@fortawesome/free-solid-svg-icons/faStop';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {
  faPlay = faPlay;
  faPause = faPause;
  faStop = faStop;
  constructor(private http: HttpClient) { }

  toggleReplay($event) {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnToggleReplay`, {replay: $event})
      .subscribe(() => {}, error => console.log(error));
  }
  ngOnInit() {
  }
  resume() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnResumeVideo`, {})
      .subscribe(() => {}, error => console.log(error));
  }
  stop() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnStopVideo`, {})
      .subscribe(() => {}, error => console.log(error));
  }
  pause() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnPause`, {})
      .subscribe(() => {}, error => console.log(error));
  }
}
