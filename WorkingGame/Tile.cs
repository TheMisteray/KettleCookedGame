using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorkingGame
{
    internal abstract class Tile : IGameObject
    {
        //Fields
        //Coordinates on tile array
        private int tilePosX;

        private int tilePosY;

        private Texture2D tex;
        private Rectangle rect;

        private const int TILE_SIDE_LENGTH = 32;

        //properties
        public int TilePosX
        {
            get => tilePosX;
        }

        public int TilePosY
        {
            get => tilePosY;
        }

        public Texture2D Tex
        {
            get => tex;
            set { tex = value; }
        }

        public Rectangle Rect
        {
            get => rect;
        }

        public int TileSideLength
        {
            get { return TILE_SIDE_LENGTH; }
        }

        //Constructor
        /// <summary>
        /// Creates a new tile
        /// </summary>
        /// <param name="x">tile's x coordinate on tile array</param>
        /// <param name="y">tile's y coordinate on tile array</param>
        /// <param name="tex">texture to draw in</param>
        public Tile(int x, int y, Texture2D tex)
        {
            tilePosX = x;
            tilePosY = y;
            this.tex = tex;

            rect = new Rectangle(x * TILE_SIDE_LENGTH, y * TILE_SIDE_LENGTH, TILE_SIDE_LENGTH, TILE_SIDE_LENGTH);
        }

        //Methods
        /// <summary>
        /// Draw tile
        /// </summary>
        /// <param name="sb">game spritebatch</param>
        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            //TODO: make lighting affect the color
            sb.Draw(tex, new Rectangle((int)(rect.X - offset.X), (int)(rect.Y - offset.Y), rect.Width, rect.Height), Color.White);
        }

        /// <summary>
        /// Denotes whether the tile should be collided with
        /// </summary>
        /// <returns>true if tile should have collision</returns>
        public virtual bool IsSolid()
        { return false; }
    }
}