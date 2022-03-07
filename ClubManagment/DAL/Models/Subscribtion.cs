using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Subscribtion : AuditableEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string SubscriberId { get; set; }
        public ApplicationUser Subscriber { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }

    }
}
