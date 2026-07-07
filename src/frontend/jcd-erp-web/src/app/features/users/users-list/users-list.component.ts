import { Component, inject, OnInit } from '@angular/core';

import { UsersFacade } from '../../../store/users/users.facade';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss',
})
export class UsersListComponent implements OnInit {
  private readonly usersFacade = inject(UsersFacade);

  readonly users = this.usersFacade.users;
  readonly loading = this.usersFacade.loading;
  readonly errorMessage = this.usersFacade.error;

  ngOnInit(): void {
    this.usersFacade.loadUsers();
  }
}
