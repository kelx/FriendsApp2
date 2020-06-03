import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/_services/auth.service';
import { AlertifyService } from 'src/_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        // console.log('Logged in successfully')
        this.alertifyService.success('Logged In successfully.')
      }, error => {
        this.alertifyService.error(error);
      }
    );
  }

  logout() {
    localStorage.removeItem('token');
    this.alertifyService.message('logged out');
  }
  loggedIn() {
    return this.authService.loggedIn();
  }

}
