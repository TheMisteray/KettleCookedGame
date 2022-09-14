using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UntitledSpoopyGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ScreenManager _screenManager;

        //Loadable Assets
        private Texture2D placeholderButtonTex;
        private Texture2D placeholderButtonHover;

        private SpriteFont staatliches30;

        //Content
        private UIButton menuButtonGame;
        private UIButton menuButtonInstruction;
        private UIButton menuButtonExit;
        private UIButton buttonToMenu;

        private UIButton gameButtonExit;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _screenManager = new ScreenManager(GraphicsDevice, _spriteBatch, _graphics);

            // Load Assets
            //TODO: make the real textures textless, we can add text with a spritefont later
            placeholderButtonTex = Content.Load<Texture2D>("button");
            placeholderButtonHover = Content.Load<Texture2D>("buttonHover");

            staatliches30 = Content.Load<SpriteFont>("arial30");

            //Load content
            menuButtonGame = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 - 50, 200, 100), _graphics);
            menuButtonInstruction = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 + 50, 200, 100), _graphics);
            menuButtonExit = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 100, _graphics.PreferredBackBufferHeight / 2 + 150, 200, 100), _graphics);

            gameButtonExit = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(_graphics.PreferredBackBufferWidth / 2-100, _graphics.PreferredBackBufferHeight/2-50, 200, 100), _graphics);
            buttonToMenu = new UIButton(placeholderButtonTex, placeholderButtonHover,
                new Rectangle(100, 350, 200, 100), _graphics);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kbState = Keyboard.GetState();

            // TODO: Add your update logic here
            switch (_screenManager.State)
            {
                case state.Menu:
                    _screenManager.MenuStateUpdate(menuButtonGame, menuButtonInstruction, menuButtonExit, this);

                    // Test: press 1 to change to victory screen
                    if (kbState.IsKeyDown(Keys.D1))
                    {
                        _screenManager.State = state.Victory;
                    }
                    // Test: press 2 to change to victory screen
                    if (kbState.IsKeyDown(Keys.D2))
                    {
                        _screenManager.State = state.Loss;
                    }
                    break;

                case state.Game:
                    _screenManager.MainGameState(gameButtonExit, this);
                    break;

                case state.Instructions:
                    break;

                case state.Victory:
                    if (buttonToMenu.ButtonJustClicked())
                        _screenManager.State = state.Menu;
                    break;

                case state.Loss:
                    // Comment
                    if (buttonToMenu.ButtonJustClicked())
                        _screenManager.State = state.Menu;
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // ScreenDrawing
            switch (_screenManager.State)
            {
                case state.Menu:
                    _screenManager.MenuState(menuButtonGame, menuButtonInstruction, menuButtonExit, staatliches30);
                    break;

                case state.Game:
                    _screenManager.MainGameState(gameButtonExit, this);
                    break;

                case state.Instructions:
                    _screenManager.InstructionsPage();
                    break;

                case state.Victory:
                    _screenManager.VictoryScreen(buttonToMenu, staatliches30);
                    break;

                case state.Loss:
                    _screenManager.LossScreen(buttonToMenu, staatliches30);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}