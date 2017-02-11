using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Serialization;
using System.IO;

namespace FrunWithXNA2
{
    public static class LevelManager
    {
        public static LevelMap CurrentLevel = new LevelMap();

        // Level Tile Textures
        static Texture2D ground;
        static Texture2D stone;
        static Texture2D redTileTex;
        static Texture2D yellowTileTex;
        
        // Backround textures
        static Texture2D backgroundTile;

        // Background Tile Sizes
        static float backgroundTileWidth = 32;
        static float backgroundTileHeight = 32;

        private static List<ScreenObject> screenObjects = new List<ScreenObject>();
        public static List<ScreenObject> ScreenObjects { get { return screenObjects; } set { screenObjects = value; } }

        public static void LoadLevel()
        {
            CurrentLevel.LevelDef = new int[,]
            {
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,0,0,0,0,0,0,0,0,1,0,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,2,2,2},
                {2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,1,1,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2},
            };            

            /// ORIGINAL LEVEL
            //CurrentLevel.LevelDef = new int[,]
            //{
            //    {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
            //    {2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2},
            //    {2,2,2,2,2,2,2,0,0,0,2,2,2,2,2,2,2,0,2,2,2,2,2,2,2},
            //    {2,2,2,2,2,2,2,0,2,0,0,2,2,2,0,0,0,0,2,2,2,2,2,2,2},
            //    {2,2,2,2,2,2,0,0,0,0,2,2,2,2,0,2,2,2,2,2,2,2,2,2,2},
            //    {2,2,2,2,2,2,0,0,0,2,2,2,0,0,2,2,2,2,2,2,2,2,2,2,2},
            //    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //    {0,0,0,0,0,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //    {0,0,0,0,0,2,2,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //    {0,0,0,0,2,0,0,0,0,1,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0},
            //    {0,0,0,0,0,0,0,0,0,1,0,2,2,0,0,0,0,0,0,1,0,0,0,0,0},
            //    {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            //};            
        }

        public static void LoadTextures(ContentManager content)
        {
            // Load the backroung textures
            backgroundTile = content.Load<Texture2D>("bground");

            // Load the level tiles
            ground = content.Load<Texture2D>("Level1/cobblestone");
            stone = content.Load<Texture2D>("Level1/wall");
            redTileTex = content.Load<Texture2D>("red_tile");
            yellowTileTex = content.Load<Texture2D>("yellow_tile");
        }

        public static void RenderLevel(SpriteBatch spriteBatch)
        {
            if (backgroundTile == null) return;

            // We redraw the backgound here
            spriteBatch.Draw(backgroundTile, new Rectangle(0, 0, 400, 304), Color.CornflowerBlue);
            spriteBatch.Draw(backgroundTile, new Rectangle(0, 304, 400, 304), Color.CornflowerBlue);
            spriteBatch.Draw(backgroundTile, new Rectangle(400, 0, 400, 304), Color.CornflowerBlue);
            spriteBatch.Draw(backgroundTile, new Rectangle(400, 304, 400, 304), Color.CornflowerBlue);

            // And the level tiles here
            for (int i = 0; i < CurrentLevel.LevelDef.GetLength(0); i++)
                for (int j = 0; j < CurrentLevel.LevelDef.GetLength(1); j++)
                {
                    if (CurrentLevel.LevelDef[i, j] == 1)
                        spriteBatch.Draw(ground, new Vector2(j * backgroundTileWidth, i * backgroundTileHeight), Color.White);
                    if (CurrentLevel.LevelDef[i, j] == 2)
                        spriteBatch.Draw(stone, new Vector2(j * backgroundTileWidth, i * backgroundTileHeight), Color.White);
                    if (CurrentLevel.LevelDef[i, j] == 3)
                        spriteBatch.Draw(redTileTex, new Vector2(j * backgroundTileWidth, i * backgroundTileHeight), Color.White);
                    if (CurrentLevel.LevelDef[i, j] == 4)
                        spriteBatch.Draw(yellowTileTex, new Vector2(j * backgroundTileWidth, i * backgroundTileHeight), Color.White);
                }
        }

        public static bool IsValidScreenPosition(float x, float y)
        {
            // We need to determine the tile at the given x and y
            int xIndex = Convert.ToInt32((x - 0) / backgroundTileWidth);
            int yIndex = Convert.ToInt32((y + 10) / backgroundTileHeight);

            //  If it is a 0 then it is open space,
            //  If it is > 0 then it is a colliable tile and yields an invalid position
            if (xIndex >= CurrentLevel.LevelDef.GetLength(1) || yIndex >= CurrentLevel.LevelDef.GetLength(0) ||
                xIndex < 0 || yIndex < 0)
                return false;

            if (CurrentLevel.LevelDef[yIndex, xIndex] > 0)
                return false;

            return true;
        }

        public static bool Explode(float x, float y, int amount)
        {
            try
            {
                if (amount != 2) throw new Exception("only 2 works for Explode amount");

                int xIndex = Convert.ToInt32((x - 0) / backgroundTileWidth);
                int yIndex = Convert.ToInt32((y + 10) / backgroundTileHeight);

                for(int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (xIndex + j >= CurrentLevel.LevelDef.GetLength(1) ||
                            yIndex + i >= CurrentLevel.LevelDef.GetLength(0) ||
                            xIndex == -1 || yIndex == -1 ||
                            yIndex + i < 0 || xIndex + j < 0)
                            return false;
                        try { CurrentLevel.LevelDef[yIndex + i, xIndex + j] = 0; }
                        catch { }
                    }
                }


                return true;
            }
            catch { }

            return false;
        }

        public static bool InjectBlock(Vector2 screenPosition, bool injectToTheLeft)
        {
            // Find the square that this screenPosition is in and inject the block to either the left
            //  or the right
            int xIndex = 0, yIndex = 0;
            xIndex = Convert.ToInt32(screenPosition.X / backgroundTileWidth);
            yIndex = Convert.ToInt32(screenPosition.Y / backgroundTileHeight);

            if (injectToTheLeft)
                xIndex--;
            else
                xIndex++;

            try
            {
                if (xIndex >= CurrentLevel.LevelDef.GetLength(1) ||
                    yIndex >= CurrentLevel.LevelDef.GetLength(0) ||
                    xIndex == -1 || yIndex == -1)
                    return false;

                CurrentLevel.LevelDef[yIndex, xIndex] = 1;
            }
            catch { return false; }

            return true;
        }

        public static bool RemoveBlock(Vector2 screenPosition, bool removeFromTheLeft)
        {
            // Find the square that this screenPosition is in and inject the block to either the left
            //  or the right
            int xIndex = 0, yIndex = 0;
            xIndex = Convert.ToInt32(screenPosition.X / backgroundTileWidth);
            yIndex = Convert.ToInt32(screenPosition.Y / backgroundTileHeight);

            if (removeFromTheLeft)
                xIndex--;
            else
                xIndex++;

            try
            {
                if (xIndex >= CurrentLevel.LevelDef.GetLength(1) ||
                    yIndex >= CurrentLevel.LevelDef.GetLength(0) || 
                    xIndex == -1 || yIndex == -1)
                    return false;

                CurrentLevel.LevelDef[yIndex, xIndex] = 0;
            }
            catch { return false; }
            return true;
        }

    }

    public class LevelMap
    {
        public int[,] LevelDef { get { return levelDef; } set { levelDef = value; } }

        int[,] levelDef;
    }
}
