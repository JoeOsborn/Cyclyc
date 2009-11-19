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
    public interface JetpackOwner
    {
        Vector2 Velocity { get; set; }

        float BottomEdge { get; set; }
        float FloorY { get; }

        bool ShouldMoveRight { get; }
        bool ShouldMoveLeft { get; }
        bool ShouldJump { get; }
        bool ShouldJet { get; }

        bool IsInAir { get; }
        bool FallingThroughGround { get; }
        bool OnGround { get; }

        void Jump();
        void Fall();
        void BeginJet();
        void MaintainJet();
        void FizzleJet();
        void StopJet();
        void Run();
        void Land();
    }
}
