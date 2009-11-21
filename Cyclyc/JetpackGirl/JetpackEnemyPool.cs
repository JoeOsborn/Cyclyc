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

namespace Cyclyc.JetpackGirl
{
    public class JetpackEnemyPool : EnemyPool
    {
        Random rgen;
        public JetpackEnemyPool(CycGame g)
            : base(g)
        {
            rgen = new Random();
        }

        public override CycEnemy MakeEnemy()
        {
            return new JetpackEnemy(CycGame.Game, this);
        }

        public JetpackEnemy Create(Challenge c, string img, int fc, bool left, int y, int w, int h, float speed, int bx, int by, int bw, int bh, int diff)
        {
            JetpackEnemy enemy = (JetpackEnemy)FindOrMakeEnemy();
            enemy.Reset(c, img, fc, left, (int)(left ? (0 - (rgen.NextDouble() * 32)) : (CycGame.View.Width/2 + (rgen.NextDouble()*32))), y, w, h, speed, bx, by, bw, bh, diff);
            return enemy;
        }
        public JetpackEnemy Create(Challenge c, string img, int fc, bool left, int y, int w, int h, float speed, int rad, int diff)
        {
            JetpackEnemy enemy = (JetpackEnemy)FindOrMakeEnemy();
            enemy.Reset(c, img, fc, left, (int)(left ? (0 - (rgen.NextDouble() * 32)) : (CycGame.View.Width/2 + (rgen.NextDouble() * 32))), y, w, h, speed, rad, diff);
            return enemy;
        }
    }
}
