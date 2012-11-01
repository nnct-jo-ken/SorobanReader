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

using OpenCvSharp;
using System.Collections;
using SD = System.Drawing;
using SDI = System.Drawing.Imaging;

using SorobanCaptureLib;

namespace SorobanTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //CaptureCamera camera;
        //CvWindow win1;

        SorobanRealtimeReader r_reader;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // カメラデバイスの選択
            //camera = new CaptureCamera(0);
            //win1 = new CvWindow();

            r_reader = new SorobanRealtimeReader("Venus USB2.0 Camera", 0.4, "hoge");

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
            
            // カメラをアクティベート
            //camera.Activate(320, 240);
            r_reader.Start();

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

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content herea
            r_reader.Stop();
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

            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().First() == Keys.A)
            {
                r_reader.Stop();
            }
            if (Keyboard.GetState(PlayerIndex.One).GetPressedKeys().First() == Keys.S)
            {
                r_reader.Start();
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
            
            //CaptureImage capture = camera.Capture();
            //SorobanReader reader = new SorobanReader();
            //IplImage cap = capture.ToIplImage();
            //IplImage process_img;
            //reader.AllMatching(cap, 6, 0.4, out process_img);
            
            if(r_reader.IsReading)
            {
                //win1.ShowImage(r_reader.Process_img);
                CaptureImage img = r_reader.Capture_img;
                spriteBatch.Begin();
                spriteBatch.Draw(img.ToTexture(GraphicsDevice), new Vector2(0, 0), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
