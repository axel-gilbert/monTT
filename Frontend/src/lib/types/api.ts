// Types pour l'API Telework Management

export interface User {
  id: number;
  email: string;
  role: string;
  createdAt: string;
  employee?: EmployeeProfile;
}

export interface EmployeeProfile {
  id: number;
  firstName: string;
  lastName: string;
  position: string;
  companyId?: number;
  companyName?: string;
}

export interface Employee {
  id: number;
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  position: string;
  role: string;
  companyId?: number;
  companyName?: string;
}

export interface EmployeeList {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  position: string;
  role: string;
  companyId?: number;
  companyName?: string;
  isAssignedToCompany: boolean;
}

export interface Company {
  id: number;
  name: string;
  managerId: number;
  employees?: EmployeeList[];
}

export interface TeleworkRequest {
  id: number;
  requestDate: string;
  teleworkDate: string;
  reason: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  managerComment?: string;
  processedAt?: string;
  employee: EmployeeList;
  processedByManager?: EmployeeList;
}

export interface CreateTeleworkRequest {
  teleworkDate: string;
  reason: string;
}

export interface ProcessTeleworkRequest {
  status: 'Approved' | 'Rejected';
  managerComment?: string;
}

export interface WeeklyPlanning {
  weekStart: string;
  weekEnd: string;
  dailyRequests: DailyTelework[];
  stats: WeeklyStats;
}

export interface DailyTelework {
  date: string;
  dayName: string;
  requests: TeleworkRequest[];
  totalRequests: number;
  approvedRequests: number;
  pendingRequests: number;
  rejectedRequests: number;
}

export interface WeeklyStats {
  totalRequests: number;
  approvedRequests: number;
  pendingRequests: number;
  rejectedRequests: number;
  approvalRate: number;
  uniqueEmployees: number;
}

export interface AuthResponse {
  token: string;
  refreshToken?: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  position: string;
  role: 'User' | 'Manager';
}

export interface UpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  position: string;
}

export interface AssignEmployeeRequest {
  employeeId: number;
  companyId: number;
}

export interface CreateCompanyRequest {
  name: string;
}

export interface UpdateCompanyRequest {
  name: string;
}

// Types pour les réponses d'erreur
export interface ApiError {
  message: string;
  status?: number;
}

// Types pour les états de chargement
export interface LoadingState {
  isLoading: boolean;
  error?: string;
} 