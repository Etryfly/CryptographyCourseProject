using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    [Serializable]
    public class Package
    {
        public static readonly int PackageDataLength = 128;

        public byte[] data = new byte[PackageDataLength];
        public Message message;

        
    }

    public enum Message
    {
        KEY, IV, FILE, END
    }
}
