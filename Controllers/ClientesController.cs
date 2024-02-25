using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Rinha.Backend.API.Data;
using Rinha.Backend.API.DomainExceptions;
using Rinha.Backend.API.Models;

namespace Rinha.Backend.API.Controllers
{
    [ApiController, Route("[controller]")]
    public class ClientesController : ControllerBase
    {
        [HttpPost, Route("{id}/transacoes")]
        public async Task<IActionResult> Transacoes([FromRoute] int id, [FromBody] TransacaoRequest request)
        {
            try
            {
                if (id < 1 || id > 5)
                    return NotFound();

                await ProcessarTransacao(request, id);

                return Ok(request);

            }
            catch (InvalidValueException ex)
            {
                return UnprocessableEntity();
            }
        }

        [HttpGet, Route("{id}/extrato")]
        public async Task<IActionResult> ObterExtrato([FromRoute] int id)
        {
            if (id < 1 || id > 5)
                return NotFound();

            List<Transacao> transacoes = new List<Transacao>();
            Saldo saldo = new Saldo();

            PostgresDb pgdb = new PostgresDb();

            using (var connection = await pgdb.GetConnectionAsync())
            {
                using (var command = new NpgsqlCommand("SELECT vl_transacao, tipo, descricao, dt_realizacao FROM TRANSACAO WHERE  cliente_id = @v1 order by dt_realizacao limit 10", connection))
                {
                    command.Parameters.AddWithValue("v1", id);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            transacoes.Add(new Transacao
                            {
                                Valor = reader.GetInt32(0),
                                Tipo = reader.GetString(1),
                                Descricao = reader.GetString(2),
                                RealizadaEm = reader.GetDateTime(3)
                            });
                        }
                    }
                }

                using (var command = new NpgsqlCommand("SELECT saldo_atual, limite FROM CLIENTE WHERE id = @v1", connection))
                {
                    command.Parameters.AddWithValue("v1", id);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            saldo.Total = reader.GetInt32(0);
                            saldo.Limite = reader.GetInt32(1);
                        }
                    }
                }
            }


            ExtratoDto extrato = new ExtratoDto { Saldo = saldo, UltimasTransacoes = transacoes };


            return Ok(extrato);
        }

        private async Task ProcessarTransacao(TransacaoRequest request, int id)
        {
            PostgresDb postgresDb = new PostgresDb();
            using (var connection = await postgresDb.GetConnectionAsync())
            {
                if (request.Tipo == "d")
                {
                    using (var command = new NpgsqlCommand("SELECT limite, saldo_atual FROM CLIENTE WHERE id = @v1", connection))
                    {
                        command.Parameters.AddWithValue("v1", id);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            if (reader.Rows > 0)
                            {
                                int limite = reader.GetInt32(0);
                                int saldoAtual = reader.GetInt32(1);

                                if ((request.Valor - saldoAtual) < limite)
                                {
                                    throw new InvalidValueException();
                                }
                            }
                        }

                    }
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        using (var command = new NpgsqlCommand("INSERT INTO TRANSACAO VALUES (@v1, @v2, @v3, @v4, @v5)", transaction.Connection))
                        {
                            command.Parameters.AddWithValue("v1", id);
                            command.Parameters.AddWithValue("v2", request.Tipo);
                            command.Parameters.AddWithValue("v3", request.Descricao);
                            command.Parameters.AddWithValue("v4", request.Valor);
                            command.Parameters.AddWithValue("v5", DateTime.Now);

                            command.ExecuteNonQuery();
                        }

                        using (var command = new NpgsqlCommand("UPDATE CLIENTE SET saldo_atual = saldo_atual + (@v1) where id = @v2", transaction.Connection))
                        {
                            command.Parameters.AddWithValue("v2", id);

                            if (request.Tipo.Equals("c"))
                            {
                                command.Parameters.AddWithValue("v1", request.Valor);
                            }
                            else
                            {
                                command.Parameters.AddWithValue("v1", request.Valor * -1);
                            }

                            command.ExecuteNonQuery();

                            transaction.Commit();
                        }
                    }
                    catch (DBConcurrencyException ex)
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
            }





        }
    }
}