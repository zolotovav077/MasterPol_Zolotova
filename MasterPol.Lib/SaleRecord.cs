using System;

namespace MasterPol.Lib
{
    public class SaleRecord
    {
        public int Id { get; set; }
        public int PartnerId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационные свойства
        public virtual BusinessPartner Partner { get; set; }
        public virtual MerchandiseItem Product { get; set; }

        // Вычисляемое поле для общей суммы
        public decimal TotalAmount => Product?.MinPartnerPrice * Quantity ?? 0;
    }
}