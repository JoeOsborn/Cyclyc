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
         protected int FrameCount
        {
            get { return 2; }
        }

        protected string assetName;
        public override string AssetName
        {
            get { return assetName; }
        }

        protected bool leftToRight;

        protected Vector2 startPosition;

        protected EnemyPool pool;

        public CycEnemy(Game1 game, EnemyPool p)
            : base(game)
        {
            pool = p;
            int[] frames = new int[FrameCount];
            int[] timings = new int[FrameCount];
            for(int i = 0; i < frames.Length; i++)
            {
                frames[i] = i;
                timings[i] = 10;
            }
            animations["default"] = new Animation(frames, timings, true);
        }

        public override void LoadContent()
        {
            base.LoadContent();
            spriteWidth = spriteSheet.Width / FrameCount;
            bounds.Width = spriteWidth;
            bounds.Height = spriteSheet.Height;
            visualWidth = spriteWidth;
        }

        public virtual void Reset(string img, bool left, int xp, int yp, int w, int h)
        {
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
            leftToRight = false;
            alive = true;
            visible = true;
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
                    pool.EnemyOffScreen(this);
                }
                if (!leftToRight && IsPastLeftEdge(gameTime))
                {
                    alive = false;
                    visible = false;
                    pool.EnemyOffScreen(this);
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
