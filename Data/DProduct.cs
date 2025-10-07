using System;
using System.Collections.Generic;
using System.Data;
using Entity;
using Microsoft.Data.SqlClient;

namespace Data
{
    public class DProduct
    {
        private readonly string _connectionString = DbConfig.ConnectionString;

        public List<Product> Read(bool? onlyActive = null)
        {
            var products = new List<Product>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ListarProductos", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@only_active", SqlDbType.Bit)
            {
                Value = onlyActive.HasValue ? onlyActive.Value : DBNull.Value
            });

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                products.Add(new Product
                {
                    ProductId = reader["product_id"] is DBNull ? 0 : Convert.ToInt32(reader["product_id"]),
                    Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString()!,
                    Price = reader["price"] is DBNull ? 0m : Convert.ToDecimal(reader["price"]),
                    Stock = reader["stock"] is DBNull ? 0 : Convert.ToInt32(reader["stock"]),
                    Active = reader["active"] is DBNull ? false : Convert.ToBoolean(reader["active"])
                });
            }
            return products;
        }

        public Product? GetById(int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ObtenerProducto", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@product_id", SqlDbType.Int) { Value = productId });
            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Product
                {
                    ProductId = reader["product_id"] is DBNull ? 0 : Convert.ToInt32(reader["product_id"]),
                    Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString()!,
                    Price = reader["price"] is DBNull ? 0m : Convert.ToDecimal(reader["price"]),
                    Stock = reader["stock"] is DBNull ? 0 : Convert.ToInt32(reader["stock"]),
                    Active = reader["active"] is DBNull ? false : Convert.ToBoolean(reader["active"])
                };
            }
            return null;
        }

        public void Create(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_InsertProduct", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 255) { Value = product.Name ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = product.Price });
            command.Parameters.Add(new SqlParameter("@stock", SqlDbType.Int) { Value = product.Stock });
            command.Parameters.Add(new SqlParameter("@active", SqlDbType.Bit) { Value = product.Active });

            connection.Open();
            var result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                product.ProductId = Convert.ToInt32(result);
            }
        }

        public void Update(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UpdateProduct", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@product_id", SqlDbType.Int) { Value = product.ProductId });
            command.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 255) { Value = product.Name ?? string.Empty });
            command.Parameters.Add(new SqlParameter("@price", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = product.Price });
            command.Parameters.Add(new SqlParameter("@stock", SqlDbType.Int) { Value = product.Stock });
            command.Parameters.Add(new SqlParameter("@active", SqlDbType.Bit) { Value = product.Active });

            connection.Open();
            command.ExecuteNonQuery();
        }

        public int Delete(int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_EliminarProducto", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@product_id", SqlDbType.Int) { Value = productId });

            connection.Open();
            var result = command.ExecuteScalar();
            return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
        }

        public int HardDelete(int productId, bool cascade = false)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("dbo.sp_EliminarProductoHard", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@product_id", SqlDbType.Int) { Value = productId });
            command.Parameters.Add(new SqlParameter("@cascade", SqlDbType.Bit) { Value = cascade });

            connection.Open();
            var result = command.ExecuteScalar();
            return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
        }

        public void Reactivate(int productId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_ReactivarProducto", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@product_id", SqlDbType.Int) { Value = productId });

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
