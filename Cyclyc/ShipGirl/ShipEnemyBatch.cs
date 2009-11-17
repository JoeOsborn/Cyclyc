
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

namespace Cyclyc.ShipGirl
{
    class ShipEnemyBatch
    {
        CycGame cycGame;
        public CycGame CycGame
        {
            get { return cycGame; }
        }

        protected List<ShipEnemy> enemies;
        public double TimeScale
        {
            set
            {
                foreach (ShipEnemy e in enemies)
                {
                    e.TimeScale = value;
                }
            }
        }

        public ShipEnemyBatch(CycGame g)
        {
            cycGame = g;
            enemies = new List<ShipEnemy>();
        }
        protected ShipEnemy FindFreeEnemy()
        {
            foreach (ShipEnemy e in enemies)
            {
                if (!e.Alive && !e.Visible)
                {
                    return e;
                }
            }
            return null;
        }
        public ShipEnemy Create(string img, string curveSet, bool left, int y, int w, int h, double timeScale)
        {
            ShipEnemy enemy = FindFreeEnemy();
            if (enemy == null)
            {
                enemy = new ShipEnemy(CycGame.Game);
                enemies.Add(enemy);
            }
            enemy.Reset(img, curveSet, left, y, w, h, timeScale);
            return enemy;
        }
        public void Update(GameTime gameTime)
        {
            foreach (CycSprite cs in enemies)
            {
                cs.Update(gameTime);
            }
        }
        public void Draw(GameTime gameTime)
        {
            foreach (CycSprite cs in enemies)
            {
                cs.Draw(gameTime);
            }
        }
    }
}
