<script lang="ts">
  import { onMount } from 'svelte';
  import { apiService } from '$lib/services/api';

  let testResult = '';
  let isLoading = false;

  async function testLogin() {
    isLoading = true;
    testResult = '';
    
    try {
      const response = await apiService.login({
        email: 'manager@test.com',
        password: 'password123'
      });
      
      testResult = `✅ Connexion réussie!\nToken: ${response.token.substring(0, 50)}...\nUser: ${response.user.email}`;
      
      // Test des demandes
      setTimeout(async () => {
        try {
          const requests = await apiService.getMyRequests();
          testResult += `\n\n✅ Demandes récupérées: ${requests.length} demandes`;
        } catch (error) {
          testResult += `\n\n❌ Erreur demandes: ${error}`;
        }
      }, 1000);
      
    } catch (error) {
      testResult = `❌ Erreur de connexion: ${error}`;
    } finally {
      isLoading = false;
    }
  }

  function clearResult() {
    testResult = '';
  }
</script>

<svelte:head>
  <title>Test API - Telework Management</title>
</svelte:head>

<div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
  <h1 class="text-3xl font-bold text-gray-900 mb-8">Test de l'API</h1>
  
  <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
    <h2 class="text-xl font-semibold text-gray-900 mb-4">Test de connexion et API</h2>
    
    <div class="space-y-4">
      <button
        on:click={testLogin}
        disabled={isLoading}
        class="px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 disabled:opacity-50"
      >
        {isLoading ? 'Test en cours...' : 'Tester la connexion'}
      </button>
      
      <button
        on:click={clearResult}
        class="px-6 py-3 bg-gray-600 text-white font-medium rounded-lg hover:bg-gray-700"
      >
        Effacer
      </button>
    </div>
    
    {#if testResult}
      <div class="mt-6 p-4 bg-gray-50 rounded-lg">
        <h3 class="font-medium text-gray-900 mb-2">Résultat:</h3>
        <pre class="text-sm text-gray-700 whitespace-pre-wrap">{testResult}</pre>
      </div>
    {/if}
  </div>
</div> 