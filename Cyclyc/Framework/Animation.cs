using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cyclyc.Framework
{
    public class Animation
    {
        protected bool playing;
        public bool Playing
        {
            get { return playing; }
            set { if (value) { Play(); } else { Stop(); } }
        }
        protected int currentTick;
        protected int nextTickOver;
        protected int currentFrameIdx;
        public int CurrentFrame
        {
            get { return frames[currentFrameIdx]; }
        }
        protected int[] frames;
        protected int[] frameTimings;
        protected bool loops;

        public Animation(int[] frameList, int[] timings, bool loop)
        {
            loops = loop;
            frames = frameList;
            frameTimings = timings;
            currentTick = 0;
            nextTickOver = 0;
            currentFrameIdx = 0;
            playing = false;
        }

        public void Tick()
        {
            if (!playing) { return; }
            currentTick++;
            if (currentTick == nextTickOver)
            {
                if (currentFrameIdx == frames.Count() - 1)
                {
                    if (loops)
                    {
                        Play();
                    }
                    else
                    {
                        Stop();
                    }
                }
                else
                {
                    currentFrameIdx++;
                    nextTickOver = currentTick + frameTimings[currentFrameIdx];
                }
            }
        }
        public void Play()
        {
            currentFrameIdx = 0;
            nextTickOver = frameTimings[0];
            currentTick = 0;
            playing = true;
        }
        public void Stop()
        {
            playing = false;
        }
    }
}
