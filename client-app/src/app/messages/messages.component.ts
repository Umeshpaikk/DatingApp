import { Component, OnInit } from '@angular/core';
import { Message } from '../_Models/message';
import { Pagination } from '../_Models/Pagination';
import { UserParams } from '../_Models/userParam';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages: Partial< Message[]> = [];
  pagination: Pagination;
  messageparam: UserParams;
  loading = false;

  constructor(private mesgservice: MessageService) { 
    this.messageparam = mesgservice.getMsgParams();
    this.messageparam.pageNumber =  1;
    this.messageparam.pageSize = 2;
  }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.mesgservice.getMessages(this.messageparam).subscribe(resp =>
     {
        this.messages = resp.result;
        this.pagination = resp.pagination;
        this.loading = false
    })
  }

  deleteMessage(id?: number) {

    if(id !== undefined){
      this.mesgservice.deleteMessage(id).subscribe(() => {
        this.messages.splice(this.messages.findIndex(m => m?.id === id), 1);
      })
  }
  }


  pageChanged(event: any)
  {
    this.messageparam.pageNumber = event.page;
    this.mesgservice.setMsgParams(this.messageparam);
    this.loadMessages();
  }

}
