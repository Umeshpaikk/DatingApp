import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { Member } from '../_Models/member';
import { MembersService } from '../_services/members.service';

@Injectable({
  providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {

  constructor(private memservice: MembersService){}

  resolve(route: ActivatedRouteSnapshot): Observable<Member> {
    //@ts-ignore
    return this.memservice.getMember(route.paramMap.get('username'));
  }
}
