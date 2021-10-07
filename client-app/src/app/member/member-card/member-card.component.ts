import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_Models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
@Input() member: Member;
membersName : string [] = [];
  constructor(private memberservice: MembersService, private toast:ToastrService
    , public presence : PresenceService) { 
      this.presence.onlineUsers$.subscribe( res => {
        this.membersName =  res;
      })
    }

  ngOnInit(): void {

  }

  addLikes(member: Member){
    console.log("aadd likes");

    this.memberservice.addLike(member.username).subscribe(resp =>{
      this.toast.success("You have liked " + member.knownAs);
    });
  }

}
