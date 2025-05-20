namespace Notification.Application.DTOs
{
    public class NotificationTemplateRequest
    {
        public Guid TemplateId { get; set; }
        public Guid UtilisateurId { get; set; }
        public Dictionary<string, string> Donnees { get; set; } = new();
    }

}
