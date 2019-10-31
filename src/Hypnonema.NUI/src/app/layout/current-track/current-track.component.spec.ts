import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentTrackComponent } from './current-track.component';

describe('CurrentTrackComponent', () => {
  let component: CurrentTrackComponent;
  let fixture: ComponentFixture<CurrentTrackComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CurrentTrackComponent ]
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
