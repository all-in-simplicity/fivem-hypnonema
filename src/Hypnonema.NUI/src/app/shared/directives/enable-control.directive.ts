import {Directive, Input} from '@angular/core';
import {NgControl} from '@angular/forms';

@Directive({
  selector: '[appEnableControl]'
})
export class EnableControlDirective {
  constructor(private ngControl: NgControl) {
  }

  @Input() set appEnableControl(condition: boolean) {
    if (this.ngControl) {
      if (this.ngControl.control) {
        if (condition) {
          this.ngControl.control.enable();
        } else {
          this.ngControl.control.disable();
        }
      }
    }
  }
}
