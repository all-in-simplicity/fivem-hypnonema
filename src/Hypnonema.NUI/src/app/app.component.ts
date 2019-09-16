import { Component, HostListener } from '@angular/core';
import { environment } from '../environments/environment';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ReleaseResponse } from './release-response';
import { ToastrService } from 'ngx-toastr';

// @ts-ignore
import compareVersions from 'compare-versions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  appVisible = !environment.production;
  checkedForUpdates = false;
  title = 'HypnonemaNUI';

  getHeaders(): HttpHeaders {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');
    headers.append('User-Agent', 'Hypnonema');
    return headers;
  }
  getLatestRelease(): Observable<ReleaseResponse> {
    const repoUrl = 'https://api.github.com/repos/thiago-dev/fivem-hypnonema/releases/latest';
    return this.http.get<ReleaseResponse>(repoUrl, { headers: this.getHeaders()});
  }
  constructor(private router: Router, private http: HttpClient, private toastr: ToastrService) {}
  @HostListener('window:message', ['$event'])
  handleNuiMessage(event: any) {
    if (!event) { return; }
    if (event.data.type === 'HypnonemaNUI.ShowUI') {
      if (!this.checkedForUpdates && event.data.hypnonemaVersion) {
        this.checkedForUpdates = true;
        this.getLatestRelease().subscribe((data: ReleaseResponse) => {
          if (compareVersions.compare(data.tag_name, event.data.hypnonemaVersion, '>')) {
            this.toastr.info('There is a new Update available. You may want to check it out if you want new features.',
              `Hypnonema Update ${data.tag_name} available!`, { timeOut: 8000});
          }
        });
      }
      this.router.navigateByUrl('playback');
      this.appVisible = true;
    }
    if (event.data.type === 'HypnonemaNUI.HideUI') {
      this.appVisible = false;
    }
  }
}
