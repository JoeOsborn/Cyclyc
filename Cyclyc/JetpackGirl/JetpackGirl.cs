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

namespace Cyclyc.JetpackGirl
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class JetpackGirl : CycSprite
    {
        public override string AssetName
        {
            get { return "jetpack_standin"; }
        }
        KeyboardState kb;
        protected float jpFuel;
        protected bool jumpReleased;

        public JetpackGirl(Game1 game)
            : base(game)
        {
            jumpReleased = true;
            jpFuel = MaxJPFuel;
            bounds = new Rectangle(0, 0, 12, 16);
            // TODO: Construct any child components here
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

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected float MaxSpeedX
        {
            get { return 1.0f; }
        }
        protected float DefaultSpeedX
        {
            get { return 0; }
        }
        protected float MaxSpeedFallY
        {
            get { return 2.0f; }
        }
        protected float MaxSpeedRiseY
        {
            get { return -2.5f; }
        }
        protected float MaxJPFuel
        {
            get { return 180.0f; }
        }
        protected float Gravity
        {
            get { return 0.1f; }
        }
        protected bool JetWipesVelocity
        {
            get { return true; }
        }
        protected float JetThrust
        {
            get { return 0.15f; }
        }
        protected float JumpThrust
        {
            get { return -3.0f; }
        }
        protected float DefuelRate
        {
            get { return 1.0f; }
        }
        protected float RefuelRate
        {
            get { return 0.8f; }
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            KeyboardState oldKB = kb;
            kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.D))
            {
                velocity.X = MaxSpeedX;
            }
            else if(kb.IsKeyDown(Keys.A))
            {
                velocity.X = -MaxSpeedX;
            }
            else
            {
                velocity.X = DefaultSpeedX;
            }
            if (!jumpReleased && oldKB.IsKeyDown(Keys.W) && !kb.IsKeyDown(Keys.W))
            {
                jumpReleased = true;
            }
            if (BottomEdge < FloorY)
            {
                velocity.Y = Math.Min(velocity.Y+Gravity, MaxSpeedFallY);
                //we're in the air.  do we jetpack?
                if (jumpReleased && kb.IsKeyDown(Keys.W) && jpFuel > 0)
                {
                    if (velocity.Y > 0) { velocity.Y = 0; }
                    velocity.Y = Math.Max(velocity.Y - JetThrust, MaxSpeedRiseY);
                    jpFuel -= DefuelRate;
                }
            }
            else if(velocity.Y > 0)
            {
                BottomEdge = FloorY;
                velocity.Y = 0;
            }
            if (BottomEdge == FloorY)
            {
                jpFuel = Math.Min(jpFuel + RefuelRate, MaxJPFuel);
                //do we jump?
                if (!oldKB.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.W))
                {
                    jumpReleased = false;
                    if (JetWipesVelocity)
                    {
                        velocity.Y = JumpThrust;
                    }
                    else
                    {
                        velocity.Y += JumpThrust;
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override int ScaleFactor
        {
            get { return 2; }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}