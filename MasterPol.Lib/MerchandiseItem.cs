using System;
using System.Collections.Generic;

namespace MasterPol.Lib
{
    public class MerchandiseItem
    {
        public int Id { get; set; }
        public string Article { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public decimal MinPartnerPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public virtual ICollection<SaleRecord> SalesHistory { get; set; }
    }
}