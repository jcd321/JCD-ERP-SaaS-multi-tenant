import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { TransfersActions } from '../../../store/transfers/transfers.actions';
import { TransfersFacade } from '../../../store/transfers/transfers.facade';
import { InventoryTransfer, TransferFormMode } from '../transfers.models';

@Component({
  selector: 'app-transfers-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, TranslatePipe],
  templateUrl: './transfers-list.component.html',
  styleUrl: './transfers-list.component.scss',
})
export class TransfersListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly transfersFacade = inject(TransfersFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly transfers = this.transfersFacade.transfers;
  readonly page = this.transfersFacade.page;
  readonly totalPages = this.transfersFacade.totalPages;
  readonly totalCount = this.transfersFacade.totalCount;
  readonly loading = this.transfersFacade.loading;
  readonly saving = this.transfersFacade.saving;
  readonly errorMessage = this.transfersFacade.error;
  readonly activeSearch = this.transfersFacade.search;
  readonly productOptions = this.transfersFacade.productOptions;
  readonly warehouseOptions = this.transfersFacade.warehouseOptions;
  readonly stockLevels = this.transfersFacade.stockLevels;
  readonly lookupsLoading = this.transfersFacade.lookupsLoading;

  formMode: TransferFormMode = null;
  expandedTransferId: string | null = null;
  formSubmitError: string | null = null;

  readonly searchControl = this.fb.control('');
  readonly sourceFilterControl = this.fb.control('');
  readonly destinationFilterControl = this.fb.control('');

  readonly form = this.fb.group({
    sourceWarehouseId: ['', Validators.required],
    destinationWarehouseId: ['', Validators.required],
    transferDate: [this.todayIsoDate(), Validators.required],
    notes: [''],
    lines: this.fb.array([this.createLineGroup()]),
  });

  constructor() {
    this.actions$
      .pipe(ofType(TransfersActions.createTransferSuccess), takeUntilDestroyed())
      .subscribe(() => this.closeForm());
  }

  ngOnInit(): void {
    if (this.activeSearch()) this.searchControl.setValue(this.activeSearch());
    this.transfersFacade.loadLookups();
    this.transfersFacade.loadTransfers();
  }

  get lineControls(): FormArray {
    return this.form.controls.lines;
  }

  get modalTitle(): string {
    return this.locale.t('transfers.createTitle');
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.formSubmitError = null;
    this.transfersFacade.clearError();
    this.transfersFacade.loadLookups();
    this.form.reset({
      sourceWarehouseId: '',
      destinationWarehouseId: '',
      transferDate: this.todayIsoDate(),
      notes: '',
    });
    this.form.setControl('lines', this.fb.array([this.createLineGroup()]));
  }

  closeForm(): void {
    this.formMode = null;
    this.formSubmitError = null;
  }

  addLine(): void {
    this.lineControls.push(this.createLineGroup());
  }

  removeLine(index: number): void {
    if (this.lineControls.length <= 1) return;
    this.lineControls.removeAt(index);
  }

  submit(): void {
    this.formSubmitError = null;

    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      this.formSubmitError = this.locale.t('transfers.formIncomplete');
      return;
    }

    const raw = this.form.getRawValue();
    if (raw.sourceWarehouseId === raw.destinationWarehouseId) {
      this.formSubmitError = this.locale.t('transfers.sameWarehouse');
      return;
    }

    const productIds = raw.lines.map((line) => line.productId);
    if (new Set(productIds).size !== productIds.length) {
      this.formSubmitError = this.locale.t('transfers.duplicateProduct');
      return;
    }

    const stockError = this.validateLineStock(raw.sourceWarehouseId, raw.lines);
    if (stockError) {
      this.formSubmitError = stockError;
      return;
    }

    this.transfersFacade.createTransfer({
      sourceWarehouseId: raw.sourceWarehouseId,
      destinationWarehouseId: raw.destinationWarehouseId,
      transferDate: raw.transferDate ? new Date(raw.transferDate).toISOString() : null,
      notes: raw.notes.trim() || null,
      lines: raw.lines.map((line) => ({
        productId: line.productId,
        quantity: Math.max(1, Math.trunc(Number(line.quantity))),
      })),
    });
  }

  applyFilters(): void {
    this.transfersFacade.loadTransfers({
      page: 1,
      search: this.searchControl.value,
      sourceWarehouseId: this.sourceFilterControl.value || null,
      destinationWarehouseId: this.destinationFilterControl.value || null,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.transfersFacade.loadTransfers({ page });
  }

  toggleDetails(transfer: InventoryTransfer): void {
    this.expandedTransferId = this.expandedTransferId === transfer.id ? null : transfer.id;
  }

  isExpanded(transferId: string): boolean {
    return this.expandedTransferId === transferId;
  }

  formatQuantity(value: number, unitSymbol: string | null): string {
    const formatted = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unitSymbol ? `${formatted} ${unitSymbol}` : formatted;
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  get pageInfoLabel(): string {
    return this.locale.t('transfers.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('transfers.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('transfers.resultsCount', { count: String(this.totalCount()) });
  }

  getLineAvailableStock(lineIndex: number): number | null {
    if (!this.hasStockSnapshot()) return null;

    const sourceWarehouseId = this.normalizeId(this.form.controls.sourceWarehouseId.value);
    const productId = this.normalizeId(this.lineControls.at(lineIndex)?.get('productId')?.value);
    if (!sourceWarehouseId || !productId) return null;

    const stock = this.stockLevels().find(
      (level) =>
        this.normalizeId(level.productId) === productId &&
        this.normalizeId(level.warehouseId) === sourceWarehouseId,
    );

    return stock?.quantityOnHand ?? 0;
  }

  getLineStockHint(lineIndex: number): string | null {
    const sourceWarehouseId = this.form.controls.sourceWarehouseId.value;
    const productId = this.lineControls.at(lineIndex)?.get('productId')?.value;
    if (!sourceWarehouseId || !productId) return null;

    const available = this.getLineAvailableStock(lineIndex);
    if (available === null) return null;

    if (available <= 0) {
      return this.locale.t('transfers.lineNotInWarehouse', {
        product: this.getProductLabel(productId),
        warehouse: this.getWarehouseLabel(sourceWarehouseId),
      });
    }

    return this.locale.t('transfers.lineAvailableStock', { count: String(available) });
  }

  private validateLineStock(
    sourceWarehouseId: string,
    lines: { productId: string; quantity: number }[],
  ): string | null {
    if (!this.hasStockSnapshot()) return null;

    const warehouseLabel = this.getWarehouseLabel(sourceWarehouseId);
    const normalizedSourceWarehouseId = this.normalizeId(sourceWarehouseId);

    for (const line of lines) {
      const productLabel = this.getProductLabel(line.productId);
      const normalizedProductId = this.normalizeId(line.productId);
      const stock = this.stockLevels().find(
        (level) =>
          this.normalizeId(level.productId) === normalizedProductId &&
          this.normalizeId(level.warehouseId) === normalizedSourceWarehouseId,
      );

      if (!stock || stock.quantityOnHand <= 0) {
        return this.locale.t('transfers.productNotInWarehouse', {
          product: productLabel,
          warehouse: warehouseLabel,
        });
      }

      if (stock.quantityOnHand < line.quantity) {
        return this.locale.t('transfers.insufficientStock', {
          product: productLabel,
          warehouse: warehouseLabel,
          available: String(stock.quantityOnHand),
          requested: String(line.quantity),
        });
      }
    }

    return null;
  }

  private hasStockSnapshot(): boolean {
    return this.stockLevels().length > 0;
  }

  private normalizeId(value: string | null | undefined): string {
    return String(value ?? '').trim().toLowerCase();
  }

  private getProductLabel(productId: string): string {
    const normalizedId = this.normalizeId(productId);
    return this.productOptions().find((option) => this.normalizeId(option.id) === normalizedId)?.label ?? productId;
  }

  private getWarehouseLabel(warehouseId: string): string {
    const normalizedId = this.normalizeId(warehouseId);
    return this.warehouseOptions().find((option) => this.normalizeId(option.id) === normalizedId)?.label ?? warehouseId;
  }

  private createLineGroup() {
    return this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
    });
  }

  private todayIsoDate(): string {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const local = new Date(now.getTime() - offset * 60_000);
    return local.toISOString().slice(0, 16);
  }
}
