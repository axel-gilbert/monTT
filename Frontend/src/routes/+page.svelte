<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';

  let isLoginMode = true;
  let isLoading = false;
  let error = '';

  // Formulaires
  let loginForm = {
    email: '',
    password: ''
  };

  let registerForm = {
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    position: '',
    role: 'User' as 'User' | 'Manager'
  };

  onMount(() => {
    // Rediriger si déjà connecté
    if ($authStore.isAuthenticated) {
      goto('/dashboard');
    }
  });

  async function handleLogin() {
    if (!loginForm.email || !loginForm.password) {
      error = 'Veuillez remplir tous les champs';
      return;
    }

    isLoading = true;
    error = '';

    try {
      await authStore.login(loginForm.email, loginForm.password);
      goto('/dashboard');
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur de connexion';
    } finally {
      isLoading = false;
    }
  }

  async function handleRegister() {
    if (!registerForm.email || !registerForm.password || !registerForm.firstName || 
        !registerForm.lastName || !registerForm.position) {
      error = 'Veuillez remplir tous les champs obligatoires';
      return;
    }

    if (registerForm.password !== registerForm.confirmPassword) {
      error = 'Les mots de passe ne correspondent pas';
      return;
    }

    if (registerForm.password.length < 6) {
      error = 'Le mot de passe doit contenir au moins 6 caractères';
      return;
    }

    isLoading = true;
    error = '';

    try {
      await authStore.register({
        email: registerForm.email,
        password: registerForm.password,
        firstName: registerForm.firstName,
        lastName: registerForm.lastName,
        position: registerForm.position,
        role: registerForm.role
      });
      goto('/dashboard');
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur d\'inscription';
    } finally {
      isLoading = false;
    }
  }

  function switchMode() {
    isLoginMode = !isLoginMode;
    error = '';
    loginForm = { email: '', password: '' };
    registerForm = {
      email: '',
      password: '',
      confirmPassword: '',
      firstName: '',
      lastName: '',
      position: '',
      role: 'User'
    };
  }
</script>

<svelte:head>
  <title>Connexion - MonTT</title>
</svelte:head>

<div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 via-white to-indigo-50 py-12 px-4 sm:px-6 lg:px-8">
  <div class="max-w-md w-full space-y-8">
    <!-- Header -->
    <div class="text-center">
      <div class="mx-auto h-16 w-16 bg-gradient-to-r from-blue-600 to-indigo-600 rounded-2xl flex items-center justify-center mb-6">
        <svg class="h-8 w-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
        </svg>
      </div>
      <h2 class="text-3xl font-extrabold text-gray-900 mb-2">
        {isLoginMode ? 'Connexion' : 'Inscription'}
      </h2>
      <p class="text-gray-600">
        {isLoginMode ? 'Accédez à votre espace de gestion du télétravail' : 'Créez votre compte pour commencer'}
      </p>
    </div>

    <!-- Formulaire -->
    <div class="bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
      {#if error}
        <div class="mb-6 bg-red-50 border border-red-200 rounded-lg p-4">
          <div class="flex">
            <svg class="h-5 w-5 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <p class="ml-3 text-sm text-red-700">{error}</p>
          </div>
        </div>
      {/if}

      {#if isLoginMode}
        <!-- Formulaire de connexion -->
        <form on:submit|preventDefault={handleLogin} class="space-y-6">
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 mb-2">
              Adresse email
            </label>
            <input
              id="email"
              type="email"
              bind:value={loginForm.email}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="votre@email.com"
            />
          </div>

          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 mb-2">
              Mot de passe
            </label>
            <input
              id="password"
              type="password"
              bind:value={loginForm.password}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="••••••••"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            class="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-3 px-4 rounded-lg font-medium hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {#if isLoading}
              <div class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Connexion en cours...
              </div>
            {:else}
              Se connecter
            {/if}
          </button>
        </form>
      {:else}
        <!-- Formulaire d'inscription -->
        <form on:submit|preventDefault={handleRegister} class="space-y-6">
          <div class="grid grid-cols-2 gap-4">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700 mb-2">
                Prénom *
              </label>
              <input
                id="firstName"
                type="text"
                bind:value={registerForm.firstName}
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                placeholder="Jean"
              />
            </div>
            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700 mb-2">
                Nom *
              </label>
              <input
                id="lastName"
                type="text"
                bind:value={registerForm.lastName}
                required
                class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
                placeholder="Dupont"
              />
            </div>
          </div>

          <div>
            <label for="registerEmail" class="block text-sm font-medium text-gray-700 mb-2">
              Adresse email *
            </label>
            <input
              id="registerEmail"
              type="email"
              bind:value={registerForm.email}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="votre@email.com"
            />
          </div>

          <div>
            <label for="position" class="block text-sm font-medium text-gray-700 mb-2">
              Poste *
            </label>
            <input
              id="position"
              type="text"
              bind:value={registerForm.position}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="Développeur Senior"
            />
          </div>

          <div>
            <label for="role" class="block text-sm font-medium text-gray-700 mb-2">
              Rôle *
            </label>
            <select
              id="role"
              bind:value={registerForm.role}
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
            >
              <option value="User">Employé</option>
              <option value="Manager">Manager</option>
            </select>
          </div>

          <div>
            <label for="registerPassword" class="block text-sm font-medium text-gray-700 mb-2">
              Mot de passe *
            </label>
            <input
              id="registerPassword"
              type="password"
              bind:value={registerForm.password}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="••••••••"
            />
          </div>

          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-2">
              Confirmer le mot de passe *
            </label>
            <input
              id="confirmPassword"
              type="password"
              bind:value={registerForm.confirmPassword}
              required
              class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="••••••••"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            class="w-full bg-gradient-to-r from-blue-600 to-indigo-600 text-white py-3 px-4 rounded-lg font-medium hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {#if isLoading}
              <div class="flex items-center justify-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Inscription en cours...
              </div>
            {:else}
              Créer mon compte
            {/if}
          </button>
        </form>
      {/if}

      <!-- Lien pour changer de mode -->
      <div class="mt-6 text-center">
        <button
          on:click={switchMode}
          class="text-blue-600 hover:text-blue-700 font-medium transition-colors"
        >
          {isLoginMode ? 'Pas encore de compte ? S\'inscrire' : 'Déjà un compte ? Se connecter'}
        </button>
      </div>
    </div>

    <!-- Informations de test -->
    <div class="text-center text-sm text-gray-500">
      <p class="mb-2">Comptes de test disponibles :</p>
      <div class="space-y-1">
        <p><strong>Manager:</strong> manager@test.com / password123</p>
        <p><strong>Employé:</strong> marie.martin@test.com / password123</p>
      </div>
    </div>
  </div>
</div>
