import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { Role } from 'src/app/_Models/role';
import { User } from 'src/app/_Models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]> = [];
  bsModalref: BsModalRef;
  constructor(private admService: AdminService, private modalservice: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles(){
    this.admService.getUsersWithRoles().subscribe(users =>{
      this.users = users;
    })
  }
  openRolesModal(user?: User) {
    
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    this.bsModalref = this.modalservice.show(RolesModalComponent, config);
    this.bsModalref?.content?.updateSelectedRoles.subscribe( (values: Role[]) => {
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate && user) {
        this.admService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
          user.roles = [...rolesToUpdate.roles]
        })
      }
    })
    
  }

  private getRolesArray(user?: User) {
    
    const roles : Role [] = [];
    const userRoles = user?.roles;
    const availableRoles: Role[] = [
      {name: 'Admin', value: 'Admin', checked : false},
      {name: 'Moderator', value: 'Moderator', checked: false},
      {name: 'Member', value: 'Member', checked: false}
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      if(userRoles){
        for (const userRole of userRoles) {
          if (role.name === userRole) {
            isMatch = true;
            role.checked = true;
            roles.push(role);
            break;
          }
        }
      }
      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    })
    return roles;
  }
  
}
