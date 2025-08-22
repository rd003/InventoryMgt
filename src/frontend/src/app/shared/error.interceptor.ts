import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { Observable, catchError, throwError } from "rxjs";
import { NotificationService } from "./notification.service";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private notificationService = inject(NotificationService);

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err) => {
        console.error(err);

        // Skip notifications for auth endpoints that are expected to fail
        if (this.shouldSkipNotification(err, req.url)) {
          return throwError(() => err);
        }

        const message = this.getErrorMessage(err, req.url);
        this.notificationService.send({
          id: "",
          message,
          severity: "error",
        });
        return throwError(() => err);
      })
    );
  }

  private shouldSkipNotification(error: HttpErrorResponse, url: string): boolean {
    // Skip notifications for specific auth endpoints that are expected to fail silently
    const silentAuthEndpoints = [
      '/auth/me',
      '/auth/refresh'
    ];

    const isSilentAuthEndpoint = silentAuthEndpoints.some(endpoint => url.includes(endpoint));

    // Skip 401 errors on silent auth endpoints (they're handled by auth interceptor)
    if (isSilentAuthEndpoint && error.status === 401) {
      return true;
    }

    // Skip 403 errors on auth endpoints (redirect is handled by auth interceptor)  
    if (isSilentAuthEndpoint && error.status === 403) {
      return true;
    }

    return false;
  }

  private getErrorMessage(error: HttpErrorResponse, url: string): string {
    // Check if server provided a custom error message
    const serverMessage = error.error?.message || error.error?.error;

    // Handle specific error codes from your backend
    if (error.error?.code) {
      switch (error.error.code) {
        case 'INVALID_CREDENTIALS':
          return 'Invalid username or password. Please try again.';
        case 'ACCOUNT_LOCKED':
          return 'Your account has been locked. Please contact support.';
        case 'INSUFFICIENT_PERMISSIONS':
          return 'You do not have permission to perform this action.';
        case 'RESOURCE_NOT_FOUND':
          return 'The requested resource was not found.';
        case 'VALIDATION_ERROR':
          return serverMessage || 'Please check your input and try again.';
      }
    }

    switch (error.status) {
      case 400:
        return serverMessage || 'Bad request. Please check your input and try again.';
      case 401:
        // Check if it's a login endpoint
        if (this.isLoginEndpoint(url)) {
          return serverMessage || 'Invalid username or password. Please try again.';
        }
        return serverMessage || 'Your session has expired. Please log in again.';
      case 403:
        return serverMessage || 'You do not have permission to access this resource.';
      case 404:
        return serverMessage || 'The requested resource was not found.';
      case 422:
        return serverMessage || 'Validation failed. Please check your input.';
      case 500:
        return serverMessage || 'Internal server error. Please try again later or contact support.';
      case 502:
        return 'Service temporarily unavailable. Please try again later.';
      case 503:
        return 'Service unavailable. Please try again later.';
      case 504:
        return 'Request timeout. Please try again.';
      case 0:
        return 'Network error. Please check your internet connection.';
      default:
        if (error.status >= 500) {
          return serverMessage || 'Server error occurred. Please try again later or contact support.';
        } else if (error.status >= 400) {
          return serverMessage || 'Client error occurred. Please check your request.';
        }
        return serverMessage || 'An unexpected error occurred. Please try again.';
    }
  }

  private isLoginEndpoint(url: string): boolean {
    // Adjust these patterns based on your API endpoints
    const loginPatterns = [
      '/auth/login',
      '/login',
      '/authenticate',
      '/signin',
      '/api/auth/login'
    ];
    return loginPatterns.some(pattern => url.includes(pattern));
  }
}