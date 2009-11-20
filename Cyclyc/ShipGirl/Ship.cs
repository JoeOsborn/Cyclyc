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
        protected KeyboardState kb;

        public bool Dying { get; set; }
        protected double respawnTimer;
        protected double RespawnDelay
        {
            get { return 1.0; }
        }

        public Ship(Game1 game)
            : base(game)
        {
            assetName = "shipGirl";
            respawnTimer = 0;
            Dying = false;
            collisionStyle = CollisionStyle.Circle;
            AddAnimation("death", new int[] { 0 }, new int[] { 5 }, true);
        }

        public Vector2 StartPosition { get; set; }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
            spriteWidth = spriteSheet.Width;
            VisualWidth = spriteWidth/2;
            VisualHeight = spriteSheet.Height/2;
            Radius = 15;
        }

        protected float MaxSpeedX
        {
            get { return 2.5f; }
        }
        protected float MaxSpeedY
        {
            get { return 2.5f; }
        }

        public void Die()
        {
            Dying = true;
            respawnTimer = RespawnDelay;
            Play("death", false);
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
            {
                respawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if(respawnTimer > 0)
                {
                    velocity.X = 0;
                    velocity.Y = 0;
                    base.Update(gameTime);
                    return;
                }
                respawnTimer = 0;
                Dying = false;
                //check to make sure no enemies are here
                position = StartPosition;
            }
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