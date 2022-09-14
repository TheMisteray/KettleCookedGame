using Microsoft.Xna.Framework.Graphics;

namespace WorkingGame
{
    internal class WallTile : Tile
    {
        public WallTile(int tilePosX, int tilePosY, Texture2D tex)
            : base(tilePosX, tilePosY, tex) { }

        /// <summary>
        /// defines that all walls have collision
        /// </summary>
        /// <returns></returns>
        public override bool IsSolid()
        {
            return true;
        }
    }
}