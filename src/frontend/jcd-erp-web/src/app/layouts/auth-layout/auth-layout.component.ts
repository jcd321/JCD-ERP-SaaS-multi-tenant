import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { TranslatePipe } from '../../core/i18n';
import { LanguageSwitcherComponent } from '../../shared/components/language-switcher/language-switcher.component';

@Component({
  selector: 'app-auth-layout',
  imports: [RouterOutlet, TranslatePipe, LanguageSwitcherComponent],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.scss',
})
export class AuthLayoutComponent {}
