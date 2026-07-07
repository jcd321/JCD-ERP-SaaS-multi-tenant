import { Component, inject, OnInit } from '@angular/core';

import { RolesFacade } from '../../../store/roles/roles.facade';

@Component({
  selector: 'app-roles-list',
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.scss',
})
export class RolesListComponent implements OnInit {
  private readonly rolesFacade = inject(RolesFacade);

  readonly roles = this.rolesFacade.roles;
  readonly loading = this.rolesFacade.loading;
  readonly errorMessage = this.rolesFacade.error;

  ngOnInit(): void {
    this.rolesFacade.loadRoles();
  }
}
