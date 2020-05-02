import { Component,  OnInit } from '@angular/core';
import { ScreenModel } from '../../../screen-model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Select } from '@ngxs/store';
import { AppState } from '../../../app-state';
import { Observable } from 'rxjs';
import { NuiService } from '../../core/nui.service';

@Component({
  selector: 'app-edit-screen',
  templateUrl: './edit-screen.component.html',
  styleUrls: ['./edit-screen.component.scss']
})
export class EditScreenComponent implements OnInit {
  @Select(AppState.getSelectedScreen)
  screen: Observable<ScreenModel>;
  screenId: number;
  screenForm: FormGroup;

  constructor(private fb: FormBuilder, private nuiService: NuiService) {
    this.screenForm = this.fb.group({
      screenName: ['', [Validators.required]],
      modelName: ['', [Validators.required]],
      renderTargetName: ['', [Validators.required]],
      soundAttenuation: [5],
      is3DAudioEnabled: [false],
      soundMaxDistance: [100],
      soundMinDistance: [10],
      globalVolume: [100],
      alwaysOn: [false],
      is3DRendered: [false],
      positionX: [0],
      positionY: [0],
      positionZ: [0],
      rotationX: [0],
      rotationY: [0],
      rotationZ: [0],
      scaleX: [0],
      scaleY: [0],
      scaleZ: [0]
    });
  }

  ngOnInit() {
    this.screen.subscribe(screen => {
      if (screen) {
        this.screenId = screen.id;
        this.screenForm.patchValue({
          screenName: screen.name,
          modelName: screen.is3DRendered ? '' : screen.targetSettings.modelName,
          renderTargetName: screen.is3DRendered ? '' : screen.targetSettings.renderTargetName,
          soundAttenuation: screen.browserSettings.soundAttenuation,
          soundMinDistance: screen.browserSettings.soundMinDistance,
          soundMaxDistance: screen.browserSettings.soundMaxDistance,
          globalVolume: screen.browserSettings.globalVolume,
          alwaysOn: screen.alwaysOn,
          is3DRendered: screen.is3DRendered,
          positionX: screen.is3DRendered ? screen.positionalSettings.positionX : 0,
          positionY: screen.is3DRendered ? screen.positionalSettings.positionY : 0,
          positionZ: screen.is3DRendered ?  screen.positionalSettings.positionZ : 0,
          rotationX: screen.is3DRendered ? screen.positionalSettings.rotationX : 0,
          rotationY: screen.is3DRendered ?  screen.positionalSettings.rotationY : 0,
          rotationZ: screen.is3DRendered ? screen.positionalSettings.rotationZ : 0,
          scaleX: screen.is3DRendered ?  screen.positionalSettings.scaleX : 0,
          scaleY: screen.is3DRendered ? screen.positionalSettings.scaleY : 0,
          scaleZ: screen.is3DRendered ? screen.positionalSettings.scaleZ : 0,
          is3DAudioEnabled: screen.browserSettings.is3DAudioEnabled,
        });
      }
    });
    this.screenForm.get('is3DRendered').valueChanges.subscribe(checked => {
      if (checked) {
        this.screenForm.get('renderTargetName').disable();
        this.screenForm.get('modelName').disable();
        this.screenForm.get('positionX').enable();
        this.screenForm.get('positionY').enable();
        this.screenForm.get('positionZ').enable();
        this.screenForm.get('rotationX').enable();
        this.screenForm.get('rotationY').enable();
        this.screenForm.get('rotationZ').enable();
        this.screenForm.get('scaleX').enable();
        this.screenForm.get('scaleY').enable();
        this.screenForm.get('scaleZ').enable();
      } else {
        this.screenForm.get('renderTargetName').enable();
        this.screenForm.get('modelName').enable();
        this.screenForm.get('positionX').disable();
        this.screenForm.get('positionY').disable();
        this.screenForm.get('positionZ').disable();
        this.screenForm.get('rotationX').disable();
        this.screenForm.get('rotationY').disable();
        this.screenForm.get('rotationZ').disable();
        this.screenForm.get('scaleX').disable();
        this.screenForm.get('scaleY').disable();
        this.screenForm.get('scaleZ').disable();
      }
    });
  }
  submit() {
    this.nuiService.editScreen(this.screenForm.get('screenName').value, this.screenId, this.screenForm.get('is3DRendered').value,
      this.screenForm.get('alwaysOn').value, this.screenForm.get('modelName').value, this.screenForm.get('renderTargetName').value,
      this.screenForm.get('globalVolume').value, this.screenForm.get('soundAttenuation').value,
      this.screenForm.get('soundMinDistance').value, this.screenForm.get('soundMaxDistance').value, this.screenForm.get('positionX').value,
      this.screenForm.get('positionY').value, this.screenForm.get('positionZ').value, this.screenForm.get('rotationX').value,
      this.screenForm.get('rotationY').value, this.screenForm.get('rotationZ').value, this.screenForm.get('scaleX').value,
      this.screenForm.get('scaleY').value, this.screenForm.get('scaleZ').value, this.screenForm.get('is3DAudioEnabled').value);
  }
}
