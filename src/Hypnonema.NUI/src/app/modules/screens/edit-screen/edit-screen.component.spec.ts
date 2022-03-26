import {ComponentFixture, TestBed, waitForAsync} from '@angular/core/testing';

import {EditScreenComponent} from './edit-screen.component';

describe('EditScreenComponent', () => {
  let component: EditScreenComponent;
  let fixture: ComponentFixture<EditScreenComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [EditScreenComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
