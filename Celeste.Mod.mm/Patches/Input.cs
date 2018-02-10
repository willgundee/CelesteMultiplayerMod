#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Celeste.Mod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static Monocle.VirtualButton;

namespace Celeste {
    public static class patch_Input
    {

        public static VirtualButton dash;
        public static VirtualButton dash2;
        [MonoModHook("Monocle.VirtualButton Celeste.Input::Dash")]
        public static VirtualButton Dash_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return dash2;
                }
                else
                {
                    return dash;
                }
            }
            set
            {
                dash = value;
                dash2 = new VirtualButton();
                dash2.Nodes.Add(new VirtualButton.KeyboardKey(Keys.J));
            }
        }
        public static VirtualButton jump;
        public static VirtualButton jump2;
        [MonoModHook("Monocle.VirtualButton Celeste.Input::Jump")]
        public static VirtualButton Jump_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return jump2;
                }
                else
                {
                    return jump;
                }
            }
            set
            {
                jump = value;
                jump2 = new VirtualButton();
                jump2.Nodes.Add(new VirtualButton.KeyboardKey(Keys.K));
            }
        }
        public static VirtualButton grab;
        public static VirtualButton grab2;
        [MonoModHook("Monocle.VirtualButton Celeste.Input::Grab")]
        public static VirtualButton Grab_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return grab2;
                }
                else
                {
                    return grab;
                }
            }
            set
            {
                grab = value;
                grab2 = new VirtualButton();
                grab2.Nodes.Add(new VirtualButton.KeyboardKey(Keys.H));
            }
        }
        public static VirtualIntegerAxis moveX;
        public static VirtualIntegerAxis moveX2;
        [MonoModHook("Monocle.VirtualIntegerAxis Celeste.Input::MoveX")]
        public static VirtualIntegerAxis MoveX_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return moveX2;
                }
                else
                {
                    return moveX;
                }
            }
            set
            {
                moveX = value;
                moveX2 = new VirtualIntegerAxis();
                moveX2.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad4, Keys.NumPad6));
            }
        }
        public static VirtualIntegerAxis moveY;
        public static VirtualIntegerAxis moveY2;
        [MonoModHook("Monocle.VirtualIntegerAxis Celeste.Input::MoveY")]
        public static VirtualIntegerAxis MoveY_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return moveY2;
                }
                else
                {
                    return moveY;
                }
            }
            set
            {
                moveY = value;
                moveY2 = new VirtualIntegerAxis();
                moveY2.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad8, Keys.NumPad5));
            }
        }
        public static VirtualJoystick aim;
        public static VirtualJoystick aim2;
        [MonoModHook("Monocle.VirtualJoystick Celeste.Input::Aim")]
        public static VirtualJoystick Aim_Safe
        {
            get
            {
                if (Environment.StackTrace.Contains("PlayerTwo"))
                {
                    return aim2;
                }
                else
                {
                    return aim;
                }
            }
            set
            {
                aim = value;
                aim2 = new VirtualJoystick(true);
                aim2.Nodes.Add(new VirtualJoystick.KeyboardKeys(VirtualInput.OverlapBehaviors.TakeNewer, Keys.NumPad4, Keys.NumPad6, Keys.NumPad8, Keys.NumPad5));
            }
        }

    }
}
