using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2.NPCs
{
    public class FlyingSeeker : Npc, ICollider
    {
        int seed = int.MaxValue;
        int amountPatrolledWithoutValidPosition = 0;

        public override void LoadContent()
        {
            if (seed < 0) seed = int.MaxValue;
            isFacingLeft = new Random(seed--).Next(0, 2) == 1 ? true : false;
            texture = Utilities.Content.Load<Texture2D>("NPCs/flyingEyes-trans");
            LevelCollider = true;

            ScreenPosition = FindStartingPosition();
        }

        private Vector2 FindStartingPosition()
        {
            // Patrolmen will scan the entire left and right side of the screen.. If there are any holes, they will enter
            if (isFacingLeft)
            {
                // Check the right side
                for (int i = 0; i < LevelManager.CurrentLevel.LevelDef.GetLength(0); i++)
                {
                    if (LevelManager.CurrentLevel.LevelDef[i, LevelManager.CurrentLevel.LevelDef.GetLength(1) - 1] == 0)
                    {
                        // enter it there
                        return new Vector2(780, 32 * i);
                    }
                }
            }
            else
            {
                // Check the left side
                for (int i = 0; i < LevelManager.CurrentLevel.LevelDef.GetLength(0); i++)
                {
                    if (LevelManager.CurrentLevel.LevelDef[i, 0] == 0)
                    {
                        // enter it there
                        return new Vector2(0, 32 * i);
                    }
                }
            }

            return new Vector2(200, 1200);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Vector2 newPos = new Vector2(ScreenPosition.X, ScreenPosition.Y);
            // Find the positino of the player
            Vector2 playerPos = PlayerManager.GetPlayer().ScreenPosition;
            if (playerPos.X > newPos.X)
                newPos.X++;
            else
                newPos.X--;
            if (playerPos.Y > newPos.Y)
                newPos.Y++;
            else
                newPos.Y--;

            if (LevelManager.IsValidScreenPosition(newPos.X, newPos.Y) && 
                LevelManager.IsValidScreenPosition(newPos.X, ScreenPosition.Y) &&
                LevelManager.IsValidScreenPosition(ScreenPosition.X, newPos.Y))
            {
                // We tried to move to an valid position
                ScreenPosition = newPos;
            }
            else
            {
                if (newPos.X < 0 || newPos.X > 850)
                    NpcManager.RemoveNpc(this);

                // Let's patrol a little
                // We need to figure out which direction made this invalid
                if (!LevelManager.IsValidScreenPosition(newPos.X, ScreenPosition.Y))
                {
                    // The left to right direction is invalid, lets move only in y
                    //ScreenPosition = new Vector2(ScreenPosition.X, newPos.Y);

                    // The left to right direction is invalid, lets explode
                    NpcManager.RemoveNpc(this);
                    LevelManager.Explode(newPos.X, newPos.Y, 2);
                    return;

                }
                else
                {
                    // Y is the bad position, lets patrol in x
                    ScreenPosition = new Vector2(newPos.X, ScreenPosition.Y);
                }
                
            }
        }

        int frameCount = 0;
        float sizeDelta = .7F;
        public override void Draw(SpriteBatch spriteBatch)
        {
            frameCount++;
            if (frameCount == 15)
            {
                if (sizeDelta == .7F)
                    sizeDelta = .75F;
                else
                    sizeDelta = .7F;

                frameCount = 0;
            }

            spriteBatch.Draw(texture, new Vector2(ScreenPosition.X, ScreenPosition.Y),
                new Rectangle(0, 0, 45, 35), Color.White, 0.0F, Vector2.Zero, sizeDelta,
                !isFacingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.0F);
        }

        #region ICollider Members

        public void CollisionInteraction(object collider)
        {
            if (collider is Player || collider is Ammo.Bullet)
                NpcManager.Npcs.Remove(this);
        }

        Rectangle collisionRect = new Rectangle(0, 0, 30, 30);
        public Rectangle GetCollisionArea()
        {
            collisionRect.X = Convert.ToInt32(ScreenPosition.X);
            collisionRect.Y = Convert.ToInt32(ScreenPosition.Y);
            return collisionRect;
        }

        #endregion
    }
}
