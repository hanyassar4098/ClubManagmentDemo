using System;
using System.Linq;

namespace DAL.Models
{
    public class SubscribtionDetail : AuditableEntity
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }


        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int SubscribtionId { get; set; }
        public Subscribtion Subscribtion { get; set; }
    }
}
