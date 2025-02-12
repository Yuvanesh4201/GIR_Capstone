import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateEntityInfoComponent } from './corporate-entity-info.component';

describe('CorporateEntityInfoComponent', () => {
  let component: CorporateEntityInfoComponent;
  let fixture: ComponentFixture<CorporateEntityInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CorporateEntityInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CorporateEntityInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
