import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { QuickPlayComponent } from './quick-play.component';


const routes: Routes = [{
  path: '',
  component: QuickPlayComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QuickPlayRoutingModule { }
