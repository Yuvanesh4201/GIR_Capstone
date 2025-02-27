import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateSubtreeComponent } from './corporate-subtree.component';

describe('CorporateSubtreeComponent', () => {
  let component: CorporateSubtreeComponent;
  let fixture: ComponentFixture<CorporateSubtreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CorporateSubtreeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CorporateSubtreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
