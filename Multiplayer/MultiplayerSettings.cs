using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Multiplayer
{
    [SettingName("Multiplayer-deline")] // We're lazy.
    public class MultiplayerSettings : EverestModuleSettings {

        public bool Enabled { get; set; } = false;

    }
}
