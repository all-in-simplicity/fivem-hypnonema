import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


const routes: Routes = [
  {
    path: 'quick-play',
    loadChildren: () => import('./modules/quick-play/quick-play.module').then(m => m.QuickPlayModule)
  },
  {
    path: 'screens',
    loadChildren: () => import('./modules/screens/screens.module').then(m => m.ScreensModule)
  },
  {
    path: 'status',
    loadChildren: () => import('./modules/status/status.module').then(m => m.StatusModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
