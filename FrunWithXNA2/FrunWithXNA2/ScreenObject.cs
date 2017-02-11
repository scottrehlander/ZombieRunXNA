using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrunWithXNA2
{
    public abstract class ScreenObject
    {
        public Vector2 ScreenPosition;
        Texture2D Texture;
        Vector2 speed;
        public float VerticalVelocity = 0;
        public bool LevelCollider = false;

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
