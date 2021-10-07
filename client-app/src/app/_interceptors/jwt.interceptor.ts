import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_Models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs/operators';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private accountservice: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let CurrentUser : any;
    this.accountservice.Currentuser$.pipe(take(1)).subscribe(response=> CurrentUser =  response);
    
    if(CurrentUser){
        request =  request.clone({
          setHeaders: {Authorization: `Bearer ${CurrentUser.token}`}
        });
    }
    else{
      console.log("Login request. Token is yet to be obtained");
    }
    return next.handle(request);
  }
}
