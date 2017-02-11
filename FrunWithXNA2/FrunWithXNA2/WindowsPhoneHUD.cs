using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace FrunWithXNA2
{
    public class WindowsPhoneHUD
    {

        #region Variables

        int moveButtonLeft = 30;
        int moveButtonTop = 419;
        int moveButtonMargin = 20;
        int moveButtonWidth = 60;
        int moveButtonHeight = 60;
        
        int jumpButtonTop = 419;
        int jumpButtonLeft = 700;
        int jumpButtonWidth = 60;
        int jumpButtonHeight = 60;

        Texture2D rightArrow;
        Texture2D leftArrow;
        Texture2D jumpButton;
        Texture2D addBlockButton;
        Texture2D removeBlockButton;

        Texture2D blackBackground;

        Texture2D healthBar;
        int healthBarStartingWidth = 100;
        // The 3rd parameter of this should equal the value of healthBarStartingWidth
        Rectangle healthBarBounds = new Rectangle(675, 20, 100, 10);

        Color overlayColor = Color.DarkGray;

        private bool rightArrowDown = false;
        public bool RightArrowDown { get { return rightArrowDown; } }

        private bool leftArrowDown = false;
        public bool LeftArrowDown { get { return leftArrowDown; } }

        private bool jumpButtonDown = false;
        public bool JumpButtonDown { get { return jumpButtonDown; } set { jumpButtonDown = value; } }

        private bool addBlockButtonDown = false;
        public bool AddBlockButtonDown { get { return addBlockButtonDown; } set { addBlockButtonDown = value; } }

        private bool removeBlockButtonDown = false;
        public bool RemoveBlockButtonDown { get { return removeBlockButtonDown; } set { removeBlockButtonDown = value; } }

        private bool shootButtonDown = false;
        public bool ShootButtonDown { get { return shootButtonDown; } set { shootButtonDown = value; } }

        #endregion


        public WindowsPhoneHUD()
        {
            // Set the opacity of the buttons
            //overlayColor.A = 0x00;
        }


        #region Screen object methods

        public void LoadContent(ContentManager content)
        {
            // Load the backroung textures
            rightArrow = content.Load<Texture2D>("Wp7Controls/arrowBlueRight");
            leftArrow = content.Load<Texture2D>("Wp7Controls/arrowBlueLeft");
            jumpButton = content.Load<Texture2D>("Wp7Controls/jumpButtonBlue");
            blackBackground = content.Load<Texture2D>("black_tile");
            healthBar = content.Load<Texture2D>("Wp7Controls/playerHealthBar");

            addBlockButton = content.Load<Texture2D>("Level1/ground");
            removeBlockButton = content.Load<Texture2D>("red_tile");
        }

        public void Update(GameTime gameTime)
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            // Reset the state of the buttons
            ReleaseAllButtons();
            
            foreach (TouchLocation touchLoc in touchCollection)
            {
                if ((touchLoc.State == TouchLocationState.Pressed) || (touchLoc.State == TouchLocationState.Moved))
                {
                    if (touchLoc.Position.Y > moveButtonTop || touchLoc.Position.Y > jumpButtonTop)
                    {
                        // Is the left button down?
                        if (touchLoc.Position.X > 0 && touchLoc.Position.X < moveButtonLeft + (moveButtonMargin) + moveButtonWidth)
                        {
                            leftArrowDown = true;
                        }

                        // Is the right button down?
                        if (touchLoc.Position.X > moveButtonLeft + (moveButtonMargin) +
                            moveButtonWidth && touchLoc.Position.X < moveButtonLeft + (moveButtonMargin * 2) + (moveButtonWidth * 2))
                        {
                            rightArrowDown = true;
                        }

                        // Is the jump button down?
                        if (touchLoc.Position.X > jumpButtonLeft)
                        {
                            jumpButtonDown = true;
                        }


                        //// Temporary //  There should bea  better framework for detecting HUD button downs
                        //// Is the add block button down?
                        //if (touchLoc.Position.X > 580 && touchLoc.Position.X < 640)
                        //{
                        //    addBlockButtonDown = true;
                        //}

                        //// Is the remove block button down?
                        //if (touchLoc.Position.X > 500 && touchLoc.Position.X < 580)
                        //{
                        //    removeBlockButtonDown = true;
                        //}
                    }
                    else
                    {
                        shootButtonDown = true;
                    }
                }
            }

            if (touchCollection.Count == 0)
            {
                ReleaseAllButtons();
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(blackBackground, new Rectangle(0, moveButtonTop, 1000, 300), Color.Black);
            spriteBatch.Draw(leftArrow, new Rectangle(moveButtonLeft, moveButtonTop, moveButtonWidth, moveButtonHeight), overlayColor);
            spriteBatch.Draw(rightArrow, new Rectangle(moveButtonLeft + (moveButtonMargin * 2) + moveButtonWidth, moveButtonTop, moveButtonWidth, moveButtonHeight), overlayColor);
            spriteBatch.Draw(jumpButton, new Rectangle(jumpButtonLeft, jumpButtonTop, jumpButtonWidth, jumpButtonHeight), overlayColor);

            // Temp buttons to add/remove blocks
            //spriteBatch.Draw(removeBlockButton, new Rectangle(520, jumpButtonTop + 10, 40, 40), overlayColor);
            //spriteBatch.Draw(addBlockButton, new Rectangle(600, jumpButtonTop + 10, 40, 40), overlayColor);

            healthBarBounds.Width = (healthBarStartingWidth / 2) * PlayerManager.GetPlayer().PlayerHealth;
            spriteBatch.Draw(healthBar, healthBarBounds, Color.White);
        }

        #endregion


        #region Functional methods

        private void ReleaseAllButtons()
        {
            rightArrowDown = false;
            leftArrowDown = false;
            jumpButtonDown = false;
            addBlockButtonDown = false;
            removeBlockButtonDown = false;
            shootButtonDown = false;
        }

        #endregion

    }
}
