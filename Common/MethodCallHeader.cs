using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MethodCallHeader
    {
        public uint Version;
        //public MethodCallOptions Options;
        public ulong InterfaceId;
        public ulong MethodId;

        public MethodCallHeader(uint version, ulong interfaceId, ulong methodId)
        {
            Version = version;
            //Options = options;
            InterfaceId = interfaceId;
            MethodId = methodId;
        }

        public static unsafe int SizeOf => sizeof(MethodCallHeader);
    }
}
