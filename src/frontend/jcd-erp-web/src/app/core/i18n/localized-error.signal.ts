import { computed, inject, Signal } from '@angular/core';

import { LocaleService } from './locale.service';

export function createLocalizedError(
  errorCode: Signal<string | null>,
  translateCode: (code: string, translate: (key: string) => string) => string,
): Signal<string | null> {
  const locale = inject(LocaleService);

  return computed(() => {
    locale.locale();
    const code = errorCode();
    return code ? translateCode(code, (key) => locale.t(key)) : null;
  });
}
