<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = true;
  let profile: any = null;
  let error = '';
  let isEditing = false;
  let isSaving = false;

  // Formulaire d'édition
  let editForm = {
    firstName: '',
    lastName: '',
    position: ''
  };

  onMount(async () => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    try {
      await loadProfile();
    } catch (error) {
      console.error('Erreur lors du chargement du profil:', error);
    } finally {
      isLoading = false;
    }
  });

  async function loadProfile() {
    try {
      profile = await apiService.getProfile();
      // Pré-remplir le formulaire d'édition
      editForm = {
        firstName: profile.firstName,
        lastName: profile.lastName,
        position: profile.position
      };
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors du chargement du profil';
    }
  }

  function startEditing() {
    isEditing = true;
  }

  function cancelEditing() {
    isEditing = false;
    // Restaurer les valeurs originales
    editForm = {
      firstName: profile.firstName,
      lastName: profile.lastName,
      position: profile.position
    };
  }

  async function saveProfile() {
    isSaving = true;
    try {
      profile = await apiService.updateProfile(editForm);
      isEditing = false;
      // Mettre à jour le store d'authentification
      authStore.updateProfile(profile);
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors de la sauvegarde';
    } finally {
      isSaving = false;
    }
  }

  function formatDate(dateString: string) {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  function getRoleColor(role: string) {
    switch (role) {
      case 'Manager': return 'bg-purple-100 text-purple-800';
      case 'User': return 'bg-blue-100 text-blue-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  function getRoleText(role: string) {
    switch (role) {
      case 'Manager': return 'Manager';
      case 'User': return 'Employé';
      default: return role;
    }
  }
</script>

<svelte:head>
  <title>Mon profil - Telework Management</title>
</svelte:head>

<div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <!-- Header -->
  <div class="mb-8">
    <div class="flex items-center space-x-3 mb-4">
      <a href="/dashboard" class="text-blue-600 hover:text-blue-700">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
        </svg>
      </a>
      <h1 class="text-3xl font-bold text-gray-900">Mon profil</h1>
    </div>
    <p class="text-gray-600">
      Gérez vos informations personnelles et professionnelles
    </p>
  </div>

  {#if isLoading}
    <div class="flex items-center justify-center py-12">
      <div class="flex items-center space-x-3">
        <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-lg text-gray-600">Chargement du profil...</span>
      </div>
    </div>
  {:else if error}
    <div class="bg-red-50 border border-red-200 rounded-xl p-6 text-center">
      <svg class="mx-auto h-12 w-12 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
      </svg>
      <h3 class="mt-4 text-lg font-medium text-red-900">Erreur</h3>
      <p class="mt-2 text-red-700">{error}</p>
      <button
        on:click={loadProfile}
        class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
      >
        Réessayer
      </button>
    </div>
  {:else if profile}
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Carte de profil -->
      <div class="lg:col-span-1">
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div class="text-center">
            <div class="w-24 h-24 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <span class="text-2xl font-bold text-blue-600">
                {profile.firstName[0]}{profile.lastName[0]}
              </span>
            </div>
            <h2 class="text-xl font-semibold text-gray-900 mb-2">
              {profile.firstName} {profile.lastName}
            </h2>
            <p class="text-gray-600 mb-4">{profile.position}</p>
            <span class="px-3 py-1 text-sm font-medium rounded-full {getRoleColor(profile.role)}">
              {getRoleText(profile.role)}
            </span>
          </div>

          <div class="mt-6 pt-6 border-t border-gray-200">
            <div class="space-y-3">
              <div class="flex items-center text-sm text-gray-600">
                <svg class="w-4 h-4 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 12a4 4 0 10-8 0 4 4 0 008 0zm0 0v1.5a2.5 2.5 0 005 0V12a9 9 0 10-9 9m4.5-1.206a8.959 8.959 0 01-4.5 1.207"></path>
                </svg>
                {profile.email}
              </div>
              <div class="flex items-center text-sm text-gray-600">
                <svg class="w-4 h-4 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
                Membre depuis {formatDate(profile.createdAt)}
              </div>
              {#if profile.companyName}
                <div class="flex items-center text-sm text-gray-600">
                  <svg class="w-4 h-4 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
                  </svg>
                  {profile.companyName}
                </div>
              {/if}
            </div>
          </div>

          {#if !isEditing}
            <div class="mt-6">
              <button
                on:click={startEditing}
                class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
              >
                Modifier le profil
              </button>
            </div>
          {/if}
        </div>
      </div>

      <!-- Formulaire d'édition -->
      <div class="lg:col-span-2">
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h3 class="text-lg font-semibold text-gray-900 mb-6">
            {isEditing ? 'Modifier le profil' : 'Informations personnelles'}
          </h3>

          {#if isEditing}
            <form on:submit|preventDefault={saveProfile} class="space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label for="firstName" class="block text-sm font-medium text-gray-700 mb-2">
                    Prénom
                  </label>
                  <input
                    type="text"
                    id="firstName"
                    bind:value={editForm.firstName}
                    required
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>

                <div>
                  <label for="lastName" class="block text-sm font-medium text-gray-700 mb-2">
                    Nom
                  </label>
                  <input
                    type="text"
                    id="lastName"
                    bind:value={editForm.lastName}
                    required
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  />
                </div>
              </div>

              <div>
                <label for="position" class="block text-sm font-medium text-gray-700 mb-2">
                  Poste
                </label>
                <input
                  type="text"
                  id="position"
                  bind:value={editForm.position}
                  required
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                />
              </div>

              <div class="flex items-center justify-end space-x-3 pt-6 border-t border-gray-200">
                <button
                  type="button"
                  on:click={cancelEditing}
                  class="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
                >
                  Annuler
                </button>
                <button
                  type="submit"
                  disabled={isSaving}
                  class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                  {isSaving ? 'Sauvegarde...' : 'Sauvegarder'}
                </button>
              </div>
            </form>
          {:else}
            <div class="space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Prénom
                  </label>
                  <p class="text-gray-900">{profile.firstName}</p>
                </div>

                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Nom
                  </label>
                  <p class="text-gray-900">{profile.lastName}</p>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Poste
                </label>
                <p class="text-gray-900">{profile.position}</p>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Email
                </label>
                <p class="text-gray-900">{profile.email}</p>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Rôle
                </label>
                <span class="px-3 py-1 text-sm font-medium rounded-full {getRoleColor(profile.role)}">
                  {getRoleText(profile.role)}
                </span>
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">
                  Date d'inscription
                </label>
                <p class="text-gray-900">{formatDate(profile.createdAt)}</p>
              </div>

              {#if profile.companyName}
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">
                    Entreprise
                  </label>
                  <p class="text-gray-900">{profile.companyName}</p>
                </div>
              {/if}
            </div>
          {/if}
        </div>

        <!-- Actions rapides -->
        <div class="mt-6 bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h3 class="text-lg font-semibold text-gray-900 mb-4">Actions rapides</h3>
          <div class="flex flex-wrap gap-4">
            <a
              href="/requests"
              class="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              Mes demandes
            </a>
            
            <a
              href="/requests/new"
              class="inline-flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
            >
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Nouvelle demande
            </a>

            {#if profile.role === 'Manager'}
              <a
                href="/company-requests"
                class="inline-flex items-center px-4 py-2 bg-purple-600 text-white rounded-lg hover:bg-purple-700 transition-colors"
              >
                <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                </svg>
                Gérer les demandes
              </a>
            {/if}
          </div>
        </div>
      </div>
    </div>
  {:else}
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
      </svg>
      <h3 class="mt-4 text-lg font-medium text-gray-900">Profil non trouvé</h3>
      <p class="mt-2 text-gray-500">
        Impossible de charger les informations de votre profil.
      </p>
    </div>
  {/if}
</div> 