using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Cyclyc.Framework;

namespace Cyclyc.JetpackGirl
{
    class HoverEnemy : JetpackEnemy
    {
        public HoverEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
            jetpack.Gravity = 0.0f;
        }
        public override bool ShouldJet
        {
            get
            {
                return false;
            }
        }
        public override bool ShouldJump
        {
            get
            {
                return false;
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
