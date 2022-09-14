using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Aiden Grieshaber
//2/18/22
//Interface for all visible game elements
namespace WorkingGame
{
    /// <summary>
    /// boilerplate interface
    /// </summary>
    internal interface IGameObject
    {
        //properties
        public Texture2D Tex { get; }

        public Rectangle Rect { get; }

        /// <summary>
        /// Draws object
        /// </summary>
        /// <param name="sb">game spritebatch</param>
        public void Draw(SpriteBatch sb, Vector2 offset) { }
    }
}