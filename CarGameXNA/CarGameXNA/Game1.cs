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
        float tempdivider = 0;
        float groundY = 753f;
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
        Car playerCar;
        Texture2D playerCarTexture;
        Texture2D playerCarWheel;
        Scrolling scrolling1, scrolling2;
        float wheelRotation = 0f, playerRotation = 0f;
        Vector2 wheelPivot;
        Vector2 playerPivot;
        Vector2 playerPosition = new Vector2(100f, 1f);
        Vector2 wheelPosition = new Vector2(100f, 1f);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1600;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            playerCar = new Car("Volvo", DriveTrainType.RearWheelDrive, 1400);

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
            playerCarTexture = Content.Load<Texture2D>("bilXNA");
            font = Content.Load<SpriteFont>("XNAFont");
            playerCarWheel = Content.Load<Texture2D>("wheel");
            wheelPivot = new Vector2(playerCarWheel.Width / 2, playerCarWheel.Height / 2);
            playerPivot = new Vector2(playerCarTexture.Width / 2, playerCarTexture.Height / 2);
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
            _elapsed_time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_elapsed_time >= 1000.0f)
            {
                _fps = _total_frames;
                _total_frames = 0;
                _elapsed_time = 0;
            }
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Q))
            {
                if (!prevKeyGearUp)
                {
                    playerCar.GearUp();
                    prevKeyGearUp = true;
                }

            }
            else prevKeyGearUp = false;
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.A))
            {
                if (!prevKeyGearDown)
                {
                    playerCar.GearDown();
                    prevKeyGearDown = true;
                }

            }
            else prevKeyGearDown = false;


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Up))
                playerCar.engine.throttlePosition += 5;
            else
                playerCar.engine.throttlePosition -= 5;


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Down))
                playerCar.brakePosition += 5;
            else
                playerCar.brakePosition -= 5;

            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.R))
            {
                playerCar.GearReverse(true);
                prevKeyReverse = true;
            }
            else
            {
                if (prevKeyReverse)
                    prevKeyReverse = false;
            }
            
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Escape))
                this.Exit();



            playerCar.Calculate(gameTime.ElapsedGameTime.TotalMilliseconds);

#warning "Trial and error to find out values to divide speed with"
            // playerPosition.X += (float)playerCar.GetSpeed() / 7f;
            wheelPosition = playerPosition + new Vector2(45, 82);
            wheelRotation += (float)playerCar.GetSpeed() / 140f;

            if ((wheelPosition.Y + playerCarWheel.Height / 2 < groundY))
                velY += 9.81f / 1000f;
            else
            {
                velY = 0;
                playerPosition.Y = groundY - 82;
            }

            tempdivider = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000d);
            if (tempdivider != 0) playerPosition.Y += velY / tempdivider;


            playerRotation = -(float)(playerCar.getForceOnCarX(gameTime.ElapsedGameTime.TotalMilliseconds) / (gameTime.ElapsedGameTime.TotalMilliseconds * 200f));



            if (playerPosition.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - playerCarTexture.Width)
                playerPosition.X = 0;
            if (gameTime.TotalGameTime.TotalSeconds - powerUpdateTime > 0.1d)
            {
                powerUpdateTime = gameTime.TotalGameTime.TotalSeconds;
                power = (power + playerCar.engine.GetPowerHP()) / 2f;
            }

            // TODO: Add your update logic here
            if (playerCar.GetSpeed() > 0)
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



            scrolling1.Update(-(float)playerCar.GetSpeed() / 7f);
            scrolling2.Update(-(float)playerCar.GetSpeed() / 7f);
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
            spriteBatch.Draw(playerCarWheel, wheelPosition, null, Color.White, wheelRotation, wheelPivot, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(playerCarWheel, wheelPosition + new Vector2(120, 0), null, Color.White, wheelRotation, wheelPivot, 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(playerCarTexture, wheelPosition + new Vector2(55, -38), null, Color.White, playerRotation, playerPivot, 1.0f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(playerCarTexture, playerPosition, Color.White);
            spriteBatch.DrawString(font, String.Format("Speed: {0}\nRpm: {1}\nGear: {2}\nPower(Hp): {3}\nFps: {4}", (int)playerCar.GetSpeed(), playerCar.engine.currentRPM, playerCar.Gear, (int)power, _fps), Vector2.Zero, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
