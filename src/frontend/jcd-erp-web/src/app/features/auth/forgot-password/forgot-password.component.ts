import { Component, inject, signal } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthFacade } from '../../../store/auth/auth.facade';
import { AuthActions } from '../../../store/auth/auth.actions';

@Component({
  selector: 'app-forgot-password',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly authFacade = inject(AuthFacade);
  private readonly actions$ = inject(Actions);

  readonly loading = this.authFacade.loading;
  readonly errorMessage = this.authFacade.error;
  readonly submitted = signal(false);

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    tenantSlug: [''],
  });

  constructor() {
    this.actions$
      .pipe(ofType(AuthActions.forgotPasswordSuccess), takeUntilDestroyed())
      .subscribe(() => this.submitted.set(true));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, tenantSlug } = this.form.getRawValue();
    this.submitted.set(false);
    this.authFacade.forgotPassword({
      email,
      tenantSlug: tenantSlug.trim() || null,
    });
  }
}
