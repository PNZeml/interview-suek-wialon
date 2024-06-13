using System.Text;

namespace Suek.Interview.Wialon.WialonProtocols;

public static class WialonIpsProtocol {
    public static class Constants {
        /// <summary>
        /// Признак начала пакета и разделитель заголовка и полезной нагрузки.
        /// </summary>
        public const byte PacketStart = 0x23;

        /// <summary>
        /// Признак конца пкета.
        /// </summary>
        public static readonly byte[] PacketEnd = [0x0D, 0x0A];
    }

    /// <summary>
    /// Определение пакетов из спецификации протокола.
    /// </summary>
    public static class Packets {
        public static readonly Definition Login = new("L", new("AL")) {
            AckCodes = new() {
                ["Success"]    = "1"u8.ToArray(),
                ["Rejected"]   = "0"u8.ToArray(),
                ["WrongCreds"] = "01"u8.ToArray()
            }
        };

        public static readonly Definition Message = new("M", new("AM"));

        public static readonly Definition ShortData = new("SD", new("ASD"));

        public static Definition ByName(string packetTypeName) {
            return packetTypeName switch {
                "L"   => Login,
                "AL"  => Login.Ack!,
                "M"   => Message,
                "AM"  => Message.Ack!,
                "SD"  => ShortData,
                "ASD" => ShortData.Ack!,
                _     => throw new ArgumentOutOfRangeException(nameof(packetTypeName), packetTypeName, null)
            };
        }

        public record Definition(string Name, Definition? Ack = default) {
            public Dictionary<string, byte[]> AckCodes { get; init; } = new();

            public byte[] Bytes => Encoding.UTF8.GetBytes(Name);
        }
    }

    public static class Version {
        public const string Ips1d1 = "1.1";

        public const string Ips2d1 = "2.1";
    }
}
