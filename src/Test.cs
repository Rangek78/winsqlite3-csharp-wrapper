using System;
using MySqlite;

class Test
{
    static void Main()
    {
        using (SqliteDatabase db = new SqliteDatabase("test.db"))
        {
            db.Execute("DROP TABLE IF EXISTS test");
            db.Execute("CREATE TABLE test (integer_value INTEGER, real_value REAL, text_value TEXT, blob_value BLOB, null_value ANY) STRICT");
            using (SqliteStatement statement = db.Prepare(
                        "INSERT INTO test (text_value, integer_value, real_value, blob_value, null_value) " +
                        "VALUES (?, ?, ?, ?, ?)"))
            {
                statement.BindValue(1, "Alice");
                statement.BindValue(2, 123L);
                statement.BindValue(3, 123.45);
                statement.BindValue(4, new byte[] { 1, 2, 3 });
                statement.BindValue(5, null);

                int result = statement.Step();

                Console.WriteLine(result);
            }
        }
    }
}

