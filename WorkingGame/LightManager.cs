using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace WorkingGame
{
    /// <summary>
    /// Handles all behavior pertaining to the light used by the player
    /// </summary>
    ///
    internal class LightManager : IGameObject
    {
        //Used to make size of light relative to tiles covered
        private const int TileDimensions = 32;

        //Width of the bottom of light at default image size
        private const int LightBottomWidth = 32;

        //Maximum duration of the light (10 seconds at 60fps)
        private const int GaugeMax = 222;

        //Frames after light is turned off before it can be turned on again
        private const int LightEnableDelay = 60;

        //Light position and angle
        private Point[] unrotatedPoints;

        private Point[] points;

        //Light behavior (enabled, disabled, etc.)
        private bool active;

        //private bool depleted; Maybe something to implement later
        private int currentGauge;

        private int gaugeChange;

        //Keeps track of frames passed for use with LightEnableDelay const
        private int elapsedFrames;

        //MouseStates for toggles
        private MouseState currState;

        private MouseState prevState;

        private Texture2D tex;
        private Rectangle unrotatedRect;
        private Rectangle rect;

        //Player object, since light position is relative to player's
        private Player player;

        private Rectangle prevPlayerPos;

        // Debug
        private bool debugMode;

        private Color color;
        private int RightClickCount;

        //Sound
        private SoundEffect lightOn;

        private SoundEffect lightOff;

        /// <summary>
        /// Provides the texture of the light
        /// </summary>
        public Texture2D Tex { get => tex; }

        /// <summary>
        /// Rectangle representation of the light, for graphics
        /// </summary>
        public Rectangle Rect { get => rect; }

        /// <summary>
        /// The current amount of time the light can be used for
        /// </summary>
        public int Gauge { get => currentGauge; }

        public int MaxGauge { get => GaugeMax; }

        /// <summary>
        /// The array of points used for light collision data
        /// </summary>
        public Point[] Points { get => points; }

        /// <summary>
        /// Debug Mode
        /// </summary>
        public bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }

        public Color Color { get => color; }

        /// <summary>
        /// Creates a new instance of light manager
        /// </summary>
        /// <param name="tex">The texture of the object</param>
        /// <param name="player">Reference to the player object</param>
        /// <param name="tilesCoveredHori">Horizontal coverage of light</param>
        /// <param name="tilesCoveredVert">Vertical coverage of light</param>
        /// <param name="debugMode">Debug mode toggle</param>
        /// <param name="lightOn">Sound for turning light on</param>
        /// <param name="lightOff">Sound for turning light off</param>
        public LightManager(Texture2D tex, Player player, int tilesCoveredHori,
            int tilesCoveredVert, bool debugMode, SoundEffect lightOn, SoundEffect lightOff)
        {
            this.tex = tex;
            this.player = player;
            this.debugMode = debugMode;
            this.lightOn = lightOn;
            this.lightOff = lightOff;

            //Light gauge is set up, to give 3 full seconds of light
            active = false;
            //depleted = false;
            currentGauge = GaugeMax;
            gaugeChange = 1;
            elapsedFrames = LightEnableDelay;

            //Creates the rectangle used for drawing the texture
            unrotatedRect = new Rectangle(0, 0, tilesCoveredHori * TileDimensions,
                tilesCoveredVert * TileDimensions);
            unrotatedRect.X = (int)((player.X + (player.Rect.Width / 2))
                - (unrotatedRect.Width / 2));
            unrotatedRect.Y = (int)(player.Y - unrotatedRect.Height);

            rect = new Rectangle(unrotatedRect.X, unrotatedRect.Y,
                unrotatedRect.Width, unrotatedRect.Height);

            float widthScalar = (rect.Width * 1.0f) / tex.Width;

            //Initializes the four points used with light, based around the eye facing 90 degrees
            //Point array follows order of: bottom left, bottom right, top left, top right
            unrotatedPoints = new Point[4];

            unrotatedPoints[0] = new Point((int)(player.X + (player.Rect.Width / 2)
                - ((LightBottomWidth * widthScalar) / 2)), (int)player.Y);
            unrotatedPoints[1] = new Point((int)(player.X + (player.Rect.Width / 2)
                + ((LightBottomWidth * widthScalar) / 2)), (int)player.Y);
            unrotatedPoints[2] = new Point(rect.X, rect.Y);
            unrotatedPoints[3] = new Point(rect.X + rect.Width, rect.Y);

            //Sets the points array equal to the array of points serving as the rotation base
            points = new Point[unrotatedPoints.Length];

            for (int a = 0; a < unrotatedPoints.Length; a++)
                points[a] = new Point(unrotatedPoints[a].X,
                    unrotatedPoints[a].Y);

            prevPlayerPos = player.Rect;
            prevState = Mouse.GetState();
            color = Color.White;
            //Adjusts points to match the current rotation of the player
            RotateLightPoints(player.Rotation);
            RightClickCount = 0;
        }

        /// <summary>
        /// Updates the points of light based on player's movement,
        /// alongside moving the position of the box to draw the
        /// light texture
        /// </summary>
        /// <param name="playerRect">Rectangle for player object</param>
        public void ShiftLightPoints(Rectangle playerRect)
        {
            int deltaX = playerRect.X - prevPlayerPos.X;
            int deltaY = playerRect.Y - prevPlayerPos.Y;

            for (int a = 0; a < points.Length; a++)
            {
                unrotatedPoints[a].X += deltaX;
                unrotatedPoints[a].Y += deltaY;
            }

            unrotatedRect.X += deltaX;
            unrotatedRect.Y += deltaY;
        }

        /// <summary>
        /// Rotates the points of lights based on the angle provided
        /// </summary>
        /// <param name="angle">Angle of rotation, in radians</param>
        public void RotateLightPoints(float angle)
        {
            int x;
            int y;

            //Sets the anchor point of the rotation
            int anchorX = player.Rect.X + (player.Rect.Width / 2);
            int anchorY = player.Rect.Y + (player.Rect.Height / 2);

            for (int a = 0; a < points.Length; a++)
            {
                x = unrotatedPoints[a].X;
                y = unrotatedPoints[a].Y;

                points[a].X = (int)(((x - anchorX) * Math.Cos(angle))
                    + ((anchorY - y) * Math.Sin(angle)) + anchorX);
                points[a].Y = (int)(anchorY - ((anchorY - y) * Math.Cos(angle))
                    + ((x - anchorX) * Math.Sin(angle)));
            }

            x = unrotatedRect.X;
            y = unrotatedRect.Y;

            rect.X = (int)(((x - anchorX) * Math.Cos(angle))
                    + ((anchorY - y) * Math.Sin(angle)) + anchorX);
            rect.Y = (int)(anchorY - ((anchorY - y) * Math.Cos(angle))
                    + ((x - anchorX) * Math.Sin(angle)));
        }

        /// <summary>
        /// Checks if light intersects with enemy collision
        /// </summary>
        /// <param name="enemyRect">Enemy collision</param>
        /// <returns>Whether light makes contact with enemy collision</returns>
        public bool Intersects(Rectangle enemyRect)
        {
            if (!active)
                return false;

            //Creates an array to represent the points of the rectangle
            Point[] p = new Point[4];

            //Breaks up the rectangle into four points
            p[0] = new Point(enemyRect.X, enemyRect.Y);
            p[1] = new Point(enemyRect.X + enemyRect.Width, enemyRect.Y);
            p[2] = new Point(enemyRect.X, enemyRect.Y + enemyRect.Height);
            p[3] = new Point(enemyRect.X + enemyRect.Width, enemyRect.Y + enemyRect.Height);

            //Stores all the points of the rectangle into an 2D array
            Point[,] rectPoints = { { p[0], p[1] }, { p[0], p[2] }, { p[1], p[3] }, { p[2], p[3] } };

            //Checks if any lines of the rectangle intersect with the lines of the light
            for (int a = 0; a < rectPoints.GetLength(0); a++)
            {
                if (LineIntersection(points[0], points[2], rectPoints[a, 0], rectPoints[a, 1]))
                    return true;

                if (LineIntersection(points[2], points[3], rectPoints[a, 0], rectPoints[a, 1]))
                    return true;

                if (LineIntersection(points[1], points[3], rectPoints[a, 0], rectPoints[a, 1]))
                    return true;
            }

            //Checks if points of enemy rectangle are within polygon formed by light points
            //Used due to collision blindspot in the middle of the light
            for(int b = 0; b < p.Length; b++)
            {
                int intersectionAmt = 0;
                Point extreme = new Point(int.MaxValue, p[b].Y);

                if (LineIntersection(points[0], points[2], p[b], extreme))
                    intersectionAmt++;

                if (LineIntersection(points[2], points[3], p[b], extreme))
                    intersectionAmt++;

                if (LineIntersection(points[1], points[3], p[b], extreme))
                    intersectionAmt++;

                //If number of intersections is odd, point is inside polygon
                if(intersectionAmt % 2 == 1)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if two line segments intersect, returns true if so
        /// Implementation based on:
        /// http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/
        /// </summary>
        /// <param name="tri1">First point of the light line</param>
        /// <param name="tri2">Second point of the light line</param>
        /// <param name="rect1">First point of the rect line</param>
        /// <param name="rect2">Second point of the rect line</param>
        /// <returns>True if the line segments interesct, false otherwise</returns>
        public bool LineIntersection(Point tri1, Point tri2, Point rect1, Point rect2)
        {
            //Gets the paramters of the line segments
            float dxt = tri2.X - tri1.X;
            float dyt = tri2.Y - tri1.Y;
            float dxr = rect2.X - rect1.X;
            float dyr = rect2.Y - rect1.Y;

            //Solving for t1 and t2
            float determinant = (dyt * dxr - dxt * dyr);
            float t1 = ((tri1.X - rect1.X) * dyr + (rect1.Y - tri1.Y) * dxr) / determinant;

            //If t1 is infinity, lines are parallel (or close enough to parallel)
            if (float.IsInfinity(t1))
                return false;

            float t2 = ((rect1.X - tri1.X) * dyt + (tri1.Y - rect1.Y) * dxt) / -determinant;

            //Segments intersect if t1 and t2 are between 0 and 1, inclusive
            return ((t1 >= 0) && (t1 <= 1)) && ((t2 >= 0) && (t2 <= 1));
        }

        public void Update(GameTime gameTime)
        {
            currState = Mouse.GetState();

            elapsedFrames++;

            if (debugMode)
            {
                currentGauge = MaxGauge;
            }

            //Player is carrying a box and light can't be turned on
            if (player.IsPickedUp)
            {
                // Makes sure the light can't be on if the box is picked up
                active = false;

                if (!debugMode && (elapsedFrames >= LightEnableDelay))
                {
                    //Increments gauge while inactive
                    currentGauge += gaugeChange * 2;
                }

                //Makes sure that gauge never exceeds maximum
                if (currentGauge >= GaugeMax)
                    currentGauge = GaugeMax;
            }

            //Light is currently enabled
            else if (active)
            {
                if (!debugMode)
                {
                    //Decrements gauge while active
                    currentGauge -= gaugeChange;
                }

                //Set recharge counter down
                if (!debugMode)
                    elapsedFrames = 0;

                if (currState.LeftButton == ButtonState.Pressed &&
                    prevState.LeftButton == ButtonState.Released)
                {
                    active = false;
                    lightOff.CreateInstance().Play();
                }

                //Disables light if gauge is fully drained
                if (currentGauge <= 0)
                    active = false;
            }

            //Light is not yet enabled
            else
            {
                //Checks if light button was pressed, and has light
                if (currState.LeftButton == ButtonState.Pressed &&
                    prevState.LeftButton == ButtonState.Released &&
                    currentGauge > 0)
                {
                    active = true;
                    lightOn.CreateInstance().Play();
                }

                if (currState.RightButton == ButtonState.Pressed &&
                    prevState.RightButton == ButtonState.Released
                    && currentGauge > 0)
                {
                    currentGauge -= 9;
                    elapsedFrames = 0;
                    RightClickCount++;
                    if (RightClickCount < Palette.Amount)
                    {
                        color = Palette.ChangeColorIncre(RightClickCount);
                    }
                    else
                    {
                        RightClickCount = 0;
                        color = Palette.ChangeColorIncre(RightClickCount);
                    }
                }

                if (!debugMode && (elapsedFrames >= LightEnableDelay))
                {
                    //Increments gauge while inactive
                    currentGauge += gaugeChange * 2;
                }

                //Makes sure that gauge never exceeds maximum
                if (currentGauge >= GaugeMax)
                    currentGauge = GaugeMax;
            }

            //Updates light positions to current position and angle
            ShiftLightPoints(player.Rect);
            RotateLightPoints(player.Rotation);
            prevPlayerPos = player.Rect;

            prevState = currState;
        }

        /// <summary>
        /// Draws object
        /// </summary>
        /// <param name="sb">game spritebatch</param>
        public void Draw(SpriteBatch sb, Vector2 offset)
        {
            if (active)
            {
                sb.Draw(tex,
                    new Rectangle((int)(rect.X - offset.X),
                    (int)(rect.Y - offset.Y), rect.Width, rect.Height),
                    null,
                    color
                    ,
                    player.Rotation,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    1);
            }
        }

        /// <summary>
        /// Resets the light whenever a level is loaded.
        /// </summary>
        public void ResetLight()
        {
            currentGauge = MaxGauge;
            active = false;
            RightClickCount = 0;
            color = Palette.ChangeColorIncre(RightClickCount);
        }
    }
}