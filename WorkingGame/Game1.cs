using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;

namespace WorkingGame
{
    //Game1 clas
    //Stores all objects used in-game and manages their behavior
    public class Game1 : Game
    {
        //Defines the managers used within the class
        private GraphicsDeviceManager _graphics;

        private SpriteBatch _spriteBatch;
        private ScreenManager _screenManager;
        private LevelManager _levelManager;

        //Loadable Assets
        private Texture2D placeholderButtonTex;

        private Texture2D placeholderButtonHover;
        private Texture2D arrowRight;
        private Texture2D arrowRightHover;
        private Texture2D arrowLeft;
        private Texture2D arrowLeftHover;
        private Texture2D restart;
        private Texture2D restartHover;
        private Texture2D objectPlaceHolder;
        private Texture2D healthBar;
        private Texture2D healthBarOutline;
        private Texture2D eyeballClosed;

        private Texture2D floorTex;
        private Texture2D wallTex;
        private Texture2D enemyPathTex;
        private Texture2D playerTex;
        private Texture2D playerLight;
        private Texture2D enemyTex;
        private Texture2D lightMask;
        private Texture2D goalTex;
        private Texture2D menuBkg;
        private Texture2D winBkg;
        private Texture2D loseBkg;
        private Texture2D contScreen;
        private Texture2D ratJumpScare;

        private SpriteFont font;

        //Content
        private UIButton menuButtonGame;

        private UIButton menuButtonInstruction;
        private UIButton menuButtonExit;
        private UIButton buttonToMenu;
        private UIButton buttonInstructionsToMenu;
        private UIButton rightArrow;
        private UIButton leftArrow;
        private UIButton restartButtonInstructions;

        private UIButton gameButtonExit;

        private Player player;
        private LightManager lightManager;

        //private Rectangle objectRect;
        //private Rectangle objectRectMain;
        private Rectangle healthRect;

        private KeyboardState kbState;
        private KeyboardState previousKbState;

        //Shaders
        private float colorTime;

        public static Effect effect2;

        private RenderTarget2D lightsTarget;
        private RenderTarget2D mainTarget;
        private Texture2D haze;

        //Number 0 - 1 for brightness
        private float brightness;

        private float jumpScareTimer;
        private int jumpScareThreshhold;
        private byte jumpScarePrev;
        private byte jumpScareCurr;
        private Rectangle[] jumpScareRect;

        //timer
        private int timer;

        // Debug
        private bool debugMode;

        //Sound
        private SoundEffect pickupBoxSFX;

        private SoundEffect placeBoxSFX;
        private SoundEffect jumpscareSFX;
        private SoundEffect lightOn;
        private SoundEffect lightOff;
        protected SoundEffect pathComplete;

        /// <summary>
        /// Constructs a new instance of Game1
        /// </summary>
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the non-content based fields of Game1
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            kbState = Keyboard.GetState();
            previousKbState = Keyboard.GetState();
            debugMode = false;
            brightness = .5f;

            colorTime = 0;

            timer = 90 * 1;

            base.Initialize();
        }

        /// <summary>
        /// Loads in all textures and other content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenManager = new ScreenManager(GraphicsDevice, _spriteBatch, _graphics);

            // Load Assets
            //TODO: make the real textures textless, we can add text with a spritefont later
            placeholderButtonTex = Content.Load<Texture2D>("ButtonAlt");
            placeholderButtonHover = Content.Load<Texture2D>("ButtonHoverAlt");
            objectPlaceHolder = Content.Load<Texture2D>("itemPlaceholder");
            menuBkg = Content.Load<Texture2D>("menuBkg");
            winBkg = Content.Load<Texture2D>("winBkg");
            loseBkg = Content.Load<Texture2D>("loseBkg");

            // Instructions assets
            arrowRight = Content.Load<Texture2D>("arrowRight");
            arrowRightHover = Content.Load<Texture2D>("arrowRightHover");
            arrowLeft = Content.Load<Texture2D>("arrowLeft");
            arrowLeftHover = Content.Load<Texture2D>("arrowLeftHover");
            restart = Content.Load<Texture2D>("restart");
            restartHover = Content.Load<Texture2D>("restartHover");

            contScreen = Content.Load<Texture2D>("contScreenk");
            ratJumpScare = Content.Load<Texture2D>("ratJumpScare");

            //Jumpscare
            jumpScareTimer = 0;
            jumpScareThreshhold = 180;
            jumpScareRect = new Rectangle[3]
            {new Rectangle(0,0,800,480),
             new Rectangle(800,0,800,480),
             new Rectangle(1600,0,800,480) };
            jumpScarePrev = 2;
            jumpScareCurr = 1;

