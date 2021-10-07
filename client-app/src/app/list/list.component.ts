import { Component, OnInit } from '@angular/core';
import { Member } from '../_Models/member';
import { Pagination } from '../_Models/Pagination';
import { UserParams } from '../_Models/userParam';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.css']
})
export class ListComponent implements OnInit {
  members: Partial<Member[]>;
  likesParam: UserParams;
  predicate: string = "liked";
  pagination: Pagination;

  constructor(private memservice: MembersService) { 
    this.likesParam = this.memservice.getLikesParams();
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    console.log(this.predicate);
    this.memservice.getLikes(this.likesParam).subscribe( response => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
    }

    pageChanged(event: any)
    {
      this.likesParam.pageNumber = event.page;
      this.memservice.setLikesParams(this.likesParam);
      this.loadLikes();
    }

}
