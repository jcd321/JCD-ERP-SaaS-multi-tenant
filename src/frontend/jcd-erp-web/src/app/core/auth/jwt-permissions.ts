const PERMISSION_CLAIM = 'permission';

export function extractPermissionsFromAccessToken(accessToken: string): string[] | null {
  try {
    const [, payload] = accessToken.split('.');
    if (!payload) {
      return null;
    }

    const normalized = payload.replace(/-/g, '+').replace(/_/g, '/');
    const decoded = JSON.parse(atob(normalized)) as Record<string, unknown>;
    const raw = decoded[PERMISSION_CLAIM];

    if (raw === undefined || raw === null) {
      return [];
    }

    return Array.isArray(raw) ? raw.map(String) : [String(raw)];
  } catch {
    return null;
  }
}
