import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

// @ts-ignore
import compareVersions from 'compare-versions';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

interface ReleaseResponse {
  name: string;
  created_at: Date;
  tag_name: string;
}

@Injectable({
  providedIn: 'root'
})
export class UpdateCheckService {
  repoUrl = 'https://api.github.com/repos/thiago-dev/fivem-hypnonema/releases/latest';
  constructor(private http: HttpClient, private toastr: ToastrService) { }

  private getHeaders(): HttpHeaders {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');
    headers.append('User-Agent', 'Hypnonema');
    return headers;
  }

  private getLatestRelease(): Observable<ReleaseResponse> {
    return this.http.get<ReleaseResponse>(this.repoUrl, { headers: this.getHeaders()});
  }

  check(currentVersion) {
    this.getLatestRelease().subscribe((data: ReleaseResponse) => {
      if (compareVersions.compare(data.tag_name, currentVersion, '>')) {
        this.toastr.warning('There is a new Update available. You may want to check it out if you want new features.',
          `Hypnonema Update ${data.tag_name} available!`, { timeOut: 8000, positionClass: 'toast-top-right'});
      }
    });
  }
}
