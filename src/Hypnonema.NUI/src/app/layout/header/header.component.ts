import { Component, OnInit } from '@angular/core';
import { faBars } from '@fortawesome/free-solid-svg-icons/faBars';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons/faArrowLeft';
import { faFilm } from '@fortawesome/free-solid-svg-icons/faFilm';
import { faHome } from '@fortawesome/free-solid-svg-icons/faHome';
import { faCogs } from '@fortawesome/free-solid-svg-icons/faCogs';
import { faWindowClose } from '@fortawesome/free-solid-svg-icons/faWindowClose';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { faHistory } from '@fortawesome/free-solid-svg-icons/faHistory';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  faBars = faBars;
  faFilm = faFilm;
  faHome = faHome;
  faCogs = faCogs;
  faHistory = faHistory;
  faClose = faWindowClose;

  faArrowLeft = faArrowLeft;
  isNavbarCollapsed = true;
  constructor(private http: HttpClient) { }

  ngOnInit() {
  }
  close() {
    this.http.post(`http://${environment.resourceName}/Hypnonema.OnHideNUI`, {})
      .subscribe(() => {}, error => console.log(error));
  }
}
