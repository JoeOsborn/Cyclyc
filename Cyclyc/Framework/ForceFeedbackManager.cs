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

//borrowed from http://blog.nostatic.org/2009/08/xna-force-feedback-helper-class.html
public class Vibration
{
    float leftMotor;
    float rightMotor;
    float msLeft;

    public float LeftMotor
    {
        get { return leftMotor; }
        set { leftMotor = value; }
    }

    public float RightMotor
    {
        get { return rightMotor; }
        set { rightMotor = value; }
    }

    public float MSLeft
    {
        get { return msLeft; }
        set { msLeft = value; }
    }

    public Vibration(float leftMotor, float rightMotor, float durationMS)
    {
        this.leftMotor = leftMotor;
        this.rightMotor = rightMotor;
        this.msLeft = durationMS;
    }
}

public class ForceFeedbackManager
{
    private PlayerIndex playerIndex;
    List<Vibration> vibrations = new List<Vibration>();

    public ForceFeedbackManager(PlayerIndex playerIndex)
    {
        this.playerIndex = playerIndex;
    }

    public void AddVibration(float leftMotor, float rightMotor, float durationMS)
    {
        vibrations.Add(new Vibration(leftMotor, rightMotor, durationMS));
    }

    public void Update(float msElapsed)
    {
        List<Vibration> toDelete = new List<Vibration>();

        foreach (Vibration vibration in vibrations)
        {
            vibration.MSLeft -= msElapsed;

            if (vibration.MSLeft < 0.0f)
            {
                toDelete.Add(vibration);
            }
        }

        foreach (Vibration vibration in toDelete)
        {
            vibrations.Remove(vibration);
        }

        float leftMotor;
        float rightMotor;

        GetVibration(out leftMotor, out rightMotor);

        GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
    }

    public void GetVibration(out float leftMotor, out float rightMotor)
    {
        leftMotor = 0.0f;
        rightMotor = 0.0f;

        foreach (Vibration vibration in vibrations)
        {
            leftMotor = Math.Max(leftMotor, vibration.LeftMotor);
            rightMotor = Math.Max(rightMotor, vibration.RightMotor);
        }
    }
}