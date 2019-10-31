import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScreensComponent } from './screens.component';
import { ScreensRoutingModule } from './screens-routing.module';
import {
  MatButtonModule,
  MatCardModule, MatCheckboxModule, MatDialog, MatDialogModule,
  MatFormFieldModule,
  MatIconModule, MatInputModule,
  MatSelectModule, MatSliderModule,
  MatStepperModule,
  MatTableModule, MatTooltipModule
} from '@angular/material';
import { CreateNewScreenComponent } from './create-new-screen/create-new-screen.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EditScreenComponent } from './edit-screen/edit-screen.component';
import { QuickPlayModule } from '../quick-play/quick-play.module';



@NgModule({
  declarations: [
    ScreensComponent,
    CreateNewScreenComponent,
    EditScreenComponent
  ],
  imports: [
    CommonModule,
    ScreensRoutingModule,
    MatIconModule,
    MatButtonModule,
    MatTableModule,
    MatDialogModule,
    MatCardModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatStepperModule,
    MatInputModule,
    MatSliderModule,
    MatCheckboxModule,
    FormsModule,
    MatTooltipModule,
    QuickPlayModule
  ]
})
export class ScreensModule { }
