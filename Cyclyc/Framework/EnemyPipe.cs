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
    public class EnemyPipe
    {
        public Game1 Game { get; set; }
        public SpriteBatch SpriteBatch
        {
            get { return Game.SpriteBatch; }
        }

        public CycGame TopGame { get; set; }
        public CycGame BottomGame { get; set; }

        public int X { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        protected int PipeMargin
        {
            get { return 0; }
        }

        List<EnemyNotch> topNotches;
        List<EnemyNotch> bottomNotches;

        Texture2D bg;

        public EnemyPipe(Game1 g)
        {
            Game = g;
            topNotches = new List<EnemyNotch>();
            bottomNotches = new List<EnemyNotch>();
        }

        protected int UpPipeX { get { return X; } }
        protected int UpPipeStartY { get { return Height - 20; } }
        protected int DownPipeX { get { return X; } }
        protected int DownPipeStartY { get { return 0; } }

        public void RegisterDifficultyNotch(CycGame dst, int difficulty)
        {
            float triggerTime = ((((int)Game.CurrentMeasure) / 4) + 1) * 4 - 1;
            if (dst == TopGame)
            {
                //send a notch to the upper edge of the pipe
                //warning magic number
                EnemyNotch n = new EnemyNotch(this, UpPipeX, UpPipeStartY, 20);
                //assumption: notches have uniform height
                n.TargetY = topNotches.Count() * n.Height;
                if (topNotches.Count() == 0)
                {
                    n.TargetX = DownPipeX;
                }
                n.Duration = (float)((triggerTime - Game.CurrentMeasure)*(4.0/3.0));
                topNotches.Add(n);
                n.Initialize();
                n.LoadContent();         
            }
            else
            {
                //send a notch to the bottom edge of the pipe
                EnemyNotch n = new EnemyNotch(this, DownPipeX, DownPipeStartY, 20);
                n.TargetY = Height - bottomNotches.Count() * n.Height - 20 - PipeMargin;
                n.TargetX = DownPipeX;
                n.Duration = (float)((triggerTime - Game.CurrentMeasure) * (4.0 / 3.0));
                bottomNotches.Add(n);
                n.Initialize();
                n.LoadContent();
            }
        }
        public void ClearNotches(CycGame src, int amt)
        {
            if (src == TopGame)
            {
                for(int i = 0; i < amt; i++)
                {
                    if (i >= topNotches.Count)
                    {
                        break;
                    }
                    EnemyNotch n = topNotches[i];
                    n.ShuffleOut((X == 0) ? this.Width : X - n.Width, 0);
                }
                if (topNotches.Count > 0)
                {
                    topNotches.RemoveRange(0, Math.Min(topNotches.Count, amt));
                }
                for (int i = 0; i < topNotches.Count; i++)
                {
                    EnemyNotch n = topNotches[i];
                    if (i == 0)
                    {
                        n.TargetX = DownPipeX;
                    }
                    n.TargetY = i * n.Height;
                    n.Duration = 2.0f / 3.0f;
                }
            }
            else
            {
                for (int i = 0; i < amt; i++)
                {
                    if (i >= bottomNotches.Count)
                    {
                        break;
                    }
                    EnemyNotch n = bottomNotches[i];
                    n.ShuffleOut((X == 0) ? this.Width : X - n.Width, this.Height - Width / 2 - n.Height);
                }
                if (bottomNotches.Count > 0)
                {
                    bottomNotches.RemoveRange(0, Math.Min(bottomNotches.Count, amt));
                }
                for (int i = 0; i < bottomNotches.Count; i++)
                {
                    EnemyNotch n = bottomNotches[i];
                    n.TargetY = Height - i * n.Height - 12 - PipeMargin;
                    n.Duration = 2.0f / 3.0f;
                }
            }
        }

        public void ClearNotch(EnemyNotch n)
        {
            if (topNotches.Contains(n)) { topNotches.Remove(n); }
            if (bottomNotches.Contains(n)) { bottomNotches.Remove(n); }
        }

        public void Initialize()
        {
            foreach (EnemyNotch n in topNotches)
            {
                n.Initialize();
            }
            foreach (EnemyNotch n in bottomNotches)
            {
                n.Initialize();
            }
        }

        public void LoadContent()
        {
            bg = Game.Content.Load<Texture2D>("tubeLeft");
            foreach (EnemyNotch n in topNotches)
            {
                n.LoadContent();
            }
            foreach (EnemyNotch n in bottomNotches)
            {
                n.LoadContent();
            }
        }

        public void Update(GameTime gt)
        {
            foreach (EnemyNotch n in topNotches)
            {
                n.Update(gt);
            }
            foreach (EnemyNotch n in bottomNotches)
            {
                n.Update(gt);
            }
        }

        public void Draw(GameTime gt)
        {
            SpriteBatch.Draw(bg, new Rectangle(X, 0, Width, Height), null, Color.White, 0, Vector2.Zero, X == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            foreach (EnemyNotch n in topNotches)
            {
                n.Draw(gt);
            }
            foreach (EnemyNotch n in bottomNotches)
            {
                n.Draw(gt);
            }
        }
    }
}
