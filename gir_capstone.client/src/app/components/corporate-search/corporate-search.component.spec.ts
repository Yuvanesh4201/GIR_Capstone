import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateSearchComponent } from './corporate-search.component';

describe('CorporateSearchComponent', () => {
  let component: CorporateSearchComponent;
  let fixture: ComponentFixture<CorporateSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CorporateSearchComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CorporateSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
