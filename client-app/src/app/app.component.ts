import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_Models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  AppTitle = 'unable to load application';
  Users : any;
  constructor(private accountservice : AccountService, private presenceservice: PresenceService)  {

  }
  ngOnInit(): void {

    this.setCurrentUser();

 

  }

  setCurrentUser(){
    const user : User = JSON.parse(localStorage.getItem('user')!);
    if(user){
      this.accountservice.SetCurrentUser(user);
      this.presenceservice.createHubConnection(user);
    }
  }
  
}
