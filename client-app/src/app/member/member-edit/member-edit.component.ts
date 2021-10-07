import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_Models/member';
import { User } from 'src/app/_Models/user';
import { AccountService } from 'src/app/_services/account.service';
import {MembersService } from 'src/app/_services/members.service'

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm : NgForm;
  member: Member;
  user: User;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any){

    console.log($event);
    
    if(this.editForm?.dirty){
      $event.returnValue = true;
    }
    
  }

  constructor(private accountservice:AccountService, private memberservice: MembersService, private toaster: ToastrService) { 
    this.accountservice.Currentuser$.pipe(take(1)).subscribe(user=> this.user =  user);
  }

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember(){
    this.memberservice.getMember(this.user.username).subscribe( member => this.member = member);
  }

  updateMembers(){
    
    this.memberservice.updateMember(this.member).subscribe( () => {
      this.toaster.success("Success");
      this.editForm.reset(this.member);
    });
  }
}
