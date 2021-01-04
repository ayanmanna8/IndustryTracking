import { StoresComponent } from './stores/stores.component';
import { LogInComponent } from './log-in/log-in.component';
import { NgModule, Component } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { StoreDetailsComponent } from './stores/store-details/store-details.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full'},
  { path: 'login', component: LogInComponent},
  { path: 'stores', component: StoresComponent},
  { path: 'storedetails', component: StoreDetailsComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
