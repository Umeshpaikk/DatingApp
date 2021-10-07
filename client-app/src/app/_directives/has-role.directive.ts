import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_Models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit{
  user: User;
  @Input() appHasRole: string[];

  constructor(private viewcontainerref: ViewContainerRef, private templateref: TemplateRef<any>,
    private accservice: AccountService) { 
      this.accservice.Currentuser$.pipe(take(1)).subscribe(user =>
        this.user = user)
    }

  ngOnInit(): void {
    if(!this.user?.roles || this.user ==null){
        this.viewcontainerref.clear();
        return;
    }

    if(this.user?.roles.some(r => this.appHasRole.includes(r)))
    {
      this.viewcontainerref.createEmbeddedView(this.templateref);
    } 
    else 
    {
      this.viewcontainerref.clear();
    }


   }

}
