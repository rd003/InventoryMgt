import { ChangeDetectionStrategy, Component, signal } from "@angular/core";
import { MatButtonModule } from "@angular/material/button";
import { MatIconModule } from "@angular/material/icon";
import { RouterModule } from "@angular/router";

@Component({
  selector: "app-sidebar",
  imports: [MatButtonModule, MatIconModule, RouterModule],
  template: `
    <div class="sidebar" [class.collapsed]="isCollapsed()">
      <!-- Header -->
      <div class="sidebar-header">
        <div class="logo-container">
          <mat-icon class="logo-icon">inventory_2</mat-icon>
          @if(!isCollapsed()){
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
        <div class="nav-section">
          @if(!isCollapsed()){
          <div class="nav-section-title">Main</div>
          }
          
          <a 
            mat-button 
            routerLink="/home" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>dashboard</mat-icon>
            @if(!isCollapsed()){
            <span>Dashboard</span>
            }
          </a>

          <a 
            mat-button 
            routerLink="/categories" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>category</mat-icon>
            @if(!isCollapsed()){
            <span>Categories</span>
            }
          </a>

          <a 
            mat-button 
            routerLink="/products" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>inventory</mat-icon>
            @if(!isCollapsed()){
            <span>Products</span>
            }
          </a>
        </div>

        <div class="nav-section">
          @if(!isCollapsed()){
          <div class="nav-section-title">Operations</div>
          }
          
          <a 
            mat-button 
            routerLink="/stock" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>warehouse</mat-icon>
            @if(!isCollapsed()){
            <span>Stock</span>
            }
          </a>

          <a 
            mat-button 
            routerLink="/purchases" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>shopping_cart</mat-icon>
            @if(!isCollapsed()){
            <span>Purchases</span>
            }
          </a>

          <a 
            mat-button 
            routerLink="/sales" 
            routerLinkActive="active"
            class="nav-item"
          >
            <mat-icon>point_of_sale</mat-icon>
            @if(!isCollapsed()){
            <span>Sales</span>
            }
          </a>
        </div>
      </nav>

      <!-- Bottom Section -->
      <div class="sidebar-bottom">
        <button mat-button class="nav-item auth-btn">
          <mat-icon>logout</mat-icon>
          @if(!isCollapsed()){
          <span>Logout</span>
          }
        </button>
        
        <a
          href="https://github.com/rd003/InventoryMgt"
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
  styles: [
    `
      .sidebar {
        width: 280px;
        height: 100vh;
        background: linear-gradient(180deg, #1e293b 0%, #334155 100%);
        border-right: 1px solid #e2e8f0;
        display: flex;
        flex-direction: column;
        transition: width 0.3s ease;
        box-shadow: 4px 0 6px -1px rgba(0, 0, 0, 0.1);
        position: relative;
        z-index: 1000;
      }

      .sidebar.collapsed {
        width: 64px;
      }

      .sidebar-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 16px;
        border-bottom: 1px solid rgba(255, 255, 255, 0.1);
        min-height: 64px;
      }

      .logo-container {
        display: flex;
        align-items: center;
        gap: 12px;
        color: white;
      }

      .logo-icon {
        color: #60a5fa;
        font-size: 28px;
        width: 28px;
        height: 28px;
      }

      .logo-text {
        font-size: 18px;
        font-weight: 600;
        white-space: nowrap;
      }

      .collapse-btn {
        color: #94a3b8 !important;
        background: rgba(255, 255, 255, 0.1) !important;
        transition: all 0.2s ease;
      }

      .collapse-btn:hover {
        background: rgba(255, 255, 255, 0.2) !important;
        color: white !important;
      }

      .nav-menu {
        flex: 1;
        padding: 24px 0;
        overflow-y: auto;
      }

      .nav-section {
        margin-bottom: 32px;
      }

      .nav-section-title {
        padding: 0 24px 12px;
        font-size: 12px;
        font-weight: 600;
        color: #94a3b8;
        text-transform: uppercase;
        letter-spacing: 0.5px;
      }

      .nav-item {
        width: 100%;
        height: 48px;
        display: flex !important;
        align-items: center;
        gap: 16px;
        padding: 0 24px !important;
        margin: 2px 0;
        color: #cbd5e1 !important;
        text-decoration: none;
        border-radius: 0 !important;
        font-weight: 500;
        transition: all 0.2s ease;
        justify-content: flex-start !important;
        position: relative;
      }

      .sidebar.collapsed .nav-item {
        padding: 0 20px !important;
        justify-content: center !important;
      }

      .nav-item:hover {
        background: rgba(255, 255, 255, 0.1) !important;
        color: white !important;
      }

      .nav-item.active {
        background: linear-gradient(90deg, #3b82f6, #1d4ed8) !important;
        color: white !important;
        position: relative;
      }

      .nav-item.active::before {
        content: '';
        position: absolute;
        left: 0;
        top: 0;
        bottom: 0;
        width: 4px;
        background: #60a5fa;
      }

      .nav-item mat-icon {
        font-size: 20px;
        width: 20px;
        height: 20px;
      }

      .nav-item span {
        white-space: nowrap;
      }

      .sidebar-bottom {
        padding: 16px;
        border-top: 1px solid rgba(255, 255, 255, 0.1);
        display: flex;
        flex-direction: column;
        gap: 8px;
      }

      .auth-btn {
        background: rgba(59, 130, 246, 0.1) !important;
        border: 1px solid rgba(59, 130, 246, 0.3) !important;
        color: #60a5fa !important;
      }

      .auth-btn:hover {
        background: rgba(59, 130, 246, 0.2) !important;
        color: white !important;
      }

      .github-btn {
        color: #94a3b8 !important;
        align-self: center;
        transition: all 0.2s ease;
      }

      .github-btn:hover {
        color: white !important;
        background: rgba(255, 255, 255, 0.1) !important;
      }

      /* Scrollbar styling */
      .nav-menu::-webkit-scrollbar {
        width: 4px;
      }

      .nav-menu::-webkit-scrollbar-track {
        background: transparent;
      }

      .nav-menu::-webkit-scrollbar-thumb {
        background: rgba(255, 255, 255, 0.2);
        border-radius: 2px;
      }

      .nav-menu::-webkit-scrollbar-thumb:hover {
        background: rgba(255, 255, 255, 0.3);
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent {
  isCollapsed = signal(false);

  toggleSidebar() {
    this.isCollapsed.update(value => !value);
  }
}