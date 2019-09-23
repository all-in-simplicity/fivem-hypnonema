import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
  path: 'playback',
  loadChildren: './+playback/playback.module#PlaybackModule'
  },
  {
    path: 'settings',
    loadChildren: './+settings/settings.module#SettingsModule'
  },
  {
    path: 'history',
    loadChildren: './+history/history.module#HistoryModule'
  }];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
