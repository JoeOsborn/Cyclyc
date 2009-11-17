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
    public class CycGame : Object
    {
        List<CycSprite> sprites;
        Game1 game;
        public Game1 Game
        {
            get { return game; }
        }
        public GraphicsDevice GraphicsDevice
        {
            get { return Game.GraphicsDevice; }
        }
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

        public CycGame(Game1 g)
        {
            game = g;
            sprites = new List<CycSprite>();
        }

        public void AddSprite(CycSprite cs)
        {
            sprites.Add(cs);
            cs.View = view;
        }

        public virtual void Initialize()
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.Initialize();
            }
        }

        public virtual void LoadContent()
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.LoadContent();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (CycSprite sprite in sprites)
            {
                sprite.Update(gameTime);
            }
        }
        protected virtual void SetupFilters()
        {
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.None;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.None;
        }
        protected virtual Color ClearColor()
        {
            return Color.Gray;
        }
        protected virtual void DrawInnards(GameTime gt)
        {
            GraphicsDevice.Clear(ClearColor());
        }
        public virtual void Draw(GameTime gameTime)
        {
            Viewport defaultVP = GraphicsDevice.Viewport;
            GraphicsDevice.Viewport = view;
            ((Game1)Game).SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            SetupFilters();
            DrawInnards(gameTime);
            foreach (CycSprite sprite in sprites)
            {
                sprite.Draw(gameTime);
            }
            ((Game1)Game).SpriteBatch.End();
            GraphicsDevice.Viewport = defaultVP;
        }
    }
}