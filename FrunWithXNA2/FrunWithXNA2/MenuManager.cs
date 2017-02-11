using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;

namespace FrunWithXNA2
{
    public static class MenuManager
    {
        // Here we switch on GameState to determine what Menu to use
        private static GameMenu menu = null;

        public static void Update(GameTime gameTime)
        {
            // If this is a new state then we need a new Menu            
            switch (GameStateManager.GameState)
            {
                case GameStateEnum.MainMenu:
                    menu = new MainMenu();
                    break;
                case GameStateEnum.GameOver:
                    menu = new GameOverMenu();
                    break;
            }
            

            // Call update for the active Menu
            if (menu != null)
                menu.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // Call draw on the active menu
            if (menu != null)
                menu.Draw(spriteBatch);
        }
         
    }

    public class GameMenu
    {
        protected Texture2D menuBackground;
        protected Color menuColorOverlay;
        SpriteFont courierFont;

        List<MenuItem> menuItems = new List<MenuItem>();
        List<StringItem> stringItems = new List<StringItem>();

        public GameMenu()
        {
            courierFont = Utilities.Content.Load<SpriteFont>("Menus/Courier.New.MenuFont");
        }

        // This function should be overriden to handle the action taken when a user clicks on a menu item
        protected virtual void HandleAction(string actionName) { throw new Exception("You forgot to implment the menu functions."); }

        public virtual void Update(GameTime gameTime)
        {
            // This will notify the super user class when a menu item was selected
            
            // Handle User Input
            // Check if the user pressed the screen
            TouchCollection touchCollection = TouchPanel.GetState();

            // Loop through all the places touched by the user
            foreach (TouchLocation location in touchCollection)
            {
                // Check the bounds for each menu item
                foreach (MenuItem item in menuItems)
                {
                    if (location.Position.X > item.ItemBounds.X && location.Position.X < item.ItemBounds.X + item.ItemBounds.Width)
                    {
                        if (location.Position.Y > item.ItemBounds.Y && location.Position.Y < item.ItemBounds.Y + item.ItemBounds.Height)
                        {
                            // This item has been pressed, notify the super class
                            HandleAction(item.ActionName);
                        }
                    }
                }
            }

            // Work around for keyboard testing
            if (Keyboard.GetState().GetPressedKeys().Contains<Keys>(Keys.Space))
            {
                HandleAction("Proceed");
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            // Load a default background if it's not set in the specific manager
            if (menuBackground == null)
                menuBackground = Utilities.Content.Load<Texture2D>("bground");
            if (menuColorOverlay == null)
                menuColorOverlay = Color.White;

            spriteBatch.Draw(menuBackground, new Rectangle(0, 0, 1000, 800), menuColorOverlay);

            // Draw all of the menu items
            foreach (MenuItem item in menuItems)
            {
                spriteBatch.Draw(item.ItemView, item.ItemBounds, item.OverlayColor);
            }

            // Draw all of the string items
            foreach (StringItem item in stringItems)
            {
                spriteBatch.DrawString(courierFont, item.Text, item.ItemBounds, item.Color, 0, Vector2.Zero,
                    1, SpriteEffects.None, 0F);
            }
        }

        protected void AddMenuItem(string actionName, Rectangle itemBounds, Texture2D itemView)
        {
            MenuItem item = new MenuItem() { ItemBounds = itemBounds, ItemView = itemView, ActionName = actionName, OverlayColor = Color.White };
            menuItems.Add(item);
        }

        protected void AddMenuText(string text, Vector2 itemBounds, Color color)
        {
            stringItems.Add(
                new StringItem() 
                    { Text = text, ItemBounds = itemBounds, Color = color }
            );

        }

        // A collection to associate all of the data about a menuItem
        private struct MenuItem
        {
            public string ActionName;
            public Rectangle ItemBounds;
            public Texture2D ItemView;
            public Color OverlayColor;
        }

        private struct StringItem
        {
            public Color Color;
            public string Text;
            public Vector2 ItemBounds;
            public float FontSize;
        }
    }

    public class MainMenu : GameMenu
    {
        private const string MENU_ACTION_START = "Start Game";

        public MainMenu()
        {
            // Set the background for this menu
            menuBackground = Utilities.Content.Load<Texture2D>("bground");
            menuColorOverlay = Color.Green;
            
            // Add a menu item
            AddMenuItem(MENU_ACTION_START, new Rectangle(300, 200, 150, 100), Utilities.Content.Load<Texture2D>("menus/new"));
        }

        protected override void HandleAction(string actionName)
        {
            switch(actionName)
            {
                case MENU_ACTION_START:
                    // The retry was pressed
                    GameStateManager.GameState = GameStateEnum.Playing;
                    break;
                case "Proceed": // Hardcoded action for testing purposes
                    GameStateManager.GameState = GameStateEnum.Playing;
                    break;
            }

            
        }

    }

    public class GameOverMenu : GameMenu
    {
        private const string MENU_ACTION_RETRY = "Retry";

        public GameOverMenu()
        {
            menuBackground = Utilities.Content.Load<Texture2D>("bground");
            menuColorOverlay = Color.Red;

            AddMenuItem("GameOverImage", new Rectangle(200, 100, 364, 45), Utilities.Content.Load<Texture2D>("menus/gameOver"));
            AddMenuItem(MENU_ACTION_RETRY, new Rectangle(300, 200, 170, 42), Utilities.Content.Load<Texture2D>("menus/retry"));

            PlayerStatistics stats = PlayerManager.GetPlayer().PlayerStats;
            AddMenuText("Zombies Killed: ", new Vector2(125, 300), Color.White);
            AddMenuText(stats.ZombiesKilled.ToString(), new Vector2(450, 300), Color.White);

            AddMenuText("Time Alive: ", new Vector2(125, 350), Color.White);
            AddMenuText(string.Format("{0:0.0}", stats.PlayerAliveTimer.TotalSeconds) + " seconds", 
                new Vector2(450, 350), Color.White);

            AddMenuText("Score: ", new Vector2(125, 400), Color.White);
            AddMenuText(string.Format("{0}", stats.ZombiesKilled * stats.PlayerAliveTimer.TotalSeconds), 
                new Vector2(450, 400), Color.White);

            // Automatically submit scores for now:
            try
            {
                //bool success = Utilities.HighScoreUtilities.SubmitHighScore("Scott", stats.ZombiesKilled * stats.PlayerAliveTimer.TotalSeconds);
            }
            catch { }

        }

        protected override void HandleAction(string actionName)
        {
            switch (actionName)
            {
                case MENU_ACTION_RETRY:
                    GameStateManager.GameState = GameStateEnum.Playing;
                    break;
                case "Proceed": // Hardcoded action for testing purposes
                    GameStateManager.GameState = GameStateEnum.Playing;
                    break;
            }
        }
    }
}
