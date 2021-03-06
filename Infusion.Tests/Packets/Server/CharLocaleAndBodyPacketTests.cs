﻿using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class CharLocaleAndBodyPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x1B, // packet
                0x00, 0x00, 0x00, 0x01, // player id
                0x00, 0x00, 0x00, 0x00, // unknown
                0x01, 0x90, // body type
                0x12, 0x97, // xloc
                0x05, 0x46, // yloc
                0x00, // unknown
                0x0A, // zloc
                0x03, // facing
                0x00, 0x00, 0x7F, 0x00, // unknown
                0x00, 0x06, 0x10, 0x00, // unknown
                0x00, // unknown
                0x04, 0x70, // server boundary width - 8
                0x05, 0x00, // server boundary height
                0x00, 0x00, // unknown
                0x00, 0x00, 0x00, 0x00, // unknown
            });

            var packet = new CharLocaleAndBodyPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(new ObjectId(0x00000001));
            packet.BodyType.Should().Be((ModelId)0x0190);
            packet.Location.X.Should().Be(0x1297);
            packet.Location.Y.Should().Be(0x0546);
            packet.Location.Z.Should().Be(0x0A);
            packet.Direction.Should().Be(Direction.Southeast);
            packet.MovementType.Should().Be(MovementType.Walk);
        }

        [TestMethod]
        public void Can_deserialize_packet_with_negative_z_coord()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x1B, // packet
                0x00, 0x00, 0x00, 0x01, // player id
                0x00, 0x00, 0x00, 0x00, // unknown
                0x01, 0x90, // body type
                0x12, 0x97, // xloc
                0x05, 0x46, // yloc
                0x00, // unknown
                0xF0, // zloc
                0x03, // facing
                0x00, 0x00, 0x7F, 0x00, // unknown
                0x00, 0x06, 0x10, 0x00, // unknown
                0x00, // unknown
                0x04, 0x70, // server boundary width - 8
                0x05, 0x00, // server boundary height
                0x00, 0x00, // unknown
                0x00, 0x00, 0x00, 0x00, // unknown
            });

            var packet = new CharLocaleAndBodyPacket();
            packet.Deserialize(rawPacket);

            unchecked
            {
                packet.Location.Z.Should().Be((sbyte)0xF0);
            }
        }
    }
}
