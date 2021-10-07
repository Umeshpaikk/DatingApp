import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_Models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() username: string;
  messageContent: string;
  @ViewChild('messageForm') messageForm: NgForm;
  messages: Message[] = [];

  constructor(public memservice: MessageService) { 
    // this.memservice.messageThread$.subscribe((msgs:any) => {
    //   this.messages = msgs.result;
    //   console.log("messages");
    //   console.log(this.messages);
    // })
  }

  ngOnInit(): void {
  }

  sendMessage(){
    this.memservice.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }

}
