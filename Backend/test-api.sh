#!/bin/bash

# Script de test pour l'API de Gestion du Télétravail
# Assurez-vous que l'API est démarrée sur http://localhost:5000

echo "🧪 Test de l'API de Gestion du Télétravail"
echo "=========================================="

# Test 1: Vérifier que l'API répond
echo "1. Test de connectivité..."
if curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/ | grep -q "200"; then
    echo "✅ API accessible"
else
    echo "❌ API non accessible. Assurez-vous qu'elle est démarrée sur http://localhost:5000"
    exit 1
fi

# Test 2: Inscription d'un manager
echo "2. Test d'inscription manager..."
MANAGER_RESPONSE=$(curl -s -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "manager@test.com",
    "password": "password123",
    "firstName": "Jean",
    "lastName": "Dupont",
    "position": "Manager",
    "role": "Manager"
  }')

if echo "$MANAGER_RESPONSE" | grep -q "token"; then
    echo "✅ Inscription manager réussie"
    MANAGER_TOKEN=$(echo "$MANAGER_RESPONSE" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
else
    echo "❌ Échec inscription manager"
    echo "Réponse: $MANAGER_RESPONSE"
fi

# Test 3: Inscription d'un employé
echo "3. Test d'inscription employé..."
EMPLOYEE_RESPONSE=$(curl -s -X POST "http://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "employee@test.com",
    "password": "password123",
    "firstName": "Marie",
    "lastName": "Martin",
    "position": "Développeur",
    "role": "User"
  }')

if echo "$EMPLOYEE_RESPONSE" | grep -q "token"; then
    echo "✅ Inscription employé réussie"
    EMPLOYEE_TOKEN=$(echo "$EMPLOYEE_RESPONSE" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
else
    echo "❌ Échec inscription employé"
    echo "Réponse: $EMPLOYEE_RESPONSE"
fi

# Test 4: Connexion manager
echo "4. Test de connexion manager..."
LOGIN_RESPONSE=$(curl -s -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "manager@test.com",
    "password": "password123"
  }')

if echo "$LOGIN_RESPONSE" | grep -q "token"; then
    echo "✅ Connexion manager réussie"
else
    echo "❌ Échec connexion manager"
    echo "Réponse: $LOGIN_RESPONSE"
fi

# Test 5: Connexion employé
echo "5. Test de connexion employé..."
LOGIN_RESPONSE=$(curl -s -X POST "http://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "employee@test.com",
    "password": "password123"
  }')

if echo "$LOGIN_RESPONSE" | grep -q "token"; then
    echo "✅ Connexion employé réussie"
else
    echo "❌ Échec connexion employé"
    echo "Réponse: $LOGIN_RESPONSE"
fi

# Test 6: Test avec token invalide
echo "6. Test avec token invalide..."
INVALID_RESPONSE=$(curl -s -w "%{http_code}" -X GET "http://localhost:5000/api/employees/profile" \
  -H "Authorization: Bearer invalid_token")

if echo "$INVALID_RESPONSE" | grep -q "401"; then
    echo "✅ Protection JWT fonctionnelle"
else
    echo "❌ Protection JWT défaillante"
fi

echo ""
echo "🎉 Tests terminés !"
echo ""
echo "📖 Documentation Swagger disponible sur: http://localhost:5000"
echo "🔑 Identifiants de test:"
echo "   Manager: manager@test.com / password123"
echo "   Employé: employee@test.com / password123" 