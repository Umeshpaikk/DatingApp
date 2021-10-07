import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_Models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
baseUrl = environment.apiUrl;
private CurrentUserSource = new ReplaySubject<User>(1);
Currentuser$ = this.CurrentUserSource.asObservable();

  constructor(private http: HttpClient, private presence : PresenceService) { 

  }

  login(model:any) {
    return this.http.post<User>(this.baseUrl+'account/login',model).pipe(
      map((response: User) => {
        const user = response;
        if(user)
        {
          console.log(user);
          this.SetCurrentUser(response);
        }
        return user;
      })
    )
  }

  register(model:any){
    return this.http.post<User>(this.baseUrl+'account/register',model).pipe(
      map( (response: User) => {
          if(response){
            this.SetCurrentUser(response);
          }

          return response;
      })
    );
  }

  SetCurrentUser(user: User){
    if(user != null)
    {
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
      Array.isArray(roles) ? user.roles = roles: user.roles.push(roles);

      localStorage.setItem('user',JSON.stringify(user));
      this.CurrentUserSource.next(user);
      this.presence.createHubConnection(user);
    }
    else{
      console.log("Logging off");
      localStorage.removeItem('user');
      this.CurrentUserSource.next(null!);
      this.presence.stopHubConnection();
    }

  }

  logout() {
     //@ts-ignore
    this.SetCurrentUser(null);
  }

  getDecodedToken(token: string){
   
    return JSON.parse(atob(token.split('.')[1]))
  }
}
