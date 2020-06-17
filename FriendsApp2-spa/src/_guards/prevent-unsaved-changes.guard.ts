import { Injectable } from '@angular/core';
import { MemberEditComponent } from 'src/app/members/member-edit/member-edit.component';
import { CanDeactivate } from '@angular/router';



// Consider using this interface for all CanDeactivate guards,
// and have your components implement this interface, too.
//
//   e.g. export class CanDeactivateGuard implements CanDeactivate<CanComponentDeactivate> {
//
// export interface CanComponentDeactivate {
// canDeactivate: () => any;
// }

@Injectable({providedIn: 'root'})
export class PrevetUnSavedChanges implements CanDeactivate<MemberEditComponent> {
  canDeactivate(
    component: MemberEditComponent) {
      if (component.editForm.dirty) {
        return confirm('Are you sure you want to continue? Any unsaved changes will be lost!')
      }
      return true;
  }
}
