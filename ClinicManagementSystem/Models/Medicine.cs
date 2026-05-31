namespace ClinicManagementSystem.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool LowStockAlert { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<PrescribedMedicine> PrescribedMedicines { get; set; }

        public Medicine()
        {
            LowStockAlert = false;
            CreatedAt = DateTime.Now;
            PrescribedMedicines = new List<PrescribedMedicine>();
        }

        public Medicine(string name, string category, string manufacturer, int stock, decimal price)
        {
            Name = name;
            Category = category;
            Manufacturer = manufacturer;
            StockQuantity = stock;
            Price = price;
            LowStockAlert = stock < 20;
            CreatedAt = DateTime.Now;
            PrescribedMedicines = new List<PrescribedMedicine>();
        }
    }
}
