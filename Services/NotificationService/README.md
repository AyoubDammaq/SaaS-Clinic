# 🏥 Notification Service - Clean Architecture

Ce projet implémente un service de gestion des notifications basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet
### 📌 Notification.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `NotificationsController.cs` : Gestion des opérations sur les notifications.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 Notification.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `NotificationTemplateRequest.cs` : DTO pour les requêtes de création ou mise à jour de templates de notification.
- `Services/` : Contient la logique métier orchestrée.
  - `NotificationService.cs` : Service principal pour les opérations sur les notifications.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
  - `INotificationService.cs`

### 📌 Notification.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `PreferenceNotification.cs` : Entité représentant les préférences de notification.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
- `Enums/` : Énumérations métier.
  - `CanalNotification.cs` : Énumération pour les canaux de notification.
  - `StatutNotification.cs` : Énumération pour les statuts des notifications.
  - `TypeNotification.cs` : Énumération pour les types de notifications.
- `Interfaces/` : Interfaces des repositories.
  - `INotificationRepository.cs` : Interface pour les opérations sur les notifications.

### 📌 Notification.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
  - `NotificationDBContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.
  - `NotificationRepository.cs` : Implémentation pour les opérations sur les notifications.
- `Migrations/` : Migrations Entity Framework Core pour la base de données.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Notification.API.Controllers`
2. Le contrôleur appelle `NotificationService` → `Notification.Application`
3. Le service utilise un repository → `Notification.Domain.Interfaces` → implémenté dans `Notification.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `Notification.API/` pour containeriser l'application :

### 📌 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande