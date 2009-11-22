using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Cyclyc.Framework
{
    public class EndingSprite : CycSprite
    {
        public EndingSprite(Game1 game, string img, Vector2 pos, int w)
            : base(game)
        {
            AssetName = img;
            Position = pos;
            SpriteWidth = w;
        }

        protected override bool StopAtRightEdge(GameTime gt)
        {
            return false;
        }
        protected override void HitRightEdge(GameTime gt)
        {
            //nop;
        }
        protected override bool StopAtLeftEdge(GameTime gt)
        {
            return false;
        }
        protected override void HitLeftEdge(GameTime gt)
        {
            //nop;
        }
        protected override bool StopAtBottomEdge(GameTime gt)
        {
            return false;
        }
        protected override void HitBottomEdge(GameTime gt)
        {
            //nop;
        }
        protected override bool StopAtTopEdge(GameTime gt)
        {
            return false;
        }
        protected override void HitTopEdge(GameTime gt)
        {
            //nop
        }
    }
}
