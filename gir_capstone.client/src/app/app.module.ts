import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GirGraphCytoComponent } from './gir-graph-cyto/gir-graph-cyto.component';
import { GirSearchComponent } from './gir-search/gir-search.component';

@NgModule({
  declarations: [
    AppComponent,
    GirGraphCytoComponent,
    GirSearchComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
