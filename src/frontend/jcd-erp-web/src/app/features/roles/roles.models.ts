export interface Role {
  id: string;
  name: string;
  description: string | null;
  isSystem: boolean;
  permissions: string[];
}
