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

namespace FrunWithXNA2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class StickPlatformer : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameStateEnum lastState = GameStateEnum.None;

        public StickPlatformer()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Utilities.Content = Content;

            // Screen size is 800x480
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.ApplyChanges();
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

            GameStateManager.GameState = GameStateEnum.MainMenu;
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
            try
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();

                // If we aren't in the game state playing, then we direct control to the Menu Manager
                if (GameStateManager.GameState != GameStateEnum.Playing)
                {
                    // The update function for MenuManager will handle the creation and user input involved
                    //  in managing Menus
                    MenuManager.Update(gameTime);

                    // Currently, all menus will reset the playing field... This may change if we create a Pause Menu
                    NpcManager.ClearNpcs();
                    return;
                }

                // Check to see if the GameState has changed since the last frame
                if (lastState != GameStateManager.GameState)
                {
                    // If the state has changed to Playing then we need to start a new game
                    if (GameStateManager.GameState == GameStateEnum.Playing)
                        StartNewGame();
                }

                // Tell the Player to update himself
                PlayerManager.GetPlayer().Update(gameTime);

                // Update all of the objects that have been added to the level (Maybe the manager should just have an Update call)
                for (int i = 0; i < LevelManager.ScreenObjects.Count; i++)
                {
                    if (LevelManager.ScreenObjects.Count > i)
                    {
                        LevelManager.ScreenObjects[i].Update(gameTime);
                    }
                }

                // Update the NPCs
                NpcManager.Update(gameTime);

                // Check and communicate any collisions that have occured
                CollisionManager.NotifyCollisions();

                base.Update(gameTime);

                // Save the state that this frame is in
                lastState = GameStateManager.GameState;
            }
            catch (Exception e) { }
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

            try
            {
                if (GameStateManager.GameState != GameStateEnum.Playing)
                { // If we are not playing, we are in a menu
                    MenuManager.Draw(spriteBatch);
                    return;
                }

                // Draw the level
                LevelManager.RenderLevel(spriteBatch);

                // Tell the player to draw itself
                if(PlayerManager.GetPlayer() != null)
                    PlayerManager.GetPlayer().Draw(spriteBatch);

                // Draw all the objects in the level
                for (int i = 0; i < LevelManager.ScreenObjects.Count; i++)
                {
                    if (LevelManager.ScreenObjects.Count > i)
                    {
                        LevelManager.ScreenObjects[i].Draw(spriteBatch);
                    }
                }

                // Draw the NPCs
                NpcManager.Draw(spriteBatch);

                CollisionManager.DrawCollisionRectangles(spriteBatch);
            }
            catch (Exception e) { }
            finally
            {
                spriteBatch.End();

                base.Draw(gameTime);
            }
        }


        private void StartNewGame()
        {
            // TODO: use this.Content to load your game content here
            LevelManager.LoadTextures(Content);

            // Load the player
            PlayerManager.CreateNewPlayer();
            
            // Load the starting level
            LevelManager.LoadLevel();
        }
    }
}
