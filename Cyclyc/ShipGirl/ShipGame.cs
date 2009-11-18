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
        ShipCircle crush;
        ShipCircle skim;
        ShipEnemyBatch enemyBatch;
        protected float crushRecovery;
        public ShipGame(Game1 game)
            : base(game)
        {
            enemyBatch = new ShipEnemyBatch(this);
        }

        public override void Initialize()
        {
            ship = new Ship(Game);
            AddSprite(ship);
            crush = new ShipCircle(Game, ship, "crushRing");
            AddSprite(crush);
            skim = new ShipCircle(Game, ship, "crushRing");
            AddSprite(skim);
            crushRecovery = 0;
            skim.ResizeTo(DefaultSkimRadius, 0);
            crush.ResizeTo(DefaultCrushRadius, 0);
            base.Initialize();
        }

        public override void LoadContent()
        {
            enemyBatch.Create("wrench", "wave", false, 150, 14, 14, 1.0);
            base.LoadContent();
        }
        public float SkimShrinkRate
        {
            get { return 1.0f; }
        }
        public float CrushGrowRate
        {
            get { return 1.0f; }
        }
        public float DefaultCrushRadius
        {
            get { return 32; }
        }
        public float DefaultSkimRadius
        {
            get { return 128; }
        }
        public float MaxCrushRadius
        {
            get { return 128; }
        }
        public float MinSkimRadius
        {
            get { return 24; }
        }
        public float SkimResizeDuration
        {
            get { return 0.25f; }
        }
        public float CrushCooldown
        {
            get { return 1.0f; }
        }
        public void KillShip()
        {
            Console.WriteLine("killed player");
        }
        public void Skim()
        {
            Console.WriteLine("skimmed");
            skim.ResizeTo(Math.Max(skim.Radius - SkimShrinkRate, MinSkimRadius), SkimResizeDuration);
            crush.ResizeTo(Math.Min(crush.Radius + CrushGrowRate, MaxCrushRadius), SkimResizeDuration);
        }
        public void Crush()
        {
            Console.WriteLine("crush");
            skim.ResizeTo(DefaultSkimRadius, CrushCooldown);
            crush.ResizeTo(DefaultCrushRadius, CrushCooldown);
            crushRecovery = CrushCooldown;
        }

        public override void Update(GameTime gameTime)
        {
            if (crushRecovery > 0)
            {
                crushRecovery -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                //ask batch to check collision
                //kill all dead things
                //kill stuff
                //--hemispherical crushing
            }
            enemyBatch.Update(gameTime);
            base.Update(gameTime);
            //check circle overlapping, ship collision
            //we'll just treat them all as circles
            List<ShipEnemy> shipCollided = enemyBatch.CollideCircle(ship.position, ship.Radius);
            if (shipCollided.Count() != 0)
            {
                KillShip();
            }
            if (crushRecovery <= 0)
            {
                List<ShipEnemy> skimCollided = enemyBatch.CollideCircle(skim.position, skim.Radius);
                if (skimCollided.Count() != 0)
                {
                    Skim();
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    Crush();
                }
            }
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