# 🏥 PatientManagementService - Clean Architecture

Ce projet implémente un service de gestion des patients basé sur une architecture propre (Clean Architecture), visant à séparer les responsabilités en couches bien définies : API, Application, Domaine et Infrastructure.

## 📁 Structure du Projet

### 📌 PatientManagementService.API - Couche Présentation (Web API)

- Contient les contrôleurs exposant les endpoints HTTP.
  - `PatientController.cs` : Gestion des opérations sur les patients.
  - `DossierMedicalController.cs` : Gestion des dossiers médicaux.
- Configuration de l’application (.NET `Program.cs`, `appsettings.json`).
- `Dockerfile` inclus pour la containerisation.

### 📌 PatientManagementService.Application - Couche Logique Applicative

- **DTOs/** : Objets de transfert utilisés pour structurer les données échangées.
  - `PatientDTO.cs` : DTO pour les données des patients.
  - `DossierMedicalDTO.cs` : DTO pour les dossiers médicaux.
  - `PatientStatDTO.cs` : DTO pour les statistiques des patients.
- **Services/** : Contient la logique métier orchestrée.
  - `PatientService.cs` : Service principal pour les opérations sur les patients.
  - `DossierMedicalService.cs` : Service pour la gestion des dossiers médicaux.
- **Interfaces/** : Interfaces des services pour l'injection de dépendances.
  - `IPatientService.cs`
  - `IDossierMedicalService.cs`

### 📌 PatientManagementService.Domain - Couche Domaine Métier

- **Entities/** : Entités métier.
  - `Patient.cs` : Entité représentant un patient.
  - `DossierMedical.cs` : Entité pour les dossiers médicaux.
- **ValueObject/** : Objets valeurs représentant des concepts métier immuables.
  - `ValueObjectStat.cs` : Objet valeur pour les statistiques.
- **Enums/** : Énumérations métier.
- **Interfaces/** : Interfaces des repositories.
  - `IPatientRepository.cs` : Interface pour les opérations sur les patients.

### 📌 PatientManagementService.Infrastructure - Couche Accès aux Données

- **Data/** : Configuration de la base de données.
  - `PatientDbContext.cs` : Contexte Entity Framework Core pour la gestion des données.
- **Repositories/** : Implémentations concrètes des interfaces définies dans le domaine.
  - `PatientRepository.cs` : Implémentation pour les opérations sur les patients.
  - `DossierMedicalRepository.cs` : Implémentation pour les dossiers médicaux.
- **Migrations/** : Migrations Entity Framework Core pour la base de données.

### 🔄 Flux de Données

1. L’utilisateur appelle un endpoint de l’API → `PatientManagementService.API.Controllers`
2. Le contrôleur appelle `PatientService` ou `DossierMedicalService` → `PatientManagementService.Application`
3. Le service utilise un repository → `PatientManagementService.Domain.Interfaces` → implémenté dans `PatientManagementService.Infrastructure`
4. Résultat renvoyé sous forme de DTO à l’API

### 🧪 Testabilité

Grâce à la séparation des couches :

- Les services peuvent être testés indépendamment.
- Les repositories peuvent être mockés via les interfaces.
- L'API reste fine, en se concentrant sur la gestion des requêtes HTTP.

### 🐳 Docker

Un fichier `Dockerfile` est disponible dans `PatientManagementService.API/` pour containeriser l'application :

```bash
docker build -t patient-management-service .
docker run -p 5001:80 patient-management-service
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
