import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { StockActions } from '../../../store/stock/stock.actions';
import { StockFacade } from '../../../store/stock/stock.facade';
import { StockLevel, StockFormMode } from '../stock.models';

@Component({
  selector: 'app-stock-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './stock-list.component.html',
  styleUrl: './stock-list.component.scss',
})
export class StockListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly stockFacade = inject(StockFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly stockLevels = this.stockFacade.stockLevels;
  readonly page = this.stockFacade.page;
  readonly totalPages = this.stockFacade.totalPages;
  readonly totalCount = this.stockFacade.totalCount;
  readonly loading = this.stockFacade.loading;
  readonly saving = this.stockFacade.saving;
  readonly errorMessage = this.stockFacade.error;
  readonly activeSearch = this.stockFacade.search;
  readonly warehouseFilter = this.stockFacade.warehouseFilter;
  readonly belowMinimumOnly = this.stockFacade.belowMinimumOnly;
  readonly productOptions = this.stockFacade.productOptions;
  readonly warehouseOptions = this.stockFacade.warehouseOptions;
  readonly lookupsLoading = this.stockFacade.lookupsLoading;

  formMode: StockFormMode = null;
  editingStockLevelId: string | null = null;
  editingStockLevel: StockLevel | null = null;
  stockLevelToDelete: StockLevel | null = null;

  readonly searchControl = this.fb.control('');
  readonly warehouseFilterControl = this.fb.control('');
  readonly belowMinimumControl = this.fb.control(false);

  readonly form = this.fb.group({
    productId: ['', Validators.required],
    warehouseId: ['', Validators.required],
    quantityOnHand: [0, [Validators.required, Validators.min(0)]],
    minQuantity: this.fb.control<number | null>(null, Validators.min(0)),
    maxQuantity: this.fb.control<number | null>(null, Validators.min(0)),
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          StockActions.createStockLevelSuccess,
          StockActions.updateStockLevelSuccess,
          StockActions.deleteStockLevelSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.stockLevelToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) this.searchControl.setValue(currentSearch);

    const warehouseId = this.warehouseFilter();
    if (warehouseId) this.warehouseFilterControl.setValue(warehouseId);

    this.belowMinimumControl.setValue(this.belowMinimumOnly());

    this.stockFacade.loadLookups();
    this.stockFacade.loadStockLevels();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('stock.createTitle')
      : this.locale.t('stock.editTitle');
  }

  get deleteMessage(): string {
    if (!this.stockLevelToDelete) return '';
    return this.locale.t('stock.deleteMessage', {
      product: this.stockLevelToDelete.productName,
      warehouse: this.stockLevelToDelete.warehouseName,
    });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingStockLevelId = null;
    this.editingStockLevel = null;
    this.form.reset({
      productId: '',
      warehouseId: '',
      quantityOnHand: 0,
      minQuantity: null,
      maxQuantity: null,
    });
    this.form.controls.productId.enable();
    this.form.controls.warehouseId.enable();
  }

  openEditForm(stockLevel: StockLevel): void {
    this.formMode = 'edit';
    this.editingStockLevelId = stockLevel.id;
    this.editingStockLevel = stockLevel;
    this.form.reset({
      productId: stockLevel.productId,
      warehouseId: stockLevel.warehouseId,
      quantityOnHand: stockLevel.quantityOnHand,
      minQuantity: stockLevel.minQuantity,
      maxQuantity: stockLevel.maxQuantity,
    });
    this.form.controls.productId.disable();
    this.form.controls.warehouseId.disable();
  }

  closeForm(): void {
    this.formMode = null;
    this.editingStockLevelId = null;
    this.editingStockLevel = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const minQuantity = raw.minQuantity === null || raw.minQuantity === undefined ? null : Number(raw.minQuantity);
    const maxQuantity = raw.maxQuantity === null || raw.maxQuantity === undefined ? null : Number(raw.maxQuantity);

    if (minQuantity !== null && maxQuantity !== null && minQuantity > maxQuantity) {
      return;
    }

    const quantities = {
      quantityOnHand: Number(raw.quantityOnHand),
      minQuantity,
      maxQuantity,
    };

    if (this.formMode === 'create') {
      this.stockFacade.createStockLevel({
        productId: raw.productId,
        warehouseId: raw.warehouseId,
        ...quantities,
      });
      return;
    }

    if (this.editingStockLevelId) {
      this.stockFacade.updateStockLevel(this.editingStockLevelId, quantities);
    }
  }

  applyFilters(): void {
    this.stockFacade.loadStockLevels({
      page: 1,
      search: this.searchControl.value,
      warehouseId: this.warehouseFilterControl.value || null,
      belowMinimumOnly: this.belowMinimumControl.value,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.stockFacade.loadStockLevels({
      page: 1,
      search: '',
      warehouseId: this.warehouseFilterControl.value || null,
      belowMinimumOnly: this.belowMinimumControl.value,
    });
  }

  clearWarehouseFilter(): void {
    this.warehouseFilterControl.setValue('');
    this.applyFilters();
  }

  toggleBelowMinimum(): void {
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.stockFacade.loadStockLevels({ page });
  }

  formatQuantity(value: number, unitSymbol: string | null): string {
    const formatted = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unitSymbol ? `${formatted} ${unitSymbol}` : formatted;
  }

  get pageInfoLabel(): string {
    return this.locale.t('stock.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('stock.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('stock.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(stockLevel: StockLevel): void {
    this.stockLevelToDelete = stockLevel;
  }

  closeDeleteDialog(): void {
    this.stockLevelToDelete = null;
  }

  confirmDelete(): void {
    if (this.stockLevelToDelete) {
      this.stockFacade.deleteStockLevel(this.stockLevelToDelete.id);
    }
  }
}
