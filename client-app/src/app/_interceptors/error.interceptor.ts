import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError( reqError => 
        {
          console.log(reqError.error);
        if(reqError)
        {
            switch(reqError.status)
            {

              case 400:
                if(reqError.error.errors)
                {
                  const modelstateErrors = [];
                  for(const key in reqError.error.errors)
                  {
                      if(reqError.error.errors[key]) modelstateErrors.push(reqError.error.errors[key]);
                  }
                  throw modelstateErrors.flat();
                }
                else{
                    this.toastr.error(reqError.status, reqError.statusText);
                }
                break;

                case 401:
                  this.toastr.error(reqError.status, reqError.error);
                  break;

                case 404:
                  this.router.navigateByUrl('/not-found');
                  break;
                case 500:
                  const navigationextra : NavigationExtras = { state: { error: reqError.error}};
                  this.router.navigateByUrl('/server-error', navigationextra);
                  break;
                default:
                  this.toastr.error("something unexpected went wrong Umesh");
                  console.log(reqError);
                  break;  
            }
        }
        return throwError(reqError);
      })
    );
  } 
}
