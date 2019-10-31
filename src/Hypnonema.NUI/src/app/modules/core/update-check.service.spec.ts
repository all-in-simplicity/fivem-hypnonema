import { TestBed } from '@angular/core/testing';

import { UpdateCheckService } from './update-check.service';

describe('UpdateCheckService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UpdateCheckService = TestBed.get(UpdateCheckService);
    expect(service).toBeTruthy();
  });
});
