# Consignes POC - Application de Gestion du Télétravail

## 🎯 Objectif du Projet

Créer une **API RESTful ASP.NET Core** pour une application web de gestion du télétravail ultra minimaliste, **bien codée**, **documentée**, et **vendue comme un vrai produit**.

### Problème résolu par l'API

L'API résout le problème de **gestion simplifiée des demandes de télétravail** dans les petites entreprises :
- Les employés peuvent soumettre des demandes de télétravail
- Les managers peuvent approuver/refuser ces demandes avec des commentaires
- Gestion des rôles et des entreprises de manière simple
- Interface minimaliste mais fonctionnelle

---

## 🏗️ Architecture Technique

### Backend - ASP.NET Core API

#### Technologies
- **Framework** : ASP.NET Core 8.0
- **Base de données** : SQLite avec fichier local
- **Authentification** : JWT (JSON Web Tokens)
- **Documentation** : Swagger/OpenAPI
- **Validation** : Data Annotations (`[Required]`, `[Range]`, etc.)

#### Structure du Projet
```
Backend/
├── Controllers/
│   ├── AuthController.cs
│   ├── EmployeeController.cs
│   ├── TeleworkRequestController.cs
│   └── CompanyController.cs
├── Services/
│   ├── AuthService.cs
│   ├── EmployeeService.cs
│   ├── TeleworkRequestService.cs
│   └── CompanyService.cs
├── Models/
│   ├── User.cs
│   ├── Employee.cs
│   ├── Company.cs
│   └── TeleworkRequest.cs
├── DTOs/
│   ├── AuthDTOs.cs
│   ├── EmployeeDTOs.cs
│   └── TeleworkRequestDTOs.cs
├── Data/
│   └── ApplicationDbContext.cs
└── Program.cs
```

---

## 🔐 Système d'Authentification

### Rôles
1. **User** (par défaut) - Employé standard
2. **Manager** - Chef d'entreprise

### Endpoints d'Authentification
- `POST /api/auth/register` - Inscription avec choix du rôle
- `POST /api/auth/login` - Connexion
- `POST /api/auth/refresh` - Renouvellement du token

### Protection des Routes
- Routes publiques : `/api/auth/*`
- Routes protégées : Toutes les autres routes nécessitent un JWT valide
- Routes Manager uniquement : Gestion des entreprises et approbation des demandes

---

## 📊 Modèles de Données

### User
```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "User" ou "Manager"
    public DateTime CreatedAt { get; set; }
}
```

### Employee
```csharp
public class Employee
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? CompanyId { get; set; }
    public Company Company { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Position { get; set; }
}
```

### Company
```csharp
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ManagerId { get; set; }
    public Employee Manager { get; set; }
    public List<Employee> Employees { get; set; }
}
```

### TeleworkRequest
```csharp
public class TeleworkRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime TeleworkDate { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; } // "Pending", "Approved", "Rejected"
    public string? ManagerComment { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int? ProcessedByManagerId { get; set; }
    public Employee? ProcessedByManager { get; set; }
}
```

---

## 🚀 Endpoints API

### Authentification
- `POST /api/auth/register` - Inscription
- `POST /api/auth/login` - Connexion

### Employés
- `GET /api/employees/profile` - Profil de l'utilisateur connecté
- `PUT /api/employees/profile` - Mise à jour du profil
- `GET /api/employees` - Liste des employés (Manager uniquement)
- `POST /api/employees/assign-to-company` - Assigner un employé à une entreprise (Manager uniquement)

### Demandes de Télétravail
- `POST /api/telework-requests` - Créer une demande
- `GET /api/telework-requests/my-requests` - Mes demandes
- `GET /api/telework-requests/company` - Demandes de l'entreprise (Manager uniquement)
- `PUT /api/telework-requests/{id}/process` - Traiter une demande (Manager uniquement)
- `GET /api/telework-requests/company/weekly-planning` - Planning hebdomadaire de l'entreprise (Manager uniquement)

### Entreprises
- `POST /api/companies` - Créer une entreprise (Manager uniquement)
- `GET /api/companies/my-company` - Mon entreprise
- `PUT /api/companies` - Mettre à jour l'entreprise (Manager uniquement)

---

## 📝 Documentation Swagger

### Exigences
- Swagger activé et configuré
- Chaque endpoint contient :
  - **Résumé personnalisé** en français
  - **Description claire** de l'endpoint
  - **Exemples de requêtes/réponses** avec `[ProducesResponseType]`
  - **Codes de retour** documentés (200, 400, 401, 403, 404, 500)

