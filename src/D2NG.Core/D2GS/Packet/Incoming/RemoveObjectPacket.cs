﻿using D2NG.Core.D2GS.Exceptions;
using D2NG.Core.D2GS.Objects;
using System.IO;
using System.Text;

namespace D2NG.Core.D2GS.Packet.Incoming
{
    public class RemoveObjectPacket : D2gsPacket
    {
        public RemoveObjectPacket(D2gsPacket packet) : base(packet.Raw)
        {
            var reader = new BinaryReader(new MemoryStream(packet.Raw), Encoding.ASCII);
            var id = reader.ReadByte();
            if (InComingPacket.RemoveObject != (InComingPacket)id)
            {
                throw new D2GSPacketException($"Invalid Packet Id {id}");
            }
            EntityType = (EntityType)reader.ReadByte();
            EntityId = reader.ReadUInt32();
            reader.Close();
        }

        public EntityType EntityType { get; }
        public uint EntityId { get; }
    }
}
