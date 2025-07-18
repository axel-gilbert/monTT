import { writable } from 'svelte/store';
import { apiService } from '$lib/services/api';
import type { User, RegisterRequest, LoginRequest } from '$lib/types/api';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
}

function createAuthStore() {
  const { subscribe, set, update } = writable<AuthState>({
    user: null,
    isAuthenticated: false,
    loading: false,
    error: null
  });

  return {
    subscribe,
    init: () => {
      // Vérifier si un token existe au démarrage
      const token = apiService.getToken();
      if (token) {
        update(state => ({ ...state, isAuthenticated: true }));
      }
    },
    login: async (email: string, password: string) => {
      update(state => ({ ...state, loading: true, error: null }));
      
      try {
        const response = await apiService.login({ email, password });
        update(state => ({
          ...state,
          user: response.user,
          isAuthenticated: true,
          loading: false,
          error: null
        }));
      } catch (error) {
        update(state => ({
          ...state,
          loading: false,
          error: error instanceof Error ? error.message : 'Erreur de connexion'
        }));
        throw error;
      }
    },
    register: async (userData: RegisterRequest) => {
      update(state => ({ ...state, loading: true, error: null }));
      
      try {
        const response = await apiService.register(userData);
        update(state => ({
          ...state,
          user: response.user,
          isAuthenticated: true,
          loading: false,
          error: null
        }));
      } catch (error) {
        update(state => ({
          ...state,
          loading: false,
          error: error instanceof Error ? error.message : 'Erreur d\'inscription'
        }));
        throw error;
      }
    },
    logout: () => {
      apiService.logout();
      set({
        user: null,
        isAuthenticated: false,
        loading: false,
        error: null
      });
    },
    clearError: () => {
      update(state => ({ ...state, error: null }));
    },
    updateUser: (user: User) => {
      update(state => ({ ...state, user }));
    },
    updateProfile: (profile: any) => {
      update(state => ({ 
        ...state, 
        user: state.user ? { ...state.user, employee: profile } : null 
      }));
    }
  };
}

export const authStore = createAuthStore(); 