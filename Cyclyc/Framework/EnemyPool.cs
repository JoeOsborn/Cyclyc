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
        public List<CycEnemy> CollideCircle(Vector2 pos, float rad)
        {
            List<CycEnemy> collided = new List<CycEnemy>();
            foreach (CycEnemy e in enemies)
            {
                if (!e.Alive) { continue; }
                float delta = (pos - e.position).Length();
                if (delta < rad || delta < e.Radius)
                {
                    collided.Add(e);
                }
            }
            return collided;
        }
        public void EnemyOffScreen(CycEnemy e)
        {
            cycGame.ChallengeIgnored(e);
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
