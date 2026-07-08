import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { LocationsActions } from '../../../store/locations/locations.actions';
import { LocationsFacade } from '../../../store/locations/locations.facade';
import { LocationFormMode, StorageLocation } from '../locations.models';

const LOCATION_TYPES = ['Zone', 'Aisle', 'Shelf', 'Bin'] as const;

@Component({
  selector: 'app-locations-list',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './locations-list.component.html',
  styleUrl: './locations-list.component.scss',
})
export class LocationsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly locationsFacade = inject(LocationsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly locationTypes = LOCATION_TYPES;
  readonly locations = this.locationsFacade.locations;
  readonly parentOptions = this.locationsFacade.parentOptions;
  readonly page = this.locationsFacade.page;
  readonly totalPages = this.locationsFacade.totalPages;
  readonly totalCount = this.locationsFacade.totalCount;
  readonly loading = this.locationsFacade.loading;
  readonly saving = this.locationsFacade.saving;
  readonly errorMessage = this.locationsFacade.error;
  readonly activeSearch = this.locationsFacade.search;

  warehouseId = '';
  warehouseName = '';

  formMode: LocationFormMode = null;
  editingLocationId: string | null = null;
  locationToDelete: StorageLocation | null = null;

  readonly searchControl = this.fb.control('');

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    parentId: [''],
    locationType: [''],
    isActive: [true],
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(
          LocationsActions.createLocationSuccess,
          LocationsActions.updateLocationSuccess,
          LocationsActions.deleteLocationSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.locationToDelete = null;
      });
  }

  ngOnInit(): void {
    this.warehouseId = this.route.snapshot.paramMap.get('warehouseId') ?? '';
    this.warehouseName = history.state?.warehouseName ?? this.warehouseId;

    const currentSearch = this.activeSearch();
    if (currentSearch) this.searchControl.setValue(currentSearch);

    this.locationsFacade.loadLocations({ warehouseId: this.warehouseId });
    this.locationsFacade.loadParentOptions(this.warehouseId);
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('locations.createTitle')
      : this.locale.t('locations.editTitle');
  }

  get deleteMessage(): string {
    return this.locationToDelete
      ? this.locale.t('locations.deleteMessage', { name: this.locationToDelete.name })
      : '';
  }

  get pageInfoLabel(): string {
    return this.locale.t('locations.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('locations.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('locations.resultsCount', { count: String(this.totalCount()) });
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingLocationId = null;
    this.locationsFacade.loadParentOptions(this.warehouseId);
    this.form.reset({ code: '', name: '', description: '', parentId: '', locationType: '', isActive: true });
  }

  openEditForm(location: StorageLocation): void {
    this.formMode = 'edit';
    this.editingLocationId = location.id;
    this.locationsFacade.loadParentOptions(this.warehouseId, location.id);
    this.form.reset({
      code: location.code,
      name: location.name,
      description: location.description ?? '',
      parentId: location.parentId ?? '',
      locationType: location.locationType ?? '',
      isActive: location.isActive,
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingLocationId = null;
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { code, name, description, parentId, locationType, isActive } = this.form.getRawValue();
    const payload = {
      code: code.trim().toUpperCase(),
      name: name.trim(),
      description: description.trim() || null,
      parentId: parentId || null,
      locationType: locationType || null,
    };

    if (this.formMode === 'create') {
      this.locationsFacade.createLocation({ warehouseId: this.warehouseId, ...payload });
      return;
    }

    if (this.editingLocationId) {
      this.locationsFacade.updateLocation(this.editingLocationId, { ...payload, isActive });
    }
  }

  applySearch(): void {
    this.locationsFacade.loadLocations({
      warehouseId: this.warehouseId,
      page: 1,
      search: this.searchControl.value,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.locationsFacade.loadLocations({ warehouseId: this.warehouseId, page: 1, search: '' });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.locationsFacade.loadLocations({ warehouseId: this.warehouseId, page });
  }

  openDeleteDialog(location: StorageLocation): void {
    this.locationToDelete = location;
  }

  closeDeleteDialog(): void {
    this.locationToDelete = null;
  }

  confirmDelete(): void {
    if (this.locationToDelete) {
      this.locationsFacade.deleteLocation(this.locationToDelete.id);
    }
  }

  locationTypeLabel(type: string): string {
    return this.locale.t(`locations.types.${type.toLowerCase()}`);
  }
}
