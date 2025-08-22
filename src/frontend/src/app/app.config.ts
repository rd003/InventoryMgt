import { ApplicationConfig, ErrorHandler } from "@angular/core";
import { provideRouter } from "@angular/router";

import { routes } from "./app.routes";
import { provideAnimationsAsync } from "@angular/platform-browser/animations/async";
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from "@angular/common/http";
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from "@angular/material/form-field";
import { GlobalErrorHander } from "./shared/global-error-handler";
import { ErrorInterceptor } from "./shared/error.interceptor";
import { MAT_DATE_LOCALE } from "@angular/material/core";
import { authInterceptor } from "./interceptors/auth.interceptor";
import { httpInterceptor } from "./interceptors/http.interceptor";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideAnimationsAsync(),
    { provide: MAT_DATE_LOCALE, useValue: "en-IN" },

    provideHttpClient(
      withInterceptorsFromDi(),
      withInterceptors([authInterceptor, httpInterceptor])),
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        subscriptSizing: "dynamic",
      },
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHander,
    },
  ],
};
