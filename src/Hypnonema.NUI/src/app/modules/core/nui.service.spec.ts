import { TestBed } from '@angular/core/testing';

import { NuiService } from './nui.service';

describe('NuiService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: NuiService = TestBed.get(NuiService);
    expect(service).toBeTruthy();
  });
});
