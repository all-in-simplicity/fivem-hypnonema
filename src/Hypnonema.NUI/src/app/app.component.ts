import { Component, HostListener } from '@angular/core';
import { environment } from '../environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  appVisible = !environment.production;
  title = 'HypnonemaNUI';

  constructor(private router: Router) {}
  @HostListener('window:message', ['$event'])
  handleNuiMessage(event: any) {
    if (!event) { return; }
    if (event.data.type === 'HypnonemaNUI.ShowUI') {
      this.router.navigateByUrl('playback');
      this.appVisible = true;
    }
    if (event.data.type === 'HypnonemaNUI.HideUI') {
      this.appVisible = false;
    }
  }
}
