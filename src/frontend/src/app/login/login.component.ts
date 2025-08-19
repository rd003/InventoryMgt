import { ChangeDetectionStrategy, Component, inject, signal } from "@angular/core";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { MatIconModule } from "@angular/material/icon";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { Router } from "@angular/router";
import { LoginModel } from "./login.model";
import { NotificationComponent } from "../shared/notification.component";

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

    hidePassword = signal(true);
    isLoading = signal(false);

    loginForm: FormGroup = this.fb.group({
        username: ['', [Validators.required, Validators.minLength(3)]],
        password: ['', [Validators.required, Validators.minLength(3)]]
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
        this.isLoading.set(true);
        const formData: LoginModel = this.loginForm.value;
        console.log(formData);
    }

    constructor() {

    }
}