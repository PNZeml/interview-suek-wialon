namespace Suek.Iterview.Wialon;

public static class WialonProtocol {
    public static class Constants {
        /// <summary>
        /// Символ стартового бита и разделителя -- '#'
        /// </summary>
        public const byte Delimiter = 0x23;

        public static readonly byte[] LoginAckBytes = [ 0x41, 0x4C ];
        
        public static class PacketTypes {
            public const string Login = "L";

            public const string LoginAck = "AL";
        }
    }

    public static class Versions {
        public const string Ips11 = "1.1";

        public const string Ips21 = "2.1";
    }
}
