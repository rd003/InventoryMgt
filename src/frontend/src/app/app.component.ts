import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { FooterComponent } from "./footer.component";
import { NotificationComponent } from "./shared/notification.component";
import { SidebarComponent } from "./sidebar/sidebar.component";

@Component({
  selector: "app-root",
  imports: [
    RouterOutlet,
    SidebarComponent,
    FooterComponent,
    NotificationComponent,
  ],
  template: `
  @if(!isAuthenticated){
    <router-outlet />
  }
  
  @else{
    <div class="app-container">
      <app-sidebar />
      <div class="main-content">
        <app-notification />
        <div class="content-page">
          <router-outlet />
        </div>
        <app-footer />
      </div>
    </div>
  }
  `,
  styles: [
    `
      .app-container {
        height: 100vh;
        width: 100%;
        display: flex;
        overflow: hidden;
      }
      
      .main-content {
        flex: 1;
        display: flex;
        flex-direction: column;
        overflow: hidden;
      }
      
      .content-page {
        padding: 24px;
        flex: 1;
        overflow-y: auto;
        background-color: #f8fafc;
      }
    `,
  ]
})
export class AppComponent {
  isAuthenticated = false;
  constructor() { }
}