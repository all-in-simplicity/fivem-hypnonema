import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Store } from '@ngxs/store';
import { AddHistoryEntry } from '@hypnonema/+history/history-state';


@Component({
  selector: 'app-playback',
  templateUrl: './playback.component.html',
  styleUrls: ['./playback.component.scss']
})
export class PlaybackComponent implements OnInit {
  playForm = new FormGroup({
    videoURL: new FormControl(''),
    videoType: new FormControl('auto'),
    customVideoType: new FormControl('')
  });
  constructor(private http: HttpClient, private store: Store) { }
  play($event: any) {
    let videoType = $event.videoType;
    if (this.playForm.get('videoType').value === 'other') {
      videoType = $event.customVideoType;
    }

    this.http.post(`http://${environment.resourceName}/Hypnonema.OnPlay`, {videoURL: $event.videoURL, videoType})
      .subscribe(() => {
      }, error => {
        console.log(`error sending ${$event.videoType}/${$event.videoURL}`);
        console.log(error);
      });
    this.store.dispatch(new AddHistoryEntry(videoType, $event.videoURL));
  }
  ngOnInit() {
  }

}
