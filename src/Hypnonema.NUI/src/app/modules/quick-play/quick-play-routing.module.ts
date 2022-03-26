import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {QuickPlayComponent} from './quick-play.component';


const routes: Routes = [{
  path: '',
  component: QuickPlayComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class QuickPlayRoutingModule {
}
