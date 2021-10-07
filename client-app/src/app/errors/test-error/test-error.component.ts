import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.css']
})
export class TestErrorComponent implements OnInit {
  baseUrl = environment.apiUrl;
  validationerror : string []  = [];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  getAutherror(){
    this.http.get(this.baseUrl+'buggy/auth').subscribe(
      resp=>{
        console.log(resp);
      },
      error =>{
        console.log(error);
      }   
    );
  }

  getNotFoundError(){
    this.http.get(this.baseUrl+'buggy/not-found').subscribe(
      resp=>{
        console.log(resp);
      },
      error =>{
        console.log(error);
      }   
    );
  }

  getServerError(){
    this.http.get(this.baseUrl+'buggy/server-error').subscribe(
      resp=>{
        console.log(resp);
      },
      error =>{
        console.log(error);
      }   
    );
  }

  getBadRequest(){
    this.http.get(this.baseUrl+'buggy/bad-request').subscribe(
      resp=>{
        console.log(resp);
      },
      error =>{
        console.log(error);
      }   
    );
  }

  getvalidationErrr(){
    this.http.post(this.baseUrl+'account/register', {}).subscribe(
      resp=>{
        console.log(resp);
      },
      error =>{
        this.validationerror =  error;
      }   
    );
  }

}
