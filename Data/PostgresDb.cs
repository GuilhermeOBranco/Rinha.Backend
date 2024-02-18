using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace Rinha.Backend.API.Data
{
    public static class PostgresDb
    {
        public static async Task<NpgsqlConnection> GetConnectionAsync()
        {
            var conn = new NpgsqlConnection("Host=db;Port=5432;Database=postgres;Username=postgres;Password=pass");
            await conn.OpenAsync();
            return conn;
        }
    }
}