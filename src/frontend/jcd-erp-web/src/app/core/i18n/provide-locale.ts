import { inject, provideAppInitializer } from '@angular/core';

import { LocaleService } from './locale.service';

export function provideLocale() {
  return provideAppInitializer(() => inject(LocaleService).init());
}
