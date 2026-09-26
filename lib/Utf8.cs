using System;
using System.Text;
using System.Runtime.InteropServices;

namespace MySqlite
{
    internal static class Utf8
    {
        internal static string PtrToString(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                return null;

            int length = 0;

            while (Marshal.ReadByte(pointer, length) != 0)
                length++;

            byte[] bytes = new byte[length];
            Marshal.Copy(pointer, bytes, 0, length);

            return Encoding.UTF8.GetString(bytes);
        }

        internal static IntPtr GetPointer(string value, out GCHandle handle)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value + "\0");
            handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return handle.AddrOfPinnedObject();
        }
    }
}

