import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_Models/message';
import { User } from '../_Models/user';
import { UserParams } from '../_Models/userParam';
import { AccountService } from './account.service';
import { getPaginatedResults, getPaginationHeader } from './paginationhelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  messageparam : UserParams;
  user: User;
  hubConnection: HubConnection;
  private MessageThreadSource = new BehaviorSubject<Message []> ([])
  messageThread$ = this.MessageThreadSource.asObservable();

  constructor(private http: HttpClient, private accountservice: AccountService) {
    this.accountservice.Currentuser$.pipe(take(1)).subscribe( user =>{
      this.user = user;
      this.messageparam = new UserParams(user);
    });
  }

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection.start().catch((error: any) => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', messages => {
      console.log("ReceiveMessageThread");
      console.log(messages.result);
      this.MessageThreadSource.next(messages.result);
      console.log("ReceiveMessageThread end");
    })

    this.hubConnection.on('NewMessage', message => {
      console.log(message);
      console.log("NewMessageX");
      this.messageThread$.pipe(take(1)).subscribe(messages => {
        console.log(messages);
        console.log("NewMessageY");
        this.MessageThreadSource.next([...messages, message])
      })
    })

  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }


  getMessages(messagesparam: UserParams){
    
    messagesparam.pagingType = "MESSAGE";
    let params = getPaginationHeader(messagesparam);

    return getPaginatedResults< Partial< Message []>>(this.baseUrl+'messages', params, this.http);
  }

    
  getMsgParams(){
    return this.messageparam;
  }
  
  setMsgParams(eparam: UserParams){
    this.messageparam = eparam;
  }


  getMessageThread(username: string){
    return this.http.get<Message []>(this.baseUrl+"messages/thread/"+username);
  }

  async sendMessage(username:string, content:string) {
    return this.hubConnection.invoke("SendMessage", {recipientUsername: username, content}).catch(err=>console.log(err))
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
