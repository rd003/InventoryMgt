import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { FooterComponent } from "./footer.component";
import { NotificationComponent } from "./shared/notification.component";
import { SidebarComponent } from "./sidebar/sidebar.component";
import { AuthStore } from "./auth/auth.store";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: "app-root",
  imports: [
    RouterOutlet,
    SidebarComponent,
    FooterComponent,
    NotificationComponent,
    MatProgressSpinnerModule,
  ],
  template: `
  @if(authStore.loading()){
    <div class="loading-container">
      <mat-spinner diameter="50"></mat-spinner>
      <p class="loading-text">Loading...</p>
    </div>
  }
  @else if(!authStore.authenticated()){
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

      .loading-container {
        height: 100vh;
        width: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        background-color: #fafafa;
        gap: 16px;
      }

      .loading-text {
        margin: 0;
        color: #666;
        font-size: 16px;
      }
    `,
  ]
})
export class AppComponent {
  authStore = inject(AuthStore);

  constructor() { }
}