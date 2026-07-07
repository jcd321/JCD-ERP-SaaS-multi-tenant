import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import {
  Component,
  effect,
  HostListener,
  inject,
  input,
  output,
  PLATFORM_ID,
} from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
  private readonly document = inject(DOCUMENT);
  private readonly platformId = inject(PLATFORM_ID);

  readonly open = input(false);
  readonly title = input('Confirmar');
  readonly message = input.required<string>();
  readonly confirmLabel = input('Confirmar');
  readonly cancelLabel = input('Cancelar');
  readonly loading = input(false);
  readonly danger = input(true);

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  readonly titleId = `confirm-dialog-title-${crypto.randomUUID()}`;

  constructor() {
    effect(() => {
      if (!isPlatformBrowser(this.platformId)) {
        return;
      }

      this.document.body.classList.toggle('modal-open', this.open());
    });
  }

  @HostListener('document:keydown.escape')
  onEscape(): void {
    if (this.open()) {
      this.cancelled.emit();
    }
  }

  onBackdropClick(): void {
    this.cancelled.emit();
  }
}
