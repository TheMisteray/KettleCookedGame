using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

//Aiden Grieshaber
//2/16/22
//Button object
namespace UntitledSpoopyGame
{
    class UIButton
    {
        //Fields
        private Texture2D button;
        private Texture2D buttonHovered;
        private Rectangle rect;
        private GraphicsDeviceManager graphics;
        private int originalX;
        private MouseState previsousMouseState;

        //properties
        public int X
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        public int width
        {
            get { return rect.Width; }
            set { rect.Width = value; }
        }

        public int height
        {
            get { return rect.Height; }
            set { rect.Height = value; }
        }

        //Constructor
        /// <summary>
        /// Create a new button
        /// </summary>
        /// <param name="button">button texture</param>
        /// <param name="buttonHovered">texture when hovered</param>
        /// <param name="rect">rectange area to draw button on</param>
        /// <param name="graphics">game graphics device</param>
        public UIButton(Texture2D button, Texture2D buttonHovered,
            Rectangle rect, GraphicsDeviceManager graphics)
        {
            this.button = button;
            this.buttonHovered = buttonHovered;
            this.rect = rect;
            this.graphics = graphics;
            originalX = X;
        }

        /// <summary>
        /// Create a new button with the given position parameters
        /// </summary>
        /// <param name="button">button texture</param>
        /// <param name="buttonHovered">button hover texture</param>
        /// <param name="x">x pos of button</param>
        /// <param name="y">y pos of button</param>
        /// <param name="width">button width</param>
        /// <param name="height">button height</param>
        /// <param name="graphics">game graphics device</param>
        public UIButton(Texture2D button, Texture2D buttonHovered,
            int x, int y, int width, int height, GraphicsDeviceManager graphics)
        {
            this.button = button;
            this.buttonHovered = buttonHovered;
            rect = new Rectangle(x, y, width, height);
            this.graphics = graphics;
            originalX = X;
        }

        //Methods
        /// <summary>
        /// Draw the button
        /// </summary>
        /// <param name="sb">the game's spritebatch</param>
        public void Draw(SpriteBatch sb)
        {
            MouseState ms = Mouse.GetState();
            Point mousePosition = new Point(ms.X, ms.Y);
            Texture2D tex = button;

            //If hovering button change texture
            if (rect.Contains(mousePosition))
            {
                tex = buttonHovered;
            }

            // draw button
            sb.Draw(tex, rect, Color.White);
        }

        /// <summary>
        /// Checks to see if button was clicked
        /// </summary>
        /// <returns>true </returns>
        public bool ButtonJustClicked()
        {
            MouseState ms = Mouse.GetState();
            Point mousePosition = new Point(ms.X, ms.Y);

            if (previsousMouseState.LeftButton == ButtonState.Pressed)
            {
                previsousMouseState = ms;
                return false;
            }
            if (rect.Contains(mousePosition) &&
                ms.LeftButton == ButtonState.Pressed)
            {
                previsousMouseState = ms;
                return true;
            }
            previsousMouseState = ms;
            return false;
        }
    }
}
