using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FrunWithXNA2.NPCs;

namespace FrunWithXNA2.Ammo
{
    enum EnumBulletState { Moving, Colliding }

    public class Bullet : ScreenObject, ICollider
    {
        // The view window for the current frame of sprite animation
        Rectangle viewRect;
        Texture2D Texture;
        Vector2 directionAndVelocity;
        Vector2 initialPosition;

        int maxDistance = 300;

        public Bullet(Vector2 directionVelocity, Vector2 startingPosition)
        {
            directionAndVelocity = directionVelocity;
            initialPosition = startingPosition;
            ScreenPosition = new Vector2(startingPosition.X, startingPosition.Y);
        }

        public void LoadContent(Texture2D t)
        {
            Texture = t;
            viewRect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public override void Update(GameTime gameTime)
        {
            if (!LevelManager.IsValidScreenPosition(ScreenPosition.X + directionAndVelocity.X, ScreenPosition.Y + directionAndVelocity.Y))
            {
                LevelManager.ScreenObjects.Remove(this);
                GC.Collect();
            }

            ScreenPosition.X += directionAndVelocity.X;
            ScreenPosition.Y += directionAndVelocity.Y;

            // Check the distance of the bullet
            if (ScreenPosition.X - initialPosition.X > maxDistance ||
                ScreenPosition.Y - initialPosition.Y > maxDistance ||
                ScreenPosition.X - initialPosition.X < -maxDistance ||
                ScreenPosition.Y - initialPosition.Y < -maxDistance)
            {
                if (LevelManager.ScreenObjects.Contains(this))
                {
                    LevelManager.ScreenObjects.Remove(this);
                    GC.Collect();
                }
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 12, 12),
                viewRect, Color.White, 0.0F, Vector2.Zero, SpriteEffects.None, 0.0F);
        }

        #region ICollider Members

        public void CollisionInteraction(object collider)
        {
            if (collider is Npc)
            {
                if (LevelManager.ScreenObjects.Contains(this))
                {
                    PlayerManager.GetPlayer().PlayerStats.ZombiesKilled++;
                    LevelManager.ScreenObjects.Remove(this);
                }
            }
        }

        Rectangle collisionRectangle = new Rectangle(0, 0, 15, 15);
        public Rectangle GetCollisionArea()
        {
            collisionRectangle.X = Convert.ToInt32(Math.Round(ScreenPosition.X));
            collisionRectangle.Y = Convert.ToInt32(Math.Round(ScreenPosition.Y));
            return collisionRectangle;
        }


        #endregion
    }
}
