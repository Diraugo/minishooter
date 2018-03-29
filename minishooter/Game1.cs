using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace minishooter
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    public class Schuss
    {
        Texture2D Textur;
        Vector2 Position;
        
        public Schuss(Texture2D pTextur,Vector2 pPosition)
        {
            this.Textur = pTextur;
            this.Position = pPosition;   
        }

        public void setPosition(float Bewegung)
        {
            this.Position.Y += Bewegung;
        }

        public Texture2D getTextur()
        {
            return this.Textur;
        }

        public Vector2 getPosition()
        {
            return this.Position;
        }

    }
    
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D HintergundTextur;
        Texture2D SchiffTextur;
        Texture2D FeindTextur;
        Texture2D MeinSchussTextur;
        Texture2D FeindSchussTextur;

        SpriteFont WartenText;

        float ShipSpeed;
        float EnemySpeed;
        float BulletSpeed;

        Vector2 SchiffPosition;
        Vector2 FeindPosition;
        Vector2 StartSpieler;
        Vector2 StartFeind;

        Rectangle SchiffBox;
        Rectangle FeindBox;
        Rectangle SchussBox;
        Rectangle SchussFeindBox;

        List<Schuss> MeineSchuesse=new List<Schuss>();
        List<Schuss> FeindSchuesse=new List<Schuss>();

        Random rnd;

        int FeindLinks;
        int WarteZeitUebrig;
        int Punkte;
        int PunkteFeind;

        GameTime WarteZeit;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            rnd = new Random();
            ShipSpeed = 150f;
            EnemySpeed = rnd.Next(100,201);
            BulletSpeed = 200f;
            StartSpieler = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight - 50);
            StartFeind = new Vector2(graphics.PreferredBackBufferWidth / 2, 50);
            SchiffPosition = StartSpieler;
            FeindPosition = StartFeind;
            
            FeindLinks = rnd.Next(0,2);
            WarteZeit = new GameTime();
            Punkte = 0;
            PunkteFeind = 0;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            HintergundTextur = Content.Load<Texture2D>("hintergrund");
            SchiffTextur = Content.Load<Texture2D>("schiff");
            FeindTextur = Content.Load<Texture2D>("schiff_feind");
            MeinSchussTextur = Content.Load<Texture2D>("schuss");
            FeindSchussTextur = Content.Load<Texture2D>("schuss_feind");
            WartenText = Content.Load<SpriteFont>("Warten");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();
            WarteZeitUebrig = 3 - (int)(gameTime.TotalGameTime.TotalSeconds - WarteZeit.TotalGameTime.TotalSeconds);
            if (gameTime.TotalGameTime.TotalSeconds - WarteZeit.TotalGameTime.TotalSeconds > 3)
            {
                if (kstate.IsKeyDown(Keys.Up))
                {
                    SchiffPosition.Y = SchiffPosition.Y - ShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Down))
                {
                    SchiffPosition.Y = SchiffPosition.Y + ShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Left))
                {
                    SchiffPosition.X = SchiffPosition.X - ShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Right))
                {
                    SchiffPosition.X = SchiffPosition.X + ShipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                if (kstate.IsKeyDown(Keys.Space))
                {
                    if (MeineSchuesse.Count < 6 && gameTime.TotalGameTime.Milliseconds % 200 == 0)
                    {
                        MeineSchuesse.Add(new Schuss(MeinSchussTextur, SchiffPosition));
                    }

                }

                if (gameTime.TotalGameTime.Milliseconds % 600 == 0)
                {
                    FeindSchuesse.Add(new Schuss(FeindSchussTextur, FeindPosition));
                }

                if (FeindLinks == 0)
                {
                    FeindPosition.X = FeindPosition.X - EnemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if ((int)FeindPosition.X < FeindTextur.Width / 2)
                    {
                        FeindLinks = 1;
                    }
                }
                else
                {
                    FeindPosition.X = FeindPosition.X + EnemySpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (FeindPosition.X > graphics.PreferredBackBufferWidth - FeindTextur.Width / 2)
                    {
                        FeindLinks = 0;
                    }
                }

                SchiffPosition.X = Math.Min(Math.Max(SchiffTextur.Width / 2, SchiffPosition.X), graphics.PreferredBackBufferWidth - SchiffTextur.Width / 2);
                SchiffPosition.Y = Math.Min(Math.Max(SchiffTextur.Height / 2, SchiffPosition.Y), graphics.PreferredBackBufferHeight - SchiffTextur.Height / 2);
                FeindPosition.X = Math.Min(Math.Max(FeindTextur.Width / 2, FeindPosition.X), graphics.PreferredBackBufferWidth - FeindTextur.Width / 2);
                FeindPosition.Y = Math.Min(Math.Max(FeindTextur.Height / 2, FeindPosition.Y), graphics.PreferredBackBufferHeight - FeindTextur.Height / 2);

                SchiffBox = new Rectangle((int)SchiffPosition.X-SchiffTextur.Width/2, (int)SchiffPosition.Y-SchiffTextur.Height/2, SchiffTextur.Width, SchiffTextur.Height);
                FeindBox = new Rectangle((int)FeindPosition.X-FeindTextur.Width/2, (int)FeindPosition.Y-FeindTextur.Height/2, FeindTextur.Width, FeindTextur.Height);
                if (SchiffBox.Intersects(FeindBox))
                {
                    WarteZeit = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                    PunkteFeind++;
                    RestartGame();
                }

                for (int i = 0; i < MeineSchuesse.Count; i++)
                {
                    MeineSchuesse[i].setPosition(-BulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    SchussBox = new Rectangle((int)MeineSchuesse[i].getPosition().X, (int)MeineSchuesse[i].getPosition().Y, MeinSchussTextur.Width, MeinSchussTextur.Height);

                    if (MeineSchuesse[i].getPosition().Y <= 0)
                    {
                        MeineSchuesse.RemoveAt(i);
                    }
                    if (SchussBox.Intersects(FeindBox))
                    {
                        Punkte++;
                        WarteZeit = new GameTime( gameTime.TotalGameTime,gameTime.ElapsedGameTime );
                        RestartGame();
                    }
                }

                for (int i = 0; i < FeindSchuesse.Count; i++)
                {
                    FeindSchuesse[i].setPosition(BulletSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    SchussFeindBox = new Rectangle((int)FeindSchuesse[i].getPosition().X, (int)FeindSchuesse[i].getPosition().Y, FeindSchussTextur.Width, FeindSchussTextur.Height);

                    if (FeindSchuesse[i].getPosition().Y <= 0)
                    {
                        FeindSchuesse.RemoveAt(i);
                    }
                    if (SchussFeindBox.Intersects(SchiffBox))
                    {
                        PunkteFeind++;
                        WarteZeit = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime);
                        RestartGame();
                    }
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            
            spriteBatch.Draw(HintergundTextur,new Vector2(0, 0), Color.White);
            if (WarteZeitUebrig >= 0)
            {
                spriteBatch.DrawString(WartenText, WarteZeitUebrig.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, 150), Color.Yellow);
            }
            foreach (var s in MeineSchuesse)
            {
                spriteBatch.Draw(s.getTextur(), s.getPosition(), null, null, new Vector2(s.getTextur().Width / 2, s.getTextur().Height / 2), 0f, null, Color.White, SpriteEffects.None, 0f);
            }
            foreach (var s in FeindSchuesse)
            {
                spriteBatch.Draw(s.getTextur(), s.getPosition(), null, null, new Vector2(s.getTextur().Width / 2, s.getTextur().Height / 2), 0f, null, Color.White, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(SchiffTextur,SchiffPosition,null,null, new Vector2(SchiffTextur.Width/2,SchiffTextur.Height/2),0f,null,Color.White,SpriteEffects.None,0f);
            spriteBatch.Draw(FeindTextur, FeindPosition, null, null, new Vector2(FeindTextur.Width / 2, FeindTextur.Height / 2), 0f, null, Color.White, SpriteEffects.None, 0f);
            spriteBatch.DrawString(WartenText, Punkte.ToString(), new Vector2(10,graphics.PreferredBackBufferHeight -20), Color.Coral);
            spriteBatch.DrawString(WartenText, PunkteFeind.ToString(), new Vector2(10, 10), Color.Coral);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void RestartGame()
        {
            SchiffPosition = StartSpieler;
            FeindPosition = StartFeind;
            MeineSchuesse.Clear();
            FeindSchuesse.Clear();
            FeindLinks = rnd.Next(0, 2);
            EnemySpeed = rnd.Next(100, 201);
            
        }
    }
}
