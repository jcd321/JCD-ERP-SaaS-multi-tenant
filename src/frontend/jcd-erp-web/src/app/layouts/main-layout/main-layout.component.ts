import { Component, computed, inject } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthFacade } from '../../store/auth/auth.facade';
import { ThemeService } from '../../core/theme/theme.service';

interface NavItem {
  label: string;
  route: string;
  icon: SafeHtml;
  permission?: string;
}

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
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
      label: 'Dashboard',
      route: '/',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/></svg>',
      ),
    },
    {
      label: 'Usuarios',
      route: '/users',
      permission: 'users.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="8" r="4"/><path d="M4 20c0-4 3.6-6 8-6s8 2 8 6"/></svg>',
      ),
    },
    {
      label: 'Roles',
      route: '/roles',
      permission: 'roles.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 3l8 4v6c0 5-3.5 8-8 8s-8-3-8-8V7l8-4z"/></svg>',
      ),
    },
    {
      label: 'Configuración',
      route: '/settings',
      permission: 'settings.view',
      icon: this.icon(
        '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="3"/><path d="M12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4"/></svg>',
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
