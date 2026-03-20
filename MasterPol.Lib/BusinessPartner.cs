using MasterPol.Data_Zolotova.Database;
using System;
using System.Collections.Generic;

namespace MasterPol.Lib
{
    public class BusinessPartner
    {
        public int Id { get; set; }
        public int PartnerTypeId { get; set; }
        public string CompanyName { get; set; }
        public string LegalAddress { get; set; }
        public string Inn { get; set; }
        public string DirectorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Навигационные свойства
        public virtual PartnerCategory PartnerType { get; set; }
        public virtual ICollection<SaleRecord> SalesHistory { get; set; }
        public string DiscountText
        {
            get
            {
                using (var repo = new PartnerRepository())
                {
                    decimal totalSales = repo.GetPartnerTotalSales(this.Id);
                    int discount = repo.CalculateDiscount(totalSales);
                    return $"{discount}%";
                }
            }
        }
    }
}