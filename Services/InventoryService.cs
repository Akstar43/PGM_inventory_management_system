using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using InventoryApp.Models;

namespace InventoryApp.Services
{
    public class InventoryService
    {
        private readonly DatabaseService _databaseService;

        public InventoryService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Products";
                using (var command = new SqliteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(MapReaderToProduct(reader));
                    }
                }
            }
            return products;
        }

        public List<Product> SearchProducts(string searchTerm)
        {
            var products = new List<Product>();
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Products WHERE Name LIKE @term OR Category LIKE @term OR Supplier LIKE @term";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@term", $"%{searchTerm}%");
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            products.Add(MapReaderToProduct(reader));
                        }
                    }
                }
            }
            return products;
        }

        public void AddProduct(Product product)
        {
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                var query = @"INSERT INTO Products (Name, Category, Price, SellingPrice, Supplier, StockQuantity, ReorderLevel) 
                              VALUES (@name, @cat, @price, @sprice, @sup, @qty, @rl)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@cat", product.Category);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@sprice", product.SellingPrice);
                    command.Parameters.AddWithValue("@sup", product.Supplier);
                    command.Parameters.AddWithValue("@qty", product.StockQuantity);
                    command.Parameters.AddWithValue("@rl", product.ReorderLevel);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                var query = @"UPDATE Products SET Name=@name, Category=@cat, Price=@price, 
                              SellingPrice=@sprice, Supplier=@sup, StockQuantity=@qty, ReorderLevel=@rl WHERE Id=@id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", product.Id);
                    command.Parameters.AddWithValue("@name", product.Name);
                    command.Parameters.AddWithValue("@cat", product.Category);
                    command.Parameters.AddWithValue("@price", product.Price);
                    command.Parameters.AddWithValue("@sprice", product.SellingPrice);
                    command.Parameters.AddWithValue("@sup", product.Supplier);
                    command.Parameters.AddWithValue("@qty", product.StockQuantity);
                    command.Parameters.AddWithValue("@rl", product.ReorderLevel);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProduct(int id)
        {
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                var query = "DELETE FROM Products WHERE Id=@id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string ProcessTransaction(int productId, int quantityChange)
        {
            using (var connection = new SqliteConnection(_databaseService.ConnectionString))
            {
                connection.Open();
                
                // Get current stock and reorder level
                int currentQty = 0;
                int reorderLevel = 0;
                string name = "";
                var selectQuery = "SELECT Name, StockQuantity, ReorderLevel FROM Products WHERE Id = @id";
                using (var cmd = new SqliteCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", productId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = reader.GetString(0);
                            currentQty = reader.GetInt32(1);
                            reorderLevel = reader.GetInt32(2);
                        }
                    }
                }

                int newQty = currentQty + quantityChange;
                if (newQty < 0) return "Error: Transaction failed. Not enough stock available.";

                var updateQuery = "UPDATE Products SET StockQuantity = @qty WHERE Id = @id";
                using (var cmd = new SqliteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@qty", newQty);
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }

                if (newQty <= reorderLevel)
                {
                    return $"Alert: {name} has reached low stock level ({newQty} remaining). Please reorder.";
                }

                return "Success: Stock updated successfully.";
            }
        }

        private Product MapReaderToProduct(SqliteDataReader reader)
        {
            return new Product
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                SellingPrice = reader.GetDecimal(reader.GetOrdinal("SellingPrice")),
                Supplier = reader.GetString(reader.GetOrdinal("Supplier")),
                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                ReorderLevel = reader.GetInt32(reader.GetOrdinal("ReorderLevel")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
