<script lang="ts">
  import { page } from '$app/stores';
  import { authStore } from '$lib/stores/auth';
  import { goto } from '$app/navigation';
  import '../app.css';

  let showUserMenu = false;

  function toggleUserMenu() {
    showUserMenu = !showUserMenu;
  }

  function handleLogout() {
    authStore.logout();
    goto('/');
  }

  function closeUserMenu() {
    showUserMenu = false;
  }

  // Fermer le menu si on clique ailleurs
  function handleClickOutside(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu')) {
      showUserMenu = false;
    }
  }
</script>

<svelte:head>
  <title>MonTT</title>
</svelte:head>

<div class="min-h-screen bg-gray-50" on:click={handleClickOutside}>
  <!-- Navigation -->
  <nav class="bg-white shadow-sm border-b border-gray-200">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <!-- Logo et navigation principale -->
        <div class="flex items-center">
          <a href="/dashboard" class="flex items-center space-x-3">
            <div class="w-8 h-8 bg-blue-600 rounded-lg flex items-center justify-center">
              <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v6H8V5z"></path>
              </svg>
            </div>
            <span class="text-xl font-bold text-gray-900">MonTT</span>
          </a>

          {#if $authStore.isAuthenticated}
            <div class="ml-10 flex items-center space-x-8">
              <a
                href="/dashboard"
                class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors {$page.url.pathname === '/dashboard' ? 'text-blue-600 bg-blue-50' : ''}"
              >
                Dashboard
              </a>
              
              {#if $authStore.user?.role !== 'Manager'}
                <a
                  href="/requests"
                  class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors {$page.url.pathname.startsWith('/requests') ? 'text-blue-600 bg-blue-50' : ''}"
                >
                  Mes demandes
                </a>
              {/if}

              {#if $authStore.user?.role === 'Manager'}
                <a
                  href="/company-requests"
                  class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors {$page.url.pathname === '/company-requests' ? 'text-blue-600 bg-blue-50' : ''}"
                >
                  Demandes entreprise
                </a>
                
                <a
                  href="/planning"
                  class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors {$page.url.pathname === '/planning' ? 'text-blue-600 bg-blue-50' : ''}"
                >
                  Planning
                </a>
                
                <a
                  href="/employees"
                  class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors {$page.url.pathname === '/employees' ? 'text-blue-600 bg-blue-50' : ''}"
                >
                  Employés
                </a>
              {/if}
            </div>
          {/if}
        </div>

        <!-- Menu utilisateur -->
        {#if $authStore.isAuthenticated}
          <div class="flex items-center">
            <div class="relative user-menu">
              <button
                on:click={toggleUserMenu}
                class="flex items-center space-x-3 text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
              >
                <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                  <span class="text-sm font-medium text-blue-600">
                    {$authStore.user?.employee?.firstName?.[0] || $authStore.user?.email?.[0] || 'U'}
                  </span>
                </div>
                <span class="text-gray-700 font-medium">
                  {$authStore.user?.employee?.firstName || $authStore.user?.email}
                </span>
                <svg class="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                </svg>
              </button>

              {#if showUserMenu}
                <div class="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-50 border border-gray-200">
                  <a
                    href="/profile"
                    class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                    on:click={closeUserMenu}
                  >
                    Mon profil
                  </a>
                  <button
                    on:click={handleLogout}
                    class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  >
                    Se déconnecter
                  </button>
                </div>
              {/if}
            </div>
          </div>
        {:else}
          <div class="flex items-center space-x-4">
            <a
              href="/"
              class="text-gray-700 hover:text-blue-600 px-3 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Connexion
            </a>
          </div>
        {/if}
      </div>
    </div>
  </nav>

  <!-- Contenu principal -->
  <main>
    <slot />
  </main>
</div>
