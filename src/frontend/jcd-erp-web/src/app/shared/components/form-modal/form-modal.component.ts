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

import { TranslatePipe } from '../../../core/i18n';

export type FormModalSize = 'md' | 'lg';

@Component({
  selector: 'app-form-modal',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './form-modal.component.html',
  styleUrl: './form-modal.component.scss',
})
export class FormModalComponent {
  private readonly document = inject(DOCUMENT);
  private readonly platformId = inject(PLATFORM_ID);

  readonly open = input(false);
  readonly title = input.required<string>();
  readonly saving = input(false);
  readonly submitLabel = input<string | null>(null);
  readonly cancelLabel = input<string | null>(null);
  readonly size = input<FormModalSize>('md');

  readonly closed = output<void>();
  readonly submitted = output<void>();

  readonly titleId = `form-modal-title-${crypto.randomUUID()}`;

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
      this.closed.emit();
    }
  }

  onBackdropClick(): void {
    this.closed.emit();
  }

  onSubmitClick(): void {
    this.submitted.emit();
  }
}
