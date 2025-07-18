<script lang="ts">
  import { onMount } from 'svelte';
  import { goto } from '$app/navigation';
  import { authStore } from '$lib/stores/auth';
  import { apiService } from '$lib/services/api';

  let isLoading = false;
  let error = '';
  let success = false;

  let form = {
    teleworkDate: '',
    reason: ''
  };

  onMount(() => {
    if (!$authStore.isAuthenticated) {
      goto('/');
      return;
    }

    // Les managers ne peuvent pas créer de demandes personnelles
    if ($authStore.user?.role === 'Manager') {
      goto('/dashboard');
      return;
    }
  });

  async function handleSubmit() {
    if (!form.teleworkDate || !form.reason) {
      error = 'Veuillez remplir tous les champs';
      return;
    }

    if (form.reason.length < 10) {
      error = 'La raison doit contenir au moins 10 caractères';
      return;
    }

    // Vérifier que la date n'est pas dans le passé
    const selectedDate = new Date(form.teleworkDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (selectedDate < today) {
      error = 'La date de télétravail ne peut pas être dans le passé';
      return;
    }

    isLoading = true;
    error = '';

    try {
      await apiService.createTeleworkRequest({
        TeleworkDate: form.teleworkDate,
        Reason: form.reason
      });
      
      success = true;
      setTimeout(() => {
        goto('/requests');
      }, 2000);
    } catch (err) {
      error = err instanceof Error ? err.message : 'Erreur lors de la création de la demande';
    } finally {
      isLoading = false;
    }
  }

  function getMinDate() {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }

  function getMaxDate() {
    const maxDate = new Date();
    maxDate.setMonth(maxDate.getMonth() + 3); // 3 mois maximum
    return maxDate.toISOString().split('T')[0];
  }
</script>

<svelte:head>
  <title>Nouvelle demande - MonTT</title>
</svelte:head>

<div class="max-w-2xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <!-- Header -->
  <div class="mb-8">
    <div class="flex items-center space-x-3 mb-4">
      <a href="/requests" class="text-blue-600 hover:text-blue-700">
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
        </svg>
      </a>
      <h1 class="text-3xl font-bold text-gray-900">Nouvelle demande de télétravail</h1>
    </div>
    <p class="text-gray-600">
      Créez une nouvelle demande de télétravail pour une date spécifique
    </p>
  </div>

  {#if success}
    <div class="bg-green-50 border border-green-200 rounded-xl p-6 text-center">
      <div class="mx-auto h-12 w-12 bg-green-100 rounded-full flex items-center justify-center mb-4">
        <svg class="h-6 w-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
        </svg>
      </div>
      <h3 class="text-lg font-medium text-green-900 mb-2">Demande créée avec succès !</h3>
      <p class="text-green-700">Vous allez être redirigé vers la liste de vos demandes...</p>
    </div>
  {:else}
    <!-- Formulaire -->
    <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
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

      <form on:submit|preventDefault={handleSubmit} class="space-y-6">
        <!-- Date de télétravail -->
        <div>
          <label for="teleworkDate" class="block text-sm font-medium text-gray-700 mb-2">
            Date de télétravail *
          </label>
          <input
            id="teleworkDate"
            type="date"
            bind:value={form.teleworkDate}
            min={getMinDate()}
            max={getMaxDate()}
            required
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
          />
          <p class="mt-1 text-sm text-gray-500">
            Sélectionnez la date pour laquelle vous souhaitez télétravailler
          </p>
        </div>

        <!-- Raison -->
        <div>
          <label for="reason" class="block text-sm font-medium text-gray-700 mb-2">
            Raison du télétravail *
          </label>
          <textarea
            id="reason"
            bind:value={form.reason}
            rows="4"
            maxlength="500"
            required
            class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors resize-none"
            placeholder="Expliquez brièvement la raison de votre demande de télétravail..."
          ></textarea>
          <div class="mt-1 flex justify-between text-sm text-gray-500">
            <span>Maximum 500 caractères</span>
            <span>{form.reason.length}/500</span>
          </div>
        </div>

        <!-- Informations -->
        <div class="bg-blue-50 border border-blue-200 rounded-lg p-4">
          <div class="flex">
            <svg class="h-5 w-5 text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <div class="ml-3">
              <h3 class="text-sm font-medium text-blue-900">Informations importantes</h3>
              <div class="mt-2 text-sm text-blue-700">
                <ul class="list-disc list-inside space-y-1">
                  <li>Votre demande sera soumise à votre manager pour approbation</li>
                  <li>Vous recevrez une notification une fois la demande traitée</li>
                  <li>Vous pouvez annuler une demande en attente à tout moment</li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <!-- Boutons -->
        <div class="flex items-center justify-end space-x-4 pt-6 border-t border-gray-200">
          <a
            href="/requests"
            class="px-6 py-3 border border-gray-300 rounded-lg text-gray-700 font-medium hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors"
          >
            Annuler
          </a>
          <button
            type="submit"
            disabled={isLoading}
            class="px-6 py-3 bg-gradient-to-r from-blue-600 to-indigo-600 text-white font-medium rounded-lg hover:from-blue-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {#if isLoading}
              <div class="flex items-center">
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Création en cours...
              </div>
            {:else}
              Créer la demande
            {/if}
          </button>
        </div>
      </form>
    </div>
  {/if}
</div> 