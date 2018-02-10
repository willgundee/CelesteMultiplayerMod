#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

using Celeste.Mod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using MonoMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Celeste {
    public class PlayerTwo : Player {
        public PlayerTwo(Vector2 position, PlayerSpriteMode spriteMode)
            : base(position, spriteMode) {
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
