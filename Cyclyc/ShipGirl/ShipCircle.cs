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
        protected float oldRadius;
        protected double resizeTime;
        protected double resizeDuration;

        protected override int SpriteWidth
        {
            get { return 292; }
        }

        public ShipCircle(Game1 game, Ship sh, string img)
            : base(game)
        {
            assetName = img;
            collisionStyle = CollisionStyle.Circle;
            Radius = 1.0f;
            oldRadius = Radius;
            VisualRadius = Radius;
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
            oldRadius = Radius;
            Radius = newRadius;
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
                VisualRadius = MathHelper.Lerp(oldRadius, Radius, (float)(resizeTime/resizeDuration));
            }
            else
            {
                VisualRadius = Radius;
            }
            position = ship.Center;
            base.Update(gameTime);
        }
    }
}
