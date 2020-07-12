﻿using D2NG.D2GS.Act;
using Serilog;
using System.IO;
using System.Text;
using D2NG.D2GS.Exceptions;

namespace D2NG.D2GS.Packet.Incoming
{
    internal class MapRevealPacket : D2gsPacket
    {
        public MapRevealPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if ((InComingPacket)id != InComingPacket.MapReveal)
            {
                throw new D2GSPacketException($"Invalid Packet Id {id}");
            }
            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();
            Area = (Area)reader.ReadByte();
            reader.Close();

            Log.Verbose($"(0x{ id,2:X2}) Map Reveal:\n" +
                $"\tLocation: ({X}, {Y})\n" +
                $"\tArea: {Area}");
        }

        public ushort X { get; }
        public ushort Y { get; }
        public Area Area { get; }
    }
}
