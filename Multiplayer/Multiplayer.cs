using Celeste.Mod;
using Microsoft.Xna.Framework;
using MonoMod.Detour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Celeste.Mod.Multiplayer
{
    public class PlayerTwo : Player
    {
        public PlayerTwo(Vector2 position, PlayerSpriteMode spriteMode)
            : base(position, spriteMode)
        {
        }

        public override void Update()
        {
            Multiplayer.IsPlayerTwo = true;
            base.Update();
            Multiplayer.IsPlayerTwo = false;
        }

    }
    public class Multiplayer : EverestModule
    {
        public static Multiplayer Instance;
        public static bool IsPlayerTwo;

        public override Type SettingsType => typeof(MultiplayerSettings);
        public static MultiplayerSettings Settings => (MultiplayerSettings)Instance._Settings;

        // The methods we want to hook.
        private readonly static MethodInfo m_IntroRespawnEnd = typeof(Player).GetMethod("IntroRespawnEnd", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private readonly static MethodInfo m_NextLevel = typeof(Level).GetMethod("NextLevel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private readonly static MethodInfo m_Begin = typeof(Level).GetMethod("Begin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        public Multiplayer()
        {
            Instance = this;
        }

        public override void Load()
        {
            // Runtime hooks are quite different from static patches.
            Type t_MultiplayerModule = GetType();
            // [trampoline] = [method we want to hook] .Detour< [signature] >( [replacement method] );
            orig_IntroRespawnEnd = m_IntroRespawnEnd.Detour<d_IntroRespawnEnd>(t_MultiplayerModule.GetMethod("IntroRespawnEnd"));
            orig_NextLevel = m_NextLevel.Detour<d_NextLevel>(t_MultiplayerModule.GetMethod("NextLevel"));
            orig_Begin = m_Begin.Detour<d_Begin>(t_MultiplayerModule.GetMethod("Begin"));
        }

        public override void Unload()
        {
            // Let's just hope that nothing else detoured this, as this is depth-based...
            RuntimeDetour.Undetour(m_IntroRespawnEnd);
            RuntimeDetour.Undetour(m_NextLevel);
            RuntimeDetour.Undetour(m_Begin);
        }

        public delegate void d_IntroRespawnEnd(Player self);
        public static d_IntroRespawnEnd orig_IntroRespawnEnd;
        public static void IntroRespawnEnd(Player self)
        {

            if (Settings.Enabled)
            {
                PlayerTwo Madeline2 = new PlayerTwo(self.Position, PlayerSpriteMode.MadelineNoBackpack)
                {
                    IntroType = PlayerTwo.IntroTypes.None
                };
                Celeste.Scene.Entities.Add(Madeline2);
            }

            orig_IntroRespawnEnd(self);
        }

        public delegate void d_NextLevel(Level self, Vector2 at, Vector2 dir);
        public static d_NextLevel orig_NextLevel;
        public static void NextLevel(Level self, Vector2 at, Vector2 dir)
        {
            if (Settings.Enabled)
            {
                //Logger.Log("game at", at.X.ToString() + " | " + at.Y.ToString());
                //Logger.Log("game dir", dir.X.ToString() + " | " + dir.Y.ToString());
                float nearestValue = float.MaxValue;
                Vector2 nearestPlayerPosition = new Vector2();
                Vector2 nearestPlayerSpeed = new Vector2();
                Celeste.Scene.Entities.FindAll<Player>().ForEach(player =>
                {
                    float tempVal = Math.Abs(player.Position.X - at.X) + Math.Abs(player.Position.Y - at.Y);
                    if (player is PlayerTwo)
                    {
                        Logger.Log("P2 diff", tempVal.ToString());
                    }
                    else
                    {
                        Logger.Log("P1 diff", tempVal.ToString());
                    }
                    if (tempVal <= nearestValue)
                    {
                        nearestValue = tempVal;
                        nearestPlayerPosition = player.Position;
                        nearestPlayerSpeed = player.Speed;
                    }
                });
                Celeste.Scene.Entities.FindAll<Player>().ForEach(player => {
                    //Logger.Log("P at", player.Position.X.ToString() + " | " + player.Position.Y.ToString());
                    //Logger.Log("P dir", player.Speed.X.ToString() + " | " + player.Speed.Y.ToString());
                    player.Position = at;
                    player.Speed = nearestPlayerSpeed;
                });
            }

            orig_NextLevel(self, at, dir);
        }

        public delegate void d_Begin(Level self);
        public static d_Begin orig_Begin;
        public static void Begin(Level self)
        {
            if (Settings.Enabled)
            {
                Player P1 = Celeste.Scene.Entities.FindFirst<Player>();
                PlayerTwo Madeline2 = new PlayerTwo(P1.Position, PlayerSpriteMode.MadelineNoBackpack)
                {
                    IntroType = PlayerTwo.IntroTypes.None
                };
                Celeste.Scene.Entities.Add(Madeline2);
            }

            orig_Begin(self);
        }

    }
}
