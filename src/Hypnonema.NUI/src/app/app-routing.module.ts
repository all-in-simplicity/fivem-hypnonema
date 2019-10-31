import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
    path: 'quick-play',
    loadChildren: './modules/quick-play/quick-play.module#QuickPlayModule'
  },
  {
    path: 'screens',
    loadChildren: './modules/screens/screens.module#ScreensModule'
  },
  {
    path: 'status',
    loadChildren: './modules/status/status.module#StatusModule'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
