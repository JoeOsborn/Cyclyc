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

namespace Cyclyc.Framework
{
    public class SongTrack
    {
        protected Game1 Game { get; set; }
        protected string SongName { get; set; }

        SoundEffect soundEffect;
        SoundEffectInstance soundInstance;

        protected double MaxVolume { get { return 0.2; } }

        public SongTrack(Game1 game, string songName, bool playNow)
        {
            Game = game;
            SongName = songName;
            ContentManager contentManager = new ContentManager(game.Services, @"Content\");
            soundEffect = contentManager.Load<SoundEffect>(SongName);
            soundInstance = soundEffect.CreateInstance();
            shouldPlay = playNow;
            if (playNow)
            {
                TargetVolume = MaxVolume;
                SrcVolume = MaxVolume;
                Volume = MaxVolume;
            }
            else
            {
                TargetVolume = 0.0;
                SrcVolume = 0.0;
                Volume = 0.0;
            }
            TargetMeasure = 0;
        }

        protected double Volume { get; set; }
        protected double SrcVolume { get; set; }
        protected double TargetVolume { get; set; }
        protected double TargetMeasure { get; set; }


        protected bool shouldPlay;
        public bool ShouldPlay
        {
            get { return shouldPlay; }
            set
            {
                if (!shouldPlay && value)
                {
                    SrcVolume = Volume;
                    TargetVolume = MaxVolume;
                    TargetMeasure = Math.Floor(Game.CurrentMeasure) + 1;
                }
                else if (shouldPlay && !value)
                {
                    SrcVolume = Volume;
                    TargetVolume = 0.0;
                    TargetMeasure = Math.Floor(Game.CurrentMeasure) + 1;
                }
                shouldPlay = value;
            }
        }

        public void Play()
        {
            soundInstance.Volume = (float)Volume;
            soundInstance.Play();
        }

        public void Update(GameTime gt)
        {
            if (TargetMeasure >= Game.CurrentMeasure)
            {
                Volume = TargetVolume;
                SrcVolume = Volume;
                soundInstance.Volume = (float)Volume;
            }
            else
            {
                Volume = MathHelper.Lerp((float)SrcVolume, (float)TargetVolume, (float)MathHelper.Clamp((float)(1 - (TargetMeasure - Game.CurrentMeasure)), 0, 1));
                soundInstance.Volume = (float)Volume;
            }
            
        }
    }
}
