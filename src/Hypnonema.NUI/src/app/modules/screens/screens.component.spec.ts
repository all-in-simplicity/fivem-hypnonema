import {ComponentFixture, TestBed, waitForAsync} from '@angular/core/testing';

import {ScreensComponent} from './screens.component';

describe('ScreensComponent', () => {
  let component: ScreensComponent;
  let fixture: ComponentFixture<ScreensComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ScreensComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScreensComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
