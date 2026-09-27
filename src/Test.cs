using System;
using MySqlite;

class Test
{
    static void Main()
    {
        using (SqliteDatabase db = new SqliteDatabase("test.db"))
        {
            byte[] data = new byte[]
            {
                0x01,
                0x02,
                0x00,
                0xFF
            };

            using (SqliteStatement statement = db.Prepare("INSERT INTO blobs (data) VALUES (?)"))
            {
                statement.BindBlob(1, data);

                int result = statement.Step();

                Console.WriteLine(result);
            }
        }
    }
}

