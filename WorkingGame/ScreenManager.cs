using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WorkingGame
{
    public enum state
    {
        Menu,
        Game,
        Instructions,
        Victory,
        Loss,
        Continue,
        Jumped
    }

    //ScreenManager class
    //Manages everything displayed on-screen
    internal class ScreenManager
    {
        //Fields
        private state screenState;

        private GraphicsDeviceManager graphicsDeviceManager;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Read-only property for the current state of the game
        /// </summary>
        public state State
        {
            get => screenState;
            set => screenState = value;
        }

        //Constructor
        /// <summary>
        /// Starts up the manager for screen states
        /// </summary>
        /// <param name="g">Game graphics device</param>
        /// <param name="s">Game spritebatch</param>
        public ScreenManager(GraphicsDevice g, SpriteBatch s, GraphicsDeviceManager gm)
        {
            graphicsDevice = g;
            spriteBatch = s;
            graphicsDeviceManager = gm;
            screenState = state.Menu;
        }

        /// <summary>
        /// Controls all drawing in the menu
        /// </summary>
        public void MenuState(UIButton menuButtonGame, UIButton menuButtonInstructions, UIButton menuButtonExit, SpriteFont font, Texture2D bkg)
        {
            graphicsDevice.Clear(Color.SandyBrown);

            spriteBatch.Draw(bkg, new Rectangle(0, 0, bkg.Width, bkg.Height), Color.White);

            menuButtonGame.Draw(spriteBatch);
            menuButtonInstructions.Draw(spriteBatch);
            menuButtonExit.Draw(spriteBatch);
        }

        /// <summary>
        /// Updates the state when the player is in the main menu of the game
        /// </summary>
        /// <param name="menuButtonGame">The button to start the game</param>
        /// <param name="menuButtonInstructions">The button to see instructions</param>
        /// <param name="menuButtonExit">The button to exit the game</param>
        /// <param name="g">The Game object</param>
        /// <param name="player">The player object</param>
        /// <param name="_levelManager">The level manager object</param>
        public void MenuStateUpdate(UIButton menuButtonGame, UIButton menuButtonInstructions, UIButton menuButtonExit, Game g, Player player, LevelManager _levelManager)
        {
            //Starts the game
            if (menuButtonGame.ButtonJustClicked())
            {
                _levelManager.ReadFile("../../../level_01.txt");
                screenState = state.Game;
                //include this line due to instructions page "demo"
                player.Position = new Vector2(
                        (graphicsDeviceManager.PreferredBackBufferWidth - player.Tex.Width / 18) / 2,
                        (graphicsDeviceManager.PreferredBackBufferHeight - player.Tex.Height / 18) / 2);
            }

            //Loads the instructions pages
            if (menuButtonInstructions.ButtonJustClicked())
            {
                _levelManager.ReadFile("../../../instructions_level.txt");
                screenState = state.Instructions;
            }
            player.Position = new Vector2(
                ((graphicsDeviceManager.PreferredBackBufferWidth - player.Tex.Width / 18) / 2) + 4,
                (graphicsDeviceManager.PreferredBackBufferHeight - player.Tex.Height / 18) / 2);

            //Exits the game
            if (menuButtonExit.ButtonJustClicked())
                g.Exit();
        }

        /// <summary>
        /// Updates the state for when the game has started
        /// </summary>
        /// <param name="player">The player object</param>
        /// <param name="gameButtonExit">The button to return to the menu</param>
        /// <param name="g">The Game object</param>
        /// <param name="_levelManager">The level manager object</param>
        /// <param name="_spriteBatch">The sprite batch object</param>
        public void MainGameState(Player player, UIButton gameButtonExit, Game g, LevelManager _levelManager, SpriteBatch _spriteBatch)
        {
            bool isGameOver = false;
            bool isGameWon = false;

            //gameButtonExit.Draw(spriteBatch);

            _levelManager.DrawLevel(_spriteBatch, graphicsDevice);
            // **** NOTE - Draw player afterwards if not it will be hidden under level
            player.Draw(_spriteBatch, _levelManager.ScreenOffset, graphicsDevice, _levelManager);

            if (isGameOver && isGameWon)
            {
                screenState = state.Victory;
            }
            else if (isGameOver && !isGameWon)
            {
                screenState = state.Jumped;
            }
        }

        /// <summary>
        /// Handles drawing light behavior for the main game, drawing the light
        /// object itself alongside the glow around the player
        /// </summary>
        /// <param name="player">The player object</param>
        /// <param name="lightManager">The light manager object</param>
        /// <param name="_spriteBatch">The SpriteBatch object</param>
        /// <param name="_levelManager">The level manager</param>
        public void MainGameLight(Player player, LightManager lightManager, SpriteBatch _spriteBatch, LevelManager _levelManager)
        {
            lightManager.Draw(_spriteBatch, _levelManager.ScreenOffset);
            player.DrawGlow(_spriteBatch, _levelManager.ScreenOffset);
        }

        /// <summary>
        /// Updates the state of the player, LightManager, and LevelManager
        /// </summary>
        /// <param name="player">The Player object</param>
        /// <param name="lightManager">The LightManager object</param>
        /// <param name="_levelManager">The LevelManager objct</param>
        /// <param name="gameTime">The GameTime object</param>
        public void MainGameStateUpdate(Player player, LightManager lightManager, LevelManager _levelManager, GameTime gameTime)
        {
            player.Update(gameTime, graphicsDeviceManager, _levelManager);
            lightManager.Update(gameTime);
            _levelManager.UpdateLevel(gameTime);
        }

        /// <summary>
        /// Handles drawing the current instructions page
        /// </summary>
        /// <param name="lightManager">The LightManager object</param>
        /// <param name="font">The font for instructional text</param>
        /// <param name="player">The Player object</param>
        /// <param name="lv">The LevelManager object</param>
        /// <param name="sb">The SpriteBatch object</param>
        public void InstructionsPage(LightManager lightManager, SpriteFont font, Player player, LevelManager lv, SpriteBatch sb)
        {
            lv.DrawLevel(sb, graphicsDevice);
            player.Draw(sb, lv.ScreenOffset, graphicsDevice, lv);
        }

        /// <summary>
        /// Handles the updating of the current instructions page
        /// </summary>
        /// <param name="player">The Player object</param>
        /// <param name="lightManager">The LightManager object</param>
        /// <param name="_levelManager">The LevelManager object</param>
        /// <param name="gameTime">The GameTime object</param>
        public void InstructionsPageUpdate(Player player, LightManager lightManager, LevelManager _levelManager, GameTime gameTime)
        {
            player.Update(gameTime, graphicsDeviceManager, _levelManager);
            lightManager.Update(gameTime);
            _levelManager.UpdateLevel(gameTime);
        }

        /// <summary>
        /// diplays victory screen
        /// </summary>
        /// <param name="buttonToMenu"></param>
        /// <param name="bkg"></param>
        public void VictoryScreen(UIButton buttonToMenu, Texture2D bkg)
        {
            spriteBatch.Draw(bkg, new Rectangle(0, 0, bkg.Width, bkg.Height), Color.White);
            buttonToMenu.Draw(spriteBatch);
        }

        /// <summary>
        /// displays loss screen
        /// </summary>
        /// <param name="buttonToMenu"></param>
        /// <param name="bkg"></param>
        public void LossScreen(UIButton buttonToMenu, Texture2D bkg)
        {
            spriteBatch.Draw(bkg, new Rectangle(0, 0, bkg.Width, bkg.Height), Color.White);
            buttonToMenu.Draw(spriteBatch);
        }

        /// <summary>
        /// RAT JUMPSCARE
        /// </summary>
        /// <param name="ratJump"></param>
        /// <param name="ratRect"></param>
        /// <param name="ratCurr"></param>
        public void Jumped(Texture2D ratJump, Rectangle[] ratRect, byte ratCurr)
        {
            spriteBatch.Draw(ratJump, new Vector2(0, 0), ratRect[ratCurr], Color.White);
        }

        /// <summary>
        /// Draws the continue screen for the game (Level Complete screen)
        /// </summary>
        /// <param name="contScreen">The image for the continue screen</param>
        public void Continue(Texture2D contScreen)
        {
            spriteBatch.Draw(contScreen, new Rectangle(0, 0, contScreen.Width, contScreen.Height), Color.White);
        }
    }
}