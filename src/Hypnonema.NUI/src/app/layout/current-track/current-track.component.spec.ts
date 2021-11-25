import {ComponentFixture, TestBed, waitForAsync} from '@angular/core/testing';

import {CurrentTrackComponent} from './current-track.component';

describe('CurrentTrackComponent', () => {
  let component: CurrentTrackComponent;
  let fixture: ComponentFixture<CurrentTrackComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [CurrentTrackComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CurrentTrackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
