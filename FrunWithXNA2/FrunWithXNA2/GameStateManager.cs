using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrunWithXNA2
{
    public static class GameStateManager
    {
        private static GameStateEnum gameState = GameStateEnum.Playing;
        public static GameStateEnum GameState
        {
            get { return gameState; }
            set
            {
                gameState = value;
            }
        }
    }

    public enum GameStateEnum
    {
        None,
        MainMenu,
        Playing,
        GameOver,
        Paused
    }
}
