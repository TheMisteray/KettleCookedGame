using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UntitledSpoopyGame
{
    public enum state
    {
        Menu,
        Game,
        Instructions,
        Victory,
        Loss
    }

    class ScreenManager
    {
        //Fields
        state screenState;
        GraphicsDeviceManager graphicsDeviceManager;
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;

        //Properties
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
        public void MenuState(UIButton menuButtonGame, UIButton menuButtonInstructions, UIButton menuButtonExit, SpriteFont staatliches30)
        {
            graphicsDevice.Clear(Color.SandyBrown);

            menuButtonGame.Draw(spriteBatch);
            menuButtonInstructions.Draw(spriteBatch);
            menuButtonExit.Draw(spriteBatch);

            spriteBatch.DrawString(staatliches30, "Untitled Spoopy Game", new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2 - staatliches30.MeasureString("Untitled Spoopy Game").Length()/2, 100), Color.Black);
        }

        public void MenuStateUpdate(UIButton menuButtonGame, UIButton menuButtonInstructions, UIButton menuButtonExit, Game g)
        {
            if (menuButtonGame.ButtonJustClicked())
                screenState = state.Game;
            if (menuButtonInstructions.ButtonJustClicked())
                screenState = state.Instructions;
            if (menuButtonExit.ButtonJustClicked())
                g.Exit();
        }

        public void MainGameState(UIButton gameButtonExit, Game g)
        {
            bool isGameOver = false;
            bool isGameWon = false;

            if (gameButtonExit.ButtonJustClicked())
            {
                screenState = state.Menu;
            }

            if (isGameOver && isGameWon)
            {
                screenState = state.Victory;
            }
            else if (isGameOver && !isGameWon)
            {
                screenState = state.Loss;
            }
        }

        public void InstructionsPage()
        {
            
        }

        public void VictoryScreen(UIButton buttonToMenu, SpriteFont staatliches30)
        {
            buttonToMenu.Draw(spriteBatch);
            spriteBatch.DrawString(staatliches30, "You Win!", 
                new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2 - staatliches30.MeasureString("You Win!").Length() / 2, 100), 
                Color.Black);
        }

        public void LossScreen(UIButton buttonToMenu, SpriteFont staatliches30)
        {
            buttonToMenu.Draw(spriteBatch);
            spriteBatch.DrawString(staatliches30, "You Lose",
                new Vector2(graphicsDeviceManager.PreferredBackBufferWidth / 2 - staatliches30.MeasureString("You Lose").Length() / 2, 100),
                Color.Black);
        }
    }
}
