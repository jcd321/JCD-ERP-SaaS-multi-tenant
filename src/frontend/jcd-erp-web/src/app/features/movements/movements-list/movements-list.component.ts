import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { MovementsActions } from '../../../store/movements/movements.actions';
import { MovementsFacade } from '../../../store/movements/movements.facade';
import { MOVEMENT_TYPES, MovementFormMode } from '../movements.models';

@Component({
  selector: 'app-movements-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, TranslatePipe],
  templateUrl: './movements-list.component.html',
  styleUrl: './movements-list.component.scss',
})
export class MovementsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly movementsFacade = inject(MovementsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly movements = this.movementsFacade.movements;
  readonly page = this.movementsFacade.page;
  readonly totalPages = this.movementsFacade.totalPages;
  readonly totalCount = this.movementsFacade.totalCount;
  readonly loading = this.movementsFacade.loading;
  readonly saving = this.movementsFacade.saving;
  readonly errorMessage = this.movementsFacade.error;
  readonly activeSearch = this.movementsFacade.search;
  readonly productOptions = this.movementsFacade.productOptions;
  readonly warehouseOptions = this.movementsFacade.warehouseOptions;
  readonly lookupsLoading = this.movementsFacade.lookupsLoading;
  readonly movementTypes = MOVEMENT_TYPES;

  formMode: MovementFormMode = null;

  readonly searchControl = this.fb.control('');
  readonly warehouseFilterControl = this.fb.control('');
  readonly movementTypeFilterControl = this.fb.control('');

  readonly form = this.fb.group({
    productId: ['', Validators.required],
    warehouseId: ['', Validators.required],
    movementType: ['IN' as 'IN' | 'OUT', Validators.required],
    quantity: [1, [Validators.required, Validators.min(0.0001)]],
    movementDate: [this.todayIsoDate(), Validators.required],
    reference: [''],
    notes: [''],
  });

  constructor() {
    this.actions$
      .pipe(ofType(MovementsActions.createMovementSuccess), takeUntilDestroyed())
      .subscribe(() => this.closeForm());
  }

  ngOnInit(): void {
    if (this.activeSearch()) this.searchControl.setValue(this.activeSearch());
    this.movementsFacade.loadLookups();
    this.movementsFacade.loadMovements();
  }

  get modalTitle(): string {
    return this.locale.t('movements.createTitle');
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.form.reset({
      productId: '',
      warehouseId: '',
      movementType: 'IN',
      quantity: 1,
      movementDate: this.todayIsoDate(),
      reference: '',
      notes: '',
    });
  }

  closeForm(): void {
    this.formMode = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    this.movementsFacade.createMovement({
      productId: raw.productId,
      warehouseId: raw.warehouseId,
      movementType: raw.movementType,
      quantity: Number(raw.quantity),
      movementDate: raw.movementDate ? new Date(raw.movementDate).toISOString() : null,
      reference: raw.reference.trim() || null,
      notes: raw.notes.trim() || null,
    });
  }

  applyFilters(): void {
    this.movementsFacade.loadMovements({
      page: 1,
      search: this.searchControl.value,
      warehouseId: this.warehouseFilterControl.value || null,
      movementType: (this.movementTypeFilterControl.value as 'IN' | 'OUT' | '') || null,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.movementsFacade.loadMovements({ page });
  }

  movementTypeLabel(type: string): string {
    return type === 'OUT' ? this.locale.t('movements.typeOut') : this.locale.t('movements.typeIn');
  }

  formatQuantity(value: number, unitSymbol: string | null): string {
    const formatted = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unitSymbol ? `${formatted} ${unitSymbol}` : formatted;
  }

  formatDate(value: string): string {
    return new Date(value).toLocaleString();
  }

  get pageInfoLabel(): string {
    return this.locale.t('movements.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('movements.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('movements.resultsCount', { count: String(this.totalCount()) });
  }

  private todayIsoDate(): string {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const local = new Date(now.getTime() - offset * 60_000);
    return local.toISOString().slice(0, 16);
  }
}
