export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
}
