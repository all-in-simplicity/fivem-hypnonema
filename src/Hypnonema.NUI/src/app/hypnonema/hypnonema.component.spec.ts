import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HypnonemaComponent } from './hypnonema.component';

describe('HypnonemaComponent', () => {
  let component: HypnonemaComponent;
  let fixture: ComponentFixture<HypnonemaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HypnonemaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HypnonemaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
