import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MessagesComponent } from '../messages/messages.component';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {}
  Users : any = {}

  constructor(public accountservice: AccountService, private router: Router, private toaster: ToastrService) { 
  }

  ngOnInit(): void {
    this.getCurrentUser();
  }

  login() {
    this.accountservice.login(this.model).subscribe(response  => {
        this.Users =  response;
        this.router.navigateByUrl("/messages");
        this.toaster.info("User "+ this.Users.username+ " Logged in successfully");
      },
      error => {
        console.log(error);
      }
      )

  }

  logout(){
    this.router.navigateByUrl("/");
    this.accountservice.logout();
  }

  getCurrentUser(){
    this.accountservice.Currentuser$.subscribe( user=> {
    },
    error => {
      console.log(error);
    }
    )
  }


}
