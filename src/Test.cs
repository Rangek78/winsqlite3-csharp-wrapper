using System;
using MySqlite;

class Test
{
    static void Main()
    {
        using (SqliteDatabase db = new SqliteDatabase("test.db"))
        {
            db.Execute("DROP TABLE IF EXISTS utf8_test");
            db.Execute("CREATE TABLE utf8_test (id INTEGER PRIMARY KEY, name TEXT, number REAL)");
            db.Execute("INSERT INTO utf8_test (name, number) VALUES ('João', 123.45)");
            db.Execute("INSERT INTO utf8_test (name, number) VALUES ('日本語', 456.78)");
            db.Execute("INSERT INTO utf8_test (name, number) VALUES ('😀', 999.99)");

            object[,] result = db.Query("SELECT * FROM utf8_test");

            for (int r = 0; r < result.GetLength(0); r++)
            {
                for (int c = 0; c < result.GetLength(1); c++)
                {
                    string name = Convert.ToString(result[r, c]);
                    Console.WriteLine("name = " + name);

                    for (int i = 0; i < name.Length; i++)
                    {
                        Console.WriteLine("U+" + ((int)name[i]).ToString("X4"));
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}

