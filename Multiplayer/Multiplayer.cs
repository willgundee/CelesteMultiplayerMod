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
using Monocle;
using Microsoft.Xna.Framework.Input;

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
        public static VirtualButton PlayerTwoDash;
        public static VirtualButton PlayerTwoJump;
        public static VirtualButton PlayerTwoGrab;
        public static VirtualIntegerAxis PlayerTwoMoveX;
        public static VirtualIntegerAxis PlayerTwoMoveY;
        public static VirtualJoystick PlayerTwoAim;

        public override Type SettingsType => typeof(MultiplayerSettings);
        public static MultiplayerSettings Settings => (MultiplayerSettings)Instance._Settings;

        // The methods we want to hook.
        private readonly static MethodInfo m_IntroRespawnEnd = typeof(Player).GetMethod("IntroRespawnEnd", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private readonly static MethodInfo m_NextLevel = typeof(Level).GetMethod("NextLevel", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        private readonly static MethodInfo m_Begin = typeof(Level).GetMethod("Begin", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // The inputs we want to hook.
        private readonly static MethodInfo m_GetDash = typeof(Input).GetProperty("Dash_Safe").GetMethod;
        private readonly static MethodInfo m_GetJump = typeof(Input).GetProperty("Jump_Safe").GetMethod;
        private readonly static MethodInfo m_GetGrab = typeof(Input).GetProperty("Grab_Safe").GetMethod;
        private readonly static MethodInfo m_GetMoveX = typeof(Input).GetProperty("MoveX_Safe").GetMethod;
        private readonly static MethodInfo m_GetMoveY = typeof(Input).GetProperty("MoveY_Safe").GetMethod;
        private readonly static MethodInfo m_GetAim = typeof(Input).GetProperty("Aim_Safe").GetMethod;

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

            orig_GetDash = m_GetDash.Detour<d_GetDash>(t_MultiplayerModule.GetMethod("GetDash"));
            orig_GetJump = m_GetJump.Detour<d_GetJump>(t_MultiplayerModule.GetMethod("GetJump"));
            orig_GetGrab = m_GetGrab.Detour<d_GetGrab>(t_MultiplayerModule.GetMethod("GetGrab"));
            orig_GetMoveX = m_GetMoveX.Detour<d_GetMoveX>(t_MultiplayerModule.GetMethod("GetMoveX"));
            orig_GetMoveY = m_GetMoveY.Detour<d_GetMoveY>(t_MultiplayerModule.GetMethod("GetMoveY"));
            orig_GetAim = m_GetAim.Detour<d_GetAim>(t_MultiplayerModule.GetMethod("GetAim"));
        }

        public override void Unload()
        {
            // Let's just hope that nothing else detoured this, as this is depth-based...
            RuntimeDetour.Undetour(m_IntroRespawnEnd);
            RuntimeDetour.Undetour(m_NextLevel);
            RuntimeDetour.Undetour(m_Begin);
            RuntimeDetour.Undetour(m_GetDash);
            RuntimeDetour.Undetour(m_GetJump);
            RuntimeDetour.Undetour(m_GetGrab);
            RuntimeDetour.Undetour(m_GetMoveX);
            RuntimeDetour.Undetour(m_GetMoveY);
            RuntimeDetour.Undetour(m_GetAim);
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
                PlayerTwoDash = new VirtualButton();
                PlayerTwoDash.Nodes.Add(new VirtualButton.KeyboardKey(Keys.J));
                PlayerTwoJump = new VirtualButton();
                PlayerTwoJump.Nodes.Add(new VirtualButton.KeyboardKey(Keys.K));
                PlayerTwoGrab = new VirtualButton();
                PlayerTwoGrab.Nodes.Add(new VirtualButton.KeyboardKey(Keys.H));
                PlayerTwoMoveX = new VirtualIntegerAxis();
                PlayerTwoMoveX.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad4, Keys.NumPad6));
                PlayerTwoMoveY = new VirtualIntegerAxis();
                PlayerTwoMoveY.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad8, Keys.NumPad5));
                PlayerTwoAim = new VirtualJoystick(true);
                PlayerTwoAim.Nodes.Add(new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad4, Keys.NumPad6, Keys.NumPad8, Keys.NumPad5));

                Player P1 = Celeste.Scene.Entities.FindFirst<Player>();
                PlayerTwo Madeline2 = new PlayerTwo(P1.Position, PlayerSpriteMode.MadelineNoBackpack)
                {
                    IntroType = PlayerTwo.IntroTypes.None
                };
                Celeste.Scene.Entities.Add(Madeline2);
            }

            orig_Begin(self);
        }

        public delegate VirtualButton d_GetDash();
        public static d_GetDash orig_GetDash;
        public static VirtualButton GetDash()
        {
            if (IsPlayerTwo)
                return PlayerTwoDash;
            return orig_GetDash();
        }

        public delegate VirtualButton d_GetJump();
        public static d_GetJump orig_GetJump;
        public static VirtualButton GetJump()
        {
            if (IsPlayerTwo)
                return PlayerTwoJump;
            return orig_GetJump();
        }

        public delegate VirtualButton d_GetGrab();
        public static d_GetGrab orig_GetGrab;
        public static VirtualButton GetGrab()
        {
            if (IsPlayerTwo)
                return PlayerTwoGrab;
            return orig_GetGrab();
        }

        public delegate VirtualIntegerAxis d_GetMoveX();
        public static d_GetMoveX orig_GetMoveX;
        public static VirtualIntegerAxis GetMoveX()
        {
            if (IsPlayerTwo)
                return PlayerTwoMoveX;
            return orig_GetMoveX();
        }

        public delegate VirtualIntegerAxis d_GetMoveY();
        public static d_GetMoveY orig_GetMoveY;
        public static VirtualIntegerAxis GetMoveY()
        {
            if (IsPlayerTwo)
                return PlayerTwoMoveY;
            return orig_GetMoveY();
        }

        public delegate VirtualJoystick d_GetAim();
        public static d_GetAim orig_GetAim;
        public static VirtualJoystick GetAim()
        {
            if (IsPlayerTwo)
                return PlayerTwoAim;
            return orig_GetAim();
        }

    }
}
