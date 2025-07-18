<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = true;
  let stats = {
    totalRequests: 0,
    pendingRequests: 0,
    approvedRequests: 0,
    rejectedRequests: 0
  };
  let recentRequests: any[] = [];
  let error = '';

  onMount(async () => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    try {
      await loadDashboardData();
    } catch (error) {
      console.error('Erreur lors du chargement du dashboard:', error);
    } finally {
      isLoading = false;
    }
  });

  async function loadDashboardData() {
    try {
      // Charger les demandes de l'utilisateur
      const requests = await apiService.getMyRequests();
      
      // Calculer les statistiques
      stats = {
        totalRequests: requests.length,
        pendingRequests: requests.filter(r => r.status === 'Pending').length,
        approvedRequests: requests.filter(r => r.status === 'Approved').length,
        rejectedRequests: requests.filter(r => r.status === 'Rejected').length
      };

      // Récupérer les demandes récentes (5 dernières)
      recentRequests = requests
        .sort((a, b) => new Date(b.requestDate).getTime() - new Date(a.requestDate).getTime())
        .slice(0, 5);

    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors du chargement des données';
    }
  }

  function getStatusColor(status: string) {
    switch (status) {
      case 'Approved': return 'bg-green-100 text-green-800';
      case 'Rejected': return 'bg-red-100 text-red-800';
      case 'Pending': return 'bg-yellow-100 text-yellow-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  }

  function getStatusText(status: string) {
    switch (status) {
      case 'Approved': return 'Approuvée';
      case 'Rejected': return 'Rejetée';
      case 'Pending': return 'En attente';
      default: return status;
    }
  }

  function formatDate(dateString: string) {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  $: isManager = $authStore.user?.role === 'Manager';
</script>

<svelte:head>
  <title>Dashboard - Telework Management</title>
</svelte:head>

<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <!-- Header -->
  <div class="mb-8">
    <h1 class="text-3xl font-bold text-gray-900">
      Bonjour, {$authStore.user?.employee?.firstName || $authStore.user?.email} !
    </h1>
    <p class="text-gray-600 mt-2">
      Bienvenue sur votre tableau de bord de gestion du télétravail
    </p>
  </div>

  {#if isLoading}
    <div class="flex items-center justify-center py-12">
      <div class="flex items-center space-x-3">
        <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-lg text-gray-600">Chargement du dashboard...</span>
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
        on:click={loadDashboardData}
        class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
      >
        Réessayer
      </button>
    </div>
  {:else}
    <!-- Statistiques -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Total</p>
            <p class="text-2xl font-bold text-gray-900">{stats.totalRequests}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-yellow-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">En attente</p>
            <p class="text-2xl font-bold text-gray-900">{stats.pendingRequests}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Approuvées</p>
            <p class="text-2xl font-bold text-gray-900">{stats.approvedRequests}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-red-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Rejetées</p>
            <p class="text-2xl font-bold text-gray-900">{stats.rejectedRequests}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Actions rapides -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
      <!-- Actions pour tous les utilisateurs -->
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <h2 class="text-lg font-semibold text-gray-900 mb-4">Actions rapides</h2>
        <div class="space-y-3">
          <a
            href="/requests/new"
            class="flex items-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
          >
            <div class="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="font-medium text-gray-900">Nouvelle demande</h3>
              <p class="text-sm text-gray-600">Créer une demande de télétravail</p>
            </div>
          </a>

          <a
            href="/requests"
            class="flex items-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
          >
            <div class="w-10 h-10 bg-green-600 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="font-medium text-gray-900">Mes demandes</h3>
              <p class="text-sm text-gray-600">Voir toutes mes demandes</p>
            </div>
          </a>

          <a
            href="/profile"
            class="flex items-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
          >
            <div class="w-10 h-10 bg-purple-600 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"></path>
              </svg>
            </div>
            <div class="ml-4">
              <h3 class="font-medium text-gray-900">Mon profil</h3>
              <p class="text-sm text-gray-600">Gérer mes informations</p>
            </div>
          </a>
        </div>
      </div>

      <!-- Actions pour les managers -->
      {#if isManager}
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h2 class="text-lg font-semibold text-gray-900 mb-4">Actions manager</h2>
          <div class="space-y-3">
            <a
              href="/company-requests"
              class="flex items-center p-4 bg-orange-50 rounded-lg hover:bg-orange-100 transition-colors"
            >
              <div class="w-10 h-10 bg-orange-600 rounded-lg flex items-center justify-center">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <h3 class="font-medium text-gray-900">Gérer les demandes</h3>
                <p class="text-sm text-gray-600">Traiter les demandes de l'entreprise</p>
              </div>
            </a>

            <a
              href="/planning"
              class="flex items-center p-4 bg-indigo-50 rounded-lg hover:bg-indigo-100 transition-colors"
            >
              <div class="w-10 h-10 bg-indigo-600 rounded-lg flex items-center justify-center">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <h3 class="font-medium text-gray-900">Planning hebdomadaire</h3>
                <p class="text-sm text-gray-600">Voir le planning de télétravail</p>
              </div>
            </a>

            <a
              href="/employees"
              class="flex items-center p-4 bg-teal-50 rounded-lg hover:bg-teal-100 transition-colors"
            >
              <div class="w-10 h-10 bg-teal-600 rounded-lg flex items-center justify-center">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <h3 class="font-medium text-gray-900">Gérer les employés</h3>
                <p class="text-sm text-gray-600">Voir et assigner les employés</p>
              </div>
            </a>
          </div>
        </div>
      {:else}
        <!-- Espace vide pour les utilisateurs normaux -->
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <h2 class="text-lg font-semibold text-gray-900 mb-4">Informations</h2>
          <div class="space-y-3">
            <div class="flex items-center p-4 bg-gray-50 rounded-lg">
              <div class="w-10 h-10 bg-gray-600 rounded-lg flex items-center justify-center">
                <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
              </div>
              <div class="ml-4">
                <h3 class="font-medium text-gray-900">Statut</h3>
                <p class="text-sm text-gray-600">Vous avez {stats.pendingRequests} demande(s) en attente</p>
              </div>
            </div>
          </div>
        </div>
      {/if}
    </div>

    <!-- Demandes récentes -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-lg font-semibold text-gray-900">Demandes récentes</h2>
        <a
          href="/requests"
          class="text-blue-600 hover:text-blue-700 text-sm font-medium"
        >
          Voir toutes →
        </a>
      </div>

      {#if recentRequests.length > 0}
        <div class="space-y-4">
          {#each recentRequests as request}
            <div class="flex items-center justify-between p-4 bg-gray-50 rounded-lg">
              <div class="flex items-center space-x-4">
                <div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                  <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                  </svg>
                </div>
                <div>
                  <p class="font-medium text-gray-900">
                    Télétravail du {formatDate(request.teleworkDate)}
                  </p>
                  <p class="text-sm text-gray-600">
                    {request.reason.length > 60 ? request.reason.substring(0, 60) + '...' : request.reason}
                  </p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span class="px-3 py-1 text-xs font-medium rounded-full {getStatusColor(request.status)}">
                  {getStatusText(request.status)}
                </span>
                <span class="text-sm text-gray-500">
                  {formatDate(request.requestDate)}
                </span>
              </div>
            </div>
          {/each}
        </div>
      {:else}
        <div class="text-center py-8">
          <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
          </svg>
          <h3 class="mt-4 text-lg font-medium text-gray-900">Aucune demande</h3>
          <p class="mt-2 text-gray-500">
            Vous n'avez pas encore créé de demande de télétravail.
          </p>
          <a
            href="/requests/new"
            class="mt-4 inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Créer ma première demande
          </a>
        </div>
      {/if}
    </div>
  {/if}
</div> 