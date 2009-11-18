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
using Cyclyc.Framework;

namespace Cyclyc.ShipGirl
{
    public class Ship : CycSprite
    {
        public override string AssetName
        {
            get { return "shipGirl"; }
        }

        protected override int SpriteWidth
        {
            get { return 157; }
        }

        public int Radius
        {
            get { return SpriteWidth; }
        }

        protected KeyboardState kb;

        public Ship(Game1 game)
            : base(game)
        {           
            // TODO: Construct any child components here
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            visualWidth = 157 / 2;
            visualHeight = 74 / 2;
            bounds = new Rectangle(0, 0, visualWidth, visualHeight);
        }

        protected float MaxSpeedX
        {
            get { return 2.0f; }
        }
        protected float MaxSpeedY
        {
            get { return 2.0f; }
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.Up) && kb.IsKeyUp(Keys.Down) && TopEdge > CeilY)
            {
                velocity.Y = -MaxSpeedY;
            }
            else if (kb.IsKeyDown(Keys.Down) && kb.IsKeyUp(Keys.Up) && BottomEdge < FloorY)
            {
                velocity.Y = MaxSpeedY;
            }
            else
            {
                velocity.Y = 0;
            }
            if (kb.IsKeyDown(Keys.Right) && kb.IsKeyUp(Keys.Left) && RightEdge < RightX)
            {
                velocity.X = MaxSpeedX;
            }
            else if (kb.IsKeyDown(Keys.Left) && kb.IsKeyUp(Keys.Right) && LeftEdge > LeftX)
            {
                velocity.X = -MaxSpeedX;
            }
            else
            {
                velocity.X = 0;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}