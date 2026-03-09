using SAAS_Projectplanningtool.Models.CRM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.CRM
{
    public enum MessageStatus
    {
        Offen = 0,
        InBearbeitung = 1,
        Geschlossen = 2
    }

    public class CustomerMessage
    {
        [Key] public string MessageId { get; set; } = Guid.NewGuid().ToString();

        public string CustomerId { get; set; } = default!;
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }

        public required string Subject { get; set; }
        public string? Content { get; set; }
        public MessageStatus Status { get; set; } = MessageStatus.Offen;

        // File attachment — stored as relative path under wwwroot/uploads/messages/
        public string? AttachmentPath { get; set; }
        public string? AttachmentFileName { get; set; }

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
        public DateTime CreatedTimestamp { get; set; } = DateTime.Now;

        public string? ClosedById { get; set; }
        [ForeignKey(nameof(ClosedById))]
        public Employee? ClosedByEmployee { get; set; }
        public DateTime? ClosedTimestamp { get; set; }
    }
}
