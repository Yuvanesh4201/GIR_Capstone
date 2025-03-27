import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GirGraphCytoComponent } from './gir-graph-cyto/gir-graph-cyto.component';
import { GirSearchComponent } from './gir-search/gir-search.component';
import { LoaderComponent } from './components/loader/loader.component';
import { LoaderInterceptorService } from './interceptors/loader-interceptor.service';
import { CorporateEntityInfoComponent } from './components/corporate-entity-info/corporate-entity-info.component';
import { OwnershipInfoComponent } from './components/ownership-info/ownership-info.component';
import { OwnershipListComponent } from './components/ownership-list/ownership-list.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { MatTableModule } from '@angular/material/table';
import { CorporateStatusLegendComponent } from './components/corporate-status-legend/corporate-status-legend.component';
import { CorporateSubtreeComponent } from './components/corporate-subtree/corporate-subtree.component';
import { CorporateSearchComponent } from './components/corporate-search/corporate-search.component';
import { GirParseFileComponent } from './gir-parse-file/gir-parse-file.component';
import { CorporateSortComponent } from './components/corporate-sort/corporate-sort.component';

@NgModule({
  declarations: [
    AppComponent,
    GirGraphCytoComponent,
    GirSearchComponent,
    LoaderComponent,
    CorporateEntityInfoComponent,
    OwnershipInfoComponent,
    OwnershipListComponent,
    CorporateStatusLegendComponent,
    CorporateSubtreeComponent,
    CorporateSearchComponent,
    GirParseFileComponent,
    CorporateSortComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    MatTableModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptorService, multi: true },
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
