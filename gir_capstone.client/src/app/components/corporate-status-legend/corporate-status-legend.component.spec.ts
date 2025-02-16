import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateStatusLegendComponent } from './corporate-status-legend.component';

describe('CorporateStatusLegendComponent', () => {
  let component: CorporateStatusLegendComponent;
  let fixture: ComponentFixture<CorporateStatusLegendComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CorporateStatusLegendComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CorporateStatusLegendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
