import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { BrandsActions } from '../../../store/brands/brands.actions';
import { BrandsFacade } from '../../../store/brands/brands.facade';
import { Brand, BrandFormMode } from '../brands.models';

@Component({
  selector: 'app-brands-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './brands-list.component.html',
  styleUrl: './brands-list.component.scss',
})
export class BrandsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly brandsFacade = inject(BrandsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly brands = this.brandsFacade.brands;
  readonly page = this.brandsFacade.page;
  readonly totalPages = this.brandsFacade.totalPages;
  readonly totalCount = this.brandsFacade.totalCount;
  readonly loading = this.brandsFacade.loading;
  readonly saving = this.brandsFacade.saving;
  readonly errorMessage = this.brandsFacade.error;
  readonly activeSearch = this.brandsFacade.search;

  formMode: BrandFormMode = null;
  editingBrandId: string | null = null;
  brandToDelete: Brand | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(BrandsActions.createBrandSuccess, BrandsActions.updateBrandSuccess, BrandsActions.deleteBrandSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.brandToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) {
      this.searchControl.setValue(currentSearch);
    }

    this.brandsFacade.loadBrands();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('brands.createTitle')
      : this.locale.t('brands.editTitle');
  }

  get deleteMessage(): string {
    if (!this.brandToDelete) {
      return '';
    }

    return this.locale.t('brands.deleteMessage', { name: this.brandToDelete.name });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingBrandId = null;
    this.form.reset({ code: '', name: '', description: '', isActive: true });
  }

  openEditForm(brand: Brand): void {
    this.formMode = 'edit';
    this.editingBrandId = brand.id;
    this.form.reset({
      code: brand.code,
      name: brand.name,
      description: brand.description ?? '',
      isActive: brand.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingBrandId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { code, name, description, isActive } = this.form.getRawValue();
    const payload = {
      code: code.trim().toUpperCase(),
      name: name.trim(),
      description: description.trim() || null,
    };

    if (this.formMode === 'create') {
      this.brandsFacade.createBrand(payload);
      return;
    }

    if (this.editingBrandId) {
      this.brandsFacade.updateBrand(this.editingBrandId, { ...payload, isActive });
    }
  }

  applySearch(): void {
    this.brandsFacade.loadBrands({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.brandsFacade.loadBrands({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) {
      return;
    }

    this.brandsFacade.loadBrands({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('brands.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('brands.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('brands.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(brand: Brand): void {
    this.brandToDelete = brand;
  }

  closeDeleteDialog(): void {
    this.brandToDelete = null;
  }

  confirmDelete(): void {
    if (!this.brandToDelete) {
      return;
    }

    this.brandsFacade.deleteBrand(this.brandToDelete.id);
  }
}
