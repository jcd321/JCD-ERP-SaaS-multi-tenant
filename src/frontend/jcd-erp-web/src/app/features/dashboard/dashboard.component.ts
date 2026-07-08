import { Component, inject, OnInit, signal } from '@angular/core';

import { TranslatePipe } from '../../core/i18n';
import { DashboardKpis, DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard',
  imports: [TranslatePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  readonly loading = signal(true);
  readonly kpis = signal<DashboardKpis | null>(null);

  ngOnInit(): void {
    this.dashboardService.getKpis().subscribe({
      next: (kpis) => {
        this.kpis.set(kpis);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  formatInventoryValue(kpis: DashboardKpis): string {
    return String(kpis.productsCount);
  }

  formatAlertsValue(kpis: DashboardKpis): string {
    return String(kpis.belowMinimumCount);
  }
}
