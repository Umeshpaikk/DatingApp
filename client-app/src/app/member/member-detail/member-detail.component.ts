import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_Models/member';
import { Message } from 'src/app/_Models/message';
import { User } from 'src/app/_Models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  member: Member;
  galleryOptions:NgxGalleryOptions[];
  galleryimages: NgxGalleryImage[]; 
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User;

  constructor(private memberService: MembersService, private router: ActivatedRoute, 
    private msgservice: MessageService, public presece: PresenceService, private accService: AccountService) {
      this.accService.Currentuser$.pipe(take(1)).subscribe(res => {
        this.user = res;
      });
     }


  ngOnInit(): void {

    this.router.data.subscribe(data => {
      this.member = data.member;
      console.log("roter data")

      
    this.router.queryParams.subscribe( params => params.tab ? this.selectTab(params.tab): this.selectTab(0))
    
    })

    

    this.galleryOptions =  [
      {
        width:'500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }
    ]

    this.galleryimages = this.getImages();
  }

  getImages(){
    const imageUrl = [];
    for(const photo of this.member.photos){
      imageUrl.push(
        {
          small: photo?.url,
          medium: photo?.url,
          large: photo?.url
        }
      );
    }
    
    return imageUrl;
  }

  loadMember(){
    const username = this.router.snapshot.paramMap.get('username');
    if(username){
        this.memberService.getMember(username).subscribe(response => {
          this.member = response;
          this.galleryimages = this.getImages();
        });
    }
  }

  loadMessages(){
    this.msgservice.getMessageThread(this.member.username).subscribe(response =>{
      this.messages = response;
    });
  }

  selectTab(tabId: number) {
    console.log(tabId);    
    this.memberTabs!.tabs[tabId]!.active = true;
  }


  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading == "Messages" && this.messages.length === 0){
        console.log("Start Hub");
        this.msgservice.createHubConnection(this.user, this.member.username);
    }
    else
    {
      console.log("Stop Hub");
      this.msgservice.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.msgservice.stopHubConnection();
  }

}
