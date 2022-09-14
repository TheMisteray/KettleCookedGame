using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WorkingGame
{
    //UIButton Class
    //Handles the on-screen interactions with buttons, such as
    //Starting the game, going to the tutorial, or returning to menu
    internal class UIButton
    {
        //Fields
        private Texture2D button;

        private Texture2D buttonHovered;
        private Rectangle rect;
        private GraphicsDeviceManager graphics;
        private int originalX;
        private MouseState previsousMouseState;
        private string text;
        private SpriteFont font;

        /// <summary>
        /// Read and write property for the x-position of the button
        /// </summary>
        public int X
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        /// <summary>
        /// Read and write property for the y-position of the button
        /// </summary>
        public int Y
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        /// <summary>
        /// Read and write property for the Width of the button
        /// </summary>
        public int Width
        {
            get { return rect.Width; }
            set { rect.Width = value; }
        }

        /// <summary>
        /// Read and write property for the height of the button
        /// </summary>
        public int Height
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
        /// <param name="text">button text</param>
        /// <param name="font">button text</param>
        public UIButton(Texture2D button, Texture2D buttonHovered,
            Rectangle rect, GraphicsDeviceManager graphics,
            string text, SpriteFont font)
        {
            this.button = button;
            this.buttonHovered = buttonHovered;
            this.rect = rect;
            this.graphics = graphics;
            originalX = X;
            this.text = text;
            this.font = font;
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
        /// <param name="text">button text</param>
        /// <param name="font">button text</param>
        public UIButton(Texture2D button, Texture2D buttonHovered,
            int x, int y, int width, int height, GraphicsDeviceManager graphics,
            string text, SpriteFont font)
        {
            this.button = button;
            this.buttonHovered = buttonHovered;
            rect = new Rectangle(x, y, width, height);
            this.graphics = graphics;
            originalX = X;
            this.text = text;
            this.font = font;
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
            if (text != null)
            {
                sb.DrawString(font, text,
                    new Vector2(rect.Center.X - (font.MeasureString(text).X / 2), rect.Center.Y - (font.MeasureString(text).Y / 2)), Color.White);
            }

        }

        /// <summary>
        /// Checks to see if button was clicked
        /// </summary>
        /// <returns>true </returns>
        public bool ButtonJustClicked()
        {
            MouseState ms = Mouse.GetState();
            Point mousePosition = new Point(ms.X, ms.Y);

            if (previsousMouseState == null)
            {
                previsousMouseState = ms;
                return false;
            }
            else if (rect.Contains(mousePosition) &&
                ms.LeftButton == ButtonState.Released &&
                previsousMouseState.LeftButton == ButtonState.Pressed)
            {
                previsousMouseState = ms;
                return true;
            }
            previsousMouseState = ms;
            return false;
        }
    }
}