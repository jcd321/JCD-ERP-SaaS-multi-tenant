import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { WarehousesActions } from '../../../store/warehouses/warehouses.actions';
import { WarehousesFacade } from '../../../store/warehouses/warehouses.facade';
import { Warehouse, WarehouseFormMode } from '../warehouses.models';

@Component({
  selector: 'app-warehouses-list',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './warehouses-list.component.html',
  styleUrl: './warehouses-list.component.scss',
})
export class WarehousesListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly warehousesFacade = inject(WarehousesFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly warehouses = this.warehousesFacade.warehouses;
  readonly page = this.warehousesFacade.page;
  readonly totalPages = this.warehousesFacade.totalPages;
  readonly totalCount = this.warehousesFacade.totalCount;
  readonly loading = this.warehousesFacade.loading;
  readonly saving = this.warehousesFacade.saving;
  readonly errorMessage = this.warehousesFacade.error;
  readonly activeSearch = this.warehousesFacade.search;

  formMode: WarehouseFormMode = null;
  editingWarehouseId: string | null = null;
  warehouseToDelete: Warehouse | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    addressLine1: ['', Validators.maxLength(200)],
    city: ['', Validators.maxLength(100)],
    stateOrProvince: ['', Validators.maxLength(100)],
    countryCode: ['', Validators.maxLength(2)],
    isDefault: [false],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          WarehousesActions.createWarehouseSuccess,
          WarehousesActions.updateWarehouseSuccess,
          WarehousesActions.deleteWarehouseSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.warehouseToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) this.searchControl.setValue(currentSearch);
    this.warehousesFacade.loadWarehouses();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('warehouses.createTitle')
      : this.locale.t('warehouses.editTitle');
  }

  get deleteMessage(): string {
    return this.warehouseToDelete
      ? this.locale.t('warehouses.deleteMessage', { name: this.warehouseToDelete.name })
      : '';
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingWarehouseId = null;
    this.form.reset({
      code: '',
      name: '',
      description: '',
      addressLine1: '',
      city: '',
      stateOrProvince: '',
      countryCode: '',
      isDefault: false,
      isActive: true,
    });
  }

  openEditForm(warehouse: Warehouse): void {
    this.formMode = 'edit';
    this.editingWarehouseId = warehouse.id;
    this.form.reset({
      code: warehouse.code,
      name: warehouse.name,
      description: warehouse.description ?? '',
      addressLine1: warehouse.addressLine1 ?? '',
      city: warehouse.city ?? '',
      stateOrProvince: warehouse.stateOrProvince ?? '',
      countryCode: warehouse.countryCode ?? '',
      isDefault: warehouse.isDefault,
      isActive: warehouse.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingWarehouseId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload = {
      code: raw.code.trim().toUpperCase(),
      name: raw.name.trim(),
      description: raw.description.trim() || null,
      addressLine1: raw.addressLine1.trim() || null,
      city: raw.city.trim() || null,
      stateOrProvince: raw.stateOrProvince.trim() || null,
      countryCode: raw.countryCode.trim().toUpperCase() || null,
      isDefault: raw.isDefault,
    };

    if (this.formMode === 'create') {
      this.warehousesFacade.createWarehouse(payload);
      return;
    }

    if (this.editingWarehouseId) {
      this.warehousesFacade.updateWarehouse(this.editingWarehouseId, {
        ...payload,
        isActive: raw.isActive,
      });
    }
  }

  applySearch(): void {
    this.warehousesFacade.loadWarehouses({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.warehousesFacade.loadWarehouses({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.warehousesFacade.loadWarehouses({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('warehouses.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('warehouses.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('warehouses.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(warehouse: Warehouse): void {
    this.warehouseToDelete = warehouse;
  }

  closeDeleteDialog(): void {
    this.warehouseToDelete = null;
  }

  confirmDelete(): void {
    if (this.warehouseToDelete) {
      this.warehousesFacade.deleteWarehouse(this.warehouseToDelete.id);
    }
  }
}
