﻿using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Tests
{
    [TestClass]
    public class ServerConnectionTests
    {
        [TestMethod]
        public void Can_receive_prelogin_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]> {FakePackets.GameServerList});

            var expectedPackets = new[] {new Packet(0xA8, FakePackets.GameServerList)};

            var connection = new ServerConnection();
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_received_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xE6}
            });


            var diagnosticStream = new DiagnosticPullStream();
            var connection = new ServerConnection(ServerConnectionStatus.Game, diagnosticStream,
                NullDiagnosticPushStream.Instance);
            connection.Receive(inputData);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0xB6, 0xA0, 0xFE, 0xE6")
                .And.Contain("0xB9, 0x80, 0x1F");
        }

        [TestMethod]
        public void Can_receive_one_game_packets()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xF9}
            });
            var expectedPackets = new[] {new Packet(0xB9, FakePackets.EnableLockedClientFeatures)};

            var connection = new ServerConnection(ServerConnectionStatus.Game, NullDiagnosticPullStream.Instance,
                NullDiagnosticPushStream.Instance);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_send_prelogin_packet()
        {
            var connection = new ServerConnection(ServerConnectionStatus.PreLogin);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.InitialLoginRequest), testStream);

            testStream.ActualBytes.Should().BeEquivalentTo(FakePackets.InitialLoginRequestEncrypted);
        }

        [TestMethod]
        public void Given_connection_in_Initial_status_When_sends_login_seed_Then_enters_PreLogin_status()
        {
            var connection = new ServerConnection(ServerConnectionStatus.Initial);
            connection.Send(FakePackets.InitialLoginSeedPacket, new TestMemoryStream());

            connection.Status.Should().Be(ServerConnectionStatus.PreLogin);
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_sent_PreLogin_packet()
        {
            var diagnosticStream = new DiagnosticPushStream("Proxy -> Server");

            var connection = new ServerConnection(ServerConnectionStatus.PreLogin, NullDiagnosticPullStream.Instance,
                diagnosticStream);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.InitialLoginRequest), testStream);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0x80, 0x61, 0x64, 0x6D, 0x69, 0x6E")
                .And.Contain("0x7A, 0x63, 0x9A, 0xED, 0x56, 0x0E");
        }

        [TestMethod]
        public void Can_send_game_packet()
        {
            var connection = new ServerConnection(ServerConnectionStatus.Game, NullDiagnosticPullStream.Instance,
                NullDiagnosticPushStream.Instance);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.GameServerLoginRequest), testStream);

            testStream.ActualBytes.Should().BeEquivalentTo(FakePackets.GameServerLoginRequestEncrypted);
        }

        [TestMethod]
        public void Can_receive_two_game_packets()
        {
            Assert.Inconclusive("Need capture more testing data");

            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xF9},
                new byte[]
                {
                    0xE6, 0x6E, 0x31, 0xDD, 0x0D, 0xBB, 0x1D, 0x02, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x23,
                    0x67, 0x14, 0xE3, 0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x23, 0x67,
                    0x14, 0xE3, 0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x23, 0x67, 0x14,
                    0xE3, 0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x23, 0x67, 0x14, 0xE3,
                    0x62, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x23, 0x67, 0x15, 0x7E, 0x73,
                    0x62, 0x0F, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x05, 0xCC, 0x26, 0xD0, 0x0D, 0x2B, 0xE5, 0xA5, 0x7A,
                    0x04, 0x87, 0xB3, 0x4D, 0x85, 0xDA, 0x45, 0xE5, 0x92, 0x66, 0x23, 0x59, 0x98, 0x60, 0xA5, 0x18, 0x08,
                    0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0x2A, 0xB2, 0x1F, 0x95, 0x14, 0xB0, 0xDD, 0x5F, 0xFF, 0x3A, 0x30,
                    0x56, 0xFE, 0x7C, 0x25, 0x68, 0x11, 0x76, 0x5E, 0x1A, 0xDD, 0x55, 0x01, 0x62, 0xDC, 0x60, 0x8C, 0xD6,
                    0xFF, 0xB1, 0x9A, 0xA6, 0x99, 0x44, 0x81, 0x02, 0x9E, 0xFD, 0x61, 0x8F, 0xB7, 0x7B, 0x03, 0xC6, 0xFE,
                    0x7C, 0x25, 0x73, 0x26, 0xAE, 0x1E, 0xC1, 0x86, 0x7E, 0x12, 0x1A, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x77,
                    0xED, 0x77, 0x68, 0x74, 0xA0, 0x83, 0x1F, 0x7E, 0x0A, 0x6F, 0x06, 0xB6, 0xBB, 0xC8, 0xDE, 0x7C, 0x25,
                    0x69, 0x70, 0xCC, 0xE1, 0x24, 0xEC, 0xA0, 0xFB, 0xB2, 0xDC, 0x60, 0x8C, 0xD6, 0xFE, 0x79, 0xC1, 0x66,
                    0x33, 0x3D, 0x25, 0xCB, 0xED, 0xE6, 0x36, 0x84, 0xC7, 0xE0, 0x61, 0xBD, 0xE5, 0xF3, 0x35, 0x69, 0x05,
                    0x90, 0xE2, 0x44, 0x20, 0x18, 0xFD, 0x53, 0x6C, 0x1A, 0x8C, 0xD6, 0xFE, 0x7C, 0x25, 0x69, 0xB9, 0x13,
                    0x80, 0xF7, 0xEB, 0x2B, 0xDE, 0x51, 0x86, 0xE7, 0x7D, 0xD9, 0x75, 0xC9, 0x8E, 0xB4, 0x14, 0xCC, 0x07,
                    0xD2, 0x67, 0x14, 0xD8, 0xD9, 0x7C, 0x19, 0xE6, 0x27, 0x7C, 0x7C, 0x25, 0x69, 0x05, 0x92, 0x66, 0x0C,
                    0x47, 0x6D, 0x56, 0x7B, 0x3A, 0x7C, 0xE3, 0x48, 0xC3, 0x5D, 0x96, 0xC4, 0x66, 0xE3, 0x84, 0x23, 0x67,
                    0x14, 0xEF, 0xDB, 0x77, 0x6E, 0x1C, 0x2C, 0xA5, 0x45, 0x2A, 0xCE, 0x95, 0x92, 0x66, 0x23, 0x67, 0x15,
                    0x9A, 0x61, 0x11, 0xD8, 0x72, 0x5B, 0xAF, 0xC2, 0x1D, 0x90, 0x83
                }
            });
            var expectedPackets = new[] {new Packet(0xB9, FakePackets.EnableLockedClientFeatures)};

            var connection = new ServerConnection();
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }
    }
}