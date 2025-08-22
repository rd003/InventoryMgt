import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { Router } from "@angular/router";
import { LoginModel } from "./login.model";
import { NotificationComponent } from "../../shared/notification.component";
import { AuthStore } from "../auth.store";

@Component({
    selector: "app-login",
    imports: [
        ReactiveFormsModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatIconModule,
        MatCheckboxModule,
        NotificationComponent
    ],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent {
    private fb = inject(FormBuilder);
    private router = inject(Router);
    authStore = inject(AuthStore);

    hidePassword = signal(true);

    // TODO: remove the value of username and password after testing
    loginForm: FormGroup = this.fb.group({
        username: ['admin', [Validators.required, Validators.minLength(3)]],
        password: ['Admin@123', [Validators.required, Validators.minLength(3)]]
    });;

    togglePasswordVisibility(): void {
        this.hidePassword.update(value => !value);
    }

    onSubmit(): void {
        if (!this.loginForm.valid) {
            Object.keys(this.loginForm.controls).forEach(key => {
                this.loginForm.get(key)?.markAsTouched();
            });
            return;
        }
        this.authStore.login(this.loginForm.value as LoginModel);
        this.router.navigate(['/dashboard']);
    }

    constructor() {
        if (this.authStore.authenticated()) {
            this.router.navigate(['/dashboard']);
        }
    }
}