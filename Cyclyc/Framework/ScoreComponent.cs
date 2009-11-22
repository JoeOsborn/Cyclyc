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


namespace Cyclyc.Framework
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ScoreComponent : DrawableGameComponent
    {
        public int Score { get; set; }
        public int Width { get; set; }
        public int Y { get; set; }

        SpriteFont font;

        public ScoreComponent(Game g)
            : base(g)
        {
            
        }
        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("ScoreFont");
            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sb = ((Game1)Game).SpriteBatch;
            string str = "SCORE: " + Score.ToString();
            Vector2 pos = new Vector2(Width / 2 - (font.MeasureString(str).X / 2), Y);
            sb.Begin(SpriteBlendMode.AlphaBlend);
            sb.DrawString(font, str, pos, Color.White);
            sb.End();
            base.Draw(gameTime);
        }
    }
}
