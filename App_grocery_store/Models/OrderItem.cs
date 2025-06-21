namespace App_grocery_store.Models
{
    public class OrderItem
    {
        public int id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
    }
}