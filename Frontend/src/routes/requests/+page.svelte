<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = true;
  let requests: any[] = [];
  let filteredRequests: any[] = [];
  let error = '';

  // Filtres
  let statusFilter = 'all';
  let dateFilter = 'all';

  onMount(async () => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    try {
      await loadRequests();
    } catch (error) {
      console.error('Erreur lors du chargement des demandes:', error);
    } finally {
      isLoading = false;
    }
  });

  async function loadRequests() {
    try {
      requests = await apiService.getMyRequests();
      applyFilters();
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors du chargement des demandes';
    }
  }

  function applyFilters() {
    filteredRequests = requests.filter(request => {
      // Filtre par statut
      if (statusFilter !== 'all' && request.status !== statusFilter) {
        return false;
      }

      // Filtre par date
      if (dateFilter !== 'all') {
        const requestDate = new Date(request.teleworkDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        switch (dateFilter) {
          case 'today':
            return requestDate.getTime() === today.getTime();
          case 'week':
            const weekFromNow = new Date(today);
            weekFromNow.setDate(today.getDate() + 7);
            return requestDate >= today && requestDate <= weekFromNow;
          case 'month':
            const monthFromNow = new Date(today);
            monthFromNow.setMonth(today.getMonth() + 1);
            return requestDate >= today && requestDate <= monthFromNow;
          case 'past':
            return requestDate < today;
          default:
            return true;
        }
      }

      return true;
    });
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
      weekday: 'long',
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  function formatDateTime(dateString: string) {
    return new Date(dateString).toLocaleString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  $: {
    applyFilters();
  }
</script>

<svelte:head>
  <title>Mes demandes - Telework Management</title>
</svelte:head>

<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <!-- Header -->
  <div class="mb-8">
    <div class="flex items-center justify-between mb-4">
      <div>
        <h1 class="text-3xl font-bold text-gray-900">Mes demandes de télétravail</h1>
        <p class="text-gray-600 mt-2">
          Gérez vos demandes de télétravail et suivez leur statut
        </p>
      </div>
      <a
        href="/requests/new"
        class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-medium rounded-lg hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all"
      >
        <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
        </svg>
        Nouvelle demande
      </a>
    </div>
  </div>

  {#if isLoading}
    <div class="flex items-center justify-center py-12">
      <div class="flex items-center space-x-3">
        <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-lg text-gray-600">Chargement des demandes...</span>
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
        on:click={loadRequests}
        class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
      >
        Réessayer
      </button>
    </div>
  {:else}
    <!-- Filtres -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <label for="statusFilter" class="block text-sm font-medium text-gray-700 mb-2">
            Filtrer par statut
          </label>
          <select
            id="statusFilter"
            bind:value={statusFilter}
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="all">Tous les statuts</option>
            <option value="Pending">En attente</option>
            <option value="Approved">Approuvées</option>
            <option value="Rejected">Rejetées</option>
          </select>
        </div>
        <div class="flex-1">
          <label for="dateFilter" class="block text-sm font-medium text-gray-700 mb-2">
            Filtrer par date
          </label>
          <select
            id="dateFilter"
            bind:value={dateFilter}
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
          >
            <option value="all">Toutes les dates</option>
            <option value="today">Aujourd'hui</option>
            <option value="week">Cette semaine</option>
            <option value="month">Ce mois</option>
            <option value="past">Passées</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Liste des demandes -->
    {#if filteredRequests.length > 0}
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date de télétravail
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Raison
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Statut
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date de demande
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Commentaire manager
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              {#each filteredRequests as request}
                <tr class="hover:bg-gray-50 transition-colors">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm font-medium text-gray-900">
                      {formatDate(request.teleworkDate)}
                    </div>
                  </td>
                  <td class="px-6 py-4">
                    <div class="text-sm text-gray-900 max-w-xs truncate" title={request.reason}>
                      {request.reason}
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span class="px-3 py-1 text-xs font-medium rounded-full {getStatusColor(request.status)}">
                      {getStatusText(request.status)}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {formatDateTime(request.requestDate)}
                  </td>
                  <td class="px-6 py-4">
                    {#if request.managerComment}
                      <div class="text-sm text-gray-900 max-w-xs truncate" title={request.managerComment}>
                        {request.managerComment}
                      </div>
                    {:else}
                      <span class="text-sm text-gray-400">-</span>
                    {/if}
                  </td>
                </tr>
              {/each}
            </tbody>
          </table>
        </div>
      </div>

      <!-- Statistiques -->
      <div class="mt-6 grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
          <div class="flex items-center">
            <div class="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-600">Total</p>
              <p class="text-lg font-bold text-gray-900">{requests.length}</p>
            </div>
          </div>
        </div>
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
          <div class="flex items-center">
            <div class="w-8 h-8 bg-yellow-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-600">En attente</p>
              <p class="text-lg font-bold text-gray-900">{requests.filter(r => r.status === 'Pending').length}</p>
            </div>
          </div>
        </div>
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
          <div class="flex items-center">
            <div class="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-600">Approuvées</p>
              <p class="text-lg font-bold text-gray-900">{requests.filter(r => r.status === 'Approved').length}</p>
            </div>
          </div>
        </div>
        <div class="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
          <div class="flex items-center">
            <div class="w-8 h-8 bg-red-100 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
              </svg>
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-600">Rejetées</p>
              <p class="text-lg font-bold text-gray-900">{requests.filter(r => r.status === 'Rejected').length}</p>
            </div>
          </div>
        </div>
      </div>
    {:else}
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
        </svg>
        <h3 class="mt-4 text-lg font-medium text-gray-900">Aucune demande trouvée</h3>
        <p class="mt-2 text-gray-500">
          {requests.length === 0 
            ? 'Vous n\'avez pas encore créé de demande de télétravail.'
            : 'Aucune demande ne correspond aux filtres sélectionnés.'}
        </p>
        {#if requests.length === 0}
          <div class="mt-6">
            <a
              href="/requests/new"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-medium rounded-lg hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all"
            >
              <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
              </svg>
              Créer votre première demande
            </a>
          </div>
        {/if}
      </div>
    {/if}
  {/if}
</div> 