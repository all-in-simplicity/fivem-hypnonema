import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatusComponent } from './status.component';
import { StatusRoutingModule } from './status-routing.module';
import {
  MatButtonModule,
  MatChipsModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatTableModule,
  MatTooltipModule
} from '@angular/material';
import { SharedModule } from '../../shared/shared.module';




@NgModule({
  declarations: [StatusComponent],
  imports: [
    CommonModule,
    StatusRoutingModule,
    MatButtonModule,
    MatTooltipModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    SharedModule,
    MatChipsModule,
  ]
})
export class StatusModule { }
