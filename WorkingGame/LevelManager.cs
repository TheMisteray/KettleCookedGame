using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WorkingGame
{
    /// <summary>
    /// Manages a level.
    /// </summary>
    internal class LevelManager
    {
        //Fields
        private Tile tileForProperties;

        private Player player;
        private int tileSideLength;

        // StreamReader
        private StreamReader sr;

        //Sound effect for completing path
        protected SoundEffect pathComplete;

        private string currentFile;

        // Textures
        private Texture2D floorTex;

        private Texture2D wallTex;
        private Texture2D enemyPathTex;
        private Texture2D enemyTex;
        private Texture2D objectTex;
        private Texture2D goalTex;

        // Variables for screen use
        private GraphicsDeviceManager _graphics;

        // Variables for the level
        private Tile[,] levelArray;

        private int levelHeight;
        private int levelWidth;

        // Variables for the camera
        private Vector2 screenOffset;

        private Vector2 maxScreenOffset;

        // Enemies
        private EnemyManager _enemyManager;

        // Boxes
        private BoxManager _boxManager;

        // Next level
        private string nextlevel;

        private string levelType;

        // Instructions
        private string nextInstruction;

        private string previousInstruction;
        private List<string> instructionsText;
        private List<int> instructionsPos;

        // Light Manager
        private LightManager _lightManager;

        private Delegate[] listeners_ActiveFrame;
        private Delegate[] listeners_PrevFrame;

        // Used to transition to win state
        private bool wonGame;

        private bool goToContinue;

        public static event OnLightInteraction OnCollisionEnter;

        public static event OnLightInteraction WhileCollision;

        public static event OnLightInteraction OnCollisionExit;

        /// <summary>
        /// Read-only property of the offset of the screen due to scrolling
        /// </summary>
        public Vector2 ScreenOffset
        {
            get { return screenOffset; }
        }

        /// <summary>
        /// Read-only property of the maximum possible screen offset
        /// </summary>
        public Vector2 MaxScreenOffset
        {
            get { return maxScreenOffset; }
        }

        /// <summary>
        /// Read-only property regarding the 2D of tile objects
        /// </summary>
        public Tile[,] LevelArray
        {
            get => levelArray;
        }

        /// <summary>
        /// Read-only property for the EnemyManager
        /// </summary>
        public EnemyManager EnemyManager
        {
            get => _enemyManager;
        }

        /// <summary>
        /// Read-only property for the BoxManager
        /// </summary>
        public BoxManager BoxManager
        {
            get => _boxManager;
        }

        /// <summary>
        /// Read and write property for whether the game has been won
        /// </summary>
        public bool WonGame
        {
            get { return wonGame; }
            set { wonGame = value; }
        }

        /// <summary>
        /// Read and write property for whether to advance to next level
        /// </summary>
        public bool GoToContinue
        {
            get { return goToContinue; }
            set { goToContinue = value; }
        }

        /// <summary>
        /// Read-only property for whether a next instruction page can be loaded
        /// </summary>
        public string NextInstruction
        {
            get { return nextInstruction; }
        }

        /// <summary>
        /// Read-only property for whether a previous instruction page can be loaded
        /// </summary>
        public string PreviousInstruction
        {
            get { return previousInstruction; }
        }

        /// <summary>
        /// Read-only property for the list of instructions for each page
        /// </summary>
        public List<string> InstructionsText
        {
            get { return instructionsText; }
        }

        /// <summary>
        /// Read-only property for the positions of each instruction for each page
        /// </summary>
        public List<int> InstructionsPos
        {
            get { return instructionsPos; }
        }

        //change the data structure later to hold list of list per each enemy in level
        private List<Vector2> testList = new List<Vector2>();

        //temporary getter
        public List<Vector2> vector2s { get => testList; }

        //Constructor
        /// <summary>
        /// Creates the level manager, and loads in a level file.
        /// </summary>
        /// <param name="levelFile">First level to load in</param>
        /// <param name="floorTex">Texture of floor tiles</param>
        /// <param name="wallTex">Texture of wall tiles</param>
        /// <param name="_graphics">Graphics instance</param>
        /// <param name="player">Player instance</param>
        public LevelManager(string levelFile, Texture2D floorTex, Texture2D wallTex, Texture2D enemyTex, Texture2D eTex, Texture2D goalTex, Texture2D objectTex, SpriteFont font, Texture2D contScreen, GraphicsDeviceManager _graphics, Player player, LightManager lightManager, SoundEffect sound)
        {
            pathComplete = sound;
            this.floorTex = floorTex;
            this.wallTex = wallTex;
            this._graphics = _graphics;
            this.player = player;
            this.enemyPathTex = eTex;
            this.enemyTex = enemyTex;
            this.objectTex = objectTex;
            this.goalTex = goalTex;
            this._lightManager = lightManager;
            _enemyManager = new EnemyManager();
            _boxManager = new BoxManager();
            nextlevel = null;
            wonGame = false;
            currentFile = null;
            nextInstruction = null;
            previousInstruction = null;
            instructionsText = null;
            instructionsPos = null;

            // Gets the length of a tile by initializing a dummy Tile
            tileForProperties = new FloorTile(0, 0, floorTex);
            tileSideLength = tileForProperties.TileSideLength;

            ReadFile(levelFile);
        }

        /// <summary>
        /// Determines the type of level to read in.
        /// </summary>
        /// <param name="fileName">File name</param>
        public void ReadFile(string fileName)
        {
            try
            {
                // Resets the level references
                nextlevel = null;
                nextInstruction = null;
                previousInstruction = null;

                if (fileName != null)
                {
                    // Reads the first line for the type of level
                    this.currentFile = fileName;
                    sr = new StreamReader(fileName);
                    levelType = sr.ReadLine().Trim().ToLower();

                    ReadLevel(fileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error gathering data." + e);
            }
        }

        //Methods
        /// <summary>
        /// Reads in a level from a comma separated text file.
        /// </summary>
        /// <param name="fileName">File that is loaded</param>
        public void ReadLevel(string fileName)
        {
            try
            {
                // Resets the player
                player.ResetPlayer();

                // Resets instructions text
                instructionsText = null;
                instructionsPos = null;

                string line;
                string[] readData;

                // Reads the level normally
                line = sr.ReadLine();
                readData = line.Split(',');

                /*
                 * Line: 1 - Sets the dimensions of the level
                 */
                levelHeight = int.Parse(readData[1]);
                levelWidth = int.Parse(readData[0]);
                maxScreenOffset.X = (levelWidth * tileSideLength) - _graphics.PreferredBackBufferWidth;
                maxScreenOffset.Y = (levelHeight * tileSideLength) - _graphics.PreferredBackBufferHeight;

                levelArray = new Tile[levelWidth, levelHeight];

                /*
                 * Line: Amount of Tiles - Load tile data
                 */
                int row = 0;
                while ((line = sr.ReadLine()) != "--- break ---")
                {
                    string[] tiles = line.Split(',');
                    for (int i = 0; i < tiles.Length; i++)
                    {
                        switch (tiles[i])
                        {
                            case ".":
                                levelArray[i, row] = new WallTile(i, row, wallTex);
                                break;

                            case "1":
                                levelArray[i, row] = new FloorTile(i, row, floorTex);
                                break;

                            case "2":
                                levelArray[i, row] = new GoalTile(i, row, goalTex);
                                break;

                            case "p":
                                levelArray[i, row] = new FloorTile(i, row, floorTex);
                                player.Position = new Vector2(i * 32, row * 32);
                                break;
                        }
                    }
                    row++;
                }

                /*
                 * Line: After Tile Loading - Load enemy data
                 */
                _enemyManager.Clear();
                int numEnemies = int.Parse(sr.ReadLine());

                for (int i = 0; i < numEnemies; i++)
                {
                    /*
                    * Line: After Number of Enemies - Read spawn from file
                    */
                    string start = sr.ReadLine();
                    string[] startCoords = start.Split(',');

                    //Start a vector2 list for pathing and set the spawn
                    Vector2 spawn = new Vector2(int.Parse(startCoords[0]) * 32, int.Parse(startCoords[1]) * 32);
                    List<Vector2> path = new List<Vector2>();
                    path.Add(spawn);

                    /*
                    * Line: After Spawn Coordinate - Read line for constructing path
                    */
                    line = sr.ReadLine();
                    readData = line.Split('|');
                    foreach (string s in readData)
                    {
                        string[] move = s.Split(',');
                        //Add more path
                        path.AddRange(MapEnemy(move[0], int.Parse(move[1]), path[path.Count - 1]));
                    }

                    _enemyManager.AddEnemy(new Enemy(enemyTex, spawn, path, floorTex, pathComplete));
                }

                /*
                * Let enemy Manager get map.
                */
                _enemyManager.GetTarget(player);

                /*
                 * Line: Break
                 */
                sr.ReadLine();

                /*
                 * Line: After Enemy Loading - Load box start data
                 */
                _boxManager.Clear();
                int numBoxes = int.Parse(sr.ReadLine());

                for (int i = 0; i < numBoxes; i++)
                {
                    /*
                     * Line: After Number of Boxes - Read location from file
                    */
                    string start = sr.ReadLine();
                    string[] startCoords = start.Split(',');

                    // Adds the box
                    _boxManager.Add(new Box(
                        new Rectangle(
                            int.Parse(startCoords[0]) * 32,
                            int.Parse(startCoords[1]) * 32,
                            objectTex.Width / 3,
                            objectTex.Height / 3),
                        objectTex));
                }
                player.IsPickedUp = false;

                /*
                 * Line: Break
                 */
                sr.ReadLine();

                // Checks if the file is supposed to be part of the instructions
                if (levelType == "instructions")
                {
                    ReadInstructions(fileName);
                }
                else
                {
                    /*
                    * Line: After Box Loading - Read name of next level
                    */
                    if ((line = sr.ReadLine()) != null)
                    {
                        nextlevel = line;
                    }
                    else
                    {
                        nextlevel = null;
                    }
                }

                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error gathering data." + e);
            }
            //declare Remap
            RemapLevel();

            // Reset Level
            _lightManager.ResetLight();
        }

        /// <summary>
        /// Reads the current page of instruction data
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        public void ReadInstructions(string fileName)
        {
            try
            {
                string line;
                bool isNumber = false;
                int position = 0;
                instructionsText = new List<string>();
                instructionsPos = new List<int>();

                /*
                 * Line: Reads for instruction text
                */
                while ((line = sr.ReadLine()) != "--- break ---")
                {
                    if (line != "null")
                    {
                        isNumber = int.TryParse(line, out position);

                        if (isNumber)
                        {
                            instructionsPos.Add(position);
                        }
                        else
                        {
                            instructionsText.Add(line);
                        }
                    }
                    else
                    {
                        instructionsText = null;
                        instructionsPos = null;
                    }
                }

                // Reads for the next page
                if ((line = sr.ReadLine()) != "null")
                {
                    nextInstruction = line;
                }
                else
                {
                    nextInstruction = null;
                }

                // Reads for the previous page
                if ((line = sr.ReadLine()) != "null")
                {
                    previousInstruction = line;
                }
                else
                {
                    previousInstruction = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error gathering data." + e);
            }
        }

        /// <summary>
        /// Updates the level.
        /// </summary>
        /// <param name="gametime">GameTime instance</param>
        public void UpdateLevel(GameTime gametime)
        {
            // Makes sure the game can't be won if the MainGame state is loaded
            wonGame = false;
            goToContinue = false;

            KeyboardState kbState = Keyboard.GetState();

            screenOffset = player.Position - new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);

            //Stops camera from going offscreen

            if (screenOffset.X < 0)
                screenOffset.X = 0;
            if (screenOffset.X > (levelWidth * tileSideLength) - _graphics.PreferredBackBufferWidth)
                screenOffset.X = (levelWidth * tileSideLength) - _graphics.PreferredBackBufferWidth;
            if (screenOffset.Y < 0)
                screenOffset.Y = 0;
            if (screenOffset.Y > (levelHeight * tileSideLength) - _graphics.PreferredBackBufferHeight)
                screenOffset.Y = (levelHeight * tileSideLength) - _graphics.PreferredBackBufferHeight;

            _enemyManager.UpdateAll(gametime);
            _boxManager.Update(player);

            FillGoals();

            //Update player frame
            if (player.IsPickedUp)
                player.Frame = 5;
            else
            {
                int fillMargin = (_lightManager.Gauge / (_lightManager.MaxGauge / 5));
                player.Frame = Math.Abs(fillMargin - 5);
            }

            //checks light
            //foreach enemy check if light collides
            for (int i = 0; i < _enemyManager.Enemies.Count; i++)
            {
                //if light interacts with any
                if (_lightManager.Intersects(_enemyManager.Enemies[i].Rect) && _lightManager.Color == _enemyManager.Enemies[i].Color)
                {
                    // Debug
                    _enemyManager.Enemies[i].LightCollision = true;

                    //gets array of all event listeners
                    listeners_ActiveFrame = OnCollisionEnter.GetInvocationList();
                    //handle in reverse order to not break
                    for (int j = listeners_ActiveFrame.Length - 1; j >= 0; j--)
                    {
                        //if the list doesnt contain listener
                        if (listeners_PrevFrame == null || !listeners_PrevFrame.Contains(listeners_ActiveFrame[j]))
                        {
                            //fire OnCollision event this should fire ONE TIME
                            if (_lightManager.Color == _enemyManager.Enemies[i].Color)
                                listeners_ActiveFrame[j].Method.Invoke(listeners_ActiveFrame[j].Target, new object[] { _enemyManager.Enemies[i] });
                        }
                        else
                        {
                            //if it does fire while holding event
                            WhileCollision?.Invoke(_enemyManager.Enemies[i]);
                        }
                    }
                    listeners_PrevFrame = listeners_ActiveFrame;
                }
                else
                {
                    // Debug
                    _enemyManager.Enemies[i].LightCollision = false;

                    //for each enemy in previous frame declare on collison exit
                    if (listeners_PrevFrame != null)
                    {
                        OnCollisionExit?.Invoke(_enemyManager.Enemies[i]);
                    }
                    //then clear so we can assign new enemies per frame
                    listeners_PrevFrame = null;
                }
            }

            // Checks if the next level should be loaded
            if (HasWon())
            {
                goToContinue = true;
                NextLevel();
            }
        }

        /// <summary>
        /// Draws the whole level to the screen with scrolling.
        /// </summary>
        /// <param name="sb">SpriteBatch instance</param>
        public void DrawLevel(SpriteBatch sb, GraphicsDevice graphics)
        {
            if (levelArray == null)
                return;
            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    Tile tileToDraw = GetTile(x, y);
                    tileToDraw.Draw(sb, ScreenOffset);
                }
            }

            _enemyManager.Draw(sb, ScreenOffset, player.DebugMode, graphics);
            _boxManager.Draw(sb, screenOffset);
        }

        /// <summary>
        /// Gets a tile at a specified coordinate.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>The tile at the coordinate.</returns>
        public Tile GetTile(int x, int y)
        {
            //Arrays in C# are handled in the order of row -> column, but for x/y coordinates we expect x to be the column and y to be the row so it is done like this

            return LevelArray[x, y];
        }

        /// <summary>
        /// Remaps the level to indicate tiles for enemy pathing
        /// </summary>
        public void RemapLevel()
        {
            foreach (EnemyParent e in _enemyManager.Enemies)
            {
                foreach (var p in e.Checkpoints)
                {
                    Tile t = GetTile((int)(p.X) / 32, (int)(p.Y) / 32);
                    t.Tex = enemyPathTex;
                }
            }
        }

        /// <summary>
        /// Adds a vector2 path
        /// </summary>
        /// <param name="direction">direction to extend path in</param>
        /// <param name="amount">num tiles to extend by</param>
        /// <param name="start">starting coordinate</param>
        /// <returns></returns>
        private List<Vector2> MapEnemy(string direction, int amount, Vector2 start)
        {
            Vector2 old = start;
            List<Vector2> newLine = new List<Vector2>();
            for (int i = 0; i < amount; i++)
            {
                switch (direction.Trim().ToUpper())
                {
                    case "N":
                        old = new Vector2(old.X, old.Y - 32);
                        break;

                    case "E":
                        old = new Vector2(old.X + 32, old.Y);
                        break;

                    case "S":
                        old = new Vector2(old.X, old.Y + 32);
                        break;

                    case "W":
                        old = new Vector2(old.X - 32, old.Y);
                        break;
                }
                newLine.Add(old);
            }
            return newLine;
        }

        /// <summary>
        /// Checks if the game has been won
        /// </summary>
        /// <returns>true if the player should beat the stage</returns>
        public bool HasWon()
        {
            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    Tile goal = GetTile(x, y);
                    if (goal is GoalTile)
                    {
                        GoalTile g = (GoalTile)goal;
                        if (!g.Filled)
                            return false;
                    }
                }
            }
            return true;
        }

        //Checking for boxes on goals
        /// <summary>
        /// Checks to see if boxes have been placed in the goals
        /// </summary>
        private void FillGoals()
        {
            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    Tile goal = GetTile(x, y);
                    if (goal is GoalTile)
                    {
                        GoalTile g = (GoalTile)goal;
                        foreach (Box b in BoxManager.Boxes)
                        {
                            if (!b.InGoal && !g.Filled && b.Rect.Intersects(g.Rect) && !(b.Held))
                            {
                                g.Filled = true;
                                b.InGoal = true;
                                b.Center(g.Rect);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If there is another level, it will transition to the next level.
        /// Else, the game transitions to the victory state.
        /// </summary>
        private void NextLevel()
        {
            if (nextlevel != null)
            {
                ReadFile(nextlevel);
            }
            else
            {
                wonGame = true;
            }
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartLevel()
        {
            if (currentFile != null)
            {
                ReadFile(currentFile);
            }
        }

        /// <summary>
        /// Progresses to the instruction page specified.
        /// </summary>
        /// <param name="nextInstruction">Instuction level file</param>
        public void NextInstructionPage(string direction)
        {
            if (direction == "right" && nextInstruction != null)
            {
                ReadFile(nextInstruction);
            }
            else if (direction == "left" && previousInstruction != null)
            {
                ReadFile(previousInstruction);
            }
        }

        /// <summary>
        /// Places the box, ensuring it doesn't go out of the bounds of the game
        /// </summary>
        /// <param name="b"></param>
        /// <param name="offset"></param>
        public void PlaceBoxSafe(Box b, Vector2 offset)
        {
            //Apply offset
            Rectangle tryDest = new Rectangle(b.Rect.X + (int)offset.X, b.Rect.Y + (int)offset.Y, b.Rect.Width, b.Rect.Height);

            //Push back in bounds
            if (CollidingWithTile(tryDest))
            {
                //track x and y direction
                bool posX = false;
                bool posY = false;

                if (offset.X > 0)
                    posX = true;
                if (offset.Y > 0)
                    posY = true;

                float xDiff = offset.X;
                float yDiff = offset.Y;

                bool moveBox = true;

                while (moveBox)
                {
                    xDiff += posX ? -1 : 1;
                    yDiff += posY ? -1 : 1;

                    //Try all directions
                    Rectangle xAltered = new Rectangle((int)(b.Rect.X + xDiff), (int)(b.Rect.Y + offset.Y), b.Rect.Width, b.Rect.Height);
                    Rectangle yAltered = new Rectangle((int)(b.Rect.X + offset.X), (int)(b.Rect.Y + yDiff), b.Rect.Width, b.Rect.Height);
                    Rectangle diagAltered = new Rectangle((int)(b.Rect.X + xDiff), (int)(b.Rect.Y + yDiff), b.Rect.Width, b.Rect.Height);

                    //Check destination in each direction and choose closest open
                    if (!CollidingWithTile(xAltered) && moveBox)
                    {
                        offset.X = xDiff;
                        moveBox = false;
                    }
                    if (!CollidingWithTile(yAltered) && moveBox)
                    {
                        offset.Y = yDiff;
                        moveBox = false;
                    }
                    //This one might be unnecessary but it will ensure no crashes
                    if (!CollidingWithTile(diagAltered) && moveBox)
                    {
                        offset.X = xDiff;
                        offset.Y = yDiff;
                        moveBox = false;
                    }
                }
            }

            b.Rect = new Rectangle(b.Rect.X + (int)offset.X, b.Rect.Y + (int)offset.Y, b.Rect.Width, b.Rect.Height);
        }

        /// <summary>
        /// Checks if a rectangle is colliding with a wall tile
        /// </summary>
        /// <param name="destination">The rectangle to be checked</param>
        /// <returns>True if the rectangle will collide with a wall tile,
        /// returns false otherwise</returns>
        private bool CollidingWithTile(Rectangle destination)
        {
            for (int x = 0; x < levelArray.GetLength(1); x++)
            {
                for (int y = 0; y < levelArray.GetLength(0); y++)
                {
                    Tile tile = GetTile(y, x);
                    //Compare projected position to current
                    if (tile is WallTile && tile.Rect.Intersects(destination))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}