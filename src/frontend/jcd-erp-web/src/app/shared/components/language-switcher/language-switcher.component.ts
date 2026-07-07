import { Component, inject } from '@angular/core';

import { LocaleId } from '../../../core/i18n/locale.models';
import { LocaleService } from '../../../core/i18n/locale.service';

@Component({
  selector: 'app-language-switcher',
  standalone: true,
  template: `
    <div class="language-switcher" role="group" [attr.aria-label]="locale.t('common.language')">
      <button
        type="button"
        class="language-switcher__btn"
        [class.language-switcher__btn--active]="locale.locale() === 'es'"
        (click)="setLocale('es')"
      >
        ES
      </button>
      <button
        type="button"
        class="language-switcher__btn"
        [class.language-switcher__btn--active]="locale.locale() === 'en'"
        (click)="setLocale('en')"
      >
        EN
      </button>
    </div>
  `,
  styles: `
    .language-switcher {
      display: inline-flex;
      gap: 0.25rem;
      padding: 0.125rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      background: var(--color-surface);
    }

    .language-switcher__btn {
      min-width: 2.25rem;
      padding: 0.25rem 0.5rem;
      border: none;
      border-radius: calc(var(--radius-md) - 2px);
      background: transparent;
      color: var(--color-text-muted);
      font-size: 0.75rem;
      font-weight: 600;
      cursor: pointer;
    }

    .language-switcher__btn--active {
      background: var(--color-primary);
      color: #fff;
    }
  `,
})
export class LanguageSwitcherComponent {
  readonly locale = inject(LocaleService);

  setLocale(locale: LocaleId): void {
    void this.locale.setLocale(locale);
  }
}
