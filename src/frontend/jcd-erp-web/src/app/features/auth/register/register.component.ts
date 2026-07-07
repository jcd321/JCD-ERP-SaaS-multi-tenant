import { Component, inject } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';

import { TranslatePipe } from '../../../core/i18n';
import { AuthFacade } from '../../../store/auth/auth.facade';

function passwordsMatch(control: AbstractControl): ValidationErrors | null {
  const password = control.get('adminPassword')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly authFacade = inject(AuthFacade);

  readonly loading = this.authFacade.loading;
  readonly errorMessage = this.authFacade.error;

  readonly form = this.fb.group(
    {
      companyName: ['', [Validators.required, Validators.minLength(2)]],
      slug: [''],
      adminFirstName: ['', [Validators.required]],
      adminLastName: ['', [Validators.required]],
      adminEmail: ['', [Validators.required, Validators.email]],
      adminPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatch },
  );

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const {
      companyName,
      slug,
      adminEmail,
      adminPassword,
      adminFirstName,
      adminLastName,
    } = this.form.getRawValue();

    this.authFacade.register({
      companyName,
      slug: slug.trim() || null,
      adminEmail,
      adminPassword,
      adminFirstName,
      adminLastName,
    });
  }
}
