using System;

namespace MySqlite
{
    public sealed class SqliteException : Exception
    {
        public int ErrorCode { get; private set; }

        public SqliteException(int errorCode, string message)
            : base("SQLite error " + errorCode + ": " + message)
        {
            ErrorCode = errorCode;
        }
    }
}
