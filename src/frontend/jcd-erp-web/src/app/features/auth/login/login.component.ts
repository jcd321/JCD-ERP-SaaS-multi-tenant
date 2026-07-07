import { Component, inject } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { AuthFacade } from '../../../store/auth/auth.facade';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly authFacade = inject(AuthFacade);

  readonly loading = this.authFacade.loading;
  readonly errorMessage = this.authFacade.error;

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    tenantSlug: [''],
    rememberMe: [false],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, tenantSlug, rememberMe } = this.form.getRawValue();

    this.authFacade.login({
      email,
      password,
      tenantSlug: tenantSlug.trim() || null,
      rememberMe,
    });
  }
}
