import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { ProductsActions } from '../../../store/products/products.actions';
import { ProductsFacade } from '../../../store/products/products.facade';
import { Product, ProductFormMode } from '../products.models';

@Component({
  selector: 'app-products-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './products-list.component.html',
  styleUrl: './products-list.component.scss',
})
export class ProductsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly productsFacade = inject(ProductsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly products = this.productsFacade.products;
  readonly lookups = this.productsFacade.lookups;
  readonly page = this.productsFacade.page;
  readonly totalPages = this.productsFacade.totalPages;
  readonly totalCount = this.productsFacade.totalCount;
  readonly loading = this.productsFacade.loading;
  readonly saving = this.productsFacade.saving;
  readonly errorMessage = this.productsFacade.error;
  readonly activeSearch = this.productsFacade.search;

  formMode: ProductFormMode = null;
  editingProductId: string | null = null;
  productToDelete: Product | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    sku: ['', [Validators.required, Validators.maxLength(50)]],
    name: ['', [Validators.required, Validators.maxLength(200)]],
    description: [''],
    categoryId: ['', Validators.required],
    brandId: [''],
    unitId: ['', Validators.required],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          ProductsActions.createProductSuccess,
          ProductsActions.updateProductSuccess,
          ProductsActions.deleteProductSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.productToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) this.searchControl.setValue(currentSearch);

    this.productsFacade.loadProducts();
    this.productsFacade.loadLookups();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('products.createTitle')
      : this.locale.t('products.editTitle');
  }

  get deleteMessage(): string {
    return this.productToDelete
      ? this.locale.t('products.deleteMessage', { name: this.productToDelete.name })
      : '';
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingProductId = null;
    this.form.reset({
      sku: '',
      name: '',
      description: '',
      categoryId: '',
      brandId: '',
      unitId: '',
      isActive: true,
    });
  }

  openEditForm(product: Product): void {
    this.formMode = 'edit';
    this.editingProductId = product.id;
    this.form.reset({
      sku: product.sku,
      name: product.name,
      description: product.description ?? '',
      categoryId: product.categoryId,
      brandId: product.brandId ?? '',
      unitId: product.unitId,
      isActive: product.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingProductId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { sku, name, description, categoryId, brandId, unitId, isActive } = this.form.getRawValue();
    const payload = {
      sku: sku.trim().toUpperCase(),
      name: name.trim(),
      description: description.trim() || null,
      categoryId,
      unitId,
      brandId: brandId.trim() || null,
    };

    if (this.formMode === 'create') {
      this.productsFacade.createProduct(payload);
      return;
    }

    if (this.editingProductId) {
      this.productsFacade.updateProduct(this.editingProductId, { ...payload, isActive });
    }
  }

  applySearch(): void {
    this.productsFacade.loadProducts({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.productsFacade.loadProducts({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.productsFacade.loadProducts({ page });
  }

  get pageInfoLabel(): string {
    return this.locale.t('products.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('products.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('products.resultsCount', { count: String(this.totalCount()) });
  }

  openDeleteDialog(product: Product): void {
    this.productToDelete = product;
  }

  closeDeleteDialog(): void {
    this.productToDelete = null;
  }

  confirmDelete(): void {
    if (!this.productToDelete) return;
    this.productsFacade.deleteProduct(this.productToDelete.id);
  }
}
