export type LocaleId = 'es' | 'en';

export const DEFAULT_LOCALE: LocaleId = 'es';
export const SUPPORTED_LOCALES: LocaleId[] = ['es', 'en'];
export const LOCALE_STORAGE_KEY = 'jcd-erp-locale';

export type TranslationDictionary = Record<string, unknown>;
