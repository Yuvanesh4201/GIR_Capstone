import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateSortComponent } from './corporate-sort.component';

describe('CorporateSortJurisdictionComponent', () => {
  let component: CorporateSortComponent;
  let fixture: ComponentFixture<CorporateSortComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CorporateSortComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CorporateSortComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
