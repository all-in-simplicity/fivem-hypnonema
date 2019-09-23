import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AddHistoryEntry, History, HistoryState } from '@hypnonema/+history/history-state';
import { Select, Store } from '@ngxs/store';
import { faClipboard } from '@fortawesome/free-solid-svg-icons/faClipboard';
import { faPlayCircle } from '@fortawesome/free-solid-svg-icons/faPlayCircle';

import * as moment from 'moment';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.scss']
})
export class HistoryComponent implements OnInit {
  faClipboard = faClipboard;
  faPlay = faPlayCircle;

  @Select(HistoryState.getHistory)
  history$: Observable<History[]>;

  constructor(private store: Store, private http: HttpClient) {
  }

  ngOnInit() {

  }
  copyToClipboard(item): void {
    const listener = (e: ClipboardEvent) => {
      e.clipboardData.setData('text/plain', (item));
      e.preventDefault();
    };

    document.addEventListener('copy', listener);
    document.execCommand('copy');
    document.removeEventListener('copy', listener);
  }

  fromNow(date: Date) {
    return moment(date).fromNow();
  }

  playAgain(entry: History) {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnPlay`, {videoURL: entry.video.url, videoType: entry.video.type})
      .subscribe(() => {
      }, error => {
        console.log(`error sending ${entry.video.type}/${entry.video.url}`);
        console.log(error);
      });
    this.store.dispatch(new AddHistoryEntry(entry.video.type, entry.video.url));
  }
}
