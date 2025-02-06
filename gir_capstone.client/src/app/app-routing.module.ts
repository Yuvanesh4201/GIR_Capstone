import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GirSearchComponent } from './gir-search/gir-search.component';
import { GirGraphCytoComponent } from './gir-graph-cyto/gir-graph-cyto.component';

const routes: Routes = [
  { path: '', component: GirSearchComponent },
  { path: 'gir-cyto-graph', component: GirGraphCytoComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
