import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable, timeout, finalize, catchError, throwError } from 'rxjs';
import { LoaderService } from '../services/loader.service';

@Injectable()
export class LoaderInterceptorService implements HttpInterceptor {
  constructor(private loaderService: LoaderService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loaderService.show(); // Show loader when request starts

    return next.handle(req).pipe(
      timeout(10000), // Set timeout for the API call (10 seconds)
      finalize(() => this.loaderService.hide()), // Hide loader when API completes (success/failure/timeout)
      catchError(error => {
        console.error('API Error:', error);
        return throwError(() => error); // Rethrow the error after logging
      })
    );
  }
}
