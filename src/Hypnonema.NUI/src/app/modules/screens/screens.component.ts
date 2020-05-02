import { Component, Inject, OnInit } from '@angular/core';
import { Select, Store } from '@ngxs/store';
import { AppState, DeleteScreen, SetSelectedScreen } from '../../app-state';
import { Observable } from 'rxjs';
import { ScreenModel } from '../../screen-model';
import { Router } from '@angular/router';
import { NuiService } from '../core/nui.service';

@Component({
  selector: 'app-screens',
  templateUrl: './screens.component.html',
  styleUrls: ['./screens.component.scss']
})
export class ScreensComponent implements OnInit {
  displayedColumns: string[] = ['actions', 'name', 'modelName', 'targetName', 'alwaysOn'];
  @Select(AppState.getScreens)
  screens$: Observable<ScreenModel[]>;
  constructor(private router: Router, private nuiService: NuiService, private store: Store) { }

  ngOnInit() {
  }

  editScreen(screen: ScreenModel) {
    this.store.dispatch(new SetSelectedScreen(screen));
    this.router.navigateByUrl('/screens/edit-screen');
  }

  deleteScreen(screen: ScreenModel) {
    this.nuiService.deleteScreen(screen.name);
  }
}

