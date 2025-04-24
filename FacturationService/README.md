# 🏥 Facturation Management Service - Clean Architecture

Ce projet implémente un service de gestion de la facturation basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 Facturation.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `FactureController.cs` : Gestion des opérations sur les factures.
  - `PaiementController.cs` : Gestion des opérations sur les paiements.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 Facturation.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `FactureDTO.cs` : DTO pour les données des factures.
  - `CreateFactureRequest.cs` : DTO pour les requêtes de création de factures.
  - `GetFacturesResponse.cs` : DTO pour les réponses de récupération des factures.
  - `UpdateFactureRequest.cs` : DTO pour les requêtes de mise à jour des factures.
  - `FactureStatsDTO.cs` : DTO pour les statistiques des factures.
- `Services/` : Contient la logique métier orchestrée.
  - `FactureService.cs` : Service principal pour les opérations sur les factures.
  - `PaiementService.cs` : Service pour la gestion des paiements.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
- `Mappers/` : Mappers pour transformer les entités en DTOs et vice-versa.

### 📌 Facturation.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `Facture.cs` : Entité représentant une facture.
  - `Paiement.cs` : Entité pour les paiements.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
  - `FactureStats.cs` : Objet valeur pour les statistiques des factures.
- `Enums/` : Énumérations métier.
  - `ModePaiement.cs` : Énumération pour les modes de paiement.
  - `FactureStatus.cs` : Énumération pour les statuts des factures.
- `Interfaces/` : Interfaces des repositories.

### 📌 Facturation.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `FactureRepository.cs` : Implémentation pour les opérations sur les factures.
- `Migrations/` : Migrations Entity Framework Core pour la base de données.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Facturation.API.Controllers`
2. Le contrôleur appelle `FactureService` ou `PaiementService` → `Facturation.Application`
3. Le service utilise un repository → `Facturation.Domain.Interfaces` → implémenté dans `Facturation.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `Facturation.API/` pour containeriser l'application :

### 📌 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande