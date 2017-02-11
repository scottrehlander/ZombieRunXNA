using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FrunWithXNA2.NPCs;

namespace FrunWithXNA2
{
    public static class NpcManager
    {
        private static List<Npc> npcs = new List<Npc>();
        public static List<Npc> Npcs { get { return npcs; } set { npcs = value; } }
        private static List<Npc> npcsToRemove = new List<Npc>();

        private static int timeWithoutNpc = 0;
        private static int INTRODUCE_NPC_INTERVAL = 45;

        static int typeOfGuys = 0;
        public static void CreateNpc()
        {
            Npc newNpc;
            if (typeOfGuys % 3 == 0)
            {
                newNpc = new Basher();
            }
            else if (typeOfGuys % 7 == 0)
            {
                newNpc = new FlyingSeeker();
                typeOfGuys = 0;
            }
            else if (typeOfGuys % 4 == 0)
            {
                newNpc = new RocketMan();
            }
            else
            {
                newNpc = new PatrolManNpc();
            }

            typeOfGuys++;

            npcs.Add(newNpc);
            npcs[npcs.Count - 1].LoadContent();
        }

        public static void RemoveNpc(Npc npcToRemove)
        {
            npcsToRemove.Add(npcToRemove);
        }

        public static void ClearNpcs()
        {
            npcs.Clear();
        }

        public static void Update(GameTime gameTime)
        {
            // If game state is game over, lose all the zombies
            if (GameStateManager.GameState == GameStateEnum.GameOver)
            {
                npcs.Clear();
                return;
            }

            //if (npcs.Count == 0)
            {
                if (timeWithoutNpc++ > INTRODUCE_NPC_INTERVAL)
                {
                    CreateNpc();
                    timeWithoutNpc = 0;
                }
            }

            foreach (Npc npc in npcs)
            {
                npc.Update(gameTime);
            }

            // Remove npcs that need to be removed
            try
            {
                for (int i = 0; i < npcsToRemove.Count; i++)
                {
                    npcs.Remove(npcsToRemove[i]);
                }
            }
            catch { }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Npc npc in npcs)
            {
                npc.Draw(spriteBatch);
            }
        }
    }
}
