﻿using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.IO;

namespace UltimaRX.Tests.IO
{
    [TestClass]
    public class HuffmanStreamTests
    {
        private readonly byte[] compressed =
        {
            0x81, 0x7A, 0xD2, 0xBF, 0xD1, 0xDB, 0x91, 0xD4, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x9D, 0x11, 0xBE, 0x6F, 0x22, 0xEA, 0x01, 0x8F, 0x88, 0x3E, 0x00, 0x00, 0x00, 0xE9, 0x8B, 0xAC,
            0x15, 0xE4, 0x0F, 0x36, 0xA3, 0xF0, 0x6C, 0x7A, 0x66, 0x40, 0xB6, 0x5B, 0x3F, 0x9F, 0xF2, 0xCE, 0x2B,
            0xA2, 0xA1, 0x4D, 0x00, 0x00, 0x03, 0xE8, 0xC8, 0x3C, 0x7C, 0x46, 0x9D, 0x50, 0x91, 0x3D, 0x58, 0x88,
            0x80, 0x00, 0x02, 0xC0, 0xCD, 0x0B, 0xE0, 0x77, 0xD0, 0xA8, 0x02, 0xF2, 0x07, 0x9B, 0x67, 0x3A, 0x43,
            0xE3, 0xD2, 0x35, 0xAB, 0xC8, 0xEC, 0x90, 0xBC, 0x00, 0x88, 0x25, 0x37, 0xA2, 0x2E, 0xB2, 0x7D, 0x4C,
            0xD1, 0x4E, 0x43, 0x83, 0x9B, 0xA4, 0x1E, 0x24, 0xED, 0x74, 0xB8, 0x69, 0x37, 0x66, 0x2C, 0x58, 0xAC,
            0x2E, 0x98, 0xBA, 0xCA, 0x0E, 0x8A, 0x24, 0x85, 0x34, 0x04, 0x24, 0x4F, 0x57, 0x9B, 0x7F, 0x9F, 0x39,
            0xAD, 0xCE, 0x43, 0xF3, 0xD3, 0x05, 0xDA, 0xD6, 0x37, 0x1E, 0x27, 0x50, 0x1D, 0xF6, 0xF7, 0xAB, 0x13,
            0x74, 0xE9, 0x8B, 0xAA, 0x45, 0xEA, 0x34, 0x46, 0x78, 0xF1, 0xC5, 0xC2, 0xD5, 0xE2, 0xF3, 0xA8, 0x1D,
            0x40, 0x42, 0x44, 0xF5, 0x62, 0x85, 0xB7, 0xBD, 0x58, 0x97, 0xCC, 0x0B, 0x20, 0xEF, 0xB7, 0xAF, 0x20,
            0x79, 0xB7, 0x9B, 0x1A, 0x81, 0xE1, 0xAB, 0xA4, 0x37, 0x6B, 0x58, 0xDC, 0x78, 0x9D, 0x5D, 0xF4, 0x91,
            0x12, 0x5A, 0x04, 0x09, 0x66, 0x22, 0x45, 0xF3, 0xAA, 0xF4, 0x38, 0x3C, 0x5D, 0xA0, 0xC6, 0x8A, 0x89,
            0xDF, 0x6F, 0x7A, 0xB1, 0x0E, 0x21, 0x2A, 0xC7, 0xB1, 0x88, 0x00, 0x12, 0x9C, 0x4A, 0x89, 0xDF, 0x5E,
            0x40, 0xF3, 0x6A, 0xF4, 0x3E, 0x88, 0xAF, 0x2D, 0x5E, 0x61, 0xB8, 0x0E, 0xD6, 0xB1, 0xB8, 0xF1, 0x1A,
            0xCE, 0xBA, 0x2E, 0xB9, 0xC7, 0x5D, 0x17, 0x56, 0xB3, 0xAE, 0x8B, 0xAD, 0x08, 0xCE, 0x8E, 0x18, 0x3C,
            0x63, 0x60, 0xF4, 0x31, 0x62, 0x58, 0xDC, 0x16, 0x30, 0xD6, 0x75, 0xD1, 0x75, 0x77, 0xEE, 0x18, 0xBA,
            0xB5, 0x9D, 0x74, 0x5D, 0x60, 0x5C, 0x71, 0x8C, 0x2F, 0x20, 0x79, 0xB5, 0x23, 0x0F, 0xCF, 0x4C, 0xD6,
            0xA1, 0xFC, 0x43, 0xE2, 0xED, 0x6A, 0xF7, 0x44, 0x57, 0x98, 0x7C, 0x4E, 0xAE, 0xBD, 0xD3, 0x17, 0x59,
            0x25, 0x0C, 0x8C, 0xEE, 0xEE, 0x81, 0xE5, 0xAB, 0xC6, 0x08, 0x64, 0x5C, 0xD8, 0xBA, 0xF9, 0x46, 0xA2,
            0x45, 0xF3, 0xBB, 0xA9, 0x61, 0x73, 0x62, 0xEB, 0x48, 0xE0, 0x09, 0xA0, 0xBC, 0x81, 0xE6, 0xD4, 0x67,
            0x98, 0x71, 0xBE, 0x78, 0xF4, 0x86, 0xCE, 0xB5, 0x8D, 0xC7, 0x89, 0x66, 0xC5, 0xD7, 0x1A, 0x54, 0xC6,
            0x17, 0x9C, 0xD8, 0xBA, 0xE3, 0xDB, 0x6C, 0xB9, 0xAB, 0x0E, 0x90, 0xFA, 0x5B, 0x39, 0x0F, 0xA7, 0x92,
            0xC8, 0x4A, 0x6D, 0xAC, 0x5D, 0x72, 0x96, 0x6C, 0x5D, 0x72, 0x6C, 0xD8, 0xBA, 0xCA, 0x6D, 0xAC, 0x5D,
            0x72, 0x8D, 0xAA, 0xF2, 0x07, 0x9B, 0x71, 0xFD, 0x1A, 0xA3, 0x7C, 0x71, 0xF3, 0x0D, 0xC0, 0x76, 0xB5,
            0x8D, 0xC7, 0x89, 0x8B, 0x16, 0x27, 0x2B, 0xA3, 0xC3, 0x25, 0x36, 0xD6, 0x2E, 0xB9, 0x4E, 0x09, 0x5F,
            0x97, 0x6B, 0x1F, 0x30, 0xC6, 0xD6, 0x2E, 0xB9, 0x36, 0x6C, 0x5D, 0x72, 0x6C, 0xD8, 0xBA, 0xA3, 0x3C,
            0xBB, 0x3B, 0x74, 0x70, 0xF9, 0x00, 0x05, 0xE4, 0x0F, 0x36, 0xB1, 0xA1, 0xF8, 0xE3, 0x7F, 0x8F, 0x1C,
            0x96, 0xB1, 0xB8, 0xF1, 0x00, 0x00, 0x00, 0x01, 0x89, 0x0F, 0xD1, 0x15, 0xE7, 0x88, 0x00, 0x00, 0x00,
            0x66, 0x7A, 0xA8, 0xC0, 0x00, 0x02, 0x72, 0x6C, 0x6A, 0xC1, 0xAB, 0xCC, 0x37, 0x01, 0xDA, 0xD4, 0x3C,
            0x1E, 0x4F, 0x3D, 0x2F, 0xE8, 0xCE, 0x00, 0x04, 0x25, 0x91, 0xC5, 0xA7, 0x61, 0x59, 0x11, 0x83, 0x9F,
            0x87, 0xD0, 0x8B, 0xA8, 0x05, 0x21, 0xB5, 0x8B, 0xAC, 0x91, 0x96, 0x35, 0x8C, 0x58, 0xB1, 0x62, 0x00,
            0x5D, 0x1E, 0x23, 0x11, 0x9D, 0x21, 0xF2, 0x78, 0xCC, 0x80, 0xE0, 0x3B, 0x5A, 0xBD, 0xE2, 0xFF, 0x30,
            0xC8, 0x9E, 0xBB, 0x6B, 0x17, 0x50, 0x00, 0x0C, 0x9B, 0x35, 0xE5, 0x0F, 0xD1, 0x15, 0xE7, 0x88, 0xDB,
            0x46, 0x14, 0x87, 0x36, 0x2E, 0xB2, 0x72, 0x05, 0x74, 0x52, 0x41, 0x2C, 0x2F, 0x83, 0x86, 0xD7, 0xB9,
            0x28, 0x90, 0xAD, 0x7B, 0x92, 0x89, 0x7C, 0xC0, 0x44, 0x52, 0x7C, 0xC3, 0xE9, 0xD3, 0x8B, 0x80, 0xED,
            0x6A, 0x33, 0xA7, 0x17, 0x68, 0x3E, 0x38, 0x99, 0x2D, 0xAC, 0x5D, 0x5D, 0xF4, 0x91, 0x12, 0x5A, 0x04,
            0x08, 0x05, 0x23, 0x40
        };

