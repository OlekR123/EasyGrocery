namespace App_grocery_store.Models
{
    public class StockReport
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category_name { get; set; }
        public decimal price { get; set; }
        public int stock { get; set; }
        public decimal total_value { get; set; }
    }
}