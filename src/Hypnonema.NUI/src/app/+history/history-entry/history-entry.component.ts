import { Component, Input, OnInit } from '@angular/core';
import { Video } from '@hypnonema/models/video';
import { History } from '@hypnonema/+history/history-state';
import * as moment from 'moment';
import { faCaretDown } from '@fortawesome/free-solid-svg-icons/faCaretDown';

@Component({
  selector: 'app-history-entry',
  templateUrl: './history-entry.component.html',
  styleUrls: ['./history-entry.component.scss']
})
export class HistoryEntryComponent implements OnInit {
  @Input()
  history: History;
  faCaretDown = faCaretDown;
  isCollapsed = true;
  constructor() { }

  ngOnInit() {
  }

  fromNow(date: Date) {
    return moment(date).fromNow();
  }
}
