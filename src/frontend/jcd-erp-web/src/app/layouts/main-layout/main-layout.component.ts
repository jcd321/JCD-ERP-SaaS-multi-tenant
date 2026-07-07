import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { AuthFacade } from '../../store/auth/auth.facade';
import { ThemeService } from '../../core/theme/theme.service';

interface NavItem {
  label: string;
  route: string;
  icon: string;
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

  readonly session = this.authFacade.session;
  readonly theme = this.themeService.theme;

  readonly navItems: NavItem[] = [
    { label: 'Dashboard', route: '/', icon: '◫' },
    { label: 'Users', route: '/users', icon: '◎' },
    { label: 'Roles', route: '/roles', icon: '⚿' },
    { label: 'Settings', route: '/settings', icon: '⚙' },
  ];

  toggleTheme(): void {
    this.themeService.toggle();
  }

  logout(): void {
    this.authFacade.logout();
  }
}
