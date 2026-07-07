import { Component, inject, OnInit } from '@angular/core';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Actions, ofType } from '@ngrx/effects';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { AuthFacade } from '../../../store/auth/auth.facade';
import { AuthActions } from '../../../store/auth/auth.actions';

@Component({
  selector: 'app-reset-password',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
})
export class ResetPasswordComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly authFacade = inject(AuthFacade);
  private readonly actions$ = inject(Actions);

  readonly loading = this.authFacade.loading;
  readonly errorMessage = this.authFacade.error;
  resetSuccess = false;

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    token: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  });

  constructor() {
    this.actions$
      .pipe(ofType(AuthActions.resetPasswordSuccess), takeUntilDestroyed())
      .subscribe(() => {
        this.resetSuccess = true;
      });
  }

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token') ?? '';
    const email = this.route.snapshot.queryParamMap.get('email') ?? '';
    this.form.patchValue({ token, email });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, token, newPassword, confirmPassword } = this.form.getRawValue();

    if (newPassword !== confirmPassword) {
      this.form.controls.confirmPassword.setErrors({ mismatch: true });
      return;
    }

    this.authFacade.resetPassword({ email, token, newPassword });
  }
}
