import { computed, DestroyRef, inject, Injectable, signal } from "@angular/core";
import { AuthService } from "./auth.service";
import { UserModel } from "./user.model";
import { HttpErrorResponse } from "@angular/common/http";
import { LoginModel } from "./login/login.model";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

export interface AuthState {
    user: UserModel | null,
    loading: boolean,
    error: HttpErrorResponse | null;
}

@Injectable({ providedIn: 'root' })
export class AuthStore {
    private readonly initialState: AuthState = {
        user: null,
        loading: true,
        error: null
    }
    private readonly authService = inject(AuthService);
    private readonly destroyRef = inject(DestroyRef);

    private readonly state = signal(this.initialState);

    user = computed(() => this.state().user);
    loading = computed(() => this.state().loading);
    loaded = computed(() => !this.state().loading);
    error = computed(() => this.state().error);
    authenticated = computed(() => !!this.state().user);

    login = (loginData: LoginModel) => {
        this.setLoading();

        this.authService.login(loginData).pipe(
            takeUntilDestroyed(this.destroyRef),
        ).subscribe(
            {
                next: () => {
                    this.loadStore();
                },
                error: (err) => {
                    this.setError(err);
                }
            }
        )
    }
    public clearState = () => {
        this.state.set({
            user: null,
            loading: false,
            error: null
        });
    }

    logout = () => {
        this.setLoading();
        this.authService.logout().pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe({
            next: () => {
                this.clearState();
            },
            error: (err) => this.setError(err)
        }
        )
    }

    private setError = (error: HttpErrorResponse) => {
        console.log(error);
        this.state.update((prevState) => ({
            ...prevState,
            error,
            loading: false
        }))
    }

    private setLoading = () => {
        this.state.update((prevState) => ({
            ...prevState,
            loading: true
        }))
    }

    loadStore = () => {
        this.setLoading();
        this.authService.me().pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(
            {
                next: (user) => {
                    this.state.update((prevState) => ({
                        ...prevState,
                        user,
                        loading: false
                    }))
                },
                error: (err) => {
                    this.setError(err);
                }
            }
        )
    }

    constructor() {
        this.loadStore();
    }
}