using System;
using System.Collections.Generic;

namespace MasterPol.Lib
{
    public class PartnerCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<BusinessPartner> Partners { get; set; }
    }
}