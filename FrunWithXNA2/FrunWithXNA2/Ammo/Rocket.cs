using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrunWithXNA2.Ammo
{
    public class Rocket : ScreenObject
    {
        Rectangle viewRect;
        Texture2D Texture;
        Vector2 directionAndVelocity;
        Vector2 initialPosition;

        bool rotationDetermined = false;

        float rotation = 0;

        public Rocket(Vector2 directionVelocity, Vector2 startingPosition)
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
                LevelManager.Explode(ScreenPosition.X, ScreenPosition.Y, 2);
                LevelManager.ScreenObjects.Remove(this);
                GC.Collect();
            }

            if (!rotationDetermined)
            {
                // Find the angle from here to the player
                float relativex = PlayerManager.GetPlayer().ScreenPosition.X - ScreenPosition.X;
                float relativey = PlayerManager.GetPlayer().ScreenPosition.Y - ScreenPosition.Y;

                if (relativex == 0 || relativey == 0)
                    return; // The player is making contact with the rocket.

                rotation = (float)Math.Tan(relativey / relativex);
                rotationDetermined = true;
            }

            ScreenPosition.X += directionAndVelocity.X;
            ScreenPosition.Y += directionAndVelocity.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle(Convert.ToInt32(ScreenPosition.X), Convert.ToInt32(ScreenPosition.Y), 20, 20),
                viewRect, Color.White, rotation, Vector2.Zero, SpriteEffects.None, 0.0F);
        }
    }
}
