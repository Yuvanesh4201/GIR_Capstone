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

@NgModule({
  declarations: [
    AppComponent,
    GirGraphCytoComponent,
    GirSearchComponent,
    LoaderComponent,
    CorporateEntityInfoComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptorService, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
