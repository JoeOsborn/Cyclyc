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

namespace Cyclyc.ShipGirl
{
    public class ShipGame : Cyclyc.Framework.CycGame
    {
        Ship ship;
        CrushCircle crush;
        SkimCircle skim;
        ShipEnemyBatch enemyBatch;
        public ShipGame(Game1 game)
            : base(game)
        {
            enemyBatch = new ShipEnemyBatch(this);
        }

        public override void Initialize()
        {
            ship = new Ship(Game);
            AddSprite(ship);
            crush = new CrushCircle(Game, ship);
            AddSprite(crush);
            skim = new SkimCircle(Game, ship);
            AddSprite(skim);
            base.Initialize();
        }

        public override void LoadContent()
        {
            enemyBatch.Create("wrench", "wave", false, 150, 14, 14, 1.0);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            enemyBatch.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void SetupFilters()
        {
            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.GaussianQuad;
            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.GaussianQuad;
            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.GaussianQuad;
        }

        protected override void DrawInnards(GameTime gt)
        {
            base.DrawInnards(gt);
            enemyBatch.Draw(gt);
        }
    }
}