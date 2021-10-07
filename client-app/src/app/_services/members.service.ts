import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { ignoreElements, map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_Models/member';
import { PaginatedResult, Pagination } from '../_Models/Pagination';
import { User } from '../_Models/user';
import { UserParams } from '../_Models/userParam';
import { AccountService } from './account.service';
import { getPaginationHeader, getPaginatedResults } from './paginationhelper';


@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[];
  memberCache = new Map();
  userparam: UserParams ;
  likesparam: UserParams;
  user: User;
  

  constructor(private http: HttpClient, private accountservice: AccountService) { 
    this.accountservice.Currentuser$.pipe(take(1)).subscribe( user =>{
      this.user = user;
      this.userparam = new UserParams(user);
      this.likesparam = new UserParams(user);
    });
  }

  getUserParams(){
    return this.userparam;
  }
  
  setUserParams(userparam: UserParams){
    this.userparam = userparam;
  }

  
  getLikesParams(){
    return this.likesparam;
  }
  
  setLikesParams(_likesparam: UserParams){
    this.likesparam = _likesparam;
  }

  resetUserParams(){
    this.userparam =  new UserParams(this.user);
    return this.userparam;
  }

  getMembers(userparam: UserParams)  {
    var resp = this.memberCache.get(Object.values(userparam).join("-"));
    if(resp){
      console.log("request found"+ resp);
      return of(resp);
    }

    console.log("No request");
    userparam.pagingType = "USER";

    let params = getPaginationHeader(userparam);
    
    return getPaginatedResults<Member[]>(this.baseUrl + 'users', params, this.http)
    .pipe(
      map((resp:any) => {
        this.memberCache.set(Object.values(userparam).join("-"), resp);
        return resp;
      })
    );
   
  }


  getMember(Name : string) {

    console.log("Gundanna");

    var member = [...this.memberCache.values()].reduce( (arr,curElement) => arr.concat(curElement.result), [])
    .find( (member:Member) => member.username === Name);

    if(member)
    {
      console.log(member);
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl+'users/'+ Name);
  }

  updateMember(member:Member){
    return this.http.put(this.baseUrl+'users', member).pipe(
      map( () =>{
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number){
    return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId, {});
  }

  deletePhoto(photoId: number){
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }


  addLike(username: string){
    return this.http.post(this.baseUrl+"likes/"+username, {});
  }

   getLikes(likesParam: UserParams){
     console.log(this.baseUrl+'likes?predicate='+likesParam.predicate);
     likesParam.pagingType = "LIKE"

     let params = getPaginationHeader(likesParam);

     return getPaginatedResults< Partial< Member []>>(this.baseUrl+'likes', params, this.http);
  }
}
