namespace Notification.Domain.Entities
{
    public class PreferenceNotification
    {
        public Guid UtilisateurId { get; set; }
        public bool RecevoirEmail { get; set; }
        public bool RecevoirSms { get; set; }
        public bool RecevoirInApp { get; set; }
        public string Langue { get; set; } = "fr";
    }
}
