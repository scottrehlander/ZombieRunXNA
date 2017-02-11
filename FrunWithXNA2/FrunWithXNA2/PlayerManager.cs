using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FrunWithXNA2.Ammo;

namespace FrunWithXNA2
{
    public static class PlayerManager
    {
        public static Player player;
        
        public static Player GetPlayer()
        {
            return player;
        }

        public static List<ScreenObject> GetPlayers()
        {
            List<ScreenObject> players = new List<ScreenObject>();
            players.Add(GetPlayer());
            return players;
        }

        public static Player CreateNewPlayer()
        {
            player = new Player();
            player.LoadContent(Utilities.Content.Load<Texture2D>("ninja-trans"));
            player.ScreenPosition = new Vector2(600, 300);
            return player;
        }

        public static void PlayerDied(Player player)
        {
            // Tell the game that we need to reset
            player = null;
            GameStateManager.GameState = GameStateEnum.GameOver;
        }
    }

    public class Player : ScreenObject, ICollider
    {
        private enum EnumPlayerState { Running = 0, Jumping, Falling }

        public int playerHealth = 2;
        public int PlayerHealth { get { return playerHealth; } }

        public Texture2D Texture;

        SpriteFont courierFont;

        PlayerStatistics playerStats = new PlayerStatistics();
        public PlayerStatistics PlayerStats { get { return playerStats; } set { playerStats = value; } }

        bool playerFacingLeft = true;
        int framesSpentMovingInSameDirection = 0;

        // The view window for the current frame of sprite animation
        Rectangle viewRect;
        
        private GamePadState controller;
        private KeyboardState keyboard;
        private MouseState mouse;
        private EnumPlayerState playerState;

        private DateTime lastShotFired = DateTime.MinValue;
        private int shotAllowedInterval = 0;
        bool shootButtonDown = false;

        private const int X_SPEED = 200;


        // If this is windows phone 7, we need a hud
        WindowsPhoneHUD hud = new WindowsPhoneHUD();

        bool jumpButtonDownPreviousFrame = false;
        
        public Player()
        {
            LevelCollider = true;
            playerState = EnumPlayerState.Running;
        }

        public void LoadContent(Texture2D t)
        {
            Texture = t;
            ChangeToSprite(0);

            courierFont = Utilities.Content.Load<SpriteFont>("Courier New");

            hud.LoadContent(Utilities.Content);
        }

        public override void Update(GameTime gameTime)
        {
            playerStats.PlayerAliveTimer += gameTime.ElapsedGameTime;

            if (GameStateManager.GameState != GameStateEnum.Playing) return;

            controller = GamePad.GetState(PlayerIndex.One);
            keyboard = Keyboard.GetState(PlayerIndex.One);
            mouse = Mouse.GetState();

            // Make sure we update the state of our hud
            hud.Update(gameTime);

            // Manage the player state
            if (VerticalVelocity < 3)
                playerState = EnumPlayerState.Falling;
            if (VerticalVelocity <= 3 && VerticalVelocity >= -3) // This allows us to double jump, neat!
                playerState = EnumPlayerState.Running;
            if (VerticalVelocity > 3)
                playerState = EnumPlayerState.Jumping;

            // Handle Keypresses
            if (controller.ThumbSticks.Left.X < 0 || keyboard.IsKeyDown(Keys.A) || hud.LeftArrowDown)
            {
                // We need to check the position before we set it
                if (LevelManager.IsValidScreenPosition(ScreenPosition.X - X_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds, ScreenPosition.Y))
                    ScreenPosition.X -= X_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

                framesSpentMovingInSameDirection++;
                HandleRunningSpriteAnimation();

                playerFacingLeft = true;
            }
            else if (controller.ThumbSticks.Left.X > 0 || keyboard.IsKeyDown(Keys.D) || hud.RightArrowDown)
            {
                // We need to check the position before we set it
                if (LevelManager.IsValidScreenPosition(ScreenPosition.X + X_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds, ScreenPosition.Y))
                    ScreenPosition.X += X_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

                framesSpentMovingInSameDirection++;
                HandleRunningSpriteAnimation();

                playerFacingLeft = false;
            }
            else
            { 
                // We are not pushing left or right
                ChangeToSprite(0);
                framesSpentMovingInSameDirection = 0;
            }

            if (controller.Buttons.A == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Space) || hud.JumpButtonDown)
            {
                if (!jumpButtonDownPreviousFrame)
                {
                    // Only allow a jump if we are in a state less than jumping
                    if ((int)playerState < (int)EnumPlayerState.Jumping)
                    {
                        //playerState = EnumPlayerState.Jumping;
                        VerticalVelocity = 15;
                    }
                }

                // We set this so that if we are holding the space button we don't jump continuously
                jumpButtonDownPreviousFrame = true;
            }
            else
            {
                // If the jump button is not pushed, lets set the variable allowing us to jump again
                jumpButtonDownPreviousFrame = false;
            }

