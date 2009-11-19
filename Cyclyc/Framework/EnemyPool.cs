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
    public class EnemyPool
    {
        CycGame cycGame;
        public CycGame CycGame
        {
            get { return cycGame; }
        }

        protected List<CycEnemy> enemies;

        public EnemyPool(CycGame g)
        {
            cycGame = g;
            enemies = new List<CycEnemy>();
        }

        public virtual CycEnemy MakeEnemy()
        {
            return new CycEnemy(CycGame.Game, this);
        }

        public CycEnemy FindOrMakeEnemy()
        {
            CycEnemy enemy = FindFreeEnemy();
            if (enemy == null)
            {
                enemy = MakeEnemy();
                enemies.Add(enemy);
            }
            return enemy;
        }
        protected CycEnemy FindFreeEnemy()
        {
            foreach (CycEnemy e in enemies)
            {
                if (!e.Alive && !e.Visible)
                {
                    return e;
                }
            }
            return null;
        }
        public List<CycEnemy> Collide(CycSprite sprite)
        {
            List<CycEnemy> collided = new List<CycEnemy>();
            foreach (CycEnemy e in enemies)
            {
                if (!e.Alive) { continue; }
                if (e.Collide(sprite))
                {
                    collided.Add(e);
                }
            }
            return collided;
        }
        public void EnemyOffScreen(CycEnemy e)
        {

        }
        public void Update(GameTime gameTime)
        {
            foreach (CycSprite cs in enemies)
            {
                cs.View = cycGame.View;
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
