using System;
using System.Collections.Generic;
using System.Data;
using Entity;
using Microsoft.Data.SqlClient;

namespace Data
{
    public class DCustomer
    {
        private readonly string _connectionString = DbConfig.ConnectionString;

        public List<Customer> Read()
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_BuscarClientes", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@name_contains", SqlDbType.NVarChar, 255) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 15) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@only_active", SqlDbType.Bit) { Value = DBNull.Value });

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader["customer_id"] is DBNull ? 0 : (int)reader["customer_id"],
                            Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString()!,
                            Address = reader["address"] is DBNull ? string.Empty : reader["address"].ToString()!,
                            Phone = reader["phone"] is DBNull ? string.Empty : reader["phone"].ToString()!,
                            Active = reader["active"] is DBNull ? false : (bool)reader["active"]
                        });
                    }
                }
            }
            return customers;
        }

        public void Create(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_InsertarCliente", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 255) { Value = customer.Name ?? string.Empty });
                command.Parameters.Add(new SqlParameter("@address", SqlDbType.NVarChar, 255) { Value = (object?)customer.Address ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 15) { Value = (object?)customer.Phone ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@active", SqlDbType.Bit) { Value = customer.Active });

                var outputId = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(outputId);

                connection.Open();
                command.ExecuteNonQuery();

                if (outputId.Value is int id)
                {
                    customer.CustomerId = id;
                }
            }
        }

        public void Update(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_ActualizarCliente", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.Int) { Value = customer.CustomerId });
                command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 255) { Value = customer.Name ?? string.Empty });
                command.Parameters.Add(new SqlParameter("@address", SqlDbType.NVarChar, 255) { Value = (object?)customer.Address ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 15) { Value = (object?)customer.Phone ?? DBNull.Value });
                command.Parameters.Add(new SqlParameter("@active", SqlDbType.Bit) { Value = customer.Active });

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_EliminarCliente", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.Int) { Value = customerId });
                command.Parameters.Add(new SqlParameter("@hard_delete", SqlDbType.Bit) { Value = 0 });

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Customer> Search(string searchTerm)
        {
            var customers = new List<Customer>();
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("sp_BuscarClientes", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@name_contains", SqlDbType.NVarChar, 255)
                {
                    Value = string.IsNullOrWhiteSpace(searchTerm) ? DBNull.Value : searchTerm
                });
                command.Parameters.Add(new SqlParameter("@phone", SqlDbType.NVarChar, 15) { Value = DBNull.Value });
                command.Parameters.Add(new SqlParameter("@only_active", SqlDbType.Bit) { Value = DBNull.Value });

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            CustomerId = reader["customer_id"] is DBNull ? 0 : (int)reader["customer_id"],
                            Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString()!,
                            Address = reader["address"] is DBNull ? string.Empty : reader["address"].ToString()!,
                            Phone = reader["phone"] is DBNull ? string.Empty : reader["phone"].ToString()!,
                            Active = reader["active"] is DBNull ? false : (bool)reader["active"]
                        });
                    }
                }
            }
            return customers;
        }
    }
}
