# 🏥 Reporting Service - Clean Architecture

Ce projet implémente un service de reporting basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 Reporting.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `ReportingController.cs` : Gestion des opérations de reporting.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 Reporting.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
- `Services/` : Contient la logique métier orchestrée.
  - `ReportingService.cs` : Service principal pour les opérations de reporting.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.

### 📌 Reporting.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `RendezVous.cs` : Entité représentant un rendez-vous.
  - `ConsultationDataModel.cs` : Modèle de données pour les consultations.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
  - `ActiveMedecin.cs` : Objet valeur pour les médecins actifs.
  - `ConsultationStats.cs` : Objet valeur pour les statistiques des consultations.
  - `DashboardStats.cs` : Objet valeur pour les statistiques du tableau de bord.
  - `DoctorStats.cs` : Objet valeur pour les statistiques des médecins.
  - `FactureStats.cs` : Objet valeur pour les statistiques des factures.
  - `RendezVousStats.cs` : Objet valeur pour les statistiques des rendez-vous.
  - `Statistique.cs` : Objet valeur pour les statistiques générales.
  - `StatistiqueClinique.cs` : Objet valeur pour les statistiques des cliniques.
- `Enums/` : Énumérations métier.
- `Interfaces/` : Interfaces des repositories.

### 📌 Reporting.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `ConsultationStateRepository.cs` : Implémentation pour les opérations sur les états des consultations.
  - `ReportingRepository.cs` : Implémentation pour les opérations de reporting.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Reporting.API.Controllers`
2. Le contrôleur appelle `ReportingService` → `Reporting.Application`
3. Le service utilise un repository → `Reporting.Domain.Interfaces` → implémenté dans `Reporting.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `Reporting.API/` pour containeriser l'application :

### 📌 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande