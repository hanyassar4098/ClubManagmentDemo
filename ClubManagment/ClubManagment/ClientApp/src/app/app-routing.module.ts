import { NgModule, Injectable } from '@angular/core';
import { Routes, RouterModule, DefaultUrlSerializer, UrlSerializer, UrlTree } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { CustomersComponent } from './components/customers/customers.component';
import { SettingsComponent } from './components/settings/settings.component';
import { AboutComponent } from './components/about/about.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AuthService } from './services/auth.service';
import { AuthGuard } from './services/auth-guard.service';
import { Utilities } from './services/utilities';
import { ActivitiesComponent } from './components/activities/activities.component';
import { SubscriptionComponent } from './components/subscription/subscription.component';
import { TodoDemoComponent } from './components/controls/todo-demo.component';


@Injectable()
export class LowerCaseUrlSerializer extends DefaultUrlSerializer {
  parse(url: string): UrlTree {
    const possibleSeparators = /[?;#]/;
    const indexOfSeparator = url.search(possibleSeparators);
    let processedUrl: string;

    if (indexOfSeparator > -1) {
      const separator = url.charAt(indexOfSeparator);
      const urlParts = Utilities.splitInTwo(url, separator);
      urlParts.firstPart = urlParts.firstPart.toLowerCase();

      processedUrl = urlParts.firstPart + separator + urlParts.secondPart;
    } else {
      processedUrl = url.toLowerCase();
    }

    return super.parse(processedUrl);
  }
}


const routes: Routes = [
  { path: '', component: SettingsComponent, canActivate: [AuthGuard], data: { title: 'Home' } },
  { path: 'login', component: LoginComponent, data: { title: 'Login' } },
  // { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard], data: { title: 'Customers' } },
  // { path: 'activities', component: ActivitiesComponent, canActivate: [AuthGuard], data: { title: 'Activities' } },
  { path: 'services', component: TodoDemoComponent, canActivate: [AuthGuard], data: { title: 'services' } },
  { path: 'settings', component: SettingsComponent, canActivate: [AuthGuard], data: { title: 'Settings' } },
  { path: 'about', component: AboutComponent, data: { title: 'About Us' } },
  { path: 'home', redirectTo: '/', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent, data: { title: 'Page Not Found' } }
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [
    AuthService,
    AuthGuard,
    { provide: UrlSerializer, useClass: LowerCaseUrlSerializer }]
})
export class AppRoutingModule { }
