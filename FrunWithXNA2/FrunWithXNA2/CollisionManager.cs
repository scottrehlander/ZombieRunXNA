using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FrunWithXNA2
{
    public static class CollisionManager
    {
        private const bool DRAW_COLLISION_RECTANGLES = false;
        
        // Global collections
        private static List<ScreenObject> collidableObjects = new List<ScreenObject>();
        private static List<CollisionPairing> alreadyCollided = new List<CollisionPairing>();

        // Optional collision items
        private static Texture2D collisionRectTexture;
        private static Color collisionColor;

        public static void NotifyCollisions()
        {
            // Populate the collidable objects
            collidableObjects = GetCollidableObjects();

            // Find any collisions
            foreach (ICollider obj in collidableObjects)
            {
                foreach (ICollider obj2 in collidableObjects)
                {
                    // Don't check against itself
                    if (obj == obj2) continue;
                    
                    if(obj.GetCollisionArea().Intersects(obj2.GetCollisionArea()))
                    {
                        // They collided

                        // Make sure we don't send the collision to the objects twice, we maintain
                        //  a custom collection of collision pairing so we can determine if the pair was
                        //  already handled as a collision
                        bool continueOn = false;
                        foreach (CollisionPairing pairing in alreadyCollided)
                        {
                            if (pairing.IsPair(obj, obj2))
                            {
                                continueOn = true;
                                break;
                            }
                        }
                        if (continueOn) continue;

                        // Notify both objects
                        (obj as ICollider).CollisionInteraction(obj2);
                        (obj2 as ICollider).CollisionInteraction(obj);
                        alreadyCollided.Add(new CollisionPairing(obj, obj2));
                    }
                }
            }
        }

        private static List<ScreenObject> GetCollidableObjects()
        {
            collidableObjects.Clear();

            // Aggregate the collidable objects from all the places they could be
            foreach (ScreenObject obj in LevelManager.ScreenObjects)
                if (obj is ICollider)
                    collidableObjects.Add(obj);
            foreach (ScreenObject obj in NpcManager.Npcs)
                if (obj is ICollider)
                    collidableObjects.Add(obj);
            foreach (ScreenObject obj in PlayerManager.GetPlayers())
                if (obj is ICollider)
                    collidableObjects.Add(obj);

            return collidableObjects;
        }

        public static void DrawCollisionRectangles(SpriteBatch spriteBatch)
        {
            if (!DRAW_COLLISION_RECTANGLES) return;
            if (collidableObjects == null) return;
            
            // Sometimes for debugging purposes, we want to see the collision rectangles
            if (collisionRectTexture == null)
            {
                collisionRectTexture = Utilities.Content.Load<Texture2D>("yellow_tile");
                collisionColor = Color.White;
                collisionColor.A = 0x05;
            }

            foreach (ICollider obj in collidableObjects)
            {
                spriteBatch.Draw(collisionRectTexture, new Vector2(obj.GetCollisionArea().X,
                    obj.GetCollisionArea().Y), new Rectangle(0, 0, obj.GetCollisionArea().Width, obj.GetCollisionArea().Height), 
                    collisionColor, 0.0F, Vector2.Zero, 1, SpriteEffects.None, 0.0F);
            }
        }

        private class CollisionPairing
        {
            private ICollider collider1;
            private ICollider collider2;

            public CollisionPairing(ICollider colliderCheck1, ICollider colliderCheck2)
            {
                collider1 = colliderCheck1;
                collider2 = colliderCheck2;
            }

            public bool IsPair(ICollider collideCheck1, ICollider collideCheck2)
            {
                // It is a pair if the two mach each other - either or
                if (collideCheck1 == collider1 && collideCheck2 == collider2)
                    return true;
                if (collideCheck1 == collider2 && collideCheck2 == collider1)
                    return true;

                return false;
            }
        }
    }
}
