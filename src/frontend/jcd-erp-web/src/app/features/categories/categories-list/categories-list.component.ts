import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { CategoriesActions } from '../../../store/categories/categories.actions';
import { CategoriesFacade } from '../../../store/categories/categories.facade';
import { CategoryFormMode, ProductCategory } from '../categories.models';

@Component({
  selector: 'app-categories-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './categories-list.component.html',
  styleUrl: './categories-list.component.scss',
})
export class CategoriesListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly categoriesFacade = inject(CategoriesFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly categories = this.categoriesFacade.categories;
  readonly parentOptions = this.categoriesFacade.parentOptions;
  readonly page = this.categoriesFacade.page;
  readonly totalPages = this.categoriesFacade.totalPages;
  readonly totalCount = this.categoriesFacade.totalCount;
  readonly loading = this.categoriesFacade.loading;
  readonly saving = this.categoriesFacade.saving;
  readonly errorMessage = this.categoriesFacade.error;
  readonly activeSearch = this.categoriesFacade.search;

  formMode: CategoryFormMode = null;
  editingCategoryId: string | null = null;
  categoryToDelete: ProductCategory | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    parentId: [''],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          CategoriesActions.createCategorySuccess,
          CategoriesActions.updateCategorySuccess,
          CategoriesActions.deleteCategorySuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.categoryToDelete = null;
      });
  }

  ngOnInit(): void {
    const currentSearch = this.activeSearch();
    if (currentSearch) this.searchControl.setValue(currentSearch);

    this.categoriesFacade.loadCategories();
    this.categoriesFacade.loadParentOptions();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('categories.createTitle')
      : this.locale.t('categories.editTitle');
  }

  get deleteMessage(): string {
    return this.categoryToDelete
      ? this.locale.t('categories.deleteMessage', { name: this.categoryToDelete.name })
      : '';
  }

  get pageInfoLabel(): string {
    return this.locale.t('categories.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('categories.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('categories.resultsCount', { count: String(this.totalCount()) });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingCategoryId = null;
    this.categoriesFacade.loadParentOptions();
    this.form.reset({ name: '', description: '', parentId: '', isActive: true });
  }

  openEditForm(category: ProductCategory): void {
    this.formMode = 'edit';
    this.editingCategoryId = category.id;
    this.categoriesFacade.loadParentOptions(category.id);
    this.form.reset({
      name: category.name,
      description: category.description ?? '',
      parentId: category.parentId ?? '',
      isActive: category.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingCategoryId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, description, parentId, isActive } = this.form.getRawValue();
    const payload = {
      name: name.trim(),
      description: description.trim() || null,
      parentId: parentId || null,
    };

    if (this.formMode === 'create') {
      this.categoriesFacade.createCategory(payload);
      return;
    }

    if (this.editingCategoryId) {
      this.categoriesFacade.updateCategory(this.editingCategoryId, { ...payload, isActive });
    }
  }

  applySearch(): void {
    this.categoriesFacade.loadCategories({ page: 1, search: this.searchControl.value });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.categoriesFacade.loadCategories({ page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.categoriesFacade.loadCategories({ page });
  }

  openDeleteDialog(category: ProductCategory): void {
    this.categoryToDelete = category;
  }

  closeDeleteDialog(): void {
    this.categoryToDelete = null;
  }

  confirmDelete(): void {
    if (this.categoryToDelete) {
      this.categoriesFacade.deleteCategory(this.categoryToDelete.id);
    }
  }
}
