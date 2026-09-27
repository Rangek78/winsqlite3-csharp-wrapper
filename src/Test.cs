using System;
using MySqlite;

class Test
{
    static void Main()
    {
        using (SqliteDatabase db = new SqliteDatabase("test.db"))
        {
            using (SqliteStatement statement = db.Prepare("INSERT INTO people (name) VALUES ('Test')"))
            {
                int result = statement.Step();
                Console.WriteLine(result);
            }
        }
    }
}

