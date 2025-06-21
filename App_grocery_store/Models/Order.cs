using System;

namespace App_grocery_store.Models
{
    public class Order
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public DateTime order_date { get; set; }
        public string status { get; set; }
        public decimal total_price { get; set; }
    }
}