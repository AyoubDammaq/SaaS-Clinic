# 🏥 Clinic Service - Clean Architecture

Ce projet implémente un service de gestion des cliniques basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

```plaintext
 Clinic.API/           # Couche présentation (Web API)
 Clinic.Application/   # Logique métier
 Clinic.Domain/        # Modèle de domaine métier
 Clinic.Infrastructure/# Accès aux données
```
### 📌 Clinic.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
- Configuration de l’application (.NET Program.cs, appsettings.json).
- Dockerfile inclus pour la containerisation.

### 📌 Clinic.Application - Couche Logique Applicative

- `DTOs/` : Objets de transfert utilisés pour structurer les données échangées.
  - `CliniqueDto`, `StatistiqueDTO`, `StatistiqueCliniqueDTO`.
- `Services/` : Contient la logique métier orchestrée.
  - `CliniqueService.cs` : Service principal pour les opérations sur les cliniques.
- `Interfaces/` : Interfaces des services pour l'injection de dépendances.


### 📌 Clinic.Domain - Couche Domaine Métier

- `Entities/` : Entités métier (ex. : `Clinique.cs`).
- `ValueObject/` : Objets valeurs représentant des concepts métier immuables.
  - `Statistique`, `StatistiqueClinique`.
- `Enums/` : Énumérations métier (ex. : type, statut...).
- `Interfaces/` : Interfaces des repositories (ex. : `ICliniqueRepository`).

### 📌 Clinic.Infrastructure - Couche Accès aux Données

- `Data/` : Configuration de la base de données (ex. : `CliniqueDbContext`).
- `Repositories/` : Implémentations concrètes des interfaces définies dans le domaine.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `Clinic.API.Controllers`
2. Le contrôleur appelle `CliniqueService` → `Clinic.Application`
3. Le service utilise un repository → `Clinic.Domain.Interfaces` → implémenté dans `Clinic.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :
- Les services peuvent être testés indépendamment. 
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier Dockerfile est disponible dans Clinic.API/ pour containeriser l'application :
```
 docker build -t clinic-api .
 docker run -p 5000:80 clinic-api
```

### 📌 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/)
- SQL Server 2019 ou version supérieure
- Visual Studio 2022 ou VS Code
- Docker Desktop

### ✍️ Auteurs

Projet développé dans le cadre d’un stage PFE au sein de [Corilus TN](https://www.corilus.be/fr/).
- Contact : [ayoub.dammak81@gmail.com](mailto:ayoub.dammak81@gmail.com)

- Licence : Privé / Sur demande
