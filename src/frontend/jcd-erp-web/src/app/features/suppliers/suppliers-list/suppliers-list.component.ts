import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { SuppliersActions } from '../../../store/suppliers/suppliers.actions';
import { SuppliersFacade } from '../../../store/suppliers/suppliers.facade';
import { Supplier, SupplierFormMode } from '../suppliers.models';

@Component({
  selector: 'app-suppliers-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './suppliers-list.component.html',
  styleUrl: './suppliers-list.component.scss',
})
export class SuppliersListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly suppliersFacade = inject(SuppliersFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly suppliers = this.suppliersFacade.suppliers;
  readonly page = this.suppliersFacade.page;
  readonly totalPages = this.suppliersFacade.totalPages;
  readonly totalCount = this.suppliersFacade.totalCount;
  readonly loading = this.suppliersFacade.loading;
  readonly saving = this.suppliersFacade.saving;
  readonly errorMessage = this.suppliersFacade.error;
  readonly activeSearch = this.suppliersFacade.search;

  formMode: SupplierFormMode = null;
  editingSupplierId: string | null = null;
  supplierToDelete: Supplier | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    legalName: ['', [Validators.required, Validators.maxLength(200)]],
    tradeName: [''],
    taxId: ['', Validators.maxLength(30)],
    email: ['', Validators.maxLength(200)],
    phone: ['', Validators.maxLength(30)],
    mobilePhone: ['', Validators.maxLength(30)],
    addressLine1: ['', Validators.maxLength(300)],
    city: ['', Validators.maxLength(100)],
    stateOrProvince: ['', Validators.maxLength(100)],
    countryCode: ['', Validators.maxLength(2)],
    notes: ['', Validators.maxLength(1000)],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          SuppliersActions.createSupplierSuccess,
          SuppliersActions.updateSupplierSuccess,
          SuppliersActions.deleteSupplierSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.supplierToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) {
      this.searchControl.setValue(currentSearch);
    }

    this.suppliersFacade.loadSuppliers();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('suppliers.createTitle')
      : this.locale.t('suppliers.editTitle');
  }

  get deleteMessage(): string {
    if (!this.supplierToDelete) {
      return '';
    }

    return this.locale.t('suppliers.deleteMessage', { name: this.supplierToDelete.legalName });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingSupplierId = null;
    this.form.reset({
      code: '',
      legalName: '',
      tradeName: '',
      taxId: '',
      email: '',
      phone: '',
      mobilePhone: '',
      addressLine1: '',
      city: '',
      stateOrProvince: '',
      countryCode: '',
      notes: '',
      isActive: true,
    });
  }

  openEditForm(supplier: Supplier): void {
    this.formMode = 'edit';
    this.editingSupplierId = supplier.id;
    this.form.reset({
      code: supplier.code,
      legalName: supplier.legalName,
      tradeName: supplier.tradeName ?? '',
      taxId: supplier.taxId ?? '',
      email: supplier.email ?? '',
      phone: supplier.phone ?? '',
      mobilePhone: supplier.mobilePhone ?? '',
      addressLine1: supplier.addressLine1 ?? '',
      city: supplier.city ?? '',
      stateOrProvince: supplier.stateOrProvince ?? '',
      countryCode: supplier.countryCode ?? '',
      notes: supplier.notes ?? '',
      isActive: supplier.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingSupplierId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const payload = {
      code: raw.code.trim().toUpperCase(),
      legalName: raw.legalName.trim(),
      tradeName: raw.tradeName.trim() || null,
      taxId: raw.taxId.trim() || null,
      email: raw.email.trim() || null,
      phone: raw.phone.trim() || null,
      mobilePhone: raw.mobilePhone.trim() || null,
      addressLine1: raw.addressLine1.trim() || null,
      city: raw.city.trim() || null,
      stateOrProvince: raw.stateOrProvince.trim() || null,
      countryCode: raw.countryCode.trim().toUpperCase() || null,
      notes: raw.notes.trim() || null,
    };

    if (this.formMode === 'create') {
      this.suppliersFacade.createSupplier(payload);
      return;
    }

    if (this.editingSupplierId) {
      this.suppliersFacade.updateSupplier(this.editingSupplierId, { ...payload, isActive: raw.isActive });
    }
  }

  applySearch(): void {
    this.suppliersFacade.loadSuppliers({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.suppliersFacade.loadSuppliers({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }

    this.suppliersFacade.loadSuppliers({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('suppliers.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('suppliers.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('suppliers.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(supplier: Supplier): void {
    this.supplierToDelete = supplier;
  }

  closeDeleteDialog(): void {
    this.supplierToDelete = null;
  }

  confirmDelete(): void {
    if (!this.supplierToDelete) {
      return;
    }

    this.suppliersFacade.deleteSupplier(this.supplierToDelete.id);
  }

  displayName(supplier: Supplier): string {
    return supplier.tradeName?.trim() || supplier.legalName;
  }
}
