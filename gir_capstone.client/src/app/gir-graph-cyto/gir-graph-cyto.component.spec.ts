import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GirGraphCytoComponent } from './gir-graph-cyto.component';

describe('GirGraphCytoComponent', () => {
  let component: GirGraphCytoComponent;
  let fixture: ComponentFixture<GirGraphCytoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GirGraphCytoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GirGraphCytoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
