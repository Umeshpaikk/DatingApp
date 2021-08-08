import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  AppTitle = 'Dating Application';
  Users : any;
  constructor(private http: HttpClient)  {

  }
  ngOnInit(): void {

    this.MakeWebCall();

  }

  MakeWebCall = () => {
    this.http.get('https://localhost:5001/api/AppUser').subscribe(
      response => {
        this.Users = response;
      },
      error => {
        console.log(error);
      }
    )
  }

}
