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
            get { return "rockGirl"; }
        }
        KeyboardState kb;
        protected float jpFuel;
        protected bool jumpReleased;

        protected override float ScaleFactor
        {
            get { return 2.0f; }
        }

        protected override int SpriteWidth
        {
            get { return 14; }
        }

        public JetpackGirl(Game1 game)
            : base(game)
        {
            jumpReleased = true;
            jpFuel = MaxJPFuel;
            bounds = new Rectangle(0, 0, 14, 16);
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

        public override void LoadContent()
        {
            animations["default"] = new Animation(new int[] { 0, 1 }, new int[] { 5, 5 }, true);
            animations["jet"] = new Animation(new int[] { 2, 3 }, new int[] { 5, 5 }, true);
            Play("default");
            base.LoadContent();
        }

        protected float MaxSpeedX
        {
            get { return 1.0f; }
        }
        protected float DefaultSpeedX
        {
            get { return -0.25f; }
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
        //need to use Wrenchless variants of these later
        protected void BeginJet()
        {
            Play("jet", true);
        }
        protected void MaintainJet()
        {
            Play("jet", false);
        }
        protected void FizzleJet()
        {
            //later, have a fizzled jet animation
            Play("default", false);
        }
        protected void Jump()
        {
            //maybe a jump anim later
            Play("default", false);
        }
        protected void Fall()
        {
            Play("default", false);
        }
        protected void Land()
        {
            Play("default", false);
        }
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
                if (jumpReleased && kb.IsKeyDown(Keys.W))
                {
                    if (jpFuel <= 0)
                    {
                        FizzleJet();
                    }
                    else
                    {
                        if (velocity.Y > 0) { velocity.Y = 0; }
                        velocity.Y = Math.Max(velocity.Y - JetThrust, MaxSpeedRiseY);
                        jpFuel -= DefuelRate;
                        if (!oldKB.IsKeyDown(Keys.W))
                        {
                            BeginJet();
                        }
                        else
                        {
                            MaintainJet();
                        }
                    }
                }
                else
                {
                    //maybe only Fall if velocity is negative?  Meh, worry about it later.
                    Fall();
                }
            }
            else if(velocity.Y > 0)
            {
                BottomEdge = FloorY;
                velocity.Y = 0;
                Land();
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
                    Jump();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}