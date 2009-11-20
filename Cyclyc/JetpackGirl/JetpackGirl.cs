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
    public class JetpackGirl : CycSprite, JetpackOwner
    {
        KeyboardState kb, oldKB;
        protected bool jumpReleased;
        protected bool attacking;
        protected double attackCounter;
        protected Jetpack jetpack;
        protected CycSprite wrench;

        public JetpackGirl(Game1 game)
            : base(game)
        {
            ScaleFactor = 2.0f;
            assetName = "rockGirl";
            collisionStyle = CollisionStyle.Box;
            jetpack = new Jetpack(this);
            spriteWidth = 14;
            jumpReleased = true;
            attackCounter = 0;
            attacking = false;
            bounds = new Rectangle(0, 0, 14, 16);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            int[] timings = TimingSequence(5, 2);
            animations["default"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["run"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["run-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["begin-jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["begin-jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["stop-jet"] = 
                new Animation(FrameSequence(2, 2), timings, true);
            animations["stop-jet-attacking"] = 
                new Animation(FrameSequence(6, 2), timings, true);
            animations["jump"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["jump-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["fall"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["fall-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["land"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["land-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            animations["run"] = 
                new Animation(FrameSequence(0, 2), timings, true);
            animations["run-attacking"] = 
                new Animation(FrameSequence(4, 2), timings, true);
            Play("default");

            wrench = new CycSprite(Game);
            wrench.Initialize();
            wrench.AddAnimation("default", new int[] { 0, 1 }, new int[] { 5, 5 }, true);
            wrench.Play("default");
            wrench.AssetName = "wrench";
            wrench.Visible = false;
            wrench.Alive = false;
            wrench.CollisionStyle = CollisionStyle.Circle;
            wrench.SpriteWidth = 14;
            wrench.ScaleFactor = 2.0f;
            wrench.Radius = 14;
            wrench.VisualRadius = 14;
            wrench.LoadContent();
            base.LoadContent();
        }

        public void BeginJet()
        {
            //later, might have 'begin jet' anims
            if (attacking)
            {
                Play("begin-jet-attacking", true);
            }
            else
            {
                Play("begin-jet", true);
            }
        }
        public void MaintainJet()
        {
            if (attacking)
            {
                Play("jet-attacking", false);
            }
            else
            {
                Play("jet", false);
            }
        }
        public void FizzleJet()
        {
            //later, have a fizzled jet animation
            if (attacking)
            {
                Play("fizzle-jet-attacking", false);
            }
            else
            {
                Play("fizzle-jet", false);
            }
        }
        public void StopJet()
        {
            if (attacking)
            {
                Play("stop-jet-attacking", true);
            }
            else
            {
                Play("stop-jet", true);
            }
        }
        public void Jump()
        {
            jumpReleased = false;
            if (attacking)
            {
                Play("jump-attacking", false);
            }
            else
            {
                Play("jump", false);
            }
        }
        public void Fall()
        {
            if (attacking)
            {
                Play("fall-attacking", false);
            }
            else
            {
                Play("fall", false);
            }
        }
        public void Land()
        {
            if (attacking)
            {
                Play("land-attacking", true);
            }
            else
            {
                Play("land", true);
            }
        }
        public void Run()
        {
            if (attacking)
            {
                Play("run-attacking", false);
            }
            else
            {
                Play("run", false);
            }
        }
        protected double AttackDuration
        {
            get { return 0.4; }
        }
        protected double AttackRatio
        {
            get { return attackCounter / AttackDuration; }
        }
        protected double AttackRadius
        {
            get { return 10.0; }
        }
        public bool IsInAir
        {
            get { return BottomEdge < FloorY; }
        }
        public bool FallingThroughGround
        {
            get { return OnGround && velocity.Y > 0; }
        }
        public bool OnGround
        {
            get { return BottomEdge == FloorY; }
        }
        public bool ShouldMoveRight
        {
            get { return kb.IsKeyDown(Keys.D); }
        }
        public bool ShouldMoveLeft
        {
            get { return kb.IsKeyDown(Keys.A); }
        }
        public bool ShouldJet
        {
            get { return jumpReleased && kb.IsKeyDown(Keys.W); }
        }
        public bool ShouldJump
        {
            get { return !oldKB.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.W); }
        }
        public override void Update(GameTime gameTime)
        {
            oldKB = kb;
            kb = Keyboard.GetState();
            if (!jumpReleased && oldKB.IsKeyDown(Keys.W) && !kb.IsKeyDown(Keys.W))
            {
                jumpReleased = true;
            }
            if (attackCounter > 0)
            {
                wrench.Visible = true;
                wrench.Alive = true;
                double ratio = 1-AttackRatio;
                double angle = ratio * 2*Math.PI;
                //ratio of rotation from circle starting at 0 = 0 degrees, .25 = 90 degrees (flip), .5 = 180 degrees, .75 = 270 degrees (flip back), 1.0 = 0 degrees
                double r = AttackRadius;
                wrench.Position = new Vector2((float)(Center.X-2 + r * Math.Cos(angle)), (float)(Center.Y + r * Math.Sin(angle)));
                if (angle > (Math.PI/2.0) && angle < (3.0*Math.PI/2.0)) { FlipImage = true; }
                else { FlipImage = false; }
                attackCounter -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (attackCounter <= 0)
            {
                wrench.Visible = false;
                wrench.Alive = false;
                FlipImage = false;
                attackCounter = 0;
                if (kb.IsKeyDown(Keys.Q) && oldKB.IsKeyUp(Keys.Q))
                {
                    attackCounter = AttackDuration;
                }
            }
            attacking = true;
            wrench.Update(gameTime);
            jetpack.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            wrench.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}