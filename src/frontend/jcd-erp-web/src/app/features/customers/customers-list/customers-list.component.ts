import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { CustomersActions } from '../../../store/customers/customers.actions';
import { CustomersFacade } from '../../../store/customers/customers.facade';
import { Customer, CustomerFormMode } from '../customers.models';

@Component({
  selector: 'app-customers-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './customers-list.component.html',
  styleUrl: './customers-list.component.scss',
})
export class CustomersListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly customersFacade = inject(CustomersFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly customers = this.customersFacade.customers;
  readonly page = this.customersFacade.page;
  readonly totalPages = this.customersFacade.totalPages;
  readonly totalCount = this.customersFacade.totalCount;
  readonly loading = this.customersFacade.loading;
  readonly saving = this.customersFacade.saving;
  readonly errorMessage = this.customersFacade.error;
  readonly activeSearch = this.customersFacade.search;

  formMode: CustomerFormMode = null;
  editingCustomerId: string | null = null;
  customerToDelete: Customer | null = null;

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
          CustomersActions.createCustomerSuccess,
          CustomersActions.updateCustomerSuccess,
          CustomersActions.deleteCustomerSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.customerToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) {
      this.searchControl.setValue(currentSearch);
    }

    this.customersFacade.loadCustomers();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('customers.createTitle')
      : this.locale.t('customers.editTitle');
  }

  get deleteMessage(): string {
    if (!this.customerToDelete) {
      return '';
    }

    return this.locale.t('customers.deleteMessage', { name: this.customerToDelete.legalName });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingCustomerId = null;
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

  openEditForm(customer: Customer): void {
    this.formMode = 'edit';
    this.editingCustomerId = customer.id;
    this.form.reset({
      code: customer.code,
      legalName: customer.legalName,
      tradeName: customer.tradeName ?? '',
      taxId: customer.taxId ?? '',
      email: customer.email ?? '',
      phone: customer.phone ?? '',
      mobilePhone: customer.mobilePhone ?? '',
      addressLine1: customer.addressLine1 ?? '',
      city: customer.city ?? '',
      stateOrProvince: customer.stateOrProvince ?? '',
      countryCode: customer.countryCode ?? '',
      notes: customer.notes ?? '',
      isActive: customer.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingCustomerId = null;
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
      this.customersFacade.createCustomer(payload);
      return;
    }

    if (this.editingCustomerId) {
      this.customersFacade.updateCustomer(this.editingCustomerId, { ...payload, isActive: raw.isActive });
    }
  }

  applySearch(): void {
    this.customersFacade.loadCustomers({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.customersFacade.loadCustomers({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }

    this.customersFacade.loadCustomers({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('customers.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('customers.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('customers.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(customer: Customer): void {
    this.customerToDelete = customer;
  }

  closeDeleteDialog(): void {
    this.customerToDelete = null;
  }

  confirmDelete(): void {
    if (!this.customerToDelete) {
      return;
    }

    this.customersFacade.deleteCustomer(this.customerToDelete.id);
  }

  displayName(customer: Customer): string {
    return customer.tradeName?.trim() || customer.legalName;
  }
}
