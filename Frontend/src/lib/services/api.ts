import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  User,
  Employee,
  EmployeeList,
  Company,
  TeleworkRequest,
  CreateTeleworkRequest,
  ProcessTeleworkRequest,
  WeeklyPlanning,
  UpdateEmployeeRequest,
  AssignEmployeeRequest,
  CreateCompanyRequest,
  UpdateCompanyRequest,
  ApiError
} from '$lib/types/api';

const API_BASE_URL = 'http://localhost:5000/api';

class ApiService {
  private token: string | null = null;

  constructor() {
    // Récupérer le token depuis le localStorage au démarrage
    if (typeof window !== 'undefined') {
      this.token = localStorage.getItem('authToken');
    }
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${API_BASE_URL}${endpoint}`;
    
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...(options.headers as Record<string, string>)
    };

    // Ajouter le token d'authentification si disponible
    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }

    const config: RequestInit = {
      ...options,
      headers
    };

    console.log('API Request:', {
      url,
      method: config.method || 'GET',
      headers: config.headers,
      body: config.body
    });

    try {
      const response = await fetch(url, config);
      
      console.log('API Response:', {
        status: response.status,
        statusText: response.statusText,
        url: response.url
      });
      
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ message: 'Erreur inconnue' }));
        console.error('API Error:', errorData);
        throw new Error(errorData.message || `Erreur ${response.status}`);
      }

      const data = await response.json();
      console.log('API Data:', data);
      return data;
    } catch (error) {
      console.error('API Exception:', error);
      if (error instanceof Error) {
        throw new Error(error.message);
      }
      throw new Error('Erreur de connexion au serveur');
    }
  }

  // Authentification
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await this.request<AuthResponse>('/Auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials)
    });
    
    this.setToken(response.token);
    return response;
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response = await this.request<AuthResponse>('/Auth/register', {
      method: 'POST',
      body: JSON.stringify(userData)
    });
    
    this.setToken(response.token);
    return response;
  }

  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    const response = await this.request<AuthResponse>('/Auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken })
    });
    
    this.setToken(response.token);
    return response;
  }

  // Employés
  async getProfile(): Promise<Employee> {
    return this.request<Employee>('/Employees/profile');
  }

  async updateProfile(data: UpdateEmployeeRequest): Promise<Employee> {
    return this.request<Employee>('/Employees/profile', {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async getAllEmployees(): Promise<EmployeeList[]> {
    return this.request<EmployeeList[]>('/Employees');
  }

  async getEmployeeById(id: number): Promise<Employee> {
    return this.request<Employee>(`/Employees/${id}`);
  }

  async assignEmployeeToCompany(data: AssignEmployeeRequest): Promise<Employee> {
    return this.request<Employee>('/Employees/assign-to-company', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  // Entreprises
  async createCompany(data: CreateCompanyRequest): Promise<Company> {
    return this.request<Company>('/Companies', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async getMyCompany(): Promise<Company> {
    return this.request<Company>('/Companies/my-company');
  }

  async updateCompany(data: UpdateCompanyRequest): Promise<Company> {
    return this.request<Company>('/Companies', {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async getMyCompanyWithEmployees(): Promise<Company> {
    return this.request<Company>('/Companies/my-company/employees');
  }

  // Demandes de télétravail
  async createTeleworkRequest(data: CreateTeleworkRequest): Promise<TeleworkRequest> {
    return this.request<TeleworkRequest>('/TeleworkRequest', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  }

  async getMyRequests(): Promise<TeleworkRequest[]> {
    return this.request<TeleworkRequest[]>('/TeleworkRequest/my-requests');
  }

  async getCompanyRequests(): Promise<TeleworkRequest[]> {
    return this.request<TeleworkRequest[]>('/TeleworkRequest/company');
  }

  async processRequest(id: number, data: ProcessTeleworkRequest): Promise<TeleworkRequest> {
    return this.request<TeleworkRequest>(`/TeleworkRequest/${id}/process`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  }

  async getWeeklyPlanning(weekStart: string): Promise<WeeklyPlanning> {
    return this.request<WeeklyPlanning>(`/TeleworkRequest/company/weekly-planning?weekStart=${weekStart}`);
  }

  // Gestion du token
  setToken(token: string) {
    this.token = token;
    if (typeof window !== 'undefined') {
      localStorage.setItem('authToken', token);
    }
  }

  getToken(): string | null {
    return this.token;
  }

  clearToken() {
    this.token = null;
    if (typeof window !== 'undefined') {
      localStorage.removeItem('authToken');
    }
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  // Déconnexion
  logout() {
    this.clearToken();
  }
}

// Instance singleton
export const apiService = new ApiService(); 