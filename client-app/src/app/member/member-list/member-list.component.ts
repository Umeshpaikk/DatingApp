import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_Models/member';
import { PaginatedResult, Pagination } from 'src/app/_Models/Pagination';
import { User } from 'src/app/_Models/user';
import { UserParams } from 'src/app/_Models/userParam';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
members : Member [] = [];
pagination: Pagination;
pagSize: number = 2;
userparam: UserParams;
genderList = [{value: 'male', display: 'Males'},{value: 'female', display: 'Females'}];

  constructor(private memberservice: MembersService) {
    this.userparam = this.memberservice.getUserParams();
   }

  ngOnInit(): void {
     this.loadMembers();
  }

  loadMembers(){
    this.memberservice.setUserParams(this.userparam);
    console.log("loadMembers call");
    this.memberservice.getMembers(this.userparam).subscribe((response) =>{
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }

  resetFilters() {
    this.userparam = this.memberservice.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any)
  {
    this.userparam.pageNumber = event.page;
    this.memberservice.setUserParams(this.userparam);
    this.loadMembers();
  }

}
