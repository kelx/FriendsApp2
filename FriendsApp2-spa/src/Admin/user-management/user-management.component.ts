import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/_services/admin.service';
import { AlertifyService } from 'src/_services/alertify.service';
import { AuthService } from 'src/_services/auth.service';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  bsModalRef: BsModalRef;
  users: User[];
  currentUserId: number;

  constructor(private adminService: AdminService, private authService: AuthService,
    private modalService: BsModalService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getUsersWithRoles();
    this.currentUserId = +this.authService.decodedToken.nameid;
    console.log(this.authService.decodedToken);
  }
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe((users: User[]) => {
      users.forEach((el, i) => {
        if (el.userName === 'Kelmen' && el.userName !== this.authService.decodedToken.unique_name) {
          el.roles.forEach((kl, j) => {
            if (kl === 'SuperAdmin') {
              users[i].roles[j] = '*';
            }
          });
        }
      });
      // for (let i = 0; i < users.length; i++) {
      //   if (users[i].userName === 'Kelmen') {
      //     for (let j = 0; j < users[i].roles.length; j++) {
      //       if (users[i].roles[j] === 'SuperAdmin') {
      //         users[i].roles[j] = '*';
      //       }
      //     }
      //   }
      // }
      this.users = users;

    }, error => {
      this.alertify.error(error);
    });
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, { initialState });
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
        }, error => {
          this.alertify.error(error);
        });
      }
    });
  }

  blockUnBlockUser(user) {
    if (!user.blockedUser) {
      if (user.id === this.currentUserId) {
        this.alertify.error('You cannot block yourself!.');
      } else {
        this.adminService.blockUser(user.id).subscribe(next => {
          this.users.find(k => k.id === user.id).blockedUser = true;
          this.alertify.message('Successfully blocked.');
        }, error => {
          this.alertify.error(error);
        });
      }
    } else {
      this.adminService.unBlockUser(user.id).subscribe(next => {
        this.users.find(k => k.id === user.id).blockedUser = false;
        this.alertify.message('Successfully Unblocked.');
      }, error => {
        this.alertify.error(error);
      });
    }
  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'SuperAdmin', value: 'SuperAdmin' },
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Mr' }
    ];
    // tslint:disable-next-line:prefer-for-of
    for (let i = 0; i < availableRoles.length; i++) {
      let isMatch = false;
      // tslint:disable-next-line:prefer-for-of
      for (let j = 0; j < userRoles.length; j++) {
        if (availableRoles[i].name === userRoles[j]) {
          isMatch = true;
          availableRoles[i].checked = true;
          roles.push(availableRoles[i]);
          break;
        }
      }
      if (!isMatch) {
        availableRoles[i].checked = false;
        roles.push(availableRoles[i]);
      }
    }
    return roles;
  }
}
