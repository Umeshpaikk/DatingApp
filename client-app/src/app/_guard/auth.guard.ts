import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_Models/user';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountservice: AccountService, private toaster: ToastrService)
  {

  }
  canActivate(): Observable<boolean> {
     return  this.accountservice.Currentuser$.pipe( 
      map( (user:User) => {
        if(user) return true;
        this.toaster.error("Error while routing. Implemented by CanActiviate from AuthGaurd ");
        return false;
      }
      )
    );
  }
  
}