            // S button creates a bullet in the direction we are facing
            if (keyboard.IsKeyDown(Keys.S) || hud.ShootButtonDown)
            {
                if (!shootButtonDown && (DateTime.Now - lastShotFired).TotalMilliseconds > shotAllowedInterval)
                {
                    lastShotFired = DateTime.Now;
                    int xSpeed = 5;
                    if (playerFacingLeft)
                        xSpeed = -5;
                    Bullet bullet = new Bullet(new Vector2(xSpeed, 0), new Vector2(ScreenPosition.X, ScreenPosition.Y));
                    bullet.LoadContent(Utilities.Content.Load<Texture2D>("Ammo/bullet"));
                    LevelManager.ScreenObjects.Add(bullet);
                }
                shootButtonDown = true;
            }
            else
            {
                shootButtonDown = false;
            }

            // E button injects a block into the level
            if (keyboard.IsKeyDown(Keys.E) || hud.AddBlockButtonDown)
            {
                LevelManager.InjectBlock(ScreenPosition, playerFacingLeft);
            }

            // R button remove a block from the level
            if (keyboard.IsKeyDown(Keys.R) || hud.RemoveBlockButtonDown)
            {
                LevelManager.RemoveBlock(ScreenPosition, playerFacingLeft);
            }


            // If this method returns the same position, then a downward collision occured and gravity couldn't be applied
            Vector2 newPos = GravityManager.ApplyGravity(this);

            //if (newPos == ScreenPosition && (playerState == EnumPlayerState.Jumping || playerState == EnumPlayerState.Falling))
            //{
            //    playerState = EnumPlayerState.Running;
            //}

            ScreenPosition = newPos;
        }

        private void HandleRunningSpriteAnimation()
        {
            // If we are running, f everything
            if (playerState != EnumPlayerState.Running)
            {
                ChangeToSprite(0);
                return;
            }

            // If we have been running in the same direction for 3 frames, then we switch the sprite
            if (framesSpentMovingInSameDirection > 3)
            {
                ChangeToSprite(1);

                // If we have been moving for more than 6 frames, reset the sprite to 0
                if (framesSpentMovingInSameDirection > 5)
                {
                    framesSpentMovingInSameDirection = 0;
                }
            }
            else
            {
                ChangeToSprite(0);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (GameStateManager.GameState != GameStateEnum.Playing) return;

            // Draw time alive
            string output = string.Format("Time Alive:     {0:00}:{1:00}:{2:00}", 
                playerStats.PlayerAliveTimer.Minutes, playerStats.PlayerAliveTimer.Seconds, playerStats.PlayerAliveTimer.Milliseconds);

            // Draw the string
            spriteBatch.DrawString(courierFont, output, new Vector2(10, 5), Color.White,
                0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

            // Draw zombies killed
            string zombiesKilled = "Zombies Killed: " + playerStats.ZombiesKilled.ToString();

            // Draw the string
            spriteBatch.DrawString(courierFont, zombiesKilled, new Vector2(10, 25), Color.White,
                0, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.Draw(Texture, new Vector2(ScreenPosition.X, ScreenPosition.Y - 9),
                viewRect, Color.White, 0.0F, Vector2.Zero, 1.4F, !playerFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);

            // Only draw the HUD on WP7
#if WINDOWS || XBOX
#else
            hud.Draw(spriteBatch);   
#endif
        }

        public void ChangeToSprite(int frameNumber)
        {
            switch (frameNumber)
            {
                case 0:
                    viewRect = new Rectangle(0, 32, 24, 32);
                    break;
                case 1:
                    viewRect = new Rectangle(24, 32, 24, 32);
                    break;
                default:
                    viewRect = new Rectangle(0, 32, 24, 32);
                    break;
            }
        }



        #region ICollider Members

        public void CollisionInteraction(object collider)
        {
            if (collider is NPCs.Npc)
            {
                playerHealth--;
                if (playerHealth == 0)
                {
                    PlayerManager.PlayerDied(this);
                }
            }
        }

        Rectangle collisionRect = new Rectangle(0, 0, 24, 32);
        public Rectangle GetCollisionArea()
        {
            collisionRect.X = Convert.ToInt32(ScreenPosition.X);
            collisionRect.Y = Convert.ToInt32(ScreenPosition.Y);
            return collisionRect;
        }

        #endregion
    }

    public class PlayerStatistics
    {
        private TimeSpan playerAliveTimer;
        public TimeSpan PlayerAliveTimer { get { return playerAliveTimer; } set { playerAliveTimer = value; } }

        private int zombiesKilled = 0;
        public int ZombiesKilled { get { return zombiesKilled; } set { zombiesKilled = value; } }
    }

}
