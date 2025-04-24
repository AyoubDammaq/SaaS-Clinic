# 🏥 AuthService - Clean Architecture

Ce projet implémente un service d'authentification basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 Auth.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `AuthController.cs` : Gestion des opérations d'authentification.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 Auth.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `UserDTO.cs` : DTO pour les données des utilisateurs.
  - `TokenResponseDTO.cs` : DTO pour les réponses contenant les tokens.
  - `ChangePasswordDTO.cs` : DTO pour les requêtes de changement de mot de passe.
  - `ChangeRoleRequestDTO.cs` : DTO pour les requêtes de changement de rôle.
  - `ForgetPasswordDTO.cs` : DTO pour les requêtes d'oubli de mot de passe.
  - `ResetPasswordRequestDTO.cs` : DTO pour les requêtes de réinitialisation de mot de passe.
- `Services/` : Contient la logique métier orchestrée.
  - `AuthService.cs` : Service principal pour les opérations d'authentification.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.
  - `IAuthService.cs`

### 📌 Auth.Domain - Couche Domaine Métier

- `Entities/` : Entités métier.
  - `User.cs` : Entité représentant un utilisateur.
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
- `Enums/` : Énumérations métier.
  - `UserRole.cs` : Énumération pour les rôles des utilisateurs.
- `Interfaces/` : Interfaces des repositories.

### 📌 Auth.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données.
  - `UserDBContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Auth.API.Controllers`
2. Le contrôleur appelle `AuthService` → `Auth.Application`
3. Le service utilise un repository → `Auth.Domain.Interfaces` → implémenté dans `Auth.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `Auth.API/` pour containeriser l'application :

### 📌 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).

- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)
- Licence : Privé / Sur demande