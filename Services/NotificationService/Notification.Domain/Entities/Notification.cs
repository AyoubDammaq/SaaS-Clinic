using Notification.Domain.Enums;

namespace Notification.Domain.Entities
{

    public class Notification
    {
        public Guid Id { get; set; } 
        public Guid UtilisateurId { get; set; }
        public string Titre { get; set; }
        public string Message { get; set; }
        public TypeNotification Type { get; set; }
        public List<CanalNotification> Canaux { get; set; }
        public StatutNotification Statut { get; set; } = StatutNotification.NON_LU;
        public DateTime DateEnvoi { get; set; }
        public DateTime? DateLecture { get; set; }
    }
}
