using System;
using MySqlite;

class Test
{
    static void Main()
    {
        using (SqliteDatabase db = new SqliteDatabase("test.db"))
        {
            db.Execute("CREATE TABLE IF NOT EXISTS blobs (id INTEGER PRIMARY KEY, data BLOB)");
            db.Execute("INSERT INTO blobs (data) VALUES (X'01020300FF')");

            object[,] result = db.Query("SELECT id, data FROM blobs");

            byte[] data = (byte[])result[1, 1];

            for (int i = 0; i < data.Length; i++)
                Console.Write(data[i].ToString("X2") + " ");
        }
    }
}

