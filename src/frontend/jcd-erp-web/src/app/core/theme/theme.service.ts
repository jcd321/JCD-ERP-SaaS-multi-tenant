import { Injectable, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const THEME_KEY = 'jcd_erp_theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly themeSignal = signal<ThemeMode>(this.loadTheme());

  readonly theme = this.themeSignal.asReadonly();

  constructor() {
    this.applyTheme(this.themeSignal());
  }

  toggle(): void {
    const next: ThemeMode = this.themeSignal() === 'light' ? 'dark' : 'light';
    this.setTheme(next);
  }

  setTheme(theme: ThemeMode): void {
    this.themeSignal.set(theme);
    localStorage.setItem(THEME_KEY, theme);
    this.applyTheme(theme);
  }

  private loadTheme(): ThemeMode {
    const stored = localStorage.getItem(THEME_KEY);
    if (stored === 'light' || stored === 'dark') {
      return stored;
    }

    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  }

  private applyTheme(theme: ThemeMode): void {
    document.documentElement.setAttribute('data-theme', theme);
  }
}
