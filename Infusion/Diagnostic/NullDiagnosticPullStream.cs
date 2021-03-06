﻿using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Diagnostic
{
    internal sealed class NullDiagnosticPullStream : IDiagnosticPullStream
    {
        public static NullDiagnosticPullStream Instance { get; } = new NullDiagnosticPullStream();

        public void Dispose()
        {
            BaseStream.Dispose();
        }

        public bool DataAvailable => BaseStream.DataAvailable;

        public int ReadByte()
        {
            return BaseStream.ReadByte();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public IPullStream BaseStream { get; set; }

        public void FinishPacket(Packet packet)
        {
        }

        public void StartPacket()
        {
        }
    }
}