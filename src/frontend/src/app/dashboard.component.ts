import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  imports: [],
  template: `
    <h1>
      Welcome
    </h1>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashbardComponent {

}
