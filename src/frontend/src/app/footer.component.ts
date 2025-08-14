import { ChangeDetectionStrategy, Component } from "@angular/core";

@Component({
  selector: "app-footer",
  imports: [],
  template: `
    Built with .NET and Angular by
    <a
      style="color: black;"
      href="https://twitter.com/ravi_devrani"
      target="_blank"
    >
      Ravindra Devrani
    </a>
  `,
  styles: [
    `
      :host {
        padding: 10px 0px;
        text-align: center;
        font-size: 16px;
      }
    `,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent { }
