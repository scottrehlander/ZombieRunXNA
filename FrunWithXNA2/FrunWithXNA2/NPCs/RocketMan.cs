using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrunWithXNA2.NPCs
{
    public class RocketMan : Npc, ICollider
    {
        private enum RocketManState
        {
            Hatching,
            Patrolling,
            Shooting
        }

        private int seed = int.MaxValue;
        private int frameIncrement = 0;
        private int spriteMode = 0;
        private float xRocketSpeed = 2.5F;
        private Texture2D eggTexture;
        private RocketManState rocketManState = RocketManState.Hatching;

        private TimeSpan hatchTime = new TimeSpan(0, 0, 3);
        private TimeSpan timeSinceLastShot = new TimeSpan(0);
        private TimeSpan shootTimeInterval = new TimeSpan(0, 0, 3);

        public override void LoadContent()
        {
            isFacingLeft = new Random().Next(0, 2) == 1 ? true : false;
            texture = Utilities.Content.Load<Texture2D>("NPCs/RocketMan");
            eggTexture = Utilities.Content.Load<Texture2D>("NPCs/egg");
            LevelCollider = true;

            ScreenPosition = FindStartingPosition();
        }

        private Vector2 FindStartingPosition()
        {
            return PickRandomOpenSpot();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (rocketManState == RocketManState.Hatching)
            {
                if (timeSinceLastShot < hatchTime)
                {
                    timeSinceLastShot += gameTime.ElapsedGameTime;
                    return;
                }

                rocketManState = RocketManState.Patrolling;
            }

            Vector2 newPos = new Vector2(ScreenPosition.X, ScreenPosition.Y);
            newPos = GravityManager.ApplyGravity(this);

            if (!isFacingLeft)
            {
                newPos.X++;
            }
            else
            {
                newPos.X--;
            }


            if (timeSinceLastShot > shootTimeInterval)
            {
                // It is time to make a shot
                Ammo.Rocket rocket = new Ammo.Rocket(
                    GetVectorForAimedProjectile(xRocketSpeed, ScreenPosition, PlayerManager.GetPlayer().ScreenPosition),
                    new Vector2(ScreenPosition.X, ScreenPosition.Y));

                rocket.LoadContent(Utilities.Content.Load<Texture2D>("Ammo/Rocket-icon"));
                LevelManager.ScreenObjects.Add(rocket);

                timeSinceLastShot = new TimeSpan(0);
            }
            else
            {
                timeSinceLastShot += gameTime.ElapsedGameTime;
            }


            if (LevelManager.IsValidScreenPosition(newPos.X, newPos.Y))
            {
                ScreenPosition = newPos;
            }
            else
            {
                isFacingLeft = !isFacingLeft;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (rocketManState == RocketManState.Hatching)
            {
                spriteBatch.Draw(eggTexture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 24, 32),
                    new Rectangle(0, 0, 35, 50), Color.White, 0.0F, Vector2.Zero,
                    isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
                return;
            }

            // Switch back and forth
            frameIncrement++;
            if (frameIncrement == 10)
            {
                frameIncrement = 0;
                spriteMode++;

                if (spriteMode == 2) spriteMode = 0;
            }

            spriteBatch.Draw(texture, new Vector2(ScreenPosition.X, ScreenPosition.Y - 15),
                new Rectangle(72 * spriteMode, 105, 52, 85), Color.White, 0.0F, Vector2.Zero, .5F,
                !isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
        }


        #region ICollision Implementation

        public void CollisionInteraction(object collider)
        {
            if (rocketManState == RocketManState.Hatching)
                return;

            if (collider is Player || collider is Ammo.Bullet)
            {
                NpcManager.Npcs.Remove(this);
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
}