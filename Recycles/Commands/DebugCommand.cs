using Rocket.API;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeathBox.Commands
{
    public class DebugCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Console;

        public string Name => "Debug";

        public string Help => "";

        public string Syntax => "/Debug";

        public List<string> Aliases => new List<string> { "Debug"};

        public List<string> Permissions => new List<string> { "Debug"};

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Main.DebugMode = !Main.DebugMode;
            Logger.Log($"[DEBUG MODE] DEBUG STATE HAS BEEN CHANGED TO {Main.DebugMode}!");
        }
    }
}
