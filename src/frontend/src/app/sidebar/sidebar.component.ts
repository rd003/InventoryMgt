import { ChangeDetectionStrategy, Component, inject, signal } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { Router, RouterModule } from "@angular/router";
import { Nav } from "../shared/models/Nav";
import NavData from '../data/route.data';
import { AuthStore } from "../auth/auth.store";

@Component({
  selector: "app-sidebar",
  imports: [MatButtonModule, MatIconModule, RouterModule],
  template: `
    <div class="sidebar" [class.collapsed]="isCollapsed()">
      <!-- Header -->
      <div class="sidebar-header">
        <div class="logo-container">
          <mat-icon class="logo-icon">inventory_2</mat-icon>
          @if(!isCollapsed()) {
            <span class="logo-text">InventoryMgt</span>
          }
        </div>
        <button 
          mat-icon-button 
          class="collapse-btn"
          (click)="toggleSidebar()"
        >
          <mat-icon>{{ isCollapsed() ? 'chevron_right' : 'chevron_left' }}</mat-icon>
        </button>
      </div>

      <!-- Navigation Menu -->
      <nav class="nav-menu">
        @for (navSection of navItems; track navSection.section) {
          <div class="nav-section">
            @if(!isCollapsed() && navSection.section) {
              <div class="nav-section-title">{{ navSection.section }}</div>
            }
            
            @for (navLink of navSection.links; track navLink.link) {
              <a 
                mat-button 
                [routerLink]="navLink.link" 
                routerLinkActive="active"
                class="nav-item"
                [title]="navLink.title"
              >
                <mat-icon>{{ navLink.icon }}</mat-icon>
                @if(!isCollapsed()) {
                  <span>{{ navLink.title }}</span>
                }
              </a>
            }
          </div>
        }
      </nav>

      <!-- Bottom Section -->
      <div class="sidebar-bottom">
        <button mat-button class="nav-item auth-btn" type="button" (click)="logout()">
          <mat-icon>logout</mat-icon>
          <span>Logout</span>
        </button>
        
        <a
          href="https://github.com/rd003/InventoryMgtAngular"
          target="_blank"
          mat-icon-button
          class="github-btn"
          title="View on GitHub"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="currentColor"
          >
            <path
              d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"
            />
          </svg>
        </a>
      </div>
    </div>
  `,
  styleUrl: "./sidebar.css",
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  isCollapsed = signal(false);
  navItems: Nav[] = NavData;
  authStore = inject(AuthStore);
  router = inject(Router);

  logout() {
    this.authStore.logout();
    this.router.navigate(['/login']);
  }

  toggleSidebar() {
    this.isCollapsed.update(value => !value);
  }
}
