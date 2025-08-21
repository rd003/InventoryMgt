import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { LoginModel } from "./login/login.model";

@Injectable({ providedIn: "root" })
export class AuthService {
    private readonly url = environment.API_BASE_URL + "/auth";
    private readonly http = inject(HttpClient);

    login = (loginData: LoginModel) => this.http.post<void>(this.url + "/login", loginData);

    me = () => this.http.get<any>(this.url + "/me");

    refresh = () => this.http.get<void>(this.url + "/refresh");

    logout = () => this.http.post<void>(this.url + "/logout", null);
}