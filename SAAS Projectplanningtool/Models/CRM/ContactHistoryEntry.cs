using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAAS_Projectplanningtool.Models.CRM
{
    public enum ContactType
    {
        Telefonat = 0,
        Email = 1,
        Chat = 2,
        Meeting = 3,
        Sonstiges = 4
    }

    public class ContactHistoryEntry
    {
        [Key] public string EntryId { get; set; } = Guid.NewGuid().ToString();

        public string CustomerId { get; set; } = default!;
        [ForeignKey(nameof(CustomerId))]
        public Customer? Customer { get; set; }


        public ContactType ContactType { get; set; } = ContactType.Telefonat;
        public required string Note { get; set; }
        public DateTime ContactDate { get; set; } = DateTime.Now;

        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public Employee? CreatedByEmployee { get; set; }
    }
}
