using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MasterPol.Lib;

namespace MasterPol.Data_Zolotova.Database
{
    public class PartnerRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public PartnerRepository()
        {
            _context = new AppDbContext();
        }

        // Получение всех партнеров с их типами и историей продаж
        public List<BusinessPartner> GetAllPartners()
        {
            return _context.BusinessPartners
                .Include(p => p.PartnerType)
                .Include(p => p.SalesHistory)
                    .ThenInclude(s => s.Product)
                .OrderBy(p => p.CompanyName)
                .ToList();
        }

        // Получение партнера по ID
        public BusinessPartner GetPartnerById(int id)
        {
            return _context.BusinessPartners
                .Include(p => p.PartnerType)
                .Include(p => p.SalesHistory)
                    .ThenInclude(s => s.Product)
                .FirstOrDefault(p => p.Id == id);
        }

        // Получение всех типов партнеров
        public List<PartnerCategory> GetAllPartnerTypes()
        {
            return _context.PartnerCategories.OrderBy(t => t.Name).ToList();
        }

        // Получение всех товаров
        public List<MerchandiseItem> GetAllProducts()
        {
            return _context.MerchandiseItems.OrderBy(p => p.Name).ToList();
        }

        // Добавление нового партнера
        public void AddPartner(BusinessPartner partner)
        {
            partner.CreatedAt = DateTime.Now;
            partner.UpdatedAt = DateTime.Now;
            _context.BusinessPartners.Add(partner);
            _context.SaveChanges();
        }

        // Обновление партнера
        public void UpdatePartner(BusinessPartner partner)
        {
            var existing = _context.BusinessPartners.Find(partner.Id);
            if (existing != null)
            {
                existing.PartnerTypeId = partner.PartnerTypeId;
                existing.CompanyName = partner.CompanyName;
                existing.LegalAddress = partner.LegalAddress;
                existing.Inn = partner.Inn;
                existing.DirectorName = partner.DirectorName;
                existing.Phone = partner.Phone;
                existing.Email = partner.Email;
                existing.Rating = partner.Rating;
                existing.UpdatedAt = DateTime.Now;

                _context.SaveChanges();
            }
        }

        // Удаление партнера
        public void DeletePartner(int id)
        {
            var partner = _context.BusinessPartners.Find(id);
            if (partner != null)
            {
                _context.BusinessPartners.Remove(partner);
                _context.SaveChanges();
            }
        }

        // Получение истории продаж партнера
        public List<SaleRecord> GetPartnerSalesHistory(int partnerId)
        {
            return _context.SaleRecords
                .Include(s => s.Product)
                .Where(s => s.PartnerId == partnerId)
                .OrderByDescending(s => s.SaleDate)
                .ToList();
        }

        // Добавление продажи
        public void AddSaleRecord(SaleRecord sale)
        {
            sale.CreatedAt = DateTime.Now;
            _context.SaleRecords.Add(sale);
            _context.SaveChanges();
        }

        // Расчет общей суммы продаж партнера
        public decimal GetPartnerTotalSales(int partnerId)
        {
            return _context.SaleRecords
                .Include(s => s.Product)
                .Where(s => s.PartnerId == partnerId)
                .Sum(s => s.Quantity * s.Product.MinPartnerPrice);
        }

        // Расчет скидки на основе общей суммы продаж
        public int CalculateDiscount(decimal totalSales)
        {
            if (totalSales > 300000) return 15;
            if (totalSales > 50000) return 10;
            if (totalSales > 10000) return 5;
            return 0;
        }

        // Проверка подключения к БД
        public bool TestConnection()
        {
            try
            {
                return _context.Database.CanConnect();
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}