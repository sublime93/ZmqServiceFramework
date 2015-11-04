using System.Runtime.InteropServices;

namespace Common
{

    /// <summary>
    /// Method call header from Jakesays
    /// </summary>
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
