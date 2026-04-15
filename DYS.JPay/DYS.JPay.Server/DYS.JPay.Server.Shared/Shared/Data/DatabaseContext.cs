using DYS.JPay.Server.Shared.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DYS.JPay.Server.Shared.Data
{
    public class DatabaseContext
    {
        private readonly SQLiteAsyncConnection _connection;

        public DatabaseContext(string dbPath)
        {
            _connection = new SQLiteAsyncConnection(dbPath);
        }

        public async Task InitializeAsync() {
            // Create tables safely
            await _connection.CreateTableAsync<Testing>();

            // Seed default admin
            //await SeedAdminUser();
            //await SeedSettings();
        }
        public SQLiteAsyncConnection Connection => _connection;

    }
}
