import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Select, Store } from '@ngxs/store';
import { Observable } from 'rxjs';
import { AppState } from '../../app-state';
import { ScreenModel } from '../../screen-model';
import { NuiService } from '../core/nui.service';



@Component({
  selector: 'app-quick-play',
  templateUrl: './quick-play.component.html',
  styleUrls: ['./quick-play.component.scss']
})
export class QuickPlayComponent implements OnInit {
  playForm: FormGroup;
  @Select(AppState.getScreens)
  screens$: Observable<ScreenModel[]>;

  selectedScreen;
  constructor(public fb: FormBuilder, private nuiService: NuiService) { }

  ngOnInit() {
    this.playForm = this.fb.group({
      url: ['', [Validators.required]],
      screen: ['', [Validators.required]],
    });
  }

  submit() {
    const videoUrl = this.playForm.get('url').value;
    this.nuiService.playVideo(this.selectedScreen.name, videoUrl);
    const refThis = this;
    setTimeout(function(this) {
      refThis.nuiService.requestDuiState(refThis.selectedScreen.name);
    }, 2500);
  }

}
