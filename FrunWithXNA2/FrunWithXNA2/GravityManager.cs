using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2
{
    public static class GravityManager
    {

        static int acceleration = 8;
        static float delta = .075F;

        public static Vector2 ApplyGravity(ScreenObject screenObject)
        {
            // Our job is to manage vertical velocity and move the object
            float newYDelta = GetVerticalTravel(screenObject);
            Vector2 newPosition = Vector2.Zero;

            // Check that this is valid position
            if (!WillCollideWithLevel(screenObject.ScreenPosition.X, screenObject.ScreenPosition.Y + newYDelta))
            {
                screenObject.VerticalVelocity += (-1 * (delta * acceleration));
                return new Vector2(screenObject.ScreenPosition.X, screenObject.ScreenPosition.Y + newYDelta);
            }
            else
            {
                screenObject.VerticalVelocity = 0;
            }

            return screenObject.ScreenPosition;
        }

        private static bool WillCollideWithLevel(float x, float y)
        {
            if(!LevelManager.IsValidScreenPosition(x, y))
            {
                return true;
            }
            return false;
        }

        private static float GetVerticalTravel(ScreenObject screenObject)
        {
            return -1 * (screenObject.VerticalVelocity * acceleration * delta);
        }

    }
}

