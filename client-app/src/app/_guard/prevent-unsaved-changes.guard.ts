import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../member/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  constructor(){
    console.log("constructed");
  }
  canDeactivate(
    component: MemberEditComponent): boolean {
      console.log("constructed  2");
      if(component.editForm?.dirty){
        return confirm("Are you sure you want to cancel the changes.?");
      }
    return true;
  }
  
}
