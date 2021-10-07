import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_Models/user';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUsersSource = new BehaviorSubject<string[]>(["sss"]);
  onlineUsers$ = this.onlineUsersSource.asObservable();


  constructor(private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User)
  {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl+'presence', {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()
    
    this.hubConnection.start().catch(e => console.log(e));
   
    this.hubConnection.on( "UserIsOnline", username => {
      this.toastr.info(username + " Is online");
    })

    this.hubConnection.on( "UserIsOffline", username => {
      this.toastr.info(username + " Is disconnected");
    })

    this.hubConnection.on("GetOnlineUsers", (usernames: any) => {
      console.log("Umesh");
      this.onlineUsersSource.next(usernames.result);
      console.log(usernames.result);
    });

    
    this.hubConnection.on("NewMessageReceived", (userName, knownAs) => {
      this.toastr.info(knownAs + "Has sent you a new Messge")
        .onTap.pipe(take(1)).subscribe(()=> this.router.navigateByUrl('/members/' + userName + '?tab=3'));
    })

  }

  /*
  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection
      .start()
      .catch(error => console.log(error));

    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames, username])
      })
    })

    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
        this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
      })
    })

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })

    this.hubConnection.on('NewMessageReceived', ({username, knownAs}) => {
      this.toastr.info(knownAs + ' has sent you a new message!')
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
    })
  }
  */

  stopHubConnection(){
    this.hubConnection.stop().catch(err => console.log(err));
  }

}
