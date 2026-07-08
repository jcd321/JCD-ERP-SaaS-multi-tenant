import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';

import { AuthActions } from './store/auth/auth.actions';
import { REFRESH_TOKEN_KEY } from './store/auth/auth.state';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent implements OnInit {
  private readonly store = inject(Store);

  ngOnInit(): void {
    if (localStorage.getItem(REFRESH_TOKEN_KEY)) {
      this.store.dispatch(AuthActions.refreshToken());
    }
  }
}
