import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HistoryRoutingModule } from './history-routing.module';
import { HistoryComponent } from './history.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbCollapseModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { HistoryEntryComponent } from './history-entry/history-entry.component';

@NgModule({
  declarations: [HistoryComponent, HistoryEntryComponent],
  imports: [
    CommonModule,
    HistoryRoutingModule,
    FontAwesomeModule,
    NgbTooltipModule,
    NgbCollapseModule,
  ]
})
export class HistoryModule { }
