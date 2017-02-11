using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2.NPCs
{
    public class PatrolManNpc : Npc, ICollider
    {
        public enum PatrolManState { Patrolling, BeingThrown }

        private int seed = int.MaxValue;
        private int frameIncrement = 0;
        private int spriteMode = 0;
        PatrolManState state = PatrolManState.Patrolling; Vector2 throwVector = Vector2.Zero;

        public override void LoadContent()
        {
            isFacingLeft = new Random().Next(0, 2) == 1 ? true : false;
            texture = Utilities.Content.Load<Texture2D>("NPCs/grn-zombie-big");
            LevelCollider = true;

            ScreenPosition = FindStartingPosition();
        }

        private Vector2 FindStartingPosition()
        {
            return PickSpotAlongEdge();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Vector2 newPos = new Vector2(ScreenPosition.X, ScreenPosition.Y);
            
            // If we are being thrown, fly towards the player
            if (state == PatrolManState.BeingThrown)
            {
                
                if (throwVector == Vector2.Zero)
                {
                    Vector2 playerPos = PlayerManager.GetPlayer().ScreenPosition;
                    if (playerPos.X > newPos.X)
                    {
                        throwVector.X = 5;
                        isFacingLeft = false;
                    }
                    else
                    {
                        throwVector.X = -5;
                        isFacingLeft = true;
                    }

                    if (playerPos.Y > newPos.Y)
                        throwVector.Y = 5;
                    else
                        throwVector.Y = -5;

                    VerticalVelocity = 15;
                }
                
                newPos = GravityManager.ApplyGravity(this);
                newPos.X += throwVector.X;
                
                if (!LevelManager.IsValidScreenPosition(newPos.X, newPos.Y))
                {
                    state = PatrolManState.Patrolling;
                    throwVector = Vector2.Zero;
                    return;
                }

                ScreenPosition = newPos;

                return;
            }

            newPos = GravityManager.ApplyGravity(this);

            if (!isFacingLeft)
            {
                newPos.X++;
            }
            else
            {
                newPos.X--;
            }

            // Fly once in awhile
            if (VerticalVelocity == 0)
            {
                if (seed < 0) seed = int.MaxValue;
                if(new Random(seed--).Next(0, 150) == 5)
                    VerticalVelocity = 10;
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
            // Switch back and forth
            frameIncrement++;
            if (frameIncrement == 10)
            {
                frameIncrement = 0;
                spriteMode++;

                if (spriteMode == 2) spriteMode = 0;
            }

            if (state != PatrolManState.Patrolling)
                spriteMode = 0;

            spriteBatch.Draw(texture, new Vector2(ScreenPosition.X, ScreenPosition.Y - 15),
                new Rectangle(72 * spriteMode, 85, 52, 85), Color.White, 0.0F, Vector2.Zero, .5F,
                !isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
        }
            

        #region ICollider Members

        public void CollisionInteraction(object collider)
        {
            if (collider is Player || collider is Ammo.Bullet)
            {
                NpcManager.Npcs.Remove(this);
            }
            else if (collider is PatrolManNpc)
            {
                if ((collider as PatrolManNpc).state != PatrolManState.BeingThrown)
                {
                    // Throw yourself
                    state = PatrolManState.BeingThrown;
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
}
