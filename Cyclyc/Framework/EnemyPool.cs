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
    public struct CollisionGroup
    {
        public CycSprite collider;
        public List<CycSprite> collided;
    }

    public class EnemyPool
    {
        CycGame cycGame;
        public CycGame CycGame
        {
            get { return cycGame; }
        }

        protected List<CycSprite> enemies;
        public List<CycSprite> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }

        public EnemyPool(CycGame g)
        {
            cycGame = g;
            enemies = new List<CycSprite>();
        }

        public virtual CycSprite MakeEnemy()
        {
            return null;
        }

        public CycSprite FindOrMakeEnemy()
        {
            CycSprite enemy = FindFreeEnemy();
            if (enemy == null)
            {
                enemy = MakeEnemy();
                enemies.Add(enemy);
            }
            return enemy;
        }
        protected CycSprite FindFreeEnemy()
        {
            foreach (CycSprite e in enemies)
            {
                if (!e.Alive && !e.Visible)
                {
                    return e;
                }
            }
            return null;
        }
        public List<CycSprite> Collide(CycSprite sprite)
        {
            List<CycSprite> collided = new List<CycSprite>();
            foreach (CycSprite e in enemies)
            {
                if (!e.Alive) { continue; }
                if (e.Collide(sprite))
                {
                    collided.Add(e);
                }
            }
            return collided;
        }
        public List<CollisionGroup> CollidePool(EnemyPool pool)
        {
            List<CollisionGroup> collisions = new List<CollisionGroup>();
            foreach (CycSprite e in enemies)
            {
                if (!e.Alive) { continue; }
                List<CycSprite> collided = pool.Collide(e);
                if (collided.Count > 0)
                {
                    collisions.Add(new CollisionGroup { collider = e, collided = collided });
                }
            }
            return collisions;
        }
        public void EnemyOffScreen(CycSprite e)
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
