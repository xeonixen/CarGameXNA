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
using CarGameEngine;

namespace CarGameXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        float groundY = 755f;
        float velY = 0f;
        int _total_frames = 0;
        float _elapsed_time = 0.0f;
        int _fps = 0;
        bool prevKeyGearUp, prevKeyGearDown, prevKeyReverse;
        float power = 0;
        double powerUpdateTime = 0;
        SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Scrolling scrolling1, scrolling2;
        float playerXPositionInWorld = 100f;
        CarController playerCar;





        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
            //graphics.IsFullScreen = true;
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
            playerCar = new CarController();
            playerCar.physics = new Car("Volvo", DriveTrainType.RearWheelDrive, 1400);

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
            font = Content.Load<SpriteFont>("XNAFont");
            playerCar.bodyTexture = Content.Load<Texture2D>("bilXNA");
            playerCar.wheelTexture = Content.Load<Texture2D>("wheel");

            scrolling1 = new Scrolling(Content.Load<Texture2D>("background"), new Vector2(0, 0));
            scrolling2 = new Scrolling(Content.Load<Texture2D>("background"), new Vector2(scrolling1.texture.Width, 0));

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Fps related
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
            // Keyboard related
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Q))
            {
                if (!prevKeyGearUp)
                {
                    playerCar.physics.GearUp();
                    prevKeyGearUp = true;
                }

            }
            else prevKeyGearUp = false;
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.A))
            {
                if (!prevKeyGearDown)
                {
                    playerCar.physics.GearDown();
                    prevKeyGearDown = true;
                }

            }
            else prevKeyGearDown = false;


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Up))
                playerCar.physics.engine.throttlePosition += 5;
            else
                playerCar.physics.engine.throttlePosition -= 5;


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Down))
                playerCar.physics.brakePosition += 5;
            else
                playerCar.physics.brakePosition -= 5;

            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.R))
            {
                playerCar.physics.GearReverse(true);
                prevKeyReverse = true;
            }
            else
            {
                if (prevKeyReverse)
                {
                    prevKeyReverse = false;
                    playerCar.physics.GearReverse(false);
                }

            }


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Escape))
                this.Exit();




            // Physics
            
            playerCar.physics.Calculate(gameTime.ElapsedGameTime.TotalMilliseconds);
            playerCar.CalculatePositions();
