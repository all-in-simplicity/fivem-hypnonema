import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Select} from '@ngxs/store';
import {Observable} from 'rxjs';
import {AppState} from '../../app-state';
import {ScreenModel} from '../../screen-model';
import {NuiService} from '../core/nui.service';

@Component({
  selector: 'app-quick-play',
  templateUrl: './quick-play.component.html',
  styleUrls: ['./quick-play.component.scss']
})
export class QuickPlayComponent implements OnInit {
  playForm: FormGroup;

  @Select(AppState.getScreens)
  screens$: Observable<ScreenModel[]>;

  constructor(public fb: FormBuilder, private nuiService: NuiService) {
  }

  get selectedScreen(): any {
    return this.playForm.get('screen').value;
  }

  get videoUrl(): any {
    return this.playForm.get('url').value;
  }

  ngOnInit() {
    this.playForm = this.fb.group({
      url: ['', [Validators.required]],
      screen: ['', [Validators.required]],
    });
  }

  submit() {
    this.nuiService.playVideo(this.selectedScreen, this.videoUrl);

    setTimeout(() => {
      this.nuiService.requestDuiState('');
    }, 1000);
  }
}
