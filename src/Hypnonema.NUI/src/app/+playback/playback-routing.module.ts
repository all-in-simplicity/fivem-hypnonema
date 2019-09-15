import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PlaybackComponent } from './playback.component';

const routes: Routes = [{
  path: '',
  component: PlaybackComponent
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PlaybackRoutingModule { }
