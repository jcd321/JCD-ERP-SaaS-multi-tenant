import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { AdjustmentsActions } from '../../../store/adjustments/adjustments.actions';
import { AdjustmentsFacade } from '../../../store/adjustments/adjustments.facade';
import { AdjustmentFormMode, InventoryAdjustment } from '../adjustments.models';

const ADJUSTMENT_REASONS = [
  'PHYSICAL_COUNT',
  'DAMAGE',
  'CORRECTION',
  'EXPIRED',
  'OTHER',
] as const;

type AdjustmentReasonCode = (typeof ADJUSTMENT_REASONS)[number];

@Component({
  selector: 'app-adjustments-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, TranslatePipe],
  templateUrl: './adjustments-list.component.html',
  styleUrl: './adjustments-list.component.scss',
})
export class AdjustmentsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly adjustmentsFacade = inject(AdjustmentsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly adjustmentReasons = ADJUSTMENT_REASONS;

  readonly adjustments = this.adjustmentsFacade.adjustments;
  readonly page = this.adjustmentsFacade.page;
  readonly totalPages = this.adjustmentsFacade.totalPages;
  readonly totalCount = this.adjustmentsFacade.totalCount;
  readonly loading = this.adjustmentsFacade.loading;
  readonly saving = this.adjustmentsFacade.saving;
  readonly errorMessage = this.adjustmentsFacade.error;
  readonly activeSearch = this.adjustmentsFacade.search;
  readonly productOptions = this.adjustmentsFacade.productOptions;
  readonly warehouseOptions = this.adjustmentsFacade.warehouseOptions;
  readonly stockLevels = this.adjustmentsFacade.stockLevels;
  readonly lookupsLoading = this.adjustmentsFacade.lookupsLoading;

  formMode: AdjustmentFormMode = null;
  expandedAdjustmentId: string | null = null;
  formSubmitError: string | null = null;

  readonly searchControl = this.fb.control('');
  readonly warehouseFilterControl = this.fb.control('');

  readonly form = this.fb.group({
    warehouseId: ['', Validators.required],
    adjustmentDate: [this.todayIsoDate(), Validators.required],
    reason: ['', Validators.required],
    notes: [''],
    lines: this.fb.array([this.createLineGroup()]),
  });

  constructor() {
    this.actions$
      .pipe(ofType(AdjustmentsActions.createAdjustmentSuccess), takeUntilDestroyed())
      .subscribe(() => this.closeForm());
  }

  ngOnInit(): void {
    if (this.activeSearch()) this.searchControl.setValue(this.activeSearch());
    this.adjustmentsFacade.loadLookups();
    this.adjustmentsFacade.loadAdjustments();
  }

  get lineControls(): FormArray {
    return this.form.controls.lines;
  }

  get modalTitle(): string {
    return this.locale.t('adjustments.createTitle');
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.formSubmitError = null;
    this.adjustmentsFacade.clearError();
    this.adjustmentsFacade.loadLookups();
    this.form.reset({
      warehouseId: '',
      adjustmentDate: this.todayIsoDate(),
      reason: '',
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
      this.formSubmitError = this.locale.t('adjustments.formIncomplete');
      return;
    }

    const raw = this.form.getRawValue();

    const productIds = raw.lines.map((line) => line.productId);
    if (new Set(productIds).size !== productIds.length) {
      this.formSubmitError = this.locale.t('adjustments.duplicateProduct');
      return;
    }

    const changeError = this.validateLineChanges(raw.warehouseId, raw.lines);
    if (changeError) {
      this.formSubmitError = changeError;
      return;
    }

    this.adjustmentsFacade.createAdjustment({
      warehouseId: raw.warehouseId,
      adjustmentDate: raw.adjustmentDate ? new Date(raw.adjustmentDate).toISOString() : null,
      reason: raw.reason,
      notes: raw.notes.trim() || null,
      lines: raw.lines.map((line) => ({
        productId: line.productId,
        countedQuantity: Math.max(0, Math.trunc(Number(line.countedQuantity))),
      })),
    });
  }

  applyFilters(): void {
    this.adjustmentsFacade.loadAdjustments({
      page: 1,
      search: this.searchControl.value,
      warehouseId: this.warehouseFilterControl.value || null,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.adjustmentsFacade.loadAdjustments({ page });
  }

  toggleDetails(adjustment: InventoryAdjustment): void {
    this.expandedAdjustmentId = this.expandedAdjustmentId === adjustment.id ? null : adjustment.id;
  }

  isExpanded(adjustmentId: string): boolean {
    return this.expandedAdjustmentId === adjustmentId;
  }

  formatQuantity(value: number, unitSymbol: string | null): string {
    const formatted = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unitSymbol ? `${formatted} ${unitSymbol}` : formatted;
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  formatReason(reason: string): string {
    const key = `adjustments.reasons.${reason}` as const;
    const translated = this.locale.t(key);
    return translated === key ? reason : translated;
  }

  reasonLabel(code: AdjustmentReasonCode): string {
    return this.locale.t(`adjustments.reasons.${code}`);
  }

  get pageInfoLabel(): string {
    return this.locale.t('adjustments.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('adjustments.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('adjustments.resultsCount', { count: String(this.totalCount()) });
  }

  getLineCurrentStock(lineIndex: number): number | null {
    if (!this.hasStockSnapshot()) return null;

    const warehouseId = this.normalizeId(this.form.controls.warehouseId.value);
    const productId = this.normalizeId(this.lineControls.at(lineIndex)?.get('productId')?.value);
    if (!warehouseId || !productId) return null;

    const stock = this.stockLevels().find(
      (level) =>
        this.normalizeId(level.productId) === productId &&
        this.normalizeId(level.warehouseId) === warehouseId,
    );

    return stock?.quantityOnHand ?? 0;
  }

  getLineStockHint(lineIndex: number): string | null {
    const warehouseId = this.form.controls.warehouseId.value;
    const productId = this.lineControls.at(lineIndex)?.get('productId')?.value;
    if (!warehouseId || !productId) return null;

    const current = this.getLineCurrentStock(lineIndex);
    if (current === null) return null;

    return this.locale.t('adjustments.lineCurrentStock', { count: String(current) });
  }

  private validateLineChanges(
    warehouseId: string,
    lines: { productId: string; countedQuantity: number }[],
  ): string | null {
    const warehouseLabel = this.getWarehouseLabel(warehouseId);
    const normalizedWarehouseId = this.normalizeId(warehouseId);

    for (const line of lines) {
      const productLabel = this.getProductLabel(line.productId);
      const normalizedProductId = this.normalizeId(line.productId);
      const countedQuantity = Math.max(0, Math.trunc(Number(line.countedQuantity)));

      const stock = this.hasStockSnapshot()
        ? this.stockLevels().find(
            (level) =>
              this.normalizeId(level.productId) === normalizedProductId &&
              this.normalizeId(level.warehouseId) === normalizedWarehouseId,
          )
        : null;

      const quantityBefore = stock?.quantityOnHand ?? 0;

      if (quantityBefore === countedQuantity) {
        return this.locale.t('adjustments.noChange', {
          product: productLabel,
          warehouse: warehouseLabel,
          quantity: String(quantityBefore),
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
      countedQuantity: [0, [Validators.required, Validators.min(0)]],
    });
  }

  private todayIsoDate(): string {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const local = new Date(now.getTime() - offset * 60_000);
    return local.toISOString().slice(0, 16);
  }
}
