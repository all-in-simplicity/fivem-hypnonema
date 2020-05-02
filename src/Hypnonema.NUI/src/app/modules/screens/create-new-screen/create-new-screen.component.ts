import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NuiService } from '../../core/nui.service';

@Component({
  selector: 'app-create-new-screen',
  templateUrl: './create-new-screen.component.html',
  styleUrls: ['./create-new-screen.component.scss']
})
export class CreateNewScreenComponent implements OnInit {
  screenForm: FormGroup;
  renderTargetForm: FormGroup;
  soundForm: FormGroup;

  constructor(public fb: FormBuilder, private nuiService: NuiService) {
  }

  get screenName(): string {
    return this.screenForm.get('name').value;
  }

  get modelName(): string {
    return this.renderTargetForm.get('modelName').value;
  }

  get renderTargetName(): string {
    return this.renderTargetForm.get('renderTargetName').value;
  }

  get is3DRendered(): boolean {
    return this.renderTargetForm.get('is3DRendered').value;
  }

  get positionX(): number {
    return this.renderTargetForm.get('positionX').value;
  }

  get positionY(): number {
    return this.renderTargetForm.get('positionY').value;
  }

  get positionZ(): number {
    return this.renderTargetForm.get('positionZ').value;
  }

  get rotationX(): number {
    return this.renderTargetForm.get('rotationX').value;
  }

  get rotationZ(): number {
    return this.renderTargetForm.get('rotationZ').value;
  }

  get rotationY(): number {
    return this.renderTargetForm.get('rotationY').value;
  }

  get scaleX(): number {
    return this.renderTargetForm.get('scaleX').value;
  }

  get scaleZ(): number {
    return this.renderTargetForm.get('scaleZ').value;
  }

  get scaleY(): number {
    return this.renderTargetForm.get('scaleY').value;
  }

  get globalVolume(): number {
    return this.soundForm.get('globalVolume').value;
  }

  get soundMaxDistance(): number {
    return this.soundForm.get('soundMaxDistance').value;
  }

  get soundMinDistance(): number {
    return this.soundForm.get('soundMinDistance').value;
  }

  get soundAttenuation(): number {
    return this.soundForm.get('soundAttenuation').value;
  }

  get is3DAudioEnabled(): boolean {
    return this.soundForm.get('is3DAudioEnabled').value;
  }

  get alwaysOn(): boolean {
    return this.screenForm.get('alwaysOn').value;
  }

  ngOnInit() {
    this.screenForm = this.fb.group({
      name: ['', [Validators.required]],
      alwaysOn: [false],
    });
    this.renderTargetForm = this.fb.group({
      modelName: ['', [Validators.required]],
      renderTargetName: ['', [Validators.required]],
      is3DRendered: [false],
      positionX: [0, [Validators.required]],
      positionY: [0, [Validators.required]],
      positionZ: [0, [Validators.required]],
      rotationX: [0, [Validators.required]],
      rotationY: [0, [Validators.required]],
      rotationZ: [0, [Validators.required]],
      scaleX: [0, [Validators.required]],
      scaleY: [0, [Validators.required]],
      scaleZ: [0, [Validators.required]]
    });
    this.soundForm = this.fb.group({
      globalVolume: [100],
      soundAttenuation: [5],
      soundMinDistance: [15],
      soundMaxDistance: [100],
      is3DAudioEnabled: [true],
    });
    this.renderTargetForm.get('is3DRendered').valueChanges.subscribe(checked => {
      if (checked) {
        this.renderTargetForm.get('modelName').disable();
        this.renderTargetForm.get('renderTargetName').disable();
        this.renderTargetForm.get('positionX').enable();
        this.renderTargetForm.get('positionY').enable();
        this.renderTargetForm.get('positionZ').enable();
        this.renderTargetForm.get('scaleX').enable();
        this.renderTargetForm.get('scaleY').enable();
        this.renderTargetForm.get('scaleZ').enable();
        this.renderTargetForm.get('rotationX').enable();
        this.renderTargetForm.get('rotationY').enable();
        this.renderTargetForm.get('rotationZ').enable();
      } else {

        this.renderTargetForm.get('modelName').enable();
        this.renderTargetForm.get('renderTargetName').enable();
        this.renderTargetForm.get('positionX').disable();
        this.renderTargetForm.get('positionY').disable();
        this.renderTargetForm.get('positionZ').disable();
        this.renderTargetForm.get('scaleX').disable();
        this.renderTargetForm.get('scaleY').disable();
        this.renderTargetForm.get('scaleZ').disable();
        this.renderTargetForm.get('rotationX').disable();
        this.renderTargetForm.get('rotationY').disable();
        this.renderTargetForm.get('rotationZ').disable();
      }
    });
  }

  createScreen() {
    this.nuiService.createScreen(this.screenName, this.alwaysOn, this.globalVolume, this.soundAttenuation, this.soundMinDistance,
      this.soundMaxDistance, this.is3DRendered, this.is3DAudioEnabled, this.modelName, this.renderTargetName, this.positionX,
      this.positionY, this.positionZ, this.rotationX, this.rotationY, this.rotationZ, this.scaleX, this.scaleY, this.scaleZ);
  }
}
