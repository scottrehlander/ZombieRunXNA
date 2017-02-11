using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2.NPCs
{
    public class Basher : Npc, ICollider
    {
        private enum BasherState
        {
            Hatching,
            Patrolling
        }

        private int seed = int.MaxValue;
        private BasherState basherState = BasherState.Hatching;
        private Texture2D eggTexture;

        public override void LoadContent()
        {
            //if (seed < 0) seed = int.MaxValue;
            isFacingLeft = new Random().Next(0, 3) == 1 ? true : false;
            texture = Utilities.Content.Load<Texture2D>("NPCs/Basher1");
            eggTexture = Utilities.Content.Load<Texture2D>("NPCs/egg");
            LevelCollider = true;

            ScreenPosition = FindStartingPosition();
        }

        private Vector2 FindStartingPosition()
        {
            return PickRandomOpenSpot();   
        }

        TimeSpan timeSinceLastBash = new TimeSpan(0,0,1);
        TimeSpan hatchTime = new TimeSpan(0, 0, 3);
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (basherState == BasherState.Hatching)
            {
                if (timeSinceLastBash < hatchTime)
                {
                    timeSinceLastBash += gameTime.ElapsedGameTime;
                    return;
                }

                basherState = BasherState.Patrolling;
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

            // If there is a block here, smash it
            //  One in four chance to remove
            timeSinceLastBash += gameTime.ElapsedGameTime;
            if (timeSinceLastBash > new TimeSpan(0, 0, 1))
            {
                if (seed < 0) seed = int.MaxValue;
                if (new Random(seed--).Next(1, 5) == 3)
                    LevelManager.RemoveBlock(ScreenPosition, isFacingLeft);
                timeSinceLastBash = new TimeSpan(0, 0, 0);
            }

            if (LevelManager.IsValidScreenPosition(newPos.X, newPos.Y))
            {
                ScreenPosition = newPos;
            }
            else
            {
                if (newPos.X < 0 || newPos.X > 850)
                    NpcManager.RemoveNpc(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (basherState == BasherState.Hatching)
            {
                spriteBatch.Draw(eggTexture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 24, 32),
                new Rectangle(0, 0, 35, 50), Color.White, 0.0F, Vector2.Zero,
                isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
                return;
            }

            spriteBatch.Draw(texture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 24, 32),
            new Rectangle(0, 0, 24, 32), Color.White, 0.0F, Vector2.Zero,
            isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
        }

        #region ICollider Members

        public void CollisionInteraction(object collider)
        {
            if (basherState == BasherState.Hatching) return;

            if (collider is Player || collider is Ammo.Bullet)
                NpcManager.Npcs.Remove(this);
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