        private readonly byte[] decompressed =
        {
            0xA9, 0x04, 0x29, 0x01, 0x61, 0x73, 0x64, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x0C, 0x00, 0x59, 0x65, 0x77, 0x00, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x46, 0x02, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x02, 0x04, 0x69,
            0x54, 0x68, 0x65, 0x20, 0x45, 0x6D, 0x70, 0x61, 0x74, 0x68, 0x20, 0x41, 0x62, 0x62, 0x65, 0x79, 0x00,
            0x0F, 0x76, 0x58, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x4D, 0x69,
            0x6E, 0x6F, 0x63, 0x00, 0x04, 0x50, 0x02, 0xB7, 0x02, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x7B, 0x16, 0x6A, 0x08, 0x60, 0x3F, 0x68, 0x00, 0x54, 0x68, 0x65, 0x20, 0x42,
            0x61, 0x72, 0x6E, 0x61, 0x63, 0x6C, 0x65, 0x00, 0x73, 0x1B, 0x77, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
            0x00, 0x00, 0xC8, 0x9F, 0x02, 0x04, 0xF4, 0x0E, 0x16, 0x02, 0x42, 0x72, 0x69, 0x74, 0x61, 0x69, 0x6E,
            0x00, 0xD3, 0xB0, 0x67, 0x7A, 0xFE, 0xFF, 0xFF, 0xFF, 0x34, 0xA0, 0x02, 0x04, 0xD4, 0xF1, 0x4B, 0x00,
            0x58, 0x03, 0x00, 0x00, 0x50, 0x02, 0xB7, 0x53, 0x77, 0x65, 0x65, 0x74, 0x20, 0x44, 0x72, 0x65, 0x61,
            0x6D, 0x73, 0x20, 0x49, 0x6E, 0x6E, 0x00, 0x04, 0x00, 0x00, 0x00, 0x60, 0x22, 0xB7, 0x02, 0x7C, 0xA0,
            0x02, 0x04, 0x47, 0xE9, 0x03, 0x4D, 0x6F, 0x6F, 0x6E, 0x67, 0x6C, 0x6F, 0x77, 0x00, 0x04, 0x00, 0x00,
            0x00, 0x04, 0x00, 0x00, 0x00, 0x50, 0x02, 0xB7, 0x02, 0x6A, 0x22, 0xB7, 0x02, 0xD0, 0xEA, 0x51, 0x00,
            0x60, 0x22, 0x54, 0x68, 0x65, 0x20, 0x53, 0x63, 0x68, 0x6F, 0x6C, 0x61, 0x72, 0x73, 0x20, 0x49, 0x6E,
            0x6E, 0x00, 0x04, 0x60, 0xED, 0x4B, 0x00, 0x20, 0x00, 0x00, 0x00, 0xD8, 0xB9, 0x4B, 0x00, 0xD0, 0x04,
            0x54, 0x72, 0x69, 0x6E, 0x73, 0x69, 0x63, 0x00, 0x40, 0x02, 0x60, 0x22, 0xB7, 0x02, 0x00, 0x10, 0x00,
            0x00, 0xC0, 0x46, 0x49, 0x00, 0x31, 0x00, 0x00, 0x00, 0xC8, 0x02, 0x40, 0x02, 0x60, 0x54, 0x68, 0x65,
            0x20, 0x54, 0x72, 0x61, 0x76, 0x65, 0x6C, 0x65, 0x72, 0x27, 0x73, 0x20, 0x49, 0x6E, 0x6E, 0x00, 0x4C,
            0xA6, 0x02, 0x04, 0x44, 0xA6, 0x02, 0x04, 0x4C, 0xA6, 0x02, 0x04, 0x05, 0x4D, 0x61, 0x67, 0x69, 0x6E,
            0x63, 0x69, 0x61, 0x00, 0xFF, 0xFF, 0x89, 0x87, 0x49, 0x00, 0x4C, 0xA6, 0x02, 0x04, 0x60, 0xA5, 0x02,
            0x04, 0x4C, 0xA6, 0x02, 0x04, 0xA9, 0xCF, 0x49, 0x00, 0x54, 0x68, 0x65, 0x20, 0x47, 0x72, 0x65, 0x61,
            0x74, 0x20, 0x48, 0x6F, 0x72, 0x6E, 0x73, 0x20, 0x54, 0x61, 0x76, 0x65, 0x72, 0x6E, 0x00, 0x04, 0xFC,
            0xA0, 0x02, 0x04, 0xF6, 0x3A, 0x4D, 0x06, 0x4A, 0x68, 0x65, 0x6C, 0x6F, 0x6D, 0x00, 0x00, 0x18, 0xA1,
            0x02, 0x04, 0x65, 0x59, 0x4B, 0x00, 0xD0, 0x06, 0x52, 0x00, 0x34, 0xA1, 0x02, 0x04, 0x28, 0x08, 0x00,
            0x00, 0x75, 0x00, 0x00, 0x54, 0x68, 0x65, 0x20, 0x4D, 0x65, 0x72, 0x63, 0x65, 0x6E, 0x61, 0x72, 0x79,
            0x20, 0x49, 0x6E, 0x6E, 0x00, 0xAC, 0x02, 0x04, 0x1F, 0xC0, 0x49, 0x00, 0x54, 0xA1, 0x02, 0x04, 0x46,
            0x3C, 0x07, 0x53, 0x6B, 0x61, 0x72, 0x61, 0x20, 0x42, 0x72, 0x61, 0x65, 0x00, 0x51, 0x00, 0xC8, 0xA2,
            0x02, 0x04, 0x80, 0xAC, 0x02, 0x04, 0x78, 0xAC, 0x02, 0x04, 0xC8, 0xA2, 0x02, 0x04, 0x80, 0xA2, 0x54,
            0x68, 0x65, 0x20, 0x46, 0x61, 0x6C, 0x63, 0x6F, 0x6E, 0x65, 0x72, 0x27, 0x73, 0x20, 0x49, 0x6E, 0x6E,
            0x00, 0xFF, 0xFF, 0xFF, 0x7F, 0xF1, 0x43, 0x00, 0xC8, 0xA2, 0x02, 0x04, 0x80, 0x08, 0x56, 0x65, 0x73,
            0x70, 0x65, 0x72, 0x00, 0xA2, 0x02, 0x04, 0x78, 0xAC, 0x02, 0x04, 0x78, 0xAC, 0x02, 0x04, 0x4D, 0x65,
            0x73, 0x73, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x54, 0x68, 0x65, 0x20, 0x49, 0x72,
            0x6F, 0x6E, 0x77, 0x6F, 0x6F, 0x64, 0x20, 0x49, 0x6E, 0x6E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x48, 0x61, 0x76, 0x65, 0x6E, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x74, 0xB7, 0x4D, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x75, 0x63, 0x6B, 0x6C, 0x65, 0x72, 0x27, 0x73, 0x20, 0x48,
            0x69, 0x64, 0x65, 0x61, 0x77, 0x61, 0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x50, 0x51, 0x10, 0x03,
            0x06, 0x00, 0x00, 0x0A, 0x55, 0x6D, 0x62, 0x72, 0x61, 0x00, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x28,
            0xA2, 0x02, 0x04, 0xAB, 0xC9, 0x4C, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x90, 0xB5,
            0x4D, 0x4D, 0x61, 0x72, 0x64, 0x6F, 0x74, 0x68, 0x27, 0x73, 0x20, 0x54, 0x6F, 0x77, 0x65, 0x72, 0x00,
            0x02, 0xC4, 0xA2, 0x02, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x78, 0xAC, 0x0B, 0x48,
            0x61, 0x76, 0x65, 0x6E, 0x00, 0x3C, 0x4D, 0x00, 0x28, 0xA1, 0x02, 0x04, 0x8F, 0x68, 0x0F, 0x76, 0x1B,
            0x14, 0x6A, 0x08, 0x43, 0xF2, 0xB6, 0x02, 0x3F, 0xF2, 0xB6, 0x02, 0xD0, 0xEA, 0x55, 0x7A, 0x65, 0x72,
            0x61, 0x61, 0x6E, 0x27, 0x73, 0x20, 0x4D, 0x61, 0x6E, 0x73, 0x69, 0x6F, 0x6E, 0x00, 0x78, 0xA2, 0x02,
            0x04, 0x60, 0xED, 0x4B, 0x00, 0x20, 0x00, 0x00, 0x00, 0xD8, 0x00, 0x00, 0x00, 0x28
        };

