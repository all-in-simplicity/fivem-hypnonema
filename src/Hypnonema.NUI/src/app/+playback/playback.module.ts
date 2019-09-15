import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlaybackRoutingModule } from './playback-routing.module';
import { PlaybackComponent } from './playback.component';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { EnableControlDirective } from '../shared/enable-control.directive';


@NgModule({
  declarations: [
    PlaybackComponent,
    EnableControlDirective
  ],
  imports: [
    CommonModule,
    PlaybackRoutingModule,
    FontAwesomeModule,
    ReactiveFormsModule,
    FormsModule,
  ]
})
export class PlaybackModule { }