            //Sound
            pickupBoxSFX = Content.Load<SoundEffect>("PickUp");
            placeBoxSFX = Content.Load<SoundEffect>("PutDown");
            jumpscareSFX = Content.Load<SoundEffect>("Squeak");
            lightOn = Content.Load<SoundEffect>("LightOn");
            lightOff = Content.Load<SoundEffect>("LightOff");
            pathComplete = Content.Load<SoundEffect>("RatPathComplete");

            //Font
            font = Content.Load<SpriteFont>("Font");

            //Load content
            menuButtonGame = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 - 80, 200, 100),
                _graphics, "Play", font);
            menuButtonInstruction = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 + 25, 200, 100),
                _graphics, "Instructions", font);
            menuButtonExit = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 + 130, 200, 100),
                _graphics, "Quit", font);
            gameButtonExit = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 - 50, 200, 100),
                _graphics, "Return", font);
            buttonToMenu = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 105, 380, 200, 100),
                _graphics, "Menu", font);

            // Instructions
            buttonInstructionsToMenu = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 50, _graphics.PreferredBackBufferHeight - 55, 200 / 2, 100 / 2),
                _graphics, "Menu", font);
            rightArrow = new UIButton(arrowRight, arrowRightHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 + 75, _graphics.PreferredBackBufferHeight - 55, 50, 50),
                _graphics, null, font);
            leftArrow = new UIButton(arrowLeft, arrowLeftHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 125, _graphics.PreferredBackBufferHeight - 55, 50, 50),
                _graphics, null, font);
            restartButtonInstructions = new UIButton(restart, restartHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 + 175, _graphics.PreferredBackBufferHeight - 50, 40, 40),
                _graphics, null, font);

            //healthBar
            healthBar = Content.Load<Texture2D>("healthBar");
            healthBarOutline = Content.Load<Texture2D>("healthBarOutline");
            //Currently only used for health bar, but probably used elsewhere too later
            eyeballClosed = Content.Load<Texture2D>("eyeball_closed");

            // Tile loading
            floorTex = Content.Load<Texture2D>("floor_tile");
            wallTex = Content.Load<Texture2D>("wall_placeholder");
            enemyPathTex = Content.Load<Texture2D>("enemy_path_tile");
            goalTex = Content.Load<Texture2D>("goal_tile");

            // PLayer loading
            playerTex = Content.Load<Texture2D>("Player");
            playerLight = Content.Load<Texture2D>("PlayerGlow");
            player = new Player(
                playerTex,
                playerLight,
                new Vector2(
                    (_graphics.PreferredBackBufferWidth - (playerTex.Width / 8)) / 2,
                    (_graphics.PreferredBackBufferHeight - (playerTex.Height / 8)) / 2),
                debugMode);

            // LightManager loading
            lightMask = Content.Load<Texture2D>("light_Dynamic");

            haze = Content.Load<Texture2D>("haze");

            //Use these constants to alter the size of the light
            const int LightWidth = 7;
            const int LightHeight = 6;

            //5 and 4 represent the width and height of the light. Can be altered at will
            lightManager = new LightManager(lightMask, player, LightWidth, LightHeight, debugMode, lightOn, lightOff);

            // Level loading
            enemyTex = Content.Load<Texture2D>("Enemy");
            _levelManager = new LevelManager(null, floorTex, wallTex, enemyTex, enemyPathTex, goalTex, objectPlaceHolder, font, contScreen, _graphics, player, lightManager, pathComplete);

            healthRect = new Rectangle(100 - (int)_levelManager.ScreenOffset.X,
                                       415 - (int)_levelManager.ScreenOffset.Y,
                                       100,
                                       24);

            //Shaders
            effect2 = Content.Load<Effect>("lighteffect");

            //Render targets
            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        /// <summary>
        /// Updates the states of the game based upon the current state of the ScreenManager
        /// </summary>
        /// <param name="gameTime">The GameTime objet</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            colorTime++;

            kbState = Keyboard.GetState();

            // TODO: Add your update logic here
            switch (_screenManager.State)
            {
                case state.Menu:
                    _screenManager.MenuStateUpdate(menuButtonGame, menuButtonInstruction, menuButtonExit, this, player, _levelManager);

                    // Resets debug mode
                    debugMode = false;
                    player.DebugMode = false;
                    lightManager.DebugMode = false;
                    break;

                case state.Game:
                    _screenManager.MainGameStateUpdate(player, lightManager, _levelManager, gameTime);

                    if (_levelManager.GoToContinue)
                    {
                        _screenManager.State = state.Continue;
                    }

                    if (player.Box == null)
                    {
                        Box b;
                        if ((b = _levelManager.BoxManager.CheckCollision(player.Rect)) != null)
                        {
                            if (player.SingleKeyPress(Keys.Space, kbState, previousKbState) && !b.InGoal)
                            {
                                player.Box = b;
                                b.Held = true;
                                pickupBoxSFX.CreateInstance().Play();
                            }
                        }
                    }
                    else if (player.Box != null)
                    {
                        if (player.SingleKeyPress(Keys.Space, kbState, previousKbState))
                        {
                            player.Box.Held = false;

                            float nonTexRotate = player.Rotation - MathHelper.PiOver2;

                            _levelManager.PlaceBoxSafe(
                                player.Box,
                                new Vector2((player.Rect.Width - 10) * (float)Math.Cos(nonTexRotate), (player.Rect.Height - 10) * (float)Math.Sin(nonTexRotate)));
                            player.Box = null;

                            placeBoxSFX.CreateInstance().Play();
                        }
                    }

                    if (player.IsDead)
                    {
                        _screenManager.State = state.Jumped;
                        player.IsDead = false;
                        jumpscareSFX.CreateInstance().Play();
                    }

                    if (_levelManager.WonGame)
                    {
                        _screenManager.State = state.Victory;
                        _levelManager.WonGame = false;
                    }

                    // Debug mode
                    if (debugMode == false && SingleKeyPress(Keys.G, kbState, previousKbState))
                    {
                        player.DebugMode = true;
                        lightManager.DebugMode = true;
                        debugMode = true;
                    }
                    else if (debugMode == true && SingleKeyPress(Keys.G, kbState, previousKbState))
                    {
                        player.DebugMode = false;
                        lightManager.DebugMode = false;
                        debugMode = false;
                    }

                    previousKbState = kbState;
                    break;

                case state.Instructions:
                    _screenManager.InstructionsPageUpdate(player, lightManager, _levelManager, gameTime);
                    if (buttonInstructionsToMenu.ButtonJustClicked())
                        _screenManager.State = state.Menu;

                    // Instruction page transition
                    if (rightArrow.ButtonJustClicked())
                    {
                        _levelManager.NextInstructionPage("right");
                    }
                    else if (leftArrow.ButtonJustClicked())
                    {
                        _levelManager.NextInstructionPage("left");
                    }

                    // Restart Instructions Page
                    if (restartButtonInstructions.ButtonJustClicked())
                    {
                        _levelManager.RestartLevel();
                    }

                    if (player.Box == null)
                    {
                        Box b;
                        if ((b = _levelManager.BoxManager.CheckCollision(player.Rect)) != null && !b.InGoal)
                        {
                            if (player.SingleKeyPress(Keys.Space, kbState, previousKbState))
                            {
                                player.Box = b;
                                b.Held = true;

                                pickupBoxSFX.CreateInstance().Play();
                            }
                        }
                    }
                    else if (player.Box != null)
                    {
                        if (player.SingleKeyPress(Keys.Space, kbState, previousKbState))
                        {
                            player.Box.Held = false;

                            float nonTexRotate = player.Rotation - MathHelper.PiOver2;

                            _levelManager.PlaceBoxSafe(
                                player.Box,
                                new Vector2((player.Rect.Width - 10) * (float)Math.Cos(nonTexRotate), (player.Rect.Height - 10) * (float)Math.Sin(nonTexRotate)));
                            player.Box = null;

                            placeBoxSFX.CreateInstance().Play();
                        }
                    }

                    previousKbState = kbState;
                    break;

                case state.Victory:
                    if (buttonToMenu.ButtonJustClicked())
                        _screenManager.State = state.Menu;
                    break;

                case state.Loss:
                    if (buttonToMenu.ButtonJustClicked())
                        _screenManager.State = state.Menu;
                    break;

                case state.Continue:
                    timer--;
                    if (timer == 0)
                    {
                        _screenManager.State = state.Game;
                        timer = 90 * 1;
                    }
                    break;

                case state.Jumped:

                    //If-else statements to have jumpscare appear for a brief period
                    //before switching back to the game over screen
                    if (jumpScareTimer > jumpScareThreshhold)
                    {
                        if (jumpScareCurr == 1)
                        {
                            if (jumpScarePrev == 0)
                            {
                                jumpScareCurr = 2;
                            }
                            else
                            {
                                jumpScareCurr = 0;
                            }

                            jumpScarePrev = jumpScareCurr;
                        }
                        else
                        {
                            jumpScareCurr = 1;
                        }
                        jumpScareTimer = 0;
                    }
                    else
                    {
                        jumpScareTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    }

                    timer--;
                    if (timer == 0)
                    {
                        _screenManager.State = state.Loss;
                        timer = 90 * 1;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws all objects to the game window based on the current state
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // ScreenDrawing
            switch (_screenManager.State)
            {
                case state.Menu:
                    _screenManager.MenuState(menuButtonGame, menuButtonInstruction, menuButtonExit, font, menuBkg);
                    break;

                case state.Game:
                    _spriteBatch.End();

                    //Alright time for shader stuff
                    //Reset graphicsDevice to light render and draw all lighting to it
                    GraphicsDevice.SetRenderTarget(lightsTarget);
                    GraphicsDevice.Clear(Color.Black);

                    //Reset spritebatch to additive
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    //Draw light
                    _screenManager.MainGameLight(player, lightManager, _spriteBatch, _levelManager);
                    _spriteBatch.Draw(haze, lightsTarget.Bounds, Color.White * brightness);
                    _spriteBatch.End();

                    //Reset to render on main
                    GraphicsDevice.SetRenderTarget(mainTarget);
                    GraphicsDevice.Clear(Color.Transparent);

                    //Setup basic blending
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //Draw game normally
                    _screenManager.MainGameState(player, gameButtonExit, this, _levelManager, _spriteBatch);
                    _spriteBatch.End();

                    //Return to main render target
                    GraphicsDevice.SetRenderTarget(null);
                    GraphicsDevice.Clear(Color.Black);

                    //Splice the two draws
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    effect2.Parameters["lightMask"].SetValue(lightsTarget);
                    effect2.CurrentTechnique.Passes[0].Apply();
                    _spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);

                    //Resets spritebatch back to normal
                    _spriteBatch.End();
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    //Draw others after effects
                    DrawLightMeter();

                    // Draws debug mode if on
                    if (debugMode)
                    {
                        DrawDebugText();
                    }

                    break;

                case state.Instructions:
                    _spriteBatch.End();

                    //Alright time for shader stuff
                    //Reset graphicsDevice to light render and draw all lighting to it
                    GraphicsDevice.SetRenderTarget(lightsTarget);
                    GraphicsDevice.Clear(Color.Black);

                    //Reset spritebatch to additive
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    //Draw light
                    _screenManager.MainGameLight(player, lightManager, _spriteBatch, _levelManager);
                    _spriteBatch.Draw(haze, lightsTarget.Bounds, Color.White * .75f);

                    _spriteBatch.End();

                    //Reset to render on main
                    GraphicsDevice.SetRenderTarget(mainTarget);
                    GraphicsDevice.Clear(Color.Transparent);

                    //Setup basic blending
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //Draw game normally
                    _screenManager.InstructionsPage(lightManager, font, player, _levelManager, _spriteBatch);
                    _spriteBatch.End();

                    //Return to main render target
                    GraphicsDevice.SetRenderTarget(null);
                    GraphicsDevice.Clear(Color.Black);

                    //Splice the two draws
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    effect2.Parameters["lightMask"].SetValue(lightsTarget);
                    effect2.CurrentTechnique.Passes[0].Apply();
                    _spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);

                    _spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);

                    //Resets spritebatch back to normal
                    _spriteBatch.End();
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                    // Manages what arrows to draw to the screen
                    if (_levelManager.NextInstruction != null)
                    {
                        rightArrow.Draw(_spriteBatch);
                    }
                    if (_levelManager.PreviousInstruction != null)
                    {
                        leftArrow.Draw(_spriteBatch);
                    }

                    // Menu button
                    buttonInstructionsToMenu.Draw(_spriteBatch);

                    // Restart button
                    restartButtonInstructions.Draw(_spriteBatch);

                    // Draws instructions text if there is any
                    if (_levelManager.InstructionsText != null && _levelManager.InstructionsPos != null)
                    {
                        for (int i = 0; i < _levelManager.InstructionsText.Count; i++)
                        {
                            _spriteBatch.DrawString(
                            font,
                            _levelManager.InstructionsText[i],
                            new Vector2(
                                _levelManager.InstructionsPos[i * 2],
                                _levelManager.InstructionsPos[(i * 2) + 1]),
                            Color.LightGray);
                        }
                    }

                    DrawLightMeter();

                    break;

                case state.Victory:
                    _screenManager.VictoryScreen(buttonToMenu, winBkg);
                    break;

                case state.Loss:
                    _screenManager.LossScreen(buttonToMenu, loseBkg);
                    break;

                case state.Continue:
                    _screenManager.Continue(contScreen);
                    break;

                case state.Jumped:
                    _screenManager.Jumped(ratJumpScare, jumpScareRect, jumpScareCurr);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Displays the light meter for the player
        /// </summary>
        public void DrawLightMeter()
        {
            //Ensures that light gauge falls within the width of the light bar
            float gaugeWidth = healthRect.Width / (lightManager.MaxGauge * 1.0f);

            _spriteBatch.Draw(healthBarOutline,
                                      new Rectangle(healthRect.X - 2,
                                                    healthRect.Y - 2,
                                                    healthBarOutline.Width,
                                                    healthBarOutline.Height),
                                      Color.White);

            _spriteBatch.Draw(healthBar, new Vector2(healthRect.X, healthRect.Y),
                new Rectangle(0, 0, (int)((healthRect.Width
                    - (lightManager.MaxGauge * gaugeWidth))
                    + (lightManager.Gauge * gaugeWidth)), healthRect.Height),
                Color.White);

            Rectangle eyeMeterRect = new Rectangle(0, 0, player.Rect.Width * 2,
                player.Rect.Height * 2);
            eyeMeterRect.X = healthRect.X - eyeMeterRect.Width;
            eyeMeterRect.Y = (healthRect.Y + (healthRect.Height / 2)) - (eyeMeterRect.Height / 2);

            //Draws the player sprite, outlined in black to indicate that this is the light meter

            _spriteBatch.Draw(playerTex,
                new Rectangle((healthRect.X - 3) - player.Rect.Width,
                    ((healthRect.Y + (healthRect.Height / 2)) - (player.Rect.Height / 2)) - 3,
                    player.Rect.Width + 6, player.Rect.Height + 6),
                new Rectangle((playerTex.Width / 6) * player.Frame, 0, playerTex.Width / 6, playerTex.Height),
                Color.Black);

            _spriteBatch.Draw(playerTex,
                new Rectangle(healthRect.X - player.Rect.Width,
                    (healthRect.Y + (healthRect.Height / 2)) - (player.Rect.Height / 2),
                    player.Rect.Width, player.Rect.Height),
                new Rectangle((playerTex.Width / 6) * player.Frame, 0, playerTex.Width / 6, playerTex.Height),
                Color.White);

            _spriteBatch.DrawString(font,
                "Light",
                new Vector2(healthRect.X, (healthRect.Y + healthRect.Height) + 2),
                Color.LightGray,
                0,
                new Vector2(0, 0),
                0.75f,
                SpriteEffects.None,
                1);
        }

        /// <summary>
        /// Draws the debug text to the screen. I think it's better to put
        /// this into a separate method than cram everything into Draw()
        /// </summary>
        public void DrawDebugText()
        {
            // Drawing player and mouse position for testing
            _spriteBatch.DrawString(
                font,
                String.Format("Player Position: ({0},{1})",
                    player.X, player.Y),
                new Vector2(0, 0),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("New Mouse Position: ({0},{1})",
                    player.NewMousePosition.X, player.NewMousePosition.Y),
                new Vector2(0, 40),
                Color.White);
            MouseState curMouse = Mouse.GetState();
            _spriteBatch.DrawString(
                font,
                String.Format("Actual Mouse Position: ({0},{1})",
                    curMouse.X, curMouse.Y),
                new Vector2(0, 20),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Screen offset: ({0},{1})",
                    _levelManager.ScreenOffset.X, _levelManager.ScreenOffset.Y),
                new Vector2(0, 60),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Num boxes {0}",
                    _levelManager.BoxManager.Count),
                new Vector2(0, 80),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Num enemies {0}",
                    _levelManager.EnemyManager.Enemies.Count),
                new Vector2(0, 100),
                Color.White);

            //Drawing light position for testing
            _spriteBatch.DrawString(
                font,
                String.Format("Light Point 1: ({0},{1})",
                    lightManager.Points[0].X, lightManager.Points[0].Y),
                new Vector2(0, 120),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Light Point 2: ({0},{1})",
                    lightManager.Points[1].X, lightManager.Points[1].Y),
                new Vector2(0, 140),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Light Point 3: ({0},{1})",
                    lightManager.Points[2].X, lightManager.Points[2].Y),
                new Vector2(0, 160),
                Color.White);
            _spriteBatch.DrawString(
                font,
                String.Format("Light Point 4: ({0},{1})",
                    lightManager.Points[3].X, lightManager.Points[3].Y),
                new Vector2(0, 180),
                Color.White);
        }

        /// <summary>
        /// Will check if a key is pressed once this frame and not the frame before
        /// </summary>
        /// <param name="key">key that is being checked for current and previous frame</param>
        /// <param name="kbState">current kb state that is being check</param>
        /// <returns>Whether this is the first frame a key has been pressed</returns>
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
    }
}