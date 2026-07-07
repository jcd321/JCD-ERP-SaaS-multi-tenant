import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { inject, Injectable, PLATFORM_ID, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import {
  DEFAULT_LOCALE,
  LOCALE_STORAGE_KEY,
  LocaleId,
  SUPPORTED_LOCALES,
  TranslationDictionary,
} from './locale.models';

@Injectable({ providedIn: 'root' })
export class LocaleService {
  private readonly http = inject(HttpClient);
  private readonly document = inject(DOCUMENT);
  private readonly platformId = inject(PLATFORM_ID);

  private readonly _locale = signal<LocaleId>(DEFAULT_LOCALE);
  private readonly _translations = signal<TranslationDictionary>({});
  private readonly _ready = signal(false);

  readonly locale = this._locale.asReadonly();
  readonly ready = this._ready.asReadonly();

  async init(): Promise<void> {
    const locale = this.resolveInitialLocale();
    await this.loadLocale(locale);
    this._ready.set(true);
  }

  async setLocale(locale: LocaleId): Promise<void> {
    if (!SUPPORTED_LOCALES.includes(locale) || locale === this._locale()) {
      return;
    }

    await this.loadLocale(locale);
  }

  t(key: string, params?: Record<string, string>): string {
    const raw = this.lookup(key, this._translations());
    if (typeof raw !== 'string') {
      return key;
    }

    if (!params) {
      return raw;
    }

    return Object.entries(params).reduce(
      (message, [name, value]) => message.replaceAll(`{{${name}}}`, value),
      raw,
    );
  }

  private async loadLocale(locale: LocaleId): Promise<void> {
    const dictionary = await firstValueFrom(
      this.http.get<TranslationDictionary>(`/i18n/${locale}.json`),
    );

    this._translations.set(dictionary);
    this._locale.set(locale);
    this.persistLocale(locale);
    this.updateDocumentLocale(locale);
  }

  private resolveInitialLocale(): LocaleId {
    if (!isPlatformBrowser(this.platformId)) {
      return DEFAULT_LOCALE;
    }

    const stored = localStorage.getItem(LOCALE_STORAGE_KEY) as LocaleId | null;
    if (stored && SUPPORTED_LOCALES.includes(stored)) {
      return stored;
    }

    const browser = navigator.language.toLowerCase();
    if (browser.startsWith('en')) {
      return 'en';
    }

    return DEFAULT_LOCALE;
  }

  private persistLocale(locale: LocaleId): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    localStorage.setItem(LOCALE_STORAGE_KEY, locale);
  }

  private updateDocumentLocale(locale: LocaleId): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.document.documentElement.lang = locale;
  }

  private lookup(key: string, dictionary: TranslationDictionary): unknown {
    return key.split('.').reduce<unknown>((current, segment) => {
      if (current && typeof current === 'object' && segment in current) {
        return (current as TranslationDictionary)[segment];
      }

      return undefined;
    }, dictionary);
  }
}
