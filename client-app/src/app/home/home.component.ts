import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
registerMode = false;
Users : any ={};
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    //this.GetUsers();
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

  GetUsers = () => {
    this.http.get('https://localhost:5001/api/Users').subscribe(
      response => {
        this.Users = response;
      },
      error => {
        console.log(error);
      }
    )
  }

  cancelRegister(event2: boolean){
    console.log("Cancel on parent");
      this.registerMode = event2;
  }

}
