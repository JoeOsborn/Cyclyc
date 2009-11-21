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
        protected Random rgen;
        protected bool hitFromLeft;

        public CycSprite Target
        {
            get;
            set;
        }

        protected override bool FlipImage
        {
            get { return IsHit ? leftToRight : velocity.X > 0; }
        }

        public JetpackEnemy(Game1 game, EnemyPool p)
            : base(game, p)
        {
            rgen = new Random();
            hitFromLeft = true;
            hitTimer = 0;
            jetpack = new Jetpack(this);
            ScaleFactor = 2.0f;
        }

        public bool KnockedOffScreen
        {
            get { return (hitTimer > 0) && (IsPastLeftEdge(null) || IsPastRightEdge(null)); }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            LoadAnimations();
            Play("default");
        }

        public double Mass
        {
            get { return (CollisionStyle == CollisionStyle.Circle) ? ((float)Radius / (float)SpriteWidth) : ((float)bounds.Width / (float)SpriteWidth); }
        }

        public void Hit(float x)
        {
            hitFromLeft = (x < position.X);
            hitTimer = rgen.NextDouble() * (1.0 / Mass) + (0.25 / Mass);
        }

        public void Reset(Challenge c, string img, int fc, bool left, int xp, int yp, int w, int h, float speed, int radius, int diff)
        {
            rgen = new Random((int)DateTime.Now.Ticks + yp + radius + h);
            hitTimer = 0;
            jetpack.MaxSpeedX = speed;
            Reset(c, img, fc, CollisionStyle.Circle, left, xp, yp, w, h, diff);
            Radius = radius;
            VisualRadius = Radius;
        }

        public void Reset(Challenge c, string img, int fc, bool left, int xp, int yp, int w, int h, float speed, int bx, int by, int bw, int bh, int diff)
        {
            rgen = new Random((int)DateTime.Now.Ticks + yp + h + by + bh);
            hitTimer = 0;
            jetpack.MaxSpeedX = speed;
            Reset(c, img, fc, CollisionStyle.Box, left, xp, yp, w, h, diff);
            bounds = new Rectangle(bx, by, bw, bh);
            VisualWidth = w;
            VisualHeight = h;
        }

        protected virtual void LoadAnimations()
        {
            //these are wrong, figure out a nice parameterizable way to avoid a lot of anim duplication?
            int[] timings = TimingSequence(8, frameCount);
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
            Play("begin-jet", false);
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
            Play("stop-jet", false);
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
        protected float TargetDistance
        {
            get { return Center.X - Target.Center.X; }
        }
        protected bool CloseToTarget
        {
            get { return Math.Abs(TargetDistance) < 100; }
        }
        protected bool TargetIsLeft
        {
            get { return TargetDistance > 0; }
        }
        public virtual bool ShouldMoveRight
        {
            //if close to player, move towards player; else, move right
            get
            {
                if (CloseToTarget)
                {
                    if (Math.Abs(TargetDistance) < 8)
                    {
                        //are we close and already moving left?  let's keep going
                        return Velocity.X < 0 ? false : true;
                    }
                    return !TargetIsLeft;
                }
                else
                {
                    return leftToRight;
                }
            }
        }
        public virtual bool ShouldMoveLeft
        {
            //if close to player, move towards player; else, move left
            get
            {
                if (CloseToTarget)
                {
                    if (Math.Abs(TargetDistance) < 8)
                    {
                        //are we close and already moving right?  let's keep going
                        return Velocity.X < 0 ? true : false;
                    }
                    return TargetIsLeft;
                }
                else
                {
                    return !leftToRight;
                }
            }
        }
        public virtual bool ShouldJet
        {
            get { return false; }
        }
        public virtual bool ShouldJump
        {
            get { return false; }
        }

        public bool IsHit
        {
            get { return hitTimer > 0; }
        }

        protected override void UpdatePosition(GameTime gameTime)
        {
            jetpack.Update(gameTime);
            if (IsHit)
            {
                hitTimer -= gameTime.ElapsedGameTime.TotalSeconds;
                if (hitFromLeft)
                {
                    velocity.X = 3;
                    leftToRight = false;
                }
                else
                {
                    velocity.X = -3;
                    leftToRight = true;
                }
            }
        }

    }
}
