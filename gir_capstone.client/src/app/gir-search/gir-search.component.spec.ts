import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GirSearchComponent } from './gir-search.component';

describe('GirSearchComponent', () => {
  let component: GirSearchComponent;
  let fixture: ComponentFixture<GirSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GirSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GirSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
