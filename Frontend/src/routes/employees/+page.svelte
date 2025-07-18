<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = true;
  let employees: any[] = [];
  let error = '';
  let selectedEmployee: any = null;
  let showAssignModal = false;
  let showDetailsModal = false;

  // Formulaire d'assignation
  let assignForm = {
    employeeId: 0,
    companyId: 1 // Par défaut, l'entreprise du manager
  };

  onMount(async () => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    if ($authStore.user?.role !== 'Manager') {
      goto('/dashboard');
      return;
    }

    try {
      await loadEmployees();
    } catch (error) {
      console.error('Erreur lors du chargement des employés:', error);
    } finally {
      isLoading = false;
    }
  });

  async function loadEmployees() {
    try {
      employees = await apiService.getAllEmployees();
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors du chargement des employés';
    }
  }

  function openAssignModal(employee: any) {
    selectedEmployee = employee;
    assignForm.employeeId = employee.id;
    showAssignModal = true;
  }

  function openDetailsModal(employee: any) {
    selectedEmployee = employee;
    showDetailsModal = true;
  }

  async function assignToCompany() {
    if (!selectedEmployee) return;

    try {
      await apiService.assignEmployeeToCompany(assignForm);
      showAssignModal = false;
      selectedEmployee = null;
      await loadEmployees();
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors de l\'assignation';
    }
  }

  function formatDate(dateString: string) {
    return new Date(dateString).toLocaleDateString('fr-FR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
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

  function getStatusColor(hasCompany: boolean) {
    return hasCompany ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800';
  }

  function getStatusText(hasCompany: boolean) {
    return hasCompany ? 'Assigné' : 'Non assigné';
  }
</script>

<svelte:head>
  <title>Gestion des employés - MonTT</title>
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
      <h1 class="text-3xl font-bold text-gray-900">Gestion des employés</h1>
    </div>
    <p class="text-gray-600">
      Gérez les employés de votre entreprise et leurs assignations
    </p>
  </div>

  {#if isLoading}
    <div class="flex items-center justify-center py-12">
      <div class="flex items-center space-x-3">
        <svg class="animate-spin h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24">
          <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
          <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
        </svg>
        <span class="text-lg text-gray-600">Chargement des employés...</span>
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
        on:click={loadEmployees}
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
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Total</p>
            <p class="text-2xl font-bold text-gray-900">{employees.length}</p>
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
            <p class="text-sm font-medium text-gray-600">Assignés</p>
            <p class="text-2xl font-bold text-gray-900">{employees.filter(e => e.companyId).length}</p>
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
            <p class="text-sm font-medium text-gray-600">Non assignés</p>
            <p class="text-2xl font-bold text-gray-900">{employees.filter(e => !e.companyId).length}</p>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
            <svg class="w-5 h-5 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-600">Managers</p>
            <p class="text-2xl font-bold text-gray-900">{employees.filter(e => e.role === 'Manager').length}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Liste des employés -->
    {#if employees.length > 0}
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Employé
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Rôle
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Position
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Statut
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date d'inscription
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              {#each employees as employee}
                <tr class="hover:bg-gray-50 transition-colors">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                        <span class="text-sm font-medium text-blue-600">
                          {employee.firstName[0]}{employee.lastName[0]}
                        </span>
                      </div>
                      <div class="ml-4">
                        <div class="text-sm font-medium text-gray-900">
                          {employee.firstName} {employee.lastName}
                        </div>
                        <div class="text-sm text-gray-500">{employee.email}</div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span class="px-3 py-1 text-xs font-medium rounded-full {getRoleColor(employee.role)}">
                      {getRoleText(employee.role)}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="text-sm text-gray-900">{employee.position}</div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap">
                    <span class="px-3 py-1 text-xs font-medium rounded-full {getStatusColor(!!employee.companyId)}">
                      {getStatusText(!!employee.companyId)}
                    </span>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {formatDate(employee.createdAt)}
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    <div class="flex items-center space-x-3">
                      <button
                        on:click={() => openDetailsModal(employee)}
                        class="text-blue-600 hover:text-blue-900"
                      >
                        Détails
                      </button>
                      {#if !employee.companyId && employee.role !== 'Manager'}
                        <button
                          on:click={() => openAssignModal(employee)}
                          class="text-green-600 hover:text-green-900"
                        >
                          Assigner
                        </button>
                      {/if}
                    </div>
                  </td>
                </tr>
              {/each}
            </tbody>
          </table>
        </div>
      </div>
    {:else}
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
        </svg>
        <h3 class="mt-4 text-lg font-medium text-gray-900">Aucun employé trouvé</h3>
        <p class="mt-2 text-gray-500">
          Aucun employé n'est disponible pour le moment.
        </p>
      </div>
    {/if}
  {/if}
</div>

<!-- Modal d'assignation -->
{#if showAssignModal && selectedEmployee}
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <h3 class="text-lg font-medium text-gray-900 mb-4">
          Assigner {selectedEmployee.firstName} {selectedEmployee.lastName}
        </h3>
        
        <div class="mb-4">
          <p class="text-sm text-gray-600 mb-2">
            <strong>Position:</strong> {selectedEmployee.position}
          </p>
          <p class="text-sm text-gray-600 mb-2">
            <strong>Email:</strong> {selectedEmployee.email}
          </p>
        </div>

        <div class="space-y-4">
          <div>
            <label for="companyId" class="block text-sm font-medium text-gray-700 mb-2">
              Assigner à votre entreprise
            </label>
            <div class="text-sm text-gray-600 p-3 bg-gray-50 rounded-lg">
              TechCorp Solutions
            </div>
          </div>
        </div>

        <div class="flex items-center justify-end space-x-3 mt-6">
          <button
            on:click={() => showAssignModal = false}
            class="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50"
          >
            Annuler
          </button>
          <button
            on:click={assignToCompany}
            class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700"
          >
            Assigner
          </button>
        </div>
      </div>
    </div>
  </div>
{/if}

<!-- Modal de détails -->
{#if showDetailsModal && selectedEmployee}
  <div class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
    <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
      <div class="mt-3">
        <h3 class="text-lg font-medium text-gray-900 mb-4">
          Détails de {selectedEmployee.firstName} {selectedEmployee.lastName}
        </h3>
        
        <div class="space-y-3">
          <div>
            <label class="block text-sm font-medium text-gray-700">Nom complet</label>
            <p class="text-sm text-gray-900">{selectedEmployee.firstName} {selectedEmployee.lastName}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700">Email</label>
            <p class="text-sm text-gray-900">{selectedEmployee.email}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700">Position</label>
            <p class="text-sm text-gray-900">{selectedEmployee.position}</p>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700">Rôle</label>
            <span class="px-3 py-1 text-xs font-medium rounded-full {getRoleColor(selectedEmployee.role)}">
              {getRoleText(selectedEmployee.role)}
            </span>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700">Statut</label>
            <span class="px-3 py-1 text-xs font-medium rounded-full {getStatusColor(!!selectedEmployee.companyId)}">
              {getStatusText(!!selectedEmployee.companyId)}
            </span>
          </div>
          
          <div>
            <label class="block text-sm font-medium text-gray-700">Date d'inscription</label>
            <p class="text-sm text-gray-900">{formatDate(selectedEmployee.createdAt)}</p>
          </div>
        </div>

        <div class="flex items-center justify-end mt-6">
          <button
            on:click={() => showDetailsModal = false}
            class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
          >
            Fermer
          </button>
        </div>
      </div>
    </div>
  </div>
{/if} 