<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = true;
  let planning: any = null;
  let error = '';
  let currentWeekStart = new Date();
  let selectedDate = new Date();

  onMount(async () => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    if ($authStore.user?.role !== 'Manager') {
      goto('/dashboard');
      return;
    }

    // Définir le début de la semaine (lundi)
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysToMonday = dayOfWeek === 0 ? 6 : dayOfWeek - 1;
    currentWeekStart = new Date(today);
    currentWeekStart.setDate(today.getDate() - daysToMonday);
    currentWeekStart.setHours(0, 0, 0, 0);

    try {
      await loadPlanning();
    } catch (error) {
      console.error('Erreur lors du chargement du planning:', error);
    } finally {
      isLoading = false;
    }
  });

  async function loadPlanning() {
    try {
      const weekStartStr = currentWeekStart.toISOString().split('T')[0];
      planning = await apiService.getWeeklyPlanning(weekStartStr);
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors du chargement du planning';
    }
  }

  function previousWeek() {
    currentWeekStart.setDate(currentWeekStart.getDate() - 7);
    loadPlanning();
  }

  function nextWeek() {
    currentWeekStart.setDate(currentWeekStart.getDate() + 7);
    loadPlanning();
  }

  function goToCurrentWeek() {
    const today = new Date();
    const dayOfWeek = today.getDay();
    const daysToMonday = dayOfWeek === 0 ? 6 : dayOfWeek - 1;
    currentWeekStart = new Date(today);
    currentWeekStart.setDate(today.getDate() - daysToMonday);
    currentWeekStart.setHours(0, 0, 0, 0);
    loadPlanning();
  }

  function formatDate(dateString: string) {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit'
    });
  }

  function formatDayName(dateString: string) {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      weekday: 'short'
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

  function getWeekRange() {
    const weekEnd = new Date(currentWeekStart);
    weekEnd.setDate(currentWeekStart.getDate() + 6);
    
    return {
      start: currentWeekStart.toLocaleDateString('fr-FR', { day: '2-digit', month: '2-digit', year: 'numeric' }),
      end: weekEnd.toLocaleDateString('fr-FR', { day: '2-digit', month: '2-digit', year: 'numeric' })
    };
  }

  $: weekRange = getWeekRange();
</script>

<svelte:head>
  <title>Planning hebdomadaire - MonTT</title>
</svelte:head>

<div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <!-- Header -->
  <div class="mb-8">
    <div class="flex items-center space-x-3 mb-4">
      <a href="/dashboard" class="text-blue-600 hover:text-blue-700">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
        </svg>
      </a>
      <h1 class="text-3xl font-bold text-gray-900">Planning hebdomadaire</h1>
    </div>
    <p class="text-gray-600">
      Vue d'ensemble des demandes de télétravail de la semaine
    </p>
  </div>

  {#if isLoading}
    <div class="flex items-center justify-center py-12">
      <div class="flex items-center space-x-3">
        <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-lg text-gray-600">Chargement du planning...</span>
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
        on:click={loadPlanning}
        class="mt-4 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
      >
        Réessayer
      </button>
    </div>
  {:else if planning}
    <!-- Navigation de la semaine -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 mb-8">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-4">
          <button
            on:click={previousWeek}
            class="p-2 text-gray-400 hover:text-gray-600 transition-colors"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
            </svg>
          </button>
          
          <h2 class="text-xl font-semibold text-gray-900">
            Semaine du {weekRange.start} au {weekRange.end}
          </h2>
          
          <button
            on:click={nextWeek}
            class="p-2 text-gray-400 hover:text-gray-600 transition-colors"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
            </svg>
          </button>
        </div>
        
        <button
          on:click={goToCurrentWeek}
          class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          Cette semaine
        </button>
      </div>
    </div>

    <!-- Statistiques de la semaine -->
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
            <p class="text-2xl font-bold text-gray-900">{planning.stats.totalRequests}</p>
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
            <p class="text-2xl font-bold text-gray-900">{planning.stats.pendingRequests}</p>
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
            <p class="text-2xl font-bold text-gray-900">{planning.stats.approvedRequests}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Taux d'approbation</p>
            <p class="text-2xl font-bold text-gray-900">{planning.stats.approvalRate.toFixed(0)}%</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Calendrier hebdomadaire -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="grid grid-cols-7 gap-px bg-gray-200">
        <!-- En-têtes des jours -->
        {#each planning.dailyRequests as day}
          <div class="bg-gray-50 p-4 text-center">
            <div class="text-sm font-medium text-gray-900">
              {formatDayName(day.date)}
            </div>
            <div class="text-lg font-bold text-gray-600">
              {formatDate(day.date)}
            </div>
            <div class="mt-2 space-y-1">
              <div class="flex justify-between text-xs">
                <span class="text-green-600">✓ {day.approvedRequests}</span>
                <span class="text-yellow-600">⏳ {day.pendingRequests}</span>
                <span class="text-red-600">✗ {day.rejectedRequests}</span>
              </div>
            </div>
          </div>
        {/each}
      </div>

      <!-- Contenu des jours -->
      <div class="grid grid-cols-7 gap-px bg-gray-200">
        {#each planning.dailyRequests as day}
          <div class="bg-white p-4 min-h-64">
            {#if day.requests.length > 0}
              <div class="space-y-2">
                {#each day.requests as request}
                  <div class="p-3 bg-gray-50 rounded-lg border-l-4 border-blue-500">
                    <div class="flex items-start justify-between">
                      <div class="flex-1">
                        <div class="text-sm font-medium text-gray-900">
                          {request.employee.firstName} {request.employee.lastName}
                        </div>
                        <div class="text-xs text-gray-600 mt-1">
                          {request.reason.length > 50 ? request.reason.substring(0, 50) + '...' : request.reason}
                        </div>
                      </div>
                      <span class="px-2 py-1 text-xs font-medium rounded-full {getStatusColor(request.status)}">
                        {getStatusText(request.status)}
                      </span>
                    </div>
                  </div>
                {/each}
              </div>
            {:else}
              <div class="flex items-center justify-center h-full text-gray-400">
                <div class="text-center">
                  <svg class="mx-auto h-8 w-8 mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                  </svg>
                  <p class="text-xs">Aucune demande</p>
                </div>
              </div>
            {/if}
          </div>
        {/each}
      </div>
    </div>

    <!-- Actions rapides -->
    <div class="mt-8 bg-white rounded-xl shadow-sm border border-gray-200 p-6">
      <h3 class="text-lg font-semibold text-gray-900 mb-4">Actions rapides</h3>
      <div class="flex flex-wrap gap-4">
        <a
          href="/company-requests"
          class="inline-flex items-center px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
        >
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
          </svg>
          Voir toutes les demandes
        </a>
        
        <a
          href="/employees"
          class="inline-flex items-center px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors"
        >
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
          </svg>
          Gérer les employés
        </a>
      </div>
    </div>
  {:else}
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
      </svg>
      <h3 class="mt-4 text-lg font-medium text-gray-900">Aucun planning disponible</h3>
      <p class="mt-2 text-gray-500">
        Aucune donnée de planning pour cette semaine.
      </p>
    </div>
  {/if}
</div> 