import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuickPlayRoutingModule } from './quick-play-routing.module';
import { QuickPlayComponent } from './quick-play.component';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
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
