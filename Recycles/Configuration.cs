using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Framework;
using SDG.NetTransport;
using SDG.NetTransport.Loopback;
using SDG.NetTransport.SteamNetworking;
using SDG.NetTransport.SteamNetworkingSockets;
using SDG.NetTransport.SystemSockets;
using SDG.Provider.Services;
using SDG.Provider.Services.Achievements;
using SDG.Provider.Services.Browser;
using SDG.Provider.Services.Cloud;
using SDG.Provider.Services.Community;
using SDG.Provider.Services.Economy;
using SDG.Provider.Services.Matchmaking;
using SDG.Provider.Services.Multiplayer;
using SDG.Provider.Services.Statistics;
using SDG.Provider.Services.Store;
using SDG.Provider.Services.Translation;
using SDG.Provider.Services.Workshop;
using SDG.SteamworksProvider.Services;
using SDG.Unturned;
using SDG.Framework.Utilities;
using SDG.Framework.Water;
using SDG.Framework.UI;
using SDG.Framework.Translations;
using SDG.Framework.Rendering;
using SDG.Framework.Modules;
using SDG.Framework.Landscapes;
using SDG.Framework.IO;
using SDG.Framework.Foliage;
using SDG.Framework.Devkit;
using SDG.Framework.Debug;
using SDG.Framework.Debug.Parsers;
using SDG.Framework.Devkit.Interactable;
using SDG.Framework.Devkit.Tools;
using SDG.Framework.Devkit.Transactions;
using SDG.Framework.Devkit.Visibility;
using SDG.Framework.IO.Deserialization;
using SDG.Framework.UI.Components;
using SDG.Framework.UI.Devkit;
using SDG.Framework.UI.Sleek2;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Commands;
using Rocket.Unturned.Effects;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Items;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using Rocket.Unturned.Serialisation;
using Rocket.Unturned.Skills;
using Rocket.Unturned;
using Rocket.Core.Assets;
using Rocket.Core.Commands;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.Permissions;
using Rocket.Core.Plugins;
using Rocket.Core.Serialization;
using Rocket.Core.Steam;
using Rocket.Core.Utils;
using Rocket.Core;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Extensions;
using Rocket.API.Serialisation;
using Steamworks;
using AOT;
using Unity;
using UnityEditor;
using UnityEngine;
using UnityEngineInternal;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using Random = System.Random;
using Logger = Rocket.Core.Logging.Logger;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace DeathBox
{
    public class Configuration : IRocketPluginConfiguration
    {
        public byte InitialDeathBoxHSize { get; set; }
        public ushort DeathBoxID { get; set; }

        public bool CanDamageDeathBox { get; set; }
        public int DisappearCooldownAfterShutdown { get; set; }
        public int NormalDisappearCooldown { get; set; }
        public bool PunchUtil { get; set; }
        public bool PunchUtil_DropWhenItemsDoesntFit { get; set; }
        public void LoadDefaults()
        {
            DeathBoxID = 366;
            InitialDeathBoxHSize = 10;
            CanDamageDeathBox = false;
            DisappearCooldownAfterShutdown = 40;
            NormalDisappearCooldown = 10;
            PunchUtil = true;
            PunchUtil_DropWhenItemsDoesntFit = true;
        }
    }
}
