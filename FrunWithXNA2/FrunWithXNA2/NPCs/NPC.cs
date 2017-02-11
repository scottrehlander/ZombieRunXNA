using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrunWithXNA2.NPCs
{
    public class Npc : ScreenObject
    {
        protected Texture2D texture;
        protected Rectangle viewRect;
        protected bool isFacingLeft = false;

        public Npc()
        {
            viewRect = new Rectangle(0, 0, 24, 32);
        }

        public virtual void LoadContent()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 24, 32),
                viewRect, Color.White, 0.0F, Vector2.Zero, SpriteEffects.None, 0.0F);
        }


        #region Utilitiy functions for NPCs

        protected Vector2 GetVectorForAimedProjectile(float projectileSpeed, Vector2 selfPos, Vector2 targetPos)
        {
            // Aim at the player and shoot
            //
            //        / |
            //    p /   |
            //    /     |  y
            //  /_______|
            //      x

            // Get the distance (p)
            //  Figure out what to divide everything by to make the distance == rocketSpeed
            //  Divide x and y by that
            float relativeX = targetPos.X - selfPos.X;
            float relativeY = targetPos.Y - selfPos.Y;
            double distance = Math.Sqrt(Math.Pow(relativeX, 2) + Math.Pow(relativeY, 2));

            // distance / x = 3
            // distance = 3x
            // distance / 3 = x
            double factor = distance / projectileSpeed;

            return new Vector2((float)(relativeX / factor), (float)(relativeY / factor));
        }

        protected Vector2 PickRandomOpenSpot()
        {
            Vector2 v;

            do
            {
                v = new Vector2(new Random().Next(0, 800), new Random().Next(100, 500));
            }
            while (!LevelManager.IsValidScreenPosition(v.X, v.Y));

            return v;
        }

        protected Vector2 PickSpotAlongEdge()
        {
            // Scan the entire left and right side of the screen.. If there are any holes, they will enter
            if (isFacingLeft)
            {
                // Check the right side
                int i = new Random().Next(LevelManager.CurrentLevel.LevelDef.GetLength(1) - 1);
                {
                    LevelManager.RemoveBlock(new Vector2(800, 32 * i), true);

                    // enter it there
                    return new Vector2(780, 32 * i);
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

        #endregion
    }
}
