using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

//Aiden Grieshaber
//2/18/22
//Main player
namespace WorkingGame
{
    internal class Player : IGameObject
    {
        //Fields
        private Rectangle rect;

        private bool dead;

        // PLayer rotation
        private float angle = MathHelper.PiOver2;

        private MouseState curMouse;
        private MouseState prevMouse;
        private Vector2 newMousePosition;

        //Player movement
        private Vector2 position;

        private float playerVelX;
        private float playerVelY;

        private const float PLAYER_SPEED = 4.0f;

        // Player dimensions
        private const int playerWidth = 64;

        private const int playerHeight = 64;
        private const int numFrames = 6;
        private int frame;

        //Box
        private Box box;

        private Texture2D tex;
        private Texture2D glow;
        private Color glowColor;

        private KeyboardState kbState;
        private KeyboardState previousKbState;

        // Debug
        private bool debugMode;

        //properties
        /// <summary>
        /// Read-only property for the rectangle representation of the Player
        /// </summary>
        public Rectangle Rect
        {
            get => rect;
        }

        /// <summary>
        /// Read-only property for the texture of the player
        /// </summary>
        public Texture2D Tex
        {
            get => tex;
        }

        /// <summary>
        /// Read-only property for the player's current angle of rotation
        /// </summary>
        public float Rotation
        {
            get => angle - MathHelper.PiOver2;
        }

        /// <summary>
        /// Read and write property for the x-position of the player
        /// </summary>
        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        /// <summary>
        /// Read and write property for the y-position of the player
        /// </summary>
        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        /// <summary>
        /// Read and write property for the vector storing player position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Read and write property for the player's horizontal velocity
        /// </summary>
        public float PlayerVelX
        {
            get { return playerVelX; }
            set { playerVelX = value; }
        }

        /// <summary>
        /// Read and write property for the player's vertical velocity
        /// </summary>
        public float PlayerVelY
        {
            get { return playerVelY; }
            set { playerVelY = value; }
        }

        /// <summary>
        /// Read-only property for the current position of the mouse
        /// </summary>
        public Vector2 NewMousePosition
        {
            get { return newMousePosition; }
        }

        /// <summary>
        /// Read and write property for the player's current state: alive or dead
        /// </summary>
        public bool IsDead
        {
            get => dead;
            set => dead = value;
        }

        /// <summary>
        /// Read and write property for the current box the player is holding,
        /// in the event the player is holding a box
        /// </summary>
        public Box Box
        {
            get => box;
            set => box = value;
        }

        /// <summary>
        /// Read and write property for whether the player is currently holding a box
        /// </summary>
        public bool IsPickedUp
        {
            get
            {
                if (box != null)
                {
                    return box.Held;
                }

                return false;
            }
            set
            {
                if (box != null)
                {
                    box.Held = value;
                }
            }
        }

        /// <summary>
        /// Read and write property for the current frame of animation of the player
        /// </summary>
        public int Frame
        {
            get => frame;
            set => frame = value;
        }

        /// <summary>
        /// Read and write property for the debug mode toggle
        /// </summary>
        public bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }

        // Constructor
        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="tex">player texture</param>
        /// <param name="position">player vector position</param>
        public Player(Texture2D tex, Texture2D glow, Vector2 position, bool debugMode)
        {
            //tileMap = _levelManager.WholeTileMap;
            this.tex = tex;
            this.glow = glow;
            this.position = position;
            rect = new Rectangle((int)position.X, (int)position.Y, playerWidth, playerHeight);
            dead = false;
            box = null;
            frame = 0;
            this.debugMode = debugMode;
            glowColor = Palette.CurEyeGlow;
            Palette.onGlow += Palette_onGlow;
        }

        /// <summary>
        /// responds to OnGlowChangeEvent
        /// </summary>
        /// <param name="color"></param>
        private void Palette_onGlow(Color color)
        {
            glowColor = color;
        }

