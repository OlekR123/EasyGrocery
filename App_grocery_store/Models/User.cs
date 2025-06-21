namespace App_grocery_store.Models
{
    public class User
    {
        public int id { get; set; }
        public string surname { get; set; }
        public string name { get; set; }
        public string patronymic { get; set; }
        public string email { get; set; }
        public int role_id { get; set; }
        public string phone { get; set; }
    }
}