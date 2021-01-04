import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'UserInterface';
  activeTab = "LogIn";

  onCurrentTabChange(nav: {activeTab: string}){
    this.activeTab = nav.activeTab;
  }
}
