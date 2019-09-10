import { Component, HostListener } from '@angular/core';
import { environment } from '../environments/environment';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  appVisible = !environment.production;
  title = 'Hypnonema';



  constructor() {}
  @HostListener('window:message', ['$event'])
  handleNuiMessage(event: any) {
    if (!event) { return; }
    switch (event.data.type) {
      case 'HypnonemaNUI.ShowUI':
        this.appVisible = true;
        break;
      case 'HypnonemaNUI.HideUI':
        this.appVisible = false;
        break;
    }
  }
}