### Exemple de documentation
```csharp
/// <summary>
/// Créer une nouvelle demande de télétravail
/// </summary>
/// <param name="request">Détails de la demande</param>
/// <returns>Demande créée avec succès</returns>
/// <response code="201">Demande créée avec succès</response>
/// <response code="400">Données invalides</response>
/// <response code="401">Non authentifié</response>
[HttpPost]
[ProducesResponseType(typeof(TeleworkRequestDto), 201)]
[ProducesResponseType(400)]
[ProducesResponseType(401)]
public async Task<IActionResult> CreateRequest(CreateTeleworkRequestDto request)
```

---

## 🎨 Frontend (Phase 2)

### Technologies
- **Framework** : SvelteKit (SSR + SPA)
- **UI** : Tailwind CSS
- **HTTP Client** : Fetch API ou librairie Svelte
- **Planning** : Bibliothèque de calendrier (FullCalendar.js ou similaire)

### Pages Minimales
1. **Page de connexion/inscription**
2. **Dashboard employé** : Voir ses demandes, créer une nouvelle demande
3. **Dashboard manager** : Voir toutes les demandes, les traiter, gérer l'entreprise
4. **Planning hebdomadaire** : Visualisation globale du télétravail de l'entreprise

### Fonctionnalités du Planning
- **Vue hebdomadaire** : Affichage des jours de télétravail de tous les employés
- **Filtres** : Par employé, par statut (approuvé/en attente/refusé)
- **Couleurs** : Code couleur selon le statut (vert=approuvé, orange=en attente, rouge=refusé)
- **Interactions** : Clic sur un événement pour voir les détails
- **Responsive** : Adaptation mobile et desktop

### Cas d'Usage Principal
- Un employé se connecte et crée une demande de télétravail
- Un manager se connecte et approuve/refuse la demande avec un commentaire
- L'employé voit le statut de sa demande
- **Le manager visualise le planning hebdomadaire** pour voir la répartition du télétravail dans l'entreprise
- **Prise de décision facilitée** grâce à la vue d'ensemble du planning

---

## 📋 Validation et Gestion d'Erreurs

### Validation des Entrées
- Utilisation des Data Annotations
- Validation côté serveur
- Messages d'erreur en français

### Codes HTTP
- `200` - Succès
- `201` - Créé avec succès
- `400` - Requête invalide
- `401` - Non authentifié
- `403` - Accès interdit
- `404` - Ressource non trouvée
- `500` - Erreur serveur

---

## 🗄️ Base de Données

### Configuration SQLite
- Fichier local : `app.db`
- Migrations Entity Framework
- Seed data pour les tests

### Données de Test
- Compte manager de test
- Compte employé de test
- Entreprise de test
- Quelques demandes de télétravail

---

## 📖 README.md

### Contenu Obligatoire
1. **Présentation du projet** et du problème résolu
2. **Technologies utilisées**
3. **Installation et démarrage**
   - Prérequis (.NET 8.0)
   - Commandes de démarrage
   - Configuration de la base de données
4. **Identifiants de test**
   - Manager : `manager@test.com` / `password123`
   - Employé : `employee@test.com` / `password123`
5. **Documentation API** (lien vers Swagger)
6. **Exemples d'utilisation**

---

## ✅ Critères de Validation

### Backend
- [ ] API RESTful fonctionnelle
- [ ] Authentification JWT
- [ ] Gestion des rôles (User/Manager)
- [ ] CRUD complet pour les entités
- [ ] Validation des données
- [ ] Gestion d'erreurs appropriée
- [ ] Documentation Swagger complète
- [ ] Base de données SQLite fonctionnelle

### Code Quality
- [ ] Architecture propre (Controllers, Services, DTOs)
- [ ] Code bien documenté
- [ ] Gestion des erreurs cohérente
- [ ] Tests unitaires (optionnel mais recommandé)

### Documentation
- [ ] README.md complet
- [ ] Swagger documenté
- [ ] Identifiants de test fournis

---

## 🎯 Priorités de Développement

1. **Phase 1** : Backend API complète
2. **Phase 2** : Frontend minimal
3. **Phase 3** : Tests et optimisations

**Objectif** : Avoir une API fonctionnelle et documentée qui démontre clairement sa valeur pour la gestion du télétravail. 