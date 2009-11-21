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
            get { return Width / 2; }
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

        protected int UpPipeX { get { return X == 0 ? (X + 2) : (X + PipeMargin + 2); } }
        protected int UpPipeStartY { get { return Height - 8; } }
        protected int DownPipeX { get { return X == 0 ? (X + PipeMargin + 2) : (X + 2); } }
        protected int DownPipeStartY { get { return PipeMargin; } }

        public void RegisterDifficultyNotch(CycGame dst, int difficulty)
        {
            float triggerTime = ((((int)Game.CurrentMeasure) / 4) + 1) * 4 - (2.0f/180f);
            if (dst == TopGame)
            {
                //send a notch to the upper edge of the pipe
                //warning magic number
                EnemyNotch n = new EnemyNotch(this, UpPipeX, UpPipeStartY, 8);
                //assumption: notches have uniform height
                n.TargetY = topNotches.Count() * n.Height;
                n.Duration = (float)(triggerTime - Game.CurrentMeasure);
                topNotches.Add(n);
                n.Initialize();
                n.LoadContent();         
            }
            else
            {
                //send a notch to the bottom edge of the pipe
                EnemyNotch n = new EnemyNotch(this, DownPipeX, DownPipeStartY, 8);
                n.TargetY = Height - bottomNotches.Count() * n.Height - 8 - PipeMargin;
                n.Duration = (float)(triggerTime - Game.CurrentMeasure);
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
                    EnemyNotch n = topNotches[i];
                    n.ShuffleOut((X == 0) ? this.Width : X - n.Width, 0);
                }
                topNotches.RemoveRange(0, amt);
                for (int i = 0; i < topNotches.Count; i++)
                {
                    EnemyNotch n = topNotches[i];
                    n.TargetY = i * n.Height;
                    n.Duration = 2.0f / 180f;
                }
            }
            else
            {
                for (int i = 0; i < amt; i++)
                {
                    EnemyNotch n = bottomNotches[i];
                    n.ShuffleOut((X == 0) ? this.Width : X - n.Width, this.Height - Width / 2 - n.Height);
                }
                bottomNotches.RemoveRange(0, amt);
                for (int i = 0; i < bottomNotches.Count; i++)
                {
                    EnemyNotch n = bottomNotches[i];
                    n.TargetY = Height - i * n.Height - 8 - PipeMargin;
                    n.Duration = 2.0f / 180f;
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
            bg = Game.Content.Load<Texture2D>("space background");
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
            SpriteBatch.Draw(bg, new Rectangle(X, 0, Width, Height), Color.White);
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
