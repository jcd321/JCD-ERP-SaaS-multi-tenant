import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { UnitsActions } from '../../../store/units/units.actions';
import { UnitsFacade } from '../../../store/units/units.facade';
import { UnitFormMode, UnitOfMeasure } from '../units.models';

@Component({
  selector: 'app-units-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './units-list.component.html',
  styleUrl: './units-list.component.scss',
})
export class UnitsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly unitsFacade = inject(UnitsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly units = this.unitsFacade.units;
  readonly page = this.unitsFacade.page;
  readonly totalPages = this.unitsFacade.totalPages;
  readonly totalCount = this.unitsFacade.totalCount;
  readonly loading = this.unitsFacade.loading;
  readonly saving = this.unitsFacade.saving;
  readonly errorMessage = this.unitsFacade.error;
  readonly activeSearch = this.unitsFacade.search;

  formMode: UnitFormMode = null;
  editingUnitId: string | null = null;
  unitToDelete: UnitOfMeasure | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    symbol: [''],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(UnitsActions.createUnitSuccess, UnitsActions.updateUnitSuccess, UnitsActions.deleteUnitSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.unitToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) {
      this.searchControl.setValue(currentSearch);
    }

    this.unitsFacade.loadUnits();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('units.createTitle')
      : this.locale.t('units.editTitle');
  }

  get deleteMessage(): string {
    if (!this.unitToDelete) {
      return '';
    }

    return this.locale.t('units.deleteMessage', { name: this.unitToDelete.name });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingUnitId = null;
    this.form.reset({ code: '', name: '', symbol: '', isActive: true });
  }

  openEditForm(unit: UnitOfMeasure): void {
    this.formMode = 'edit';
    this.editingUnitId = unit.id;
    this.form.reset({
      code: unit.code,
      name: unit.name,
      symbol: unit.symbol ?? '',
      isActive: unit.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingUnitId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { code, name, symbol, isActive } = this.form.getRawValue();
    const payload = {
      code: code.trim().toUpperCase(),
      name: name.trim(),
      symbol: symbol.trim() || null,
    };

    if (this.formMode === 'create') {
      this.unitsFacade.createUnit(payload);
      return;
    }

    if (this.editingUnitId) {
      this.unitsFacade.updateUnit(this.editingUnitId, { ...payload, isActive });
    }
  }

  applySearch(): void {
    this.unitsFacade.loadUnits({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.unitsFacade.loadUnits({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }

    this.unitsFacade.loadUnits({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('units.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('units.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('units.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(unit: UnitOfMeasure): void {
    this.unitToDelete = unit;
  }

  closeDeleteDialog(): void {
    this.unitToDelete = null;
  }

  confirmDelete(): void {
    if (!this.unitToDelete) {
      return;
    }

    this.unitsFacade.deleteUnit(this.unitToDelete.id);
  }
}
