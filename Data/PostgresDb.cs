using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Rinha.Backend.API.Data
{
    public class PostgresDb : IDisposable
    {
        private NpgsqlConnection _connection;
        public async Task<NpgsqlConnection> GetConnectionAsync()
        {
            _connection = new NpgsqlConnection("Host=db;Port=5432;Database=postgres;Username=postgres;Password=pass");
            await _connection.OpenAsync();
            return _connection;
        }

        public void Dispose()
        {
            _connection.Close();
            GC.Collect();
        }
    }
}