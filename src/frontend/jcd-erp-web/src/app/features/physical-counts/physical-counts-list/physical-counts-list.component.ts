import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { NgClass } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { translatePlatformErrorCode } from '../../../core/platform/platform-error-messages';
import { PhysicalCountsActions } from '../../../store/physical-counts/physical-counts.actions';
import { PhysicalCountsFacade } from '../../../store/physical-counts/physical-counts.facade';
import {
  PhysicalCountFormMode,
  PhysicalCountLineUpdate,
  PhysicalInventoryCount,
} from '../physical-counts.models';

@Component({
  selector: 'app-physical-counts-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe, NgClass],
  templateUrl: './physical-counts-list.component.html',
  styleUrl: './physical-counts-list.component.scss',
})
export class PhysicalCountsListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly physicalCountsFacade = inject(PhysicalCountsFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly physicalCounts = this.physicalCountsFacade.physicalCounts;
  readonly selectedCount = this.physicalCountsFacade.selectedCount;
  readonly page = this.physicalCountsFacade.page;
  readonly totalPages = this.physicalCountsFacade.totalPages;
  readonly totalCount = this.physicalCountsFacade.totalCount;
  readonly loading = this.physicalCountsFacade.loading;
  readonly detailLoading = this.physicalCountsFacade.detailLoading;
  readonly saving = this.physicalCountsFacade.saving;
  readonly errorCode = this.physicalCountsFacade.error;
  readonly errorMessage = computed(() => {
    const code = this.errorCode();
    if (!code) return null;
    return translatePlatformErrorCode(code, (key) => this.locale.t(key));
  });
  readonly activeSearch = this.physicalCountsFacade.search;
  readonly warehouseOptions = this.physicalCountsFacade.warehouseOptions;
  readonly lookupsLoading = this.physicalCountsFacade.lookupsLoading;

  formMode: PhysicalCountFormMode = null;
  expandedCountId: string | null = null;
  formSubmitError: string | null = null;
  countToComplete: PhysicalInventoryCount | null = null;
  countToCancel: PhysicalInventoryCount | null = null;
  private pendingCompleteLines: PhysicalCountLineUpdate[] | null = null;

  readonly lineDrafts = signal<Record<string, string>>({});

  readonly searchControl = this.fb.control('');
  readonly warehouseFilterControl = this.fb.control('');
  readonly statusFilterControl = this.fb.control('');

  readonly form = this.fb.group({
    warehouseId: ['', Validators.required],
    countDate: [this.todayIsoDate(), Validators.required],
    notes: [''],
  });

  constructor() {
    this.actions$
      .pipe(ofType(PhysicalCountsActions.createPhysicalCountSuccess), takeUntilDestroyed())
      .subscribe(({ id }) => {
        this.closeForm();
        this.expandedCountId = id.toLowerCase();
        this.physicalCountsFacade.loadPhysicalCount(id);
      });

    this.actions$
      .pipe(
        ofType(
          PhysicalCountsActions.updatePhysicalCountLinesSuccess,
          PhysicalCountsActions.completePhysicalCountSuccess,
          PhysicalCountsActions.cancelPhysicalCountSuccess,
        ),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.countToComplete = null;
        this.countToCancel = null;
        this.pendingCompleteLines = null;
        if (this.expandedCountId) {
          this.physicalCountsFacade.loadPhysicalCount(this.expandedCountId);
        }
      });
  }

  ngOnInit(): void {
    if (this.activeSearch()) this.searchControl.setValue(this.activeSearch());
    this.physicalCountsFacade.loadLookups();
    this.physicalCountsFacade.loadPhysicalCounts();
  }

  get modalTitle(): string {
    return this.locale.t('physicalCounts.createTitle');
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.formSubmitError = null;
    this.physicalCountsFacade.clearError();
    this.physicalCountsFacade.loadLookups();
    this.form.reset({
      warehouseId: '',
      countDate: this.todayIsoDate(),
      notes: '',
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.formSubmitError = null;
  }

  submitCreate(): void {
    this.formSubmitError = null;

    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      this.formSubmitError = this.locale.t('physicalCounts.formIncomplete');
      return;
    }

    const value = this.form.getRawValue();
    this.physicalCountsFacade.createPhysicalCount({
      warehouseId: value.warehouseId,
      countDate: new Date(value.countDate).toISOString(),
      notes: value.notes.trim() || null,
    });
  }

  applyFilters(): void {
    this.physicalCountsFacade.loadPhysicalCounts({
      page: 1,
      search: this.searchControl.value,
      warehouseId: this.warehouseFilterControl.value || undefined,
      status: this.statusFilterControl.value || undefined,
    });
  }

  clearSearch(): void {
    this.searchControl.setValue('');
    this.applyFilters();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.physicalCountsFacade.loadPhysicalCounts({ page });
  }

  isExpanded(countId: string): boolean {
    return this.expandedCountId === countId;
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'COMPLETED':
        return 'badge--success';
      case 'DRAFT':
        return 'badge--warning';
      default:
        return 'badge--muted';
    }
  }

  toggleDetails(count: PhysicalInventoryCount): void {
    if (this.isExpanded(count.id)) {
      this.expandedCountId = null;
      return;
    }

    this.expandedCountId = count.id;
    this.physicalCountsFacade.loadPhysicalCount(count.id);
    this.syncLineDrafts(this.selectedCount() ?? count);
  }

  isDraft(count: PhysicalInventoryCount): boolean {
    return count.status === 'DRAFT';
  }

  statusLabel(status: string): string {
    const key = `physicalCounts.status.${status.toLowerCase()}`;
    const translated = this.locale.t(key);
    return translated === key ? status : translated;
  }

  getLineDraft(lineId: string, fallback: number | null): string {
    const draft = this.lineDrafts()[lineId];
    if (draft !== undefined) return draft;
    return fallback === null ? '' : String(Math.trunc(fallback));
  }

  setLineDraft(lineId: string, value: string): void {
    this.lineDrafts.update((drafts) => ({ ...drafts, [lineId]: value }));
  }

  saveLines(count: PhysicalInventoryCount): void {
    this.physicalCountsFacade.clearError();
    this.physicalCountsFacade.updateLines(count.id, this.buildLineUpdates(count));
  }

  requestComplete(count: PhysicalInventoryCount): void {
    const detail = this.selectedCount() ?? count;
    const lines = this.buildLineUpdates(detail);

    if (!lines.some((line) => line.countedQuantity !== null)) {
      this.physicalCountsFacade.clearError();
      this.formSubmitError = this.locale.t('errors.physicalCountNoCountedLines');
      return;
    }

    this.formSubmitError = null;
    this.physicalCountsFacade.clearError();
    this.pendingCompleteLines = lines;
    this.countToComplete = detail;
  }

  requestCancel(count: PhysicalInventoryCount): void {
    this.countToCancel = count;
  }

  confirmComplete(): void {
    if (!this.countToComplete) return;
    this.physicalCountsFacade.complete(
      this.countToComplete.id,
      this.pendingCompleteLines ?? undefined,
    );
  }

  confirmCancel(): void {
    if (!this.countToCancel) return;
    this.physicalCountsFacade.cancel(this.countToCancel.id);
  }

  formatDate(value: string): string {
    if (!value) return '—';
    return new Date(value).toLocaleString();
  }

  formatQuantity(value: number | null, unit: string | null): string {
    if (value === null) return '—';
    const qty = Number.isInteger(value) ? String(value) : value.toFixed(2);
    return unit ? `${qty} ${unit}` : qty;
  }

  get pageInfoLabel(): string {
    return this.locale.t('physicalCounts.pageInfo', {
      page: String(this.page()),
      totalPages: String(this.totalPages()),
      totalCount: String(this.totalCount()),
    });
  }

  get searchActiveLabel(): string {
    return this.locale.t('physicalCounts.searchActive', { term: this.activeSearch() });
  }

  get resultsCountLabel(): string {
    return this.locale.t('physicalCounts.resultsCount', { count: String(this.totalCount()) });
  }

  private buildLineUpdates(count: PhysicalInventoryCount): PhysicalCountLineUpdate[] {
    return count.lines.map((line) => {
      const raw = this.getLineDraft(line.id, line.countedQuantity).trim();
      return {
        lineId: line.id,
        countedQuantity: raw === '' ? null : Math.max(0, Math.trunc(Number(raw))),
      };
    });
  }

  private syncLineDrafts(count: PhysicalInventoryCount): void {
    const drafts: Record<string, string> = {};
    for (const line of count.lines) {
      drafts[line.id] = line.countedQuantity === null ? '' : String(Math.trunc(line.countedQuantity));
    }
    this.lineDrafts.set(drafts);
  }

  private todayIsoDate(): string {
    const now = new Date();
    const offset = now.getTimezoneOffset();
    const local = new Date(now.getTime() - offset * 60_000);
    return local.toISOString().slice(0, 16);
  }
}
