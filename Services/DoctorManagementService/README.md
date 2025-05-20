# 🏥 Doctor Service - Clean Architecture

Ce projet implémente un service de gestion des médecins basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 Doctor.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `MedecinController.cs` : Gestion des opérations sur les médecins.
  - `DisponibiliteController.cs` : Gestion des disponibilités des médecins.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 Doctor.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `MedecinDTO.cs` : DTO pour les données des médecins.
  - `ActiveMedecinDTO.cs` : DTO pour les médecins actifs.
  - `StatistiqueMedecinDTO.cs` : DTO pour les statistiques des médecins.
- `Services/` : Contient la logique métier orchestrée.
  - `MedecinService.cs` : Service principal pour les opérations sur les médecins.
  - `DisponibiliteService.cs` : Service pour la gestion des disponibilités.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
  - `IMedecinService.cs`
  - `IDisponibiliteService.cs`

### 📌 Doctor.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `Medecin.cs` : Entité représentant un médecin.
  - `Disponibilite.cs` : Entité pour les disponibilités des médecins.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
  - `ActiveMedecin.cs` : Objet valeur pour les médecins actifs.
  - `StatistiqueMedecin.cs` : Objet valeur pour les statistiques.
- `Enums/` : Énumérations métier.
- `Interfaces/` : Interfaces des repositories.
  - `IMedecinRepository.cs` : Interface pour les opérations sur les médecins.

### 📌 Doctor.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
  - `DoctorDbContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `MedecinRepository.cs` : Implémentation pour les opérations sur les médecins.
  - `DisponibiliteRepository.cs` : Implémentation pour les disponibilités.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Doctor.API.Controllers`
2. Le contrôleur appelle `MedecinService` ou `DisponibiliteService` → `Doctor.Application`
3. Le service utilise un repository → `Doctor.Domain.Interfaces` → implémenté dans `Doctor.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `Doctor.API/` pour containeriser l'application :

### 📌 Prérequis

- .NET 8 SDK
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande