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

        public void RegisterDifficultyNotch(CycGame src, int difficulty)
        {
            if (src == TopGame)
            {
                //send a notch to the bottom edge of the pipe
                EnemyNotch n = new EnemyNotch(this, X+PipeMargin+2, 0, 8);
                n.TargetY = Height - bottomNotches.Count() * n.Height;
                n.Duration = (float)(1 - (Game.CurrentMeasure - (int)(Game.CurrentMeasure)));
                bottomNotches.Add(n);
                n.Initialize();
                n.LoadContent();
            }
            else
            {
                //send a notch to the upper edge of the pipe
                //warning magic number
                EnemyNotch n = new EnemyNotch(this, X+2, Height - 8 - PipeMargin, 8);
                //assumption: notches have uniform height
                n.TargetY = topNotches.Count() * n.Height + Width / 2;
                n.Duration = (float)(1 - (Game.CurrentMeasure - (int)(Game.CurrentMeasure)));
                topNotches.Add(n);
                n.Initialize();
                n.LoadContent();
            }
        }
        public void ClearNotches()
        {
            foreach(EnemyNotch n in topNotches)
            {
                n.ShuffleOut((X == 0) ? this.Width : X - n.Width, 0);
            }
            foreach (EnemyNotch n in bottomNotches)
            {
                n.ShuffleOut((X == 0) ? this.Width : X - n.Width, this.Height - Width / 2 - n.Height);
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
