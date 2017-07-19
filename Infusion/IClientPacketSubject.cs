﻿using System;
using Infusion.Packets;

namespace Infusion
{
    public interface IClientPacketSubject
    {
        void RegisterFilter(Func<Packet, Packet?> filter);

        void Subscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket;

        void Unsubscribe<TPacket>(PacketDefinition<TPacket> definition, Action<TPacket> observer)
            where TPacket : MaterializedPacket;
    }
}