import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';


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
  imports: [RouterModule.forRoot(routes, {useHash: true, relativeLinkResolution: 'legacy'})],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
