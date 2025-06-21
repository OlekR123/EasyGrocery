namespace App_grocery_store.Models
{
    public class Product
    {
        public int id { get; set; }
        public string article { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public decimal price { get; set; }
        public int stock { get; set; }
        public int supplier_id { get; set; }
        public string supplier_name { get; set; }
        public string image_url { get; set; }
        public string description { get; set; }
    }
}