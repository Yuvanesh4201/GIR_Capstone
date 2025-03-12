import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GirParseFileComponent } from './gir-parse-file.component';

describe('GirParseFileComponent', () => {
  let component: GirParseFileComponent;
  let fixture: ComponentFixture<GirParseFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GirParseFileComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GirParseFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
