import {ComponentFixture, TestBed, waitForAsync} from '@angular/core/testing';

import {AppHeaderComponent} from './app-header.component';

describe('AppHeaderComponent', () => {
  let component: AppHeaderComponent;
  let fixture: ComponentFixture<AppHeaderComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AppHeaderComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
