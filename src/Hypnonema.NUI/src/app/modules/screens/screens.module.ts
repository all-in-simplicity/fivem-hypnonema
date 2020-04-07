import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScreensComponent } from './screens.component';
import { ScreensRoutingModule } from './screens-routing.module';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSliderModule } from '@angular/material/slider';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
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
