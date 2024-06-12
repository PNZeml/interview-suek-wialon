namespace Suek.Interview.Wialon.WialonProtocols;

public static class WialonProtocol {
    public static class Constants {
        /// <summary>
        /// Символ стартового бита и разделителя -- '#'
        /// </summary>
        public const byte Delimiter = 0x23;

        public static readonly byte[] PacketEnd = [0x0D,  0x0A];

        public static class Acks {
            public static class Login {
                public static readonly byte[] Success = [0x31];

                public const string Rejected = "0";

                public const string WrongCredentials = "01";
            }

            public static class Data {
                public const string Invalid = "-1";

                public const string WrongTime = "0";

                public const string Success = "1";
            }
        }
        
        public static class PacketTypes {
            public const string Login = "L";

            public const string LoginAck = "AL";

            public const string Message = "M";

            public const string MessageAck = "AM";

            public static readonly byte[] LoginAckBytes = [ 0x41, 0x4C ];
        }
    }

    public static class Versions {
        public const string Ips11 = "1.1";

        public const string Ips21 = "2.1";
    }
}
