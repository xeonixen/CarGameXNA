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
        bool prevKeyGearUp, prevKeyGearDown;
        float power = 0;
        double powerUpdateTime=0;
        SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Car playerCar;
        Texture2D playerCarTexture;
        Vector2 playerPosition = new Vector2(0f, 600f);

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


            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().Contains<Keys>(Keys.Escape))
                this.Exit();



            playerCar.Calculate(gameTime.ElapsedGameTime.TotalMilliseconds);
            playerPosition.X += (float)playerCar.GetSpeed() / 10f;

            if (playerPosition.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - playerCarTexture.Width)
                playerPosition.X = 0;
            if (gameTime.TotalGameTime.TotalSeconds - powerUpdateTime > 0.1d)
            {
                powerUpdateTime = gameTime.TotalGameTime.TotalSeconds;
                power = (power + playerCar.engine.GetPowerHP()) / 2f;
            }
                // TODO: Add your update logic here

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
            spriteBatch.Draw(playerCarTexture, playerPosition, Color.White);
            spriteBatch.DrawString(font, String.Format("Speed: {0}\nRpm: {1}\nGear: {2}\nPower(Hp): {3}", playerCar.GetSpeed(),playerCar.engine.currentRPM, playerCar.Gear,(int)power), Vector2.Zero, Color.Black);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
