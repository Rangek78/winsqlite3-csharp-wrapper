using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MySqlite
{
    public sealed class SqliteDatabase : IDisposable
    {
        private IntPtr _handle;

        public SqliteDatabase(string filename)
        {
            int result = NativeSqlite.sqlite3_open(filename, out _handle);

            if (result != 0)
            {
                string error = GetErrorMessage();
                if (_handle != IntPtr.Zero)
                {
                    NativeSqlite.sqlite3_close(_handle);
                    _handle = IntPtr.Zero;
                }
                throw new Exception("SQLite error " + result + ": " + error);
            }
        }

        public void Execute(string sql)
        {
            GCHandle handle;
            IntPtr sqlPointer = Utf8.GetPointer(sql, out handle);

            try
            {
                int result = NativeSqlite.sqlite3_exec(_handle, sqlPointer, null, IntPtr.Zero, IntPtr.Zero);
                if (result != 0)
                {
                    ThrowDbException(result);
                }
            }
            finally
            {
                handle.Free();
            }
        }

        public object[,] Query(string sql)
        {
            using (SqliteStatement statement = new SqliteStatement(_handle, sql))
            {
                Tuple<List<SqliteRow>, List<string>> queryTuple = statement.ReadRows();
                return To2DArray(queryTuple.Item1, queryTuple.Item2);
            }
        }

        public object[,] To2DArray(List<SqliteRow> rows, List<string> columnNames)
        {
            int rowCount = rows.Count + 1;
            int columnCount = columnNames.Count;
            object[,] grid = new object[rowCount, columnCount];
            for (int c = 0; c < columnCount; c++)
            {
                grid[0, c] = columnNames[c];
            }
            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < columnCount; c++)
                {
                    string colName = columnNames[c];
                    grid[r + 1, c] = rows[r][colName];
                }
            }
            return grid;
        }

        public SqliteStatement Prepare(string sql)
        {
            return new SqliteStatement(_handle, sql);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                NativeSqlite.sqlite3_close(_handle);
                _handle = IntPtr.Zero;
            }
        }

        private void ThrowDbException(int result)
        {
            string error = GetErrorMessage();
            throw new SqliteException(result, error);
        }

        private string GetErrorMessage()
        {
            if (_handle == IntPtr.Zero)
                return "Unknown SQLite error.";

            IntPtr message = NativeSqlite.sqlite3_errmsg(_handle);

            return Utf8.PtrToString(message);
        }
    }
}
