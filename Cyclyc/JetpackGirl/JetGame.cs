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
    public class JetGame : Cyclyc.Framework.CycGame
    {
        JetpackGirl jg;
        public JetGame(Game1 game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            AddBackground("pixel city sky", 0.1f);
            AddBackground("pixel city skyline", 0.3f);
            AddBackground("pixel city foreground", 0.5f);
            
            jg = new JetpackGirl((Game1)Game);
            AddSprite(jg);

            base.Initialize();
        }

        public override void LoadContent()
        {

            base.LoadContent();
        }
        
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            
            base.Update(gameTime);
        }

        protected override Color ClearColor()
        {
            return Color.LawnGreen;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}