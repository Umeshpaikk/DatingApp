import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

  constructor(private accservice: AccountService, private toast: ToastrService){}

  canActivate(): Observable<boolean>{
    return this.accservice.Currentuser$.pipe(
      map( user=> {
        if(user.roles.includes("Admin") || user.roles.includes("Moderator")){
          return true;
        }
        this.toast.error('No admin/mod access');
        return false;
      }
      )
      
    )
  }
  
}
