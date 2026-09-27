using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MySqlite
{
    public sealed class SqliteStatement : IDisposable
    {
        private IntPtr _database;
        private IntPtr _handle;

        public SqliteStatement(IntPtr database, string sql)
        {
            _database = database;
            GCHandle handle;
            IntPtr sqlPointer = Utf8.GetPointer(sql, out handle);

            try
            {
                int result = NativeSqlite.sqlite3_prepare_v2(_database, sqlPointer, -1, out _handle, IntPtr.Zero);

                if (result != NativeSqlite.SQLITE_OK)
                    ThrowDbException(result);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BindText(int parameter, string value)
        {
            GCHandle handle;
            IntPtr valuePointer = Utf8.GetPointer(value, out handle);

            try
            {
                int result = NativeSqlite.sqlite3_bind_text(_handle, parameter, valuePointer, -1, NativeSqlite.SQLITE_TRANSIENT);

                if (result != NativeSqlite.SQLITE_OK)
                    ThrowDbException(result);
            }
            finally
            {
                handle.Free();
            }
        }

        public void BindBlob(int parameter, byte[] value)
        {
            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            try
            {
                int result = NativeSqlite.sqlite3_bind_blob(_handle, parameter, handle.AddrOfPinnedObject(), value.Length, NativeSqlite.SQLITE_TRANSIENT);

                if (result != NativeSqlite.SQLITE_OK)
                    ThrowDbException(result);
            }
            finally
            {
                handle.Free();
            }
        }

        public int Step()
        {
            return NativeSqlite.sqlite3_step(_handle);
        }

        internal Tuple<List<SqliteRow>, List<string>> ReadRows()
        {
            List<SqliteRow> rows = new List<SqliteRow>();
            int columnCount = NativeSqlite.sqlite3_column_count(_handle);
            List<string> columnNames = new List<string>();

            for (int i = 0; i < columnCount; i++)
            {
                string name = Utf8.PtrToString(NativeSqlite.sqlite3_column_name(_handle, i));
                columnNames.Add(name);
            }

            while (true)
            {
                int result = Step();

                if (result == NativeSqlite.SQLITE_DONE)
                    break;

                if (result != NativeSqlite.SQLITE_ROW)
                    ThrowDbException(result);

                rows.Add(ReadRow(columnCount));
            }

            return Tuple.Create(rows, columnNames);
        }

        private SqliteRow ReadRow(int columnCount)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            for (int i = 0; i < columnCount; i++)
            {
                string name = Utf8.PtrToString(NativeSqlite.sqlite3_column_name(_handle, i));
                object value = ReadValue(i);

                values[name] = value;
            }
            return new SqliteRow(values);
        }

        private object ReadValue(int column)
        {
            int type = NativeSqlite.sqlite3_column_type(_handle, column);

            switch (type)
            {
                case NativeSqlite.SQLITE_INTEGER:
                    return NativeSqlite.sqlite3_column_int64(_handle, column);

                case NativeSqlite.SQLITE_FLOAT:
                    return NativeSqlite.sqlite3_column_double(_handle, column);

                case NativeSqlite.SQLITE_TEXT:
                    IntPtr textPointer = NativeSqlite.sqlite3_column_text(_handle, column);
                    return Utf8.PtrToString(textPointer);

                case NativeSqlite.SQLITE_BLOB:
                    IntPtr blobPointer = NativeSqlite.sqlite3_column_blob(_handle, column);
                    int blobSize = NativeSqlite.sqlite3_column_bytes(_handle, column);

                    byte[] blob = new byte[blobSize];

                    if (blobSize > 0)
                        Marshal.Copy(blobPointer, blob, 0, blobSize);

                    return blob;

                case NativeSqlite.SQLITE_NULL:
                    return null;

                default:
                    return null;
            }
        }

        private void ThrowDbException(int result)
        {
            string error = GetErrorMessage();
            throw new SqliteException(result, error);
        }

        private string GetErrorMessage()
        {
            IntPtr message = NativeSqlite.sqlite3_errmsg(_database);
            return Utf8.PtrToString(message);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                NativeSqlite.sqlite3_finalize(_handle);
                _handle = IntPtr.Zero;
            }
        }
    }
}
