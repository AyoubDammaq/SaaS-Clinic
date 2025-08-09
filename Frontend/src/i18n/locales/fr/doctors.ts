export default {
  // Doctors
  doctorsList: "Liste des médecins",
  addDoctor: "Ajouter un médecin",
  editDoctor: "Modifier un médecin",
  deleteDoctor: "Supprimer un médecin",
  doctorDetails: "Détails du médecin",
  fullName: "Nom Complet",
  firstName: "Prénom",
  lastName: "Nom",
  specialty: "Spécialité",
  email: "Email",
  phone: "Téléphone",
  clinic: "Clinique",
  photo: "Photo",
  assignToClinic: "Assigner à une clinique",
  actions: "Actions",
  removeFromClinic: "Retirer de la clinique",
  noClinicAssigned: "Aucune clinique assignée",
  doctorsOfClinic: "Médecins de la clinique",
  availableDoctors: "Médecins disponibles",
  noDoctorsInClinic: "Aucun médecin assigné à cette clinique",
  noAvailableDoctors: "Aucun médecin disponible pour l’assignation",
  noDoctorsFound:
    "Aucun médecin trouvé correspondant à vos critères de recherche",
  takeAppointment: "Prendre rendez-vous",
  backToDoctorsList: "Retour à la liste des médecins",
  loadingDoctors: "Chargement des médecins...",
  loginRequired: "Veuillez vous connecter pour accéder à cette page",
  doctorDataNotFound: "Données du médecin non trouvées",
  manageProfile: "Gérer vos informations professionnelles et paramètres",
  previous: "Précédent",
  next: "Suivant",
  pageOf: "Page {current} sur {total}",

  // Doctor Availability
  availability: "Disponibilité",
  addAvailability: "Ajouter une disponibilité",
  editAvailability: "Modifier une disponibilité",
  deleteAvailability: "Supprimer une disponibilité",
  date: "Date",
  startTime: "Heure de début",
  endTime: "Heure de fin",
  isAvailable: "Est disponible",
  noAvailabilityFound: "Aucune disponibilité trouvée",

  // Filters
  filters: "Filtres",
  clearFilters: "Effacer les filtres",
  allSpecialties: "Toutes les spécialités",
  allClinics: "Toutes les cliniques",
  all: "Tous",
  assignment: "Affectation",
  assigned: "Assignés",
  unassigned: "Non assignés",
  availabilityDate: "Date de disponibilité",
  clinicDoctors: "Médecins de la clinique",
  filterBySpecialty: "Filtrer par spécialité",
  filterByName: "Filtrer par nom",
  filterByClinic: "Filtrer par clinique",
  search: "Recherche",

  // Statistics
  doctorsBySpecialty: "Médecins par spécialité",
  doctorsByClinic: "Médecins par clinique",

  // Messages
  doctorAddSuccess: "Médecin ajouté avec succès",
  doctorUpdateSuccess: "Médecin mis à jour avec succès",
  doctorDeleteSuccess: "Médecin supprimé avec succès",
  availabilityAddSuccess: "Disponibilité ajoutée avec succès",
  availabilityDeleteSuccess: "Disponibilité supprimée avec succès",
  confirmDelete: "Êtes-vous sûr de vouloir supprimer ce médecin?",

  // Errors
  errorFetchingDoctors: "Erreur lors de la récupération des médecins",
  errorAddingDoctor: "Erreur lors de l'ajout du médecin",
  errorUpdatingDoctor: "Erreur lors de la mise à jour du médecin",
  errorDeletingDoctor: "Erreur lors de la suppression du médecin",
  errorAddingAvailability: "Erreur lors de l'ajout de la disponibilité",
  errorDeletingAvailability:
    "Erreur lors de la suppression de la disponibilité",

  sur: "sur",

  // Validation messages
  firstNameRequired: "Le prénom est requis",
  lastNameRequired: "Le nom est requis",
  invalidEmail: "Veuillez entrer une adresse email valide.",
  phoneLength: "Le numéro de téléphone doit comporter au moins 5 caractères.",
  specialtyRequired: "Veuillez sélectionner une spécialité.",

  // Specialties
  generalPractitioner: "Médecin généraliste",
  pediatrician: "Pédiatre",
  cardiologist: "Cardiologue",
  dermatologist: "Dermatologue",
  neurologist: "Neurologue",
  psychiatrist: "Psychiatre",
  ophthalmologist: "Ophtalmologue",
  gynecologist: "Gynécologue",
  orthopedist: "Orthopédiste",
  dentist: "Dentiste",
  unknownSpecialty: "Spécialité inconnue",

  // Form Buttons
  cancel: "Annuler",
  saving: "Enregistrement...",
  update: "Mettre à jour",
  create: "Créer",
  saveSettings: "Enregistrer les paramètres",

  // DoctorsList Specific
  searchDoctors: "Rechercher des médecins...",
  notAssigned: "Non assigné",

  assignDoctorToClinic: "Assigner le médecin à une clinique",
  selectClinic: "Choisir une clinique",
  confirm: "Confirmer",
  confirmUnassignTitle: "Confirmer la désassignation",
  confirmUnassignDescription:
    "Voulez-vous vraiment désassigner ce médecin de la clinique ? Cette action est irréversible.",
  unassignDoctor: "Désassigner",
  confirmDeleteDoctorTitle: "Confirmer la suppression du médecin",
  confirmDeleteDoctorMessage:
    "Voulez-vous vraiment supprimer ce médecin ? Cette action est irréversible.",

  // DoctorProfile Specific
  doctorProfileTitle: "Profil Médecin",
  profileTab: "Profil",
  settingsTab: "Paramètres",
  scheduleTab: "Planning",
  viewClinic: "Voir la clinique",
  unknownClinic: "Inconnue",
  notificationPreferences: "Préférences de notification",
  emailNotifications: "Notifications Email",
  emailNotificationsDescription:
    "Recevoir les mises à jour sur les rendez-vous et les résultats des patients",
  smsNotifications: "Notifications SMS",
  smsNotificationsDescription:
    "Recevoir des SMS pour les notifications urgentes",
  settingsSavedSuccess: "Paramètres sauvegardés avec succès",
  accountSettings: "Paramètres du compte",
  changePassword: "Changer le mot de passe",
  disableAccount: "Désactiver le compte",

  // DoctorSchedule Specific
  doctorScheduleTitle: "Planning Médecin",
  loadingAvailabilities: "Chargement des disponibilités...",
  confirmDeleteAvailability:
    "Êtes-vous sûr de vouloir supprimer cette disponibilité ?",
  errorSavingAvailability:
    "Erreur lors de l'enregistrement de la disponibilité",
  fillAllFields: "Veuillez remplir tous les champs.",
  invalidTimeRange: "L'heure de début doit être avant l'heure de fin.",
  invalidAvailabilityId: "ID de disponibilité invalide.",
  dayCannotBeChanged: "Le jour ne peut pas être modifié lors de l'édition.",
  timePlaceholder: "ex. 09:00",
  day: "Jour",
  sunday: "Dimanche",
  monday: "Lundi",
  tuesday: "Mardi",
  wednesday: "Mercredi",
  thursday: "Jeudi",
  friday: "Vendredi",
  saturday: "Samedi",
};
