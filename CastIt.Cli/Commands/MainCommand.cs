﻿using CastIt.Cli.Commands.Player;
using CastIt.Cli.Commands.PlayLists;
using CastIt.Cli.Interfaces.Api;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CastIt.Cli.Commands
{
    [Command(Name = "castit", OptionsComparison = StringComparison.InvariantCultureIgnoreCase)]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    [Subcommand(typeof(PlayerCommands), typeof(PlayListCommands))]
    public class MainCommand : BaseCommand
    {
        public MainCommand(IConsole appConsole, ICastItApiService castItApi) 
            : base(appConsole, castItApi)
        {
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
            return base.OnExecute(app);
        }

        private static string GetVersion()
            => typeof(MainCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }
}
