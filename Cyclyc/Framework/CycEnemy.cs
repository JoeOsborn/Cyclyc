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

namespace Cyclyc.Framework
{
    public class CycEnemy : CycSprite
    {
        protected Challenge challenge;
        public Challenge Challenge
        {
            get { return challenge; }
        }

        protected bool leftToRight;
        public bool LeftToRight
        {
            get { return leftToRight; }
            set { leftToRight = value; }
        }

        protected Vector2 startPosition;

        protected EnemyPool pool;

        protected override bool FlipImage
        {
            get { return velocity.X > 0; }
        }

        public int Difficulty { get; set; }

        protected int frameCount;
        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value; }
        }

        public CycEnemy(Game1 game, EnemyPool p)
            : base(game)
        {
            pool = p;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            int[] frames = new int[FrameCount];
            int[] timings = new int[FrameCount];
            for (int i = 0; i < frames.Length; i++)
            {
                frames[i] = i;
                timings[i] = 10;
            }
            animations["default"] = new Animation(frames, timings, true);
            spriteWidth = spriteSheet.Width / FrameCount;
            bounds.Width = spriteWidth;
            bounds.Height = spriteSheet.Height;
            visualWidth = spriteWidth;
        }

        public virtual void Reset(Challenge c, string img, int fc, CollisionStyle col, bool left, int xp, int yp, int w, int h, int diff)
        {
            challenge = c;
            Difficulty = diff;
            collisionStyle = col;
            frameCount = fc;
            assetName = img;
            LoadContent();
            Play("default");
            bounds = new Rectangle(0, 0, w, h);
            int x = 0;
            if (left)
            {
                x = xp - SpriteWidth;
                leftToRight = true;
            }
            else
            {
                x = xp;
                leftToRight = false;
            }
            startPosition = new Vector2(x, yp);
            position = startPosition;
            velocity = new Vector2(0, 0);
            alive = true;
            visible = true;
            VisualWidth = w;
            VisualHeight = h;
        }
        public void Die()
        {
            alive = false;
            visible = false;
            Challenge.EnemyKilled(this);
        }
        protected virtual void UpdatePosition(GameTime gameTime)
        {

        }
        public override void Update(GameTime gameTime)
        {
            if (alive)
            {
                UpdatePosition(gameTime);
                if (leftToRight && IsPastRightEdge(gameTime))
                {
                    alive = false;
                    visible = false;
                    if (Challenge != null)
                    {
                        Challenge.EnemyIgnored(this);
                    }
                }
                if (!leftToRight && IsPastLeftEdge(gameTime))
                {
                    alive = false;
                    visible = false;
                    if (Challenge != null)
                    {
                        Challenge.EnemyIgnored(this);
                    }
                }
                //if (IsPastRightEdge(gameTime))
                //{
                //    alive = false;
                //    visible = false;
                //}
            }
            base.Update(gameTime);
        }
    }
}