#warning "Trial and error to find out values to divide speed with"
            playerCar.wheelRotation += (float)playerCar.physics.GetSpeed() / 140f;

            //if (playerCar.position.Y < groundY)
            velY += 9.81f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f;
            //else
            //{
            //    velY = 0;
            //    playerCar.position.Y = groundY;
            //}
            if (playerCar.wheel1pos.Y - groundY >= 0)
            {
                playerCar.spring1 = -(playerCar.wheel1pos.Y - groundY);
                playerCar.wheel1pos.Y = groundY;
            }
            else playerCar.spring1 = 0;

            if (playerCar.wheel2pos.Y - groundY >= 0)
            {
                playerCar.spring2 = -(playerCar.wheel2pos.Y - groundY);
                playerCar.wheel2pos.Y = groundY;
            }
            else playerCar.spring2 = 0;

            // if (playerCar.wheel2pos.Y > groundY)
            // {
            //     playerCar.wheel2pos.Y = groundY;


            // }
            // playerCar.spring2=
            float vel1=0, vel2=0;
            if (playerCar.spring1 < 0)
               vel1 = (9.81f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f) * ((playerCar.wheel1pos.Y - playerCar.spring1) - groundY) * 0.2f + (0.1f * velY);
            if (playerCar.spring2 < 0)
                vel2 = (9.81f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f) * ((playerCar.wheel2pos.Y - playerCar.spring2) - groundY) * 0.2f + (0.1f * velY);

            velY -= vel1 + vel2;

             if (vel1 > vel2)
            {
                playerCar.rotation -= ((vel1 - vel2) / 8f);
               
            }
            else
            {
                playerCar.rotation += ((vel2 - vel1 )/ 8f);
               
            }

            // velY -= (9.81f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f) * (playerCar.wheel2pos.Y - groundY) * 0.9f + (0.9f * velY);
            // if (playerCar.wheel1pos.Y > groundY)
            //    velY -= (9.81f * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f) * (playerCar.wheel1pos.Y - groundY) * 0.9f + (0.9f * velY);


            playerCar.position.Y += velY;

            float force = (float)playerCar.physics.getForceOnCarX(gameTime.ElapsedGameTime.TotalMilliseconds)/ (float)(gameTime.ElapsedGameTime.TotalMilliseconds * 800f);
            if (force > 100f || force < -100f) force = 0;
            playerCar.rotation -= force;// -(float)(playerCar.physics.getForceOnCarX(gameTime.ElapsedGameTime.TotalMilliseconds) / (gameTime.ElapsedGameTime.TotalMilliseconds * 10000f));



            if (gameTime.TotalGameTime.TotalSeconds - powerUpdateTime > 0.1d)
            {
                powerUpdateTime = gameTime.TotalGameTime.TotalSeconds;
                power = (power + playerCar.physics.engine.GetPowerHP()) / 2f;
            }

            if (playerCar.physics.GetSpeed() > 0)
            {
                if (scrolling1.vector.X + scrolling1.texture.Width <= 0)
                    scrolling1.vector.X = scrolling2.vector.X + scrolling2.texture.Width;
                if (scrolling2.vector.X + scrolling2.texture.Width <= 0)
                    scrolling2.vector.X = scrolling1.vector.X + scrolling1.texture.Width;
            }
            else
            {
                if (scrolling1.vector.X >= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    scrolling1.vector.X = scrolling2.vector.X - scrolling1.texture.Width;
                if (scrolling2.vector.X >= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    scrolling2.vector.X = scrolling1.vector.X - scrolling2.texture.Width;
            }



            scrolling1.Update(-playerCar.physics.GetSpeed() / 7f);
            scrolling2.Update(-playerCar.physics.GetSpeed() / 7f);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _total_frames++;
            GraphicsDevice.Clear(Color.LightGreen);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            scrolling1.Draw(spriteBatch);
            scrolling2.Draw(spriteBatch);
            playerCar.Draw(spriteBatch);
            spriteBatch.DrawString(font, String.Format("Speed: {0}\nRpm: {1}\nGear: {2}\nPower(Hp): {3}\nFps: {4}\nPosition: {5}", (int)Math.Abs(playerCar.physics.GetSpeed()), playerCar.physics.engine.currentRPM, playerCar.physics.Gear, (int)power, _fps, (int)playerXPositionInWorld), Vector2.Zero, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
    public class PosRot
    {
        public float distance;
        public float angle;
        public PosRot()
        {
            distance = 0f;
            angle = 0f;
        }
        public PosRot(float dist, float ang)
        {
            distance = dist;
            angle = ang;
        }
    }
    public class CarController
    {
        public Vector2 position;
        public Vector2 wheel1pos, wheel2pos;
        public Vector2 wheel1offset, wheel2offset;
        public float spring1, spring2;
        public Texture2D bodyTexture, wheelTexture;
        public float speed;
        public float rotation, wheelRotation;
        public Car physics;


        public CarController()
        {
            physics = new Car();
            speed = rotation = 0f;
            position = new Vector2(500f, 50f);
            wheel1pos = new Vector2(0f, 0f);
            wheel2pos = new Vector2(0f, 0f);
            wheel1offset = new Vector2(60f, 17.0f);
            wheel2offset = new Vector2(-60f, 17.0f);
            wheelRotation = 0f;
            spring1 = spring2 = 0f;

        }
        public void CalculatePositions()
        {
            wheel1pos = position + new Vector2(wheel1offset.X * (float)Math.Cos(rotation), wheel1offset.X * (float)Math.Sin(rotation));
            wheel1pos = wheel1pos + new Vector2((wheel1offset.Y+spring1) * (float)Math.Sin(rotation), (wheel1offset.Y+spring1) * (float)Math.Cos(rotation));
            wheel2pos = position + new Vector2(wheel2offset.X * (float)Math.Cos(rotation), wheel2offset.X * (float)Math.Sin(rotation));
            wheel2pos = wheel2pos + new Vector2((wheel2offset.Y+spring2) * (float)Math.Sin(rotation), (wheel2offset.Y+spring2) * (float)Math.Cos(rotation));
            
        }
        public void Draw(SpriteBatch spriteBatch)
        {
           // CalculatePositions();
            spriteBatch.Draw(wheelTexture, wheel1pos, null, Color.White, wheelRotation, new Vector2(wheelTexture.Width / 2, wheelTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(wheelTexture, wheel2pos, null, Color.White, wheelRotation, new Vector2(wheelTexture.Width / 2, wheelTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(bodyTexture, position, null, Color.White, rotation, new Vector2(bodyTexture.Width / 2, bodyTexture.Height / 2), 1.0f, SpriteEffects.None, 0f);
        }
    }
}
