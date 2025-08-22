import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { AuthStore } from './auth/auth.store';

@Component({
  selector: 'app-dashboard',
  imports: [],
  template: `
    <h1>
      Welcome {{user$()?.username}} !!
    </h1>
  `,
  styles: ``,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashbardComponent {
  authStore = inject(AuthStore);
  user$ = this.authStore.user;
}
