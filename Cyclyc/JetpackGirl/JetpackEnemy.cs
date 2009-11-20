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
    public class JetpackEnemy : CycEnemy, JetpackOwner
    {
        protected Jetpack jetpack;
        protected double hitTimer;
        Random rgen;

        public JetpackEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
            rgen = new Random();
            jetpack = new Jetpack(this);
            ScaleFactor = 2.0f;
        }

        public bool KnockedOffScreen
        {
            get { return (hitTimer > 0) && (leftToRight ? IsPastLeftEdge(null) : IsPastRightEdge(null)); }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            LoadAnimations();
            Play("default");
        }

        public void Hit()
        {
            hitTimer = rgen.NextDouble() * 0.3 + 0.1;
        }

        public void Reset(Challenge c, string img, int fc, bool left, int xp, int yp, int w, int h, float speed, int radius)
        {
            jetpack.MaxSpeedX = speed;
            Reset(c, img, fc, CollisionStyle.Circle, left, xp, yp, w, h);
            Radius = radius;
        }

        public void Reset(Challenge c, string img, int fc, bool left, int xp, int yp, int w, int h, float speed, int bx, int by, int bw, int bh)
        {
            jetpack.MaxSpeedX = speed;
            Reset(c, img, fc, CollisionStyle.Box, left, xp, yp, w, h);
            bounds = new Rectangle(bx, by, bw, bh);
        }

        protected virtual void LoadAnimations()
        {
            //these are wrong, figure out a nice parameterizable way to avoid a lot of anim duplication?
            int[] timings = TimingSequence(5, frameCount);
            animations["default"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["run"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["jet"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["begin-jet"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["stop-jet"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["jump"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["fall"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["land"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
            animations["run"] =
                new Animation(FrameSequence(0, frameCount), timings, true);
        }



        protected override bool StopAtRightEdge(GameTime gt)
        {
            return false;
        }
        protected override bool StopAtLeftEdge(GameTime gt)
        {
            return false;
        }

        protected override void HitLeftEdge(GameTime gt)
        {
            //nop
        }
        protected override void HitRightEdge(GameTime gt)
        {
            //nop
        }

        public void BeginJet()
        {
            Play("begin-jet", true);
        }
        public void MaintainJet()
        {
            Play("jet", false);
        }
        public void FizzleJet()
        {
            Play("fizzle-jet", false);
        }
        public void StopJet()
        {
            Play("stop-jet", true);
        }
        public void Jump()
        {
            Play("jump", false);
        }
        public void Fall()
        {
            Play("fall", false);
        }
        public void Land()
        {
            Play("land", true);
        }
        public void Run()
        {
            Play("run", false);
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
            get { return leftToRight; }
        }
        public bool ShouldMoveLeft
        {
            get { return !leftToRight; }
        }
        public bool ShouldJet
        {
            get { return false; }
        }
        public bool ShouldJump
        {
            get { return false; }
        }

        protected override void UpdatePosition(GameTime gameTime)
        {
            jetpack.Update(gameTime);
            if (hitTimer > 0)
            {
                hitTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (leftToRight)
                {
                    velocity.X = -3;
                }
                else
                {
                    velocity.X = 3;
                }
            }
        }

    }
}
