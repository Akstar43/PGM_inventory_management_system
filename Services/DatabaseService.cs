using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace InventoryApp.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private const string DbFileName = "inventory.db";

        public DatabaseService()
        {
            _connectionString = $"Data Source={DbFileName}";
            InitializeDatabase();
        }

        public string ConnectionString => _connectionString;

        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Price DECIMAL NOT NULL,
                        SellingPrice DECIMAL NOT NULL,
                        Supplier TEXT NOT NULL,
                        StockQuantity INTEGER NOT NULL,
                        ReorderLevel INTEGER NOT NULL DEFAULT 5,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }

                // Migration: Add ReorderLevel if it doesn't exist
                try
                {
                    string addColumnQuery = "ALTER TABLE Products ADD COLUMN ReorderLevel INTEGER NOT NULL DEFAULT 5;";
                    using (var command = new SqliteCommand(addColumnQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch { /* Column already exists */ }
            }
        }
    }
}
