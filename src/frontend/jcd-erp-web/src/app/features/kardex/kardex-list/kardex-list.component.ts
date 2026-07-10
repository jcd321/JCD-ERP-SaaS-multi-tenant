import { Component, inject, OnInit } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';

import { TranslatePipe, LocaleService } from '../../../core/i18n';
import { KardexFacade } from '../../../store/kardex/kardex.facade';

@Component({
  selector: 'app-kardex-list',
  standalone: true,
  imports: [ReactiveFormsModule, TranslatePipe],
  templateUrl: './kardex-list.component.html',
  styleUrl: './kardex-list.component.scss',
})
export class KardexListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly kardexFacade = inject(KardexFacade);
  private readonly locale = inject(LocaleService);

  readonly entries = this.kardexFacade.entries;
  readonly page = this.kardexFacade.page;
  readonly totalPages = this.kardexFacade.totalPages;
  readonly totalCount = this.kardexFacade.totalCount;
  readonly loading = this.kardexFacade.loading;
  readonly errorMessage = this.kardexFacade.error;
  readonly productOptions = this.kardexFacade.productOptions;
  readonly warehouseOptions = this.kardexFacade.warehouseOptions;
  readonly lookupsLoading = this.kardexFacade.lookupsLoading;
  readonly selectedProductId = this.kardexFacade.productFilter;

  readonly productFilterControl = this.fb.control('');
  readonly warehouseFilterControl = this.fb.control('');
  readonly fromDateControl = this.fb.control('');
  readonly toDateControl = this.fb.control('');

  ngOnInit(): void {
    const productId = this.selectedProductId();
    if (productId) this.productFilterControl.setValue(productId);

    const warehouseId = this.kardexFacade.warehouseFilter();
    if (warehouseId) this.warehouseFilterControl.setValue(warehouseId);

    const fromDate = this.kardexFacade.fromDateFilter();
    if (fromDate) this.fromDateControl.setValue(this.toDateInputValue(fromDate));

    const toDate = this.kardexFacade.toDateFilter();
    if (toDate) this.toDateControl.setValue(this.toDateInputValue(toDate));

    this.kardexFacade.loadLookups();
    if (productId) this.kardexFacade.loadKardex();
  }

  onProductFilterChange(): void {
    if (this.productFilterControl.value) {
      this.applyFilters();
      return;
    }

    this.kardexFacade.loadKardex({
      page: 1,
      productId: null,
      warehouseId: null,
      fromDate: null,
      toDate: null,
    });
  }

  onWarehouseFilterChange(): void {
    if (this.productFilterControl.value) this.applyFilters();
  }

  applyFilters(): void {
    if (!this.productFilterControl.value) {
      this.kardexFacade.loadKardex({
        page: 1,
        productId: null,
        warehouseId: null,
        fromDate: null,
        toDate: null,
      });
      return;
    }

    this.kardexFacade.loadKardex({
      page: 1,
      productId: this.productFilterControl.value,
      warehouseId: this.warehouseFilterControl.value || null,
      fromDate: this.toIsoDate(this.fromDateControl.value),
      toDate: this.toIsoDate(this.toDateControl.value, true),
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.kardexFacade.loadKardex({ page });
  }

  movementTypeLabel(type: string): string {
    return type === 'OUT' ? this.locale.t('kardex.typeOut') : this.locale.t('kardex.typeIn');
  }

  formatQuantity(value: number, unitSymbol: string | null): string {
    const formatted = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unitSymbol ? `${formatted} ${unitSymbol}` : formatted;
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  get pageInfoLabel(): string {
    return this.locale.t('kardex.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get resultsCountLabel(): string {
    return this.locale.t('kardex.resultsCount', { count: String(this.totalCount()) });
  }

  get selectProductHint(): string {
    return this.locale.t('kardex.selectProductHint');
  }

  private toIsoDate(value: string, endOfDay = false): string | null {
    if (!value) return null;
    const date = new Date(value);
    if (endOfDay) date.setHours(23, 59, 59, 999);
    return date.toISOString();
  }

  private toDateInputValue(iso: string): string {
    return iso.slice(0, 10);
  }
}
