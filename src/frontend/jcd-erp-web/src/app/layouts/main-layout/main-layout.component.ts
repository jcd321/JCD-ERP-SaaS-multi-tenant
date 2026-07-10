import { Component, computed, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { TranslatePipe } from '../../core/i18n';
import { AuthFacade } from '../../store/auth/auth.facade';
import { ThemeService } from '../../core/theme/theme.service';
import { LanguageSwitcherComponent } from '../../shared/components/language-switcher/language-switcher.component';

interface NavItem {
  labelKey: string;
  route: string;
  icon: SafeHtml;
  permission?: string;
}

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, TranslatePipe, LanguageSwitcherComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  private readonly authFacade = inject(AuthFacade);
  private readonly themeService = inject(ThemeService);
  private readonly sanitizer = inject(DomSanitizer);

  readonly session = this.authFacade.session;
  readonly theme = this.themeService.theme;

  readonly userInitials = computed(() => {
    const name = this.session()?.fullName?.trim() ?? '';
    if (!name) return 'JC';

    const parts = name.split(/\s+/).filter(Boolean);
    return parts
      .slice(0, 2)
      .map((part) => part[0]?.toUpperCase() ?? '')
      .join('');
  });

  private readonly allNavItems: NavItem[] = [
    {
      labelKey: 'nav.dashboard',
      route: '/',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/></svg>',
      ),
    },
    {
      labelKey: 'nav.users',
      route: '/users',
      permission: 'users.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 3.6-6 8-6s8 2 8 6"/></svg>',
      ),
    },
    {
      labelKey: 'nav.roles',
      route: '/roles',
      permission: 'roles.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 3l8 4v6c0 5-3.5 8-8 8s-8-3-8-8V7l8-4z"/></svg>',
      ),
    },
    {
      labelKey: 'nav.settings',
      route: '/settings',
      permission: 'settings.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="3"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/></svg>',
      ),
    },
    {
      labelKey: 'nav.units',
      route: '/units',
      permission: 'units.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M4 7h16M4 12h10M4 17h6"/></svg>',
      ),
    },
    {
      labelKey: 'nav.categories',
      route: '/categories',
      permission: 'categories.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M4 6h7v7H4zM13 6h7v4h-7zM13 14h7v4h-7zM4 17h7v1H4z"/></svg>',
      ),
    },
    {
      labelKey: 'nav.brands',
      route: '/brands',
      permission: 'brands.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M4 4h16v6H4zM4 14h10v6H4zM16 14h4v6h-4z"/></svg>',
      ),
    },
    {
      labelKey: 'nav.products',
      route: '/products',
      permission: 'products.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 16V8a2 2 0 00-1-1.73l-7-4a2 2 0 00-2 0l-7 4A2 2 0 003 8v8a2 2 0 001 1.73l7 4a2 2 0 002 0l7-4A2 2 0 0021 16z"/><path d="M3.3 7.7L12 12.5l8.7-4.8M12 22V12.5"/></svg>',
      ),
    },
    {
      labelKey: 'nav.customers',
      route: '/customers',
      permission: 'customers.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 00-4-4H5a4 4 0 00-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 00-3-3.87M16 3.13a4 4 0 010 7.75"/></svg>',
      ),
    },
    {
      labelKey: 'nav.suppliers',
      route: '/suppliers',
      permission: 'suppliers.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 7h18v10H3z"/><path d="M7 7V5h10v2"/><path d="M9 11h6M9 15h4"/></svg>',
      ),
    },
    {
      labelKey: 'nav.warehouses',
      route: '/warehouses',
      permission: 'warehouses.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M3 9l9-5 9 5v11H3z"/><path d="M9 22V12h6v10"/></svg>',
      ),
    },
    {
      labelKey: 'nav.stock',
      route: '/stock',
      permission: 'stock.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 7H4"/><path d="M10 11v6M14 11v6"/><path d="M6 7l1 12h10l1-12"/><path d="M9 7V5h6v2"/></svg>',
      ),
    },
    {
      labelKey: 'nav.movements',
      route: '/movements',
      permission: 'movements.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M7 7h10v10H7z"/><path d="M7 12h10"/><path d="M12 7v10"/><path d="M4 4h4M16 4h4M4 20h4M16 20h4"/></svg>',
      ),
    },
    {
      labelKey: 'nav.kardex',
      route: '/kardex',
      permission: 'kardex.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M4 19.5A2.5 2.5 0 016.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 014 19.5v-15A2.5 2.5 0 016.5 2z"/><path d="M8 7h8M8 11h8M8 15h5"/></svg>',
      ),
    },
    {
      labelKey: 'nav.transfers',
      route: '/transfers',
      permission: 'transfers.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M7 7h6l4 4v6H7z"/><path d="M13 7v4h4"/><path d="M5 17l4-4"/><path d="M15 13h4v4"/></svg>',
      ),
    },
    {
      labelKey: 'nav.adjustments',
      route: '/adjustments',
      permission: 'adjustments.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 3v18"/><path d="M5 10h14"/><path d="M8 6h8M8 18h8"/></svg>',
      ),
    },
  ];

  readonly navItems = computed(() => {
    const permissions = new Set(this.session()?.permissions ?? []);
    return this.allNavItems.filter(
      (item) => !item.permission || permissions.has(item.permission),
    );
  });

  toggleTheme(): void {
    this.themeService.toggle();
  }

  logout(): void {
    this.authFacade.logout();
  }

  private icon(svg: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }
}
