import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuickPlayRoutingModule } from './quick-play-routing.module';
import { QuickPlayComponent } from './quick-play.component';
import { MatButtonModule, MatCardModule, MatCheckboxModule, MatFormFieldModule, MatInputModule, MatSelectModule } from '@angular/material';
import { ReactiveFormsModule } from '@angular/forms';
import { EnableControlDirective } from '../../shared/directives/enable-control.directive';



@NgModule({
  declarations: [QuickPlayComponent, EnableControlDirective],
  exports: [
    EnableControlDirective
  ],
  imports: [
    CommonModule,
    QuickPlayRoutingModule,
    MatCardModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCheckboxModule
  ]
})
export class QuickPlayModule { }
