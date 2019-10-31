import { RouterModule, Routes } from '@angular/router';
import { ScreensComponent } from './screens.component';
import { NgModule } from '@angular/core';
import { CreateNewScreenComponent } from './create-new-screen/create-new-screen.component';
import { EditScreenComponent } from './edit-screen/edit-screen.component';


const routes: Routes = [{
  path: '',
  component: ScreensComponent
},
  {
    path: 'create-new-screen',
    component: CreateNewScreenComponent
  },
  {
    path: 'edit-screen',
    component: EditScreenComponent,
  }
  ];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
  ],
  exports: [
    RouterModule
  ]
})
export class ScreensRoutingModule { }
