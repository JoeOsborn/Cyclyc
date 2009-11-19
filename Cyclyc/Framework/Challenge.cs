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
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;

namespace Cyclyc.Framework
{
    public enum ChallengeState
    {
        NotYet,
        Skipped,
        Active,
        Deployed
    }
    public class Challenge
    {
        protected CycGame cycGame;
        public CycGame CycGame
        {
            get { return cycGame; }
            set { cycGame = value; }
        }
        protected Game1 game;
        public Game1 Game
        {
            get { return game; }
            set { game = value; }
        }

        protected ChallengeState state;
        public ChallengeState State
        {
            get { return state; }
            set { state = value; }
        }

        protected int measure;

        protected List<ChallengeBeat> beats;

        protected int enemiesIgnored;
        public int EnemiesIgnored
        {
            get { return enemiesIgnored; }
        }
        protected int enemiesKilled;
        public int EnemiesKilled
        {
            get { return enemiesKilled; }
        }
        protected int enemyCount;
        public int EnemyCount
        {
            get { return enemyCount; }
        }

        public Challenge(int m)
        {
            state = ChallengeState.NotYet;
            measure = m;
            beats = new List<ChallengeBeat>();
            enemiesIgnored = 0;
            enemiesKilled = 0;
            enemyCount = 0;
        }

        public void AddBeat(ChallengeBeat beat)
        {
            beats.Add(beat);
            enemyCount += beat.Enemies.Length;
        }

        public void EnemyKilled(CycEnemy enemy)
        {
            enemiesKilled++;
        }

        public void EnemyIgnored(CycEnemy enemy)
        {
            enemiesIgnored++;
        }

        public void Process(float expectedGrade, float actualGrade, bool changeState)
        {
            if (state == ChallengeState.NotYet && game.CurrentMeasure >= measure)
            {
                if (expectedGrade > actualGrade)
                {
                    if (changeState)
                    {
                        state = ChallengeState.Skipped;
                    }
                }
                else
                {
                    if (changeState) 
                    { 
                        state = ChallengeState.Active; 
                    }
                }
            }
            else if (state == ChallengeState.Active)
            {
                bool anyUnsent = false;
                foreach (ChallengeBeat beat in beats)
                {
                    if (beat.Unsent && (beat.Beat + (measure * 4)) <= game.CurrentBeat)
                    {
                        beat.Unsent = false;
                        foreach (EnemyMaker em in beat.Enemies)
                        {
                            em(this);
                        }
                    }
                    else
                    {
                        anyUnsent = true;
                    }
                }
                if (!anyUnsent && changeState)
                {
                    state = ChallengeState.Deployed;
                }
            }
        }

    }
}
