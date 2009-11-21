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
    public class EnemyNotch
    {
        protected Game1 Game 
        {
            get
            {
                return Pipe.Game;
            }
        }

        protected EnemyPipe Pipe { get; set; }
        public SpriteBatch SpriteBatch
        {
            get { return Pipe.SpriteBatch; }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        protected int StartX { get; set; }
        protected int StartY { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        protected bool shuffling;
        protected float rollTick;
        protected float yReachedTick;
        protected float duration;
        public float Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                rollTick = duration;
            }
        }


        Texture2D texture;

        public EnemyNotch(EnemyPipe p, int x, int y, int w)
        {
            Pipe = p;
            shuffling = false;
            X = x;
            Y = y;
            StartY = Y;
            Width = w;
            Height = 8;
        }

        public void Initialize()
        {
            
        }

        public void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>("wrench");
        }

        public void ShuffleOut(int destX, int destY)
        {
            TargetX = destX;
            TargetY = destY;
            shuffling = true;
            Duration = 0.05f;
        }

        public void Update(GameTime gt)
        {
            if (rollTick > 0)
            {
                rollTick -= (float)(gt.ElapsedGameTime.TotalSeconds);
                //doesn't really work, try an epsilon and if that doesn't work use curves instead, geez
                if (Y != TargetY)
                {
                    Y = (int)(MathHelper.Lerp(StartY, TargetY, rollTick / Duration));
                    yReachedTick = rollTick;
                }
                else
                {
                    X = (int)(MathHelper.Lerp(StartX, TargetX, (rollTick - yReachedTick) / (Duration - yReachedTick)));
                }
            }
            else if (shuffling)
            {
                Pipe.ClearNotch(this);
            }
        }

        public void Draw(GameTime gt)
        {
            SpriteBatch.Draw(texture, new Rectangle(X, Y, Width, Height), Color.White);
        }
    }
}
