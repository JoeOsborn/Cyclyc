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

namespace Cyclyc.ShipGirl
{
    class ShipCircle : CycSprite
    {
        protected Ship ship;
        //for these, position is the center
        protected float radius;

        public ShipCircle(Game1 game, Ship sh)
            : base(game)
        {
            radius = 1.0f;
            ship = sh;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            position.X = (int)(ship.position.X + ship.bounds.Center.X);
            position.Y = (int)(ship.position.Y + ship.bounds.Center.Y);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle srcRect = bounds;
            //modify srcRect.X for animation frame
            Rectangle dstRect = bounds;
            //modify dstRect.X, .Y for position, viewport
            dstRect.X = (int)((position.X*ScaleFactor) - radius);
            dstRect.Y = (int)((position.Y*ScaleFactor) - radius);
            dstRect.Width = (int)(radius * ScaleFactor * 2);
            dstRect.Height = (int)(radius * ScaleFactor * 2);

            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
        }
    }
}
