import {Component, OnInit} from '@angular/core';
import {Select, Store} from '@ngxs/store';
import {AppState} from '../../app-state';
import {Observable} from 'rxjs';
import {ScreenStatus} from '../../screen-model';
import {HttpClient} from '@angular/common/http';
import {NuiService} from '../core/nui.service';

@Component({
  selector: 'app-status',
  templateUrl: './status.component.html',
  styleUrls: ['./status.component.scss']
})
export class StatusComponent implements OnInit {
  displayedColumns: string[] = ['actions', 'name', 'isPaused', 'currentTime', 'duration', 'currentSource'];

  @Select(AppState.getStatuses)
  screens$: Observable<ScreenStatus[]>;

  @Select(AppState.isLoading)
  isLoading: Observable<boolean>;

  constructor(private http: HttpClient, private store: Store, private nuiService: NuiService) {
  }

  ngOnInit() {
    this.refresh();
  }

  suspendScreen(screen: ScreenStatus) {
    this.nuiService.closeScreen(screen.screenName);

    setTimeout(() => {
      this.nuiService.requestDuiState(screen.screenName);
    }, 1000);
  }

  refresh() {
    this.nuiService.requestDuiState('');
  }
}
