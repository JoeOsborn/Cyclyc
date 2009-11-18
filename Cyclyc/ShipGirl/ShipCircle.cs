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
        protected string assetName;
        public override string AssetName
        {
            get
            {
                return assetName;
            }
        }
        protected Ship ship;
        //for these circles, position is the center
        protected float oldRadius;
        protected float radius;
        protected float visualRadius;
        protected double resizeTime;
        protected double resizeDuration;

        public float Radius
        {
            get { return radius; }
        }

        protected override int SpriteWidth
        {
            get { return 292; }
        }

        public ShipCircle(Game1 game, Ship sh, string img)
            : base(game)
        {
            assetName = img;
            radius = 1.0f;
            oldRadius = radius;
            visualRadius = radius;
            ship = sh;
            resizeDuration = 0.0;
            resizeTime = 0.0;
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

        public void ResizeTo(float newRadius, double duration)
        {
            //interpolate visualRadius to radius
            oldRadius = radius;
            radius = newRadius;
            resizeTime = 0;
            resizeDuration = duration;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (resizeTime < resizeDuration)
            {
                resizeTime += gameTime.ElapsedGameTime.TotalSeconds;
                visualRadius = MathHelper.Lerp(oldRadius, radius, (float)(resizeTime/resizeDuration));
            }
            else
            {
                visualRadius = radius;
            }
            position = ship.Center;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Rectangle srcRect = new Rectangle(XForSprite(currentAnimation.CurrentFrame), 0, SpriteWidth, spriteSheet.Height);
            //modify srcRect.X for animation frame
            Rectangle dstRect = srcRect;
            //modify dstRect.X, .Y for position, viewport
            dstRect.X = (int)((position.X*ScaleFactor) - visualRadius);
            dstRect.Y = (int)((position.Y*ScaleFactor) - visualRadius);
            dstRect.Width = (int)(visualRadius * ScaleFactor * 2);
            dstRect.Height = (int)(visualRadius * ScaleFactor * 2);

            SpriteBatch.Draw(spriteSheet, dstRect, srcRect, Color.White);
        }
    }
}
