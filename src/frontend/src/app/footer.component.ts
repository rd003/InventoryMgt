import { ChangeDetectionStrategy, Component } from "@angular/core";

@Component({
  selector: "app-footer",
  imports: [],
  template: `
    <div class="footer-content">
      <span class="footer-text">
        Built with .NET and Angular by
        <a
          href="https://twitter.com/ravi_devrani"
          target="_blank"
          class="author-link"
        >
          Ravindra Devrani
        </a>
      </span>
      <div class="footer-divider"></div>
      <span class="copyright">
        © {{ currentYear }} InventoryMgt. All rights reserved.
      </span>
    </div>
  `,
  styles: [
    `
      :host {
        background: white;
        border-top: 1px solid #e2e8f0;
        padding: 16px 0;
        margin-top: auto;
      }

      .footer-content {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 16px;
        flex-wrap: wrap;
        font-size: 14px;
        color: #64748b;
      }

      .footer-text {
        display: flex;
        align-items: center;
        gap: 4px;
      }

      .author-link {
        color: #3b82f6;
        text-decoration: none;
        font-weight: 500;
        transition: color 0.2s ease;
      }

      .author-link:hover {
        color: #1d4ed8;
        text-decoration: underline;
      }

      .footer-divider {
        width: 1px;
        height: 16px;
        background: #e2e8f0;
      }

      .copyright {
        color: #94a3b8;
        font-size: 13px;
      }

      @media (max-width: 640px) {
        .footer-content {
          flex-direction: column;
          gap: 8px;
        }

        .footer-divider {
          display: none;
        }
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
}