import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OwnershipInfoComponent } from './ownership-info.component';

describe('OwnershipInfoComponent', () => {
  let component: OwnershipInfoComponent;
  let fixture: ComponentFixture<OwnershipInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OwnershipInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OwnershipInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
