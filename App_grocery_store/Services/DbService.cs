using App_grocery_store.Models;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using Npgsql;
using System.Linq;
using System.Collections.Generic;
using App_grocery_store.Data;

namespace App_grocery_store.Services
{
    public class DbService
    {
        private readonly GroceryDbContext _context;

        public DbService()
        {
            _context = new GroceryDbContext();
        }

        // Аутентификация
        public User Authenticate(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == email);
            if (user == null)
                return null;

            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT password FROM users WHERE email = @email", connection))
                {
                    command.Parameters.AddWithValue("email", email);
                    var hashedPassword = (string)command.ExecuteScalar();
                    if (hashedPassword != null && BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                        return user;
                }
            }
            return null;
        }

        // Регистрация
        public int RegisterUser(User user, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM register_user(@surname, @name, @patronymic, @email, @password, @role_id, @phone)", connection))
                {
                    command.Parameters.AddWithValue("surname", user.surname);
                    command.Parameters.AddWithValue("name", user.name);
                    command.Parameters.AddWithValue("patronymic", user.patronymic);
                    command.Parameters.AddWithValue("email", user.email);
                    command.Parameters.AddWithValue("password", hashedPassword);
                    command.Parameters.AddWithValue("role_id", user.role_id);
                    command.Parameters.AddWithValue("phone", user.phone);
                    command.Parameters.Add(new NpgsqlParameter("new_user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                    command.ExecuteNonQuery();
                    return (int)command.Parameters["new_user_id"].Value;
                }
            }
        }

        // Обновление пользователя
        public void UpdateUser(User user, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM update_user(@id, @surname, @name, @patronymic, @email, @password, @role_id, @phone)", connection))
                {
                    command.Parameters.AddWithValue("id", user.id);
                    command.Parameters.AddWithValue("surname", user.surname);
                    command.Parameters.AddWithValue("name", user.name);
                    command.Parameters.AddWithValue("patronymic", user.patronymic);
                    command.Parameters.AddWithValue("email", user.email);
                    command.Parameters.AddWithValue("password", hashedPassword);
                    command.Parameters.AddWithValue("role_id", user.role_id);
                    command.Parameters.AddWithValue("phone", user.phone);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаление пользователя
        public void DeleteUser(int userId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM delete_user(@id)", connection))
                {
                    command.Parameters.AddWithValue("id", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение ролей пользователей
        public List<UserRole> GetUserRoles()
        {
            return _context.UserRoles.ToList();
        }

        // Получение пользователей
        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        // Получение товаров с фильтрацией
        public List<Product> GetProducts(int? categoryId = null, string searchText = null)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("SELECT * FROM search_products(@p_name)", connection))
                    {
                        command.Parameters.AddWithValue("p_name", searchText);
                        using (var reader = command.ExecuteReader())
                        {
                            var products = new List<Product>();
                            while (reader.Read())
                            {
                                products.Add(new Product
                                {
                                    id = reader.GetInt32(0),
                                    article = reader.GetString(1),
                                    name = reader.GetString(2),
                                    category_id = reader.GetInt32(3),
                                    category_name = reader.GetString(4),
                                    price = reader.GetDecimal(5),
                                    stock = reader.GetInt32(6),
                                    supplier_id = reader.GetInt32(7),
                                    supplier_name = reader.GetString(8),
                                    image_url = reader.GetString(9),
                                    description = reader.GetString(10)
                                });
                            }
                            return products;
                        }
                    }
                }
            }

            var query = _context.Products.AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(p => p.category_id == categoryId.Value);
            return query.ToList();
        }

        // Создание товара
        public int CreateProduct(Product product)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM create_product(@article, @name, @category_id, @price, @stock, @supplier_id, @image_url, @description)", connection))
                {
                    command.Parameters.AddWithValue("article", product.article);
                    command.Parameters.AddWithValue("name", product.name);
                    command.Parameters.AddWithValue("category_id", product.category_id);
                    command.Parameters.AddWithValue("price", product.price);
                    command.Parameters.AddWithValue("stock", product.stock);
                    command.Parameters.AddWithValue("supplier_id", product.supplier_id);
                    command.Parameters.AddWithValue("image_url", product.image_url);
                    command.Parameters.AddWithValue("description", product.description);
                    command.Parameters.Add(new NpgsqlParameter("new_product_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                    command.ExecuteNonQuery();
                    return (int)command.Parameters["new_product_id"].Value;
                }
            }
        }

        // Обновление товара
        public void UpdateProduct(Product product)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM update_product(@id, @article, @name, @category_id, @price, @stock, @supplier_id, @image_url, @description)", connection))
                {
                    command.Parameters.AddWithValue("id", product.id);
                    command.Parameters.AddWithValue("article", product.article);
                    command.Parameters.AddWithValue("name", product.name);
                    command.Parameters.AddWithValue("category_id", product.category_id);
                    command.Parameters.AddWithValue("price", product.price);
                    command.Parameters.AddWithValue("stock", product.stock);
                    command.Parameters.AddWithValue("supplier_id", product.supplier_id);
                    command.Parameters.AddWithValue("image_url", product.image_url);
                    command.Parameters.AddWithValue("description", product.description);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаление товара
        public void DeleteProduct(int productId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM delete_product(@id)", connection))
                {
                    command.Parameters.AddWithValue("id", productId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение категорий
        public List<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        // Создание категории
        public int CreateCategory(Category category)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM create_category(@name)", connection))
                {
                    command.Parameters.AddWithValue("name", category.name);
                    command.Parameters.Add(new NpgsqlParameter("new_category_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                    command.ExecuteNonQuery();
                    return (int)command.Parameters["new_category_id"].Value;
                }
            }
        }

        // Обновление категории
        public void UpdateCategory(Category category)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM update_category(@id, @name)", connection))
                {
                    command.Parameters.AddWithValue("id", category.id);
                    command.Parameters.AddWithValue("name", category.name);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаление категории
        public void DeleteCategory(int categoryId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM delete_category(@id)", connection))
                {
                    command.Parameters.AddWithValue("id", categoryId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение поставщиков
        public List<Supplier> GetSuppliers()
        {
            return _context.Suppliers.ToList();
        }

        // Создание поставщика
        public int CreateSupplier(Supplier supplier)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM create_supplier(@organization_name, @address, @phone_number, @ogrn)", connection))
                {
                    command.Parameters.AddWithValue("organization_name", supplier.organization_name);
                    command.Parameters.AddWithValue("address", supplier.address);
                    command.Parameters.AddWithValue("phone_number", supplier.phone_number);
                    command.Parameters.AddWithValue("ogrn", supplier.ogrn);
                    command.Parameters.Add(new NpgsqlParameter("new_supplier_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                    command.ExecuteNonQuery();
                    return (int)command.Parameters["new_supplier_id"].Value;
                }
            }
        }

        // Обновление поставщика
        public void UpdateSupplier(Supplier supplier)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM update_supplier(@id, @organization_name, @address, @phone_number, @ogrn)", connection))
                {
                    command.Parameters.AddWithValue("id", supplier.id);
                    command.Parameters.AddWithValue("organization_name", supplier.organization_name);
                    command.Parameters.AddWithValue("address", supplier.address);
                    command.Parameters.AddWithValue("phone_number", supplier.phone_number);
                    command.Parameters.AddWithValue("ogrn", supplier.ogrn);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаление поставщика
        public void DeleteSupplier(int supplierId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM delete_supplier(@id)", connection))
                {
                    command.Parameters.AddWithValue("id", supplierId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение заказов
        public List<Order> GetOrders(int? userId = null)
        {
            var query = _context.Orders.AsQueryable();
            if (userId.HasValue)
                query = query.Where(o => o.user_id == userId.Value);
            return query.ToList();
        }

        // Получение элементов заказа
        public List<OrderItem> GetOrderItems(int orderId)
        {
            return _context.OrderItems.Where(oi => oi.order_id == orderId).ToList();
        }

        // Создание заказа
        public int CreateOrder(int userId, List<CartItem> cartItems)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                int orderId;
                using (var command = new NpgsqlCommand("SELECT * FROM create_order(@user_id)", connection))
                {
                    command.Parameters.AddWithValue("user_id", userId);
                    command.Parameters.Add(new NpgsqlParameter("new_order_id", NpgsqlTypes.NpgsqlDbType.Integer) { Direction = ParameterDirection.Output });
                    command.ExecuteNonQuery();
                    orderId = (int)command.Parameters["new_order_id"].Value;
                }

                foreach (var item in cartItems)
                {
                    using (var command = new NpgsqlCommand("SELECT * FROM add_order_item(@order_id, @product_id, @quantity)", connection))
                    {
                        command.Parameters.AddWithValue("order_id", orderId);
                        command.Parameters.AddWithValue("product_id", item.product.id);
                        command.Parameters.AddWithValue("quantity", item.quantity);
                        command.ExecuteNonQuery();
                    }
                }

                return orderId;
            }
        }

        // Удаление заказа
        public void DeleteOrder(int orderId)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM delete_order(@id)", connection))
                {
                    command.Parameters.AddWithValue("id", orderId);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Обновление статуса заказа
        public void UpdateOrderStatus(int orderId, string status)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT grocery_store.update_order_status(@order_id, @status::grocery_store.order_status)", connection))
                {
                    command.Parameters.AddWithValue("order_id", orderId);
                    command.Parameters.AddWithValue("status", status);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Отчёт по остаткам товаров
        public List<StockReport> GetStockReport()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["GroceryDbConnection"].ConnectionString;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM get_stock_report()", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var reports = new List<StockReport>();
                        while (reader.Read())
                        {
                            reports.Add(new StockReport
                            {
                                id = reader.GetInt32(0),
                                name = reader.GetString(1),
                                category_name = reader.GetString(2),
                                price = reader.GetDecimal(3),
                                stock = reader.GetInt32(4),
                                total_value = reader.GetDecimal(5)
                            });
                        }
                        return reports;
                    }
                }
            }
        }
    }
}