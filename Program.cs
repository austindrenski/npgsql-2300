using System;
using Npgsql;

// ReSharper disable PossibleNullReferenceException
namespace npgsql_2300
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var conn = new NpgsqlConnection($"Host=localhost;Port=5432;Username={args[0]};Password={args[1]};"))
            {
                conn.Open();
                var expected = TimeSpan.FromHours(24);

                using (var cmd = new NpgsqlCommand("DROP TABLE IF EXISTS some_table;", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("CREATE TABLE some_table (some_time TIME);", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("INSERT INTO some_table (some_time) VALUES (@p1);", conn))
                {
                    cmd.Parameters.AddWithValue("@p1", expected);
                    cmd.ExecuteNonQuery();
                }

                using (var cmd = new NpgsqlCommand("SELECT some_time FROM some_table;", conn))
                {
                    var value = (TimeSpan) cmd.ExecuteScalar();

                    if (expected == value)
                        Console.WriteLine("Completed successfully");

                    Console.WriteLine($"expected: {expected}");
                    Console.WriteLine($"received: {value}");
                }
            }
        }
    }
}