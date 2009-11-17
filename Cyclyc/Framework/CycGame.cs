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
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;


namespace Cyclyc.Framework
{
    public class CycGame : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<CycSprite> sprites;

        Viewport view;
        public Viewport View
        {
            get { return view; }
            set 
            { 
                view = value;
                foreach (CycSprite sprite in sprites)
                {
                    sprite.View = view;
                }
            }
        }

        public CycGame(Game1 game) : base(game)
        {
            sprites = new List<CycSprite>();
        }

        public void AddSprite(CycSprite cs)
        {
            sprites.Add(cs);
            cs.View = view;
        }

        public override void Initialize()
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.Initialize();
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.LoadContent();
            }   
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
//            Matrix Projection = Matrix.CreateScale(2.0f) * Matrix.CreateTranslation(view.X-20, view.Y, 0);
            //((Game1)Game).SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, Projection);
            ((Game1)Game).SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.None;
            foreach (CycSprite sprite in sprites)
            {
                sprite.Draw(gameTime);
            }
            base.Draw(gameTime);
            ((Game1)Game).SpriteBatch.End();
        }
    }
}