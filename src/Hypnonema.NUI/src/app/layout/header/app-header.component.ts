import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NuiService } from '../../modules/core/nui.service';

@Component({
  selector: 'app-header',
  templateUrl: './app-header.component.html',
  styleUrls: ['./app-header.component.scss']
})
export class AppHeaderComponent implements OnInit {

  constructor(private nuiService: NuiService) { }

  ngOnInit() {
  }
  close() {
    this.nuiService.hideNUI();
  }
}
