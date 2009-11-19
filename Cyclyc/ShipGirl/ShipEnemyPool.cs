
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
    public class ShipEnemyPool : EnemyPool
    {
        public ShipEnemyPool(CycGame g) : base(g)
        {
        }
        public ShipEnemy Create(Challenge c, string img, string curveSet, bool left, int y, int w, int h, double timeScale)
        {
            ShipEnemy enemy = (ShipEnemy)FindFreeEnemy();
            if (enemy == null)
            {
                enemy = new ShipEnemy(CycGame.Game, this);
                enemies.Add(enemy);
            }
            Console.WriteLine("y:" + y);
            enemy.Reset(c, img, curveSet, left, left ? 0 : 800, y, w, h, timeScale);
            return enemy;
        }
    }
}
