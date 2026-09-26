using System;
using System.Collections.Generic;

namespace MySqlite
{
    public sealed class SqliteRow
    {
        private readonly Dictionary<string, object> _values;

        internal SqliteRow(Dictionary<string, object> values)
        {
            _values = values;
        }

        public object this[string columnName]
        {
            get
            {
                return _values[columnName];
            }
        }
    }
}
