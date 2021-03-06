﻿using CommandLine;
using System;

namespace Infusion.Cli
{
    [Verb("hl", HelpText = "Starts a headless Infusion client.")]
    internal sealed class LaunchHeadlessOptions
    {
        [Option("pipe", Default = "infusionPipe", HelpText = "Pipe name to listen for commands.")]
        public string PipeName { get; set; }

        [Option("server", Required = true, HelpText = "Ultima Online server address.")]
        public string ServerAddress { get; set; }

        [Option("serverPort", Default = 2593, HelpText = "Ultima Online server port number.")]
        public int ServerPort { get; set; }

        [Option("proxyPort", Default = 60000, HelpText = "Infusion proxy port number.")]
        public int ProxyPort { get; set; }

        [Option("encrypt", Default = false, HelpText = "Turns on client encryption.")]
        public bool Encrypt { get; set; }

        [Option("clientVersion", Required = true, HelpText = "Client version, determines client behavior, protocol and encryption (if used).")]
        public Version ClientVersion { get; set; }

        [Option("account", Required = true, HelpText = "Ultima Online account name.")]
        public string AccountName { get; set; }

        [Option("password", Required = true, HelpText = "Ultima Online account password.")]
        public string AccountPassword { get; set; }

        [Option("shard", Required = true, HelpText = "Ultima Online shard name.")]
        public string ShardName { get; set; }

        [Option("char", Required = true, HelpText = "Ultima Online character name.")]
        public string CharacterName { get; set; }

        [Option("script", Required = false, HelpText = "Initial script file name.")]
        public string ScriptFileName { get; set; }
    }
}
