﻿using System;

namespace D2NG.Core.D2GS.Packet.Outgoing
{
    internal class RightSkillOnLocationPacket : D2gsPacket
    {
        public RightSkillOnLocationPacket(Point point) :
            base(
                BuildPacket(
                    (byte)OutGoingPacket.RightSkillOnLocation,
                    BitConverter.GetBytes(point.X),
                    BitConverter.GetBytes(point.Y)
                )
            )
        {
        }
    }
}