        [TestMethod]
        public void Can_compress_block()
        {
            var baseStream = new MemoryStream(decompressed);
            var huffmanStream = new HuffmanStream(baseStream);

            var actualCompressed = new byte[compressed.Length];
            huffmanStream.Write(actualCompressed, 0, decompressed.Length);

            actualCompressed.Should().BeEquivalentTo(compressed);
        }

        [TestMethod]
        public void Can_decompress_using_ReadByte()
        {
            var baseStream = new MemoryStream(compressed);
            var huffmanStream = new HuffmanStream(baseStream);

            byte[] actualDecompressed = new byte[decompressed.Length];

            int actualDecompressedLength = 0;
            int b;
            while ((b = huffmanStream.ReadByte()) >= 0)
            {
                actualDecompressed[actualDecompressedLength] = (byte)b;
                actualDecompressedLength++;
            }

            actualDecompressedLength.Should().Be(decompressed.Length);
            actualDecompressed.Should().BeEquivalentTo(decompressed);
        }

        [TestMethod]
        public void Can_decompress_block()
        {
            var baseStream = new MemoryStream(compressed);
            var huffmanStream = new HuffmanStream(baseStream);

            byte[] actualDecompressed = new byte[decompressed.Length];
            int actualDecompressedLength = huffmanStream.Read(actualDecompressed, 0, decompressed.Length - 1);

            actualDecompressedLength.Should().Be(decompressed.Length);
            actualDecompressed.Should().BeEquivalentTo(decompressed);
        }
    }
}