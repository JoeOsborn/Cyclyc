using System;
using System.Collections.Generic;
using System.Linq;
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
using Cyclyc.JetpackGirl;
using Cyclyc.ShipGirl;

namespace Cyclyc
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    enum GameState
    {
        Splash,
        Instructions,
        Playing
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        ShipGame shipGame;
        JetGame jetGame;
        GameState State { get; set; }
        public bool Playing
        {
            get { return State == GameState.Playing; }
        }

        EnemyPipe leftPipe;
        EnemyPipe rightPipe;

        public static Random Random;

        public double timePlayed;

        protected float splashTimer = 0;
        protected float SplashDuration
        {
            get { return 3.0f; }
        }

        ScreenComponent splash1, splash2, splash3, p1Instructions, p2Instructions;
        ScoreComponent p1Score, p2Score;
        ScreenComponent endingScreen;
        protected Dictionary<string, SoundEffect> sfx;
        public Game1()
        {
            sfx = new Dictionary<string, SoundEffect>();
            Random = new Random();
            State = GameState.Splash;
            //400 beats in; seconds = (bpm * mps)
            //timePlayed = (SongOutro * 4.0) / ((float)Tempo * (1.0 / 60.0));
            timePlayed = 0;
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

            leftPipe = new EnemyPipe(this);
            rightPipe = new EnemyPipe(this);

            shipGame = new ShipGame(this);
            jetGame = new JetGame(this);
            shipGame.NextGame = jetGame;
            jetGame.NextGame = shipGame;

            leftPipe.TopGame = shipGame;
            leftPipe.BottomGame = jetGame;
            rightPipe.TopGame = shipGame;
            rightPipe.BottomGame = jetGame;

            shipGame.LeftPipe = leftPipe;
            shipGame.RightPipe = rightPipe;
            jetGame.LeftPipe = leftPipe;
            jetGame.RightPipe = rightPipe;

            shipGame.PlayerIndex = PlayerIndex.One;
            jetGame.PlayerIndex = PlayerIndex.Two;

            splashTimer = 0;
        }
        public double SongTotalEnding
        {
            get { return 108; }
        }
        public double SongEnd
        {
            get { return 105; }
        }
        public bool SongIsOver
        {
            get { return CurrentMeasure >= SongEnd; }
        }
        public double SongOutro
        {
            get { return 95; }
        }
        public bool SongIsEnding
        {
            get { return CurrentMeasure >= SongOutro; }
        }
        public double SongDenouement
        {
            get { return 90; }
        }
        public bool SongIsEndingSoon
        {
            get { return CurrentMeasure >= SongDenouement; }
        }
        public double OutroRatio
        {
            get
            {
                double measuresOut = (CurrentMeasure - SongOutro);
                double outroLength = SongEnd - SongOutro;
                return Math.Min(1.0, measuresOut / outroLength);
            }
        }
        public int Tempo
        {
            get { return 180; }
        }
        public double CurrentBeat
        {
            get { return (timePlayed / 60.0) * Tempo; }
        }
        public double CurrentMeasure
        {
            get { return (int)(CurrentBeat / 4.0); }
        }

        public static float RandomBetween(float min, float max)
        {
            return min + (float)Random.NextDouble() * (max - min);
        }

        public SoundEffectInstance SoundInstance(string n)
        {
            if (sfx.ContainsKey(n))
            {
                return sfx[n].CreateInstance();
            }
            sfx[n] = Content.Load<SoundEffect>(n);
            return sfx[n].CreateInstance();
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            shipGame.Initialize();
            jetGame.Initialize();
            leftPipe.Initialize();
            rightPipe.Initialize();

            p1Instructions = new ScreenComponent(this, "sgIntro");
            p2Instructions = new ScreenComponent(this, "rpgIntro");

            splash1 = new ScreenComponent(this, "splash1");
            splash2 = new ScreenComponent(this, "splash2");
            splash3 = new ScreenComponent(this, "splash3");

            endingScreen = new ScreenComponent(this, "ending");

            Components.Add(splash1);
            splashTimer = SplashDuration;
            base.Initialize();
        }

        protected int PipeWidth
        {
            get { return 24; }
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Viewport upperView = GraphicsDevice.Viewport;
            upperView.Height /= 2;
            upperView.X = PipeWidth;
            upperView.Width -= PipeWidth*2;
            Viewport lowerView = upperView;
            lowerView.Y += lowerView.Height;

            shipGame.View = upperView;
            jetGame.View = lowerView;

            shipGame.LoadContent();
            jetGame.LoadContent();

            leftPipe.Height = GraphicsDevice.Viewport.Height;
            leftPipe.Width = PipeWidth;
            rightPipe.X = lowerView.X + lowerView.Width;
            rightPipe.Width = PipeWidth;
            rightPipe.Height = leftPipe.Height;
            //add a couple of pipes!

            p1Score = new ScoreComponent(this);
            p1Score.Width = GraphicsDevice.Viewport.Width;
            p1Score.Y = 16;
            p2Score = new ScoreComponent(this);
            p2Score.Width = GraphicsDevice.Viewport.Width;
            p2Score.Y = 316;

            leftPipe.LoadContent();
            rightPipe.LoadContent();
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            base.UnloadContent();
        }

        public void PlayIfNotPlaying(SoundEffectInstance snd)
        {
            if (snd.State == SoundState.Stopped)
            {
                snd.Play();
            }
        }

        protected void StartPlaying()
        {
            foreach (SongTrack s in shipGame.Songs)
            {
                s.Play();
            }
            foreach (SongTrack s in jetGame.Songs)
            {
                s.Play();
            }
            State = GameState.Playing;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            //update beat and measure counters
            if (Playing)
            {
                timePlayed += gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (State == GameState.Playing || State == GameState.Instructions)
            {
                shipGame.Update(gameTime);
                jetGame.Update(gameTime);
                leftPipe.Update(gameTime);
                rightPipe.Update(gameTime);
            }
            if (State == GameState.Instructions)
            {
                if ((p1Instructions != null) && (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.RightShift)))
                {
                    Components.Remove(p1Instructions);
                    p1Instructions = null;
                }
                if ((p2Instructions != null) && (GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.A) || Keyboard.GetState().IsKeyDown(Keys.LeftShift)))
                {
                    Components.Remove(p2Instructions);
                    p2Instructions = null;
                }
                if ((p1Instructions == null) && (p2Instructions == null))
                {
                    StartPlaying();
                }
            }
            if (State == GameState.Splash)
            {
                //update splash
                if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start) || GamePad.GetState(PlayerIndex.Two).IsButtonDown(Buttons.Start) || Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    if (Components.Contains(splash1))
                    {
                        Components.Remove(splash1);
                    }
                    if (Components.Contains(splash2))
                    {
                        Components.Remove(splash2);
                    }
                    if (Components.Contains(splash3))
                    {
                        Components.Remove(splash3);
                    }
                    Components.Add(p1Instructions);
                    Components.Add(p2Instructions);
                    State = GameState.Instructions;
                }
                splashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (splashTimer <= 0)
                {
                    if (Components.Contains(splash1))
                    {
                        Components.Remove(splash1);
                        Components.Add(splash2);
                        splashTimer = SplashDuration;
                    }
                    else if (Components.Contains(splash2))
                    {
                        Components.Remove(splash2);
                        Components.Add(splash3);
                    }
                }
            }
            if (State == GameState.Playing && SongIsOver)
            {
                if (!Components.Contains(p1Score))
                {
                    Components.Add(p1Score);
                }
                if (!Components.Contains(p2Score))
                {
                    Components.Add(p2Score);
                }
                p1Score.Score = shipGame.Score;
                p2Score.Score = jetGame.Score;
                if (CurrentMeasure >= SongTotalEnding && !Components.Contains(endingScreen))
                {
                    Components.Add(endingScreen);
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Viewport defaultVP = GraphicsDevice.Viewport;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (State == GameState.Playing || State == GameState.Instructions)
            {
                shipGame.Draw(gameTime);
                jetGame.Draw(gameTime);
                SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                leftPipe.Draw(gameTime);
                rightPipe.Draw(gameTime);
                SpriteBatch.End();
            }
            base.Draw(gameTime);
            GraphicsDevice.Viewport = defaultVP;
        }
    }
}
