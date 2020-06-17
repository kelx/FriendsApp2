import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/_services/user.service';
import { AlertifyService } from 'src/_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from 'src/_services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {

  constructor(private userService: UserService, private router: Router, private authService: AuthService,
              private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User>  {
      return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
        catchError(error => {
          this.alertify.error('Problem retrieving your data.');
          this.router.navigate(['/members']);
          return of(null);
        })
      );
    }
}
