using Microsoft.Data.SqlClient;

namespace Data;

public static class DbConfig
{
    
    public static readonly string ConnectionString =
        "Server=Andre;Database=INVOICESDBA;Integrated Security=True;TrustServerCertificate=true;";
    public static void TestConnection()
    {
        using var conn = new SqlConnection(ConnectionString);
        conn.Open();
    }
}
