﻿using D2NG.BNCS.Hashing;
using System;
using System.Text;

namespace D2NG.BNCS.Packet
{
    public class RealmLogonRequestPacket : BncsPacket
    {
        public RealmLogonRequestPacket(
            uint clientToken,
            uint serverToken,
            string realmTitle,
            string password) :
            base(
                BuildPacket(
                    Sid.LOGONREALMEX,
                    BitConverter.GetBytes(clientToken),
                    Bsha1.DoubleHash(clientToken, serverToken, password),
                    Encoding.ASCII.GetBytes(realmTitle + "\0")
                )
            )
        {
        }
    }
}
