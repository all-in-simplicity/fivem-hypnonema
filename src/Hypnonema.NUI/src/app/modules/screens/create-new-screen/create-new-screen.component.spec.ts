import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateNewScreenComponent } from './create-new-screen.component';

describe('CreateNewScreenComponent', () => {
  let component: CreateNewScreenComponent;
  let fixture: ComponentFixture<CreateNewScreenComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateNewScreenComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateNewScreenComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