        //Methods
        /// <summary>
        /// Draw player
        /// </summary>
        /// <param name="sb">game spritebatch</param>
        /// <param name="_lv">level amnager instance</param>
        public void Draw(SpriteBatch sb, Vector2 offset, GraphicsDevice graphics, LevelManager _lv)
        {
            // Normal player
            if (!debugMode)
            {
                sb.Draw(
                tex,
                position - offset,
                new Rectangle((tex.Width / numFrames) * frame, 0, tex.Width / numFrames, tex.Height),
                Color.White,
                angle + MathHelper.PiOver2,
                new Vector2(tex.Height / 2, tex.Height / 2),
                .7f, SpriteEffects.None, 1);
            }

            // Debug player
            else
            {
                Texture2D debugRect = new Texture2D(graphics, playerWidth, playerHeight);

                Color[] color = new Color[playerWidth * playerHeight];
                for (int i = 0; i < color.Length; ++i)
                {
                    color[i] = Color.White;
                }
                debugRect.SetData(color);

                // Player changes color if collision with enemy
                if (_lv.EnemyManager.Enemies.Count == 0)
                {
                    sb.Draw(
                        debugRect,
                        position - offset,
                        new Rectangle((tex.Width / numFrames) * frame, 0, (tex.Width / numFrames) - 16, tex.Height - 16),
                        Color.White,
                        0,
                        new Vector2(tex.Height / 2, tex.Height / 2),
                        .7f, SpriteEffects.None, 1);
                }
                else
                {
                    foreach (Enemy e in _lv.EnemyManager.Enemies)
                    {
                        if (e.PlayerCollision)
                        {
                            sb.Draw(
                            debugRect,
                            position - offset,
                            new Rectangle((tex.Width / numFrames) * frame, 0, (tex.Width / numFrames) - 16, tex.Height - 16),
                            Color.Red,
                            0,
                            new Vector2(tex.Height / 2, tex.Height / 2),
                            .7f, SpriteEffects.None, 1);
                        }
                        else
                        {
                            sb.Draw(
                            debugRect,
                            position - offset,
                            new Rectangle((tex.Width / numFrames) * frame, 0, (tex.Width / numFrames) - 16, tex.Height - 16),
                            Color.White,
                            0,
                            new Vector2(tex.Height / 2, tex.Height / 2),
                            .7f, SpriteEffects.None, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the glow of light around the player
        /// </summary>
        /// <param name="sb">The sprite batch used to draw the player</param>
        /// <param name="offset">The current offset of the screen due to scrolling</param>
        public void DrawGlow(SpriteBatch sb, Vector2 offset)
        {
            sb.Draw(
                glow,
                position - offset,
                null,
                glowColor,
                0,
                new Vector2(glow.Height / 2, glow.Height / 2),
                1, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Updates a player
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="_graphics"></param>
        /// <param name="_lv"></param>
        public void Update(GameTime gameTime, GraphicsDeviceManager _graphics, LevelManager _lv)
        {
            dead = false;
            rect = new Rectangle((int)position.X - (rect.Width / 2), (int)position.Y - (rect.Height / 2), playerWidth, playerHeight);

            // Player movement with keyboard
            kbState = Keyboard.GetState();

            playerVelX = 0.0f;
            playerVelY = 0.0f;

            //Player moves
            if (kbState.IsKeyDown(Keys.W))
            {
                playerVelY = -PLAYER_SPEED;
            }
            if (kbState.IsKeyDown(Keys.S))
            {
                playerVelY = PLAYER_SPEED;
            }
            if (kbState.IsKeyDown(Keys.A))
            {
                playerVelX = -PLAYER_SPEED;
            }
            if (kbState.IsKeyDown(Keys.D))
            {
                playerVelX = PLAYER_SPEED;
            }

            // Player doesn't go out of screen bounds
            if (position.X <= 0 + playerWidth)
            {
                position.X = 0 + playerWidth;
            }
            if (position.Y <= 0 + playerHeight)
            {
                position.Y = 0 + playerHeight;
            }

            //Check collision with all walls
            Rectangle playerDest = new Rectangle((int)(rect.X + playerVelX), (int)(rect.Y + playerVelY), rect.Width, rect.Height);

            if (CollidingWithTile(_lv, playerDest))
            {
                //track x and y direction
                bool posX = false;
                bool posY = false;

                if (playerVelX > 0)
                    posX = true;
                if (PlayerVelY > 0)
                    posY = true;

                float xDiff = playerVelX;
                float yDiff = playerVelY;

                bool movePlayer = true;

                while (movePlayer)
                {
                    xDiff += posX ? -1 : 1;
                    yDiff += posY ? -1 : 1;

                    //Try all directions
                    Rectangle xAltered = new Rectangle((int)(rect.X + xDiff), (int)(rect.Y + playerVelY), rect.Width, rect.Height);
                    Rectangle yAltered = new Rectangle((int)(rect.X + playerVelX), (int)(rect.Y + yDiff), rect.Width, rect.Height);
                    Rectangle diagAltered = new Rectangle((int)(rect.X + xDiff), (int)(rect.Y + yDiff), rect.Width, rect.Height);

                    //Check destination in each direction and choose closest open
                    if (!CollidingWithTile(_lv, xAltered) && movePlayer)
                    {
                        playerVelX = xDiff;
                        movePlayer = false;
                    }
                    if (!CollidingWithTile(_lv, yAltered) && movePlayer)
                    {
                        playerVelY = yDiff;
                        movePlayer = false;
                    }
                    //This one might be unnecessary but it will ensure no crashes
                    if (!CollidingWithTile(_lv, diagAltered) && movePlayer)
                    {
                        playerVelX = xDiff;
                        playerVelY = yDiff;
                        movePlayer = false;
                    }
                }
            }

            position.X += playerVelX;
            position.Y += playerVelY;

            // Enemy collision
            if (!debugMode)
            {
                foreach (Enemy e in _lv.EnemyManager.Enemies)
                {
                    if ((this.rect.Center.ToVector2() - e.Rect.Center.ToVector2()).Length() < ((this.rect.Width / 2) + (e.Rect.Height / 2)))
                    {
                        dead = true;
                    }
                }
            }
            else
            {
                foreach (Enemy e in _lv.EnemyManager.Enemies)
                {
                    if ((this.rect.Center.ToVector2() - e.Rect.Center.ToVector2()).Length() < ((this.rect.Width / 2) + (e.Rect.Height / 2)))
                    {
                        e.PlayerCollision = true;
                    }
                    else
                    {
                        e.PlayerCollision = false;
                    }
                }
            }

            // Player rotation with mouse
            curMouse = Mouse.GetState();

            // Aligns the mouse to the player position in order for the player to rotate
            newMousePosition = new Vector2(curMouse.X, curMouse.Y);
            if (position.X >= _graphics.PreferredBackBufferWidth / 2
                && position.Y >= _graphics.PreferredBackBufferHeight / 2
                && _lv.ScreenOffset.X < _lv.MaxScreenOffset.X
                && _lv.ScreenOffset.Y < _lv.MaxScreenOffset.Y)
            {
                newMousePosition.X = (newMousePosition.X * 2 + ((position.X + playerWidth) - _graphics.PreferredBackBufferWidth));
                newMousePosition.Y = (newMousePosition.Y * 2 + ((position.Y + playerHeight) - _graphics.PreferredBackBufferHeight));
            }
            else if (position.X >= _graphics.PreferredBackBufferWidth / 2
                && _lv.ScreenOffset.X < _lv.MaxScreenOffset.X
                && _lv.ScreenOffset.Y != _lv.MaxScreenOffset.Y)
            {
                newMousePosition.X = (newMousePosition.X * 2 + ((position.X + playerWidth) - _graphics.PreferredBackBufferWidth));
            }
            else if (position.Y >= _graphics.PreferredBackBufferHeight / 2
                && _lv.ScreenOffset.Y < _lv.MaxScreenOffset.Y
                && _lv.ScreenOffset.X != _lv.MaxScreenOffset.X)
            {
                newMousePosition.Y = (newMousePosition.Y * 2 + ((position.Y + playerHeight) - _graphics.PreferredBackBufferHeight));
            }
            else if (_lv.ScreenOffset.X == _lv.MaxScreenOffset.X
                || _lv.ScreenOffset.Y == _lv.MaxScreenOffset.Y)
            {
                newMousePosition.X = newMousePosition.X + _lv.ScreenOffset.X;
                newMousePosition.Y = newMousePosition.Y + _lv.ScreenOffset.Y;
            }

            //Vector2 vectorToMouse = Vector2.Normalize(Vector2.Subtract(position, new Vector2(curMouse.X, curMouse.Y)));

            if ((position - newMousePosition).Length() > .5f)
            {
                Vector2 vectorToMouse = Vector2.Normalize(position - NewMousePosition);
                Vector2 rotationAngle = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 v = Vector2.Lerp(rotationAngle, vectorToMouse, .2f);
                angle = (float)Math.Atan2(v.Y, v.X);
            }

            prevMouse = curMouse;
        }

        /// <summary>
        /// Checks whether the player has collided with a rectangle
        /// </summary>
        /// <param name="check">The rectangle to check for collision</param>
        /// <returns></returns>
        public bool CheckCollision(Rectangle check)
        {
            if (this.rect.Intersects(check))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines the location to place the box/block based on the direction
        /// the player is currently moving in
        /// </summary>
        /// <param name="isPickedUp">Whether the player is holding the box</param>
        /// <param name="originalRect">The rectangle representing the box in the
        /// event the player is not currently holding onto it</param>
        /// <param name="currentRect">The rectangle representing the box in the
        /// event the player is holding onto it</param>
        /// <returns>An altered rectangle for the box to accomodate
        /// player manipulation of its position</returns>
        public Rectangle BlockPlacement(bool isPickedUp, Rectangle originalRect, Rectangle currentRect)
        {
            if (isPickedUp)
            {
                if (previousKbState.IsKeyDown(Keys.D))
                {
                    currentRect.X = rect.X + 25;
                    currentRect.Y = rect.Y;
                }
                if (kbState.IsKeyDown(Keys.A))
                {
                    currentRect.X = rect.X - 25;
                    currentRect.Y = rect.Y;
                }
                if (kbState.IsKeyDown(Keys.W))
                {
                    currentRect.Y = rect.Y - 25;
                    currentRect.X = rect.X;
                }
                if (kbState.IsKeyDown(Keys.S))
                {
                    currentRect.Y = rect.Y + 25;
                    currentRect.X = rect.X;
                }
                return currentRect;
            }
            else
            {
                return originalRect;
            }
        }

        /// <summary>
        /// Will check if a key is pressed once this frame and not the frame before
        /// </summary>
        /// <param name="key">key that is being checked for current and previous frame</param>
        /// <param name="kbState">current kb state that is being check</param>
        /// <returns></returns>
        public bool SingleKeyPress(Keys key, KeyboardState kbState, KeyboardState previousKbState)
        {
            if (previousKbState != null)
            {
                return kbState.IsKeyDown(key) && !previousKbState.IsKeyDown(key);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the player's movement is going to make them collide
        /// with a wall. Returns true if so, false otherwise
        /// </summary>
        /// <param name="_lv">The level manager that stores tiles</param>
        /// <param name="destination">The predicted destination of the player</param>
        /// <returns></returns>
        private bool CollidingWithTile(LevelManager _lv, Rectangle destination)
        {
            //Loops through every tile in the level to check potential wall collision
            for (int x = 0; x < _lv.LevelArray.GetLength(1); x++)
            {
                for (int y = 0; y < _lv.LevelArray.GetLength(0); y++)
                {
                    Tile tile = _lv.GetTile(y, x);
                    //Compare projected position to current
                    if (tile is WallTile && tile.Rect.Intersects(destination))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Resets the player whenever a level is loaded.
        /// </summary>
        public void ResetPlayer()
        {
            Position = new Vector2(
                        (800 - tex.Width / 18) / 2,
                        (480 - tex.Height / 18) / 2);
        }
    }
}