using Domino.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace Domino
{


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        

        #region Variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;                // SpriteBatch to draw the sprites 
        public delegate void DelegateObject(string s);  
        Color ColorToDraw;
        
        bool PlayerHasWon;
        bool PlayerHasLost;
        int MaxScore=2;   

        // Screen resolution vars
        int screenWidth = 1366, screenHeight = 768;

        // Audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // Main menu states
        enum GameState
        {
            MainMenu,
            Options,
            About,
            Playing,
            Shuffling,
            DifficultyLevel,
            TileColor,
            EndOfRound
        }

        GameState CurrentGameState = GameState.MainMenu;
        
        
        MenuButton btnPlay;
        MenuButton btnOptions;
        MenuButton btnCredits;
        MenuButton btnExit;
        MenuButton btnKeepPlaying;
        MenuButton btnMainMenuDoubleSix;
        MenuButton btnDifficultyLevel;
        MenuButton btnDominoTileColor;
        MenuButton btnWhite;
        MenuButton btnYellow;
        MenuButton btnBlue;
        MenuButton btnRed;
        MenuButton btnGreen;
        MenuButton btnExpert;
        MenuButton btnEasy;
        MenuButton btnVeryEasy;
        MenuButton btnNormal;
        MenuButton btnPassTurn;
        MenuButton btnNextRound;

        Texture2D Congratulations;
        Texture2D SorryTryAgain;

        bool GameStarted = false;
        bool GameLocked;
        bool StartOfNewGame = true;                         
        bool StartOfRound = false;

        Tile LastTileTaken = null;
        Player PlayerWhoLastPlayed;

        Vector2 TablePosition;              // where to position the board
        Vector2 DraggableSquarePosition;     // the draggable square


        Random rand = new Random();         // Se crea una variable de tipo random (random)
        
        Texture2D Background;
        Texture2D EndOfRoundBackground;
        
        SpriteFont FontLetras;              // Font to write info with
        SpriteFont FontCent;


        // Se crean los cuatro jugadores
        Player player1 = new Player("Anthony", false, true);
        Player player2 = new Player("Player 2", false, false);
        Player player3 = new Player("Leonardo", false, false);
        Player player4 = new Player("Player 4", false, false);

        Player PlayerWhoWonLastRound;

        bool IsPlayerPassing = false;
        bool EndOfRound = false;

        int TeamOneTotalPoints = 0;
        int TeamTwoTotalPoints = 0;
        



        public Table Table1;


        Tile LastDominoTilePlayed;

        Tile TileToUpdate = null;

        public List<Tile> AllTilesList = new List<Tile>();                  // A sprite for the player and a list of automated sprites
        public List<Tile> AllTilesListForDealing = new List<Tile>(); 
 

        public List<Player> PlayersList = new List<Player>();               // List of players

        
        DifficultyLevel CurrentDifficultyLevel = DifficultyLevel.VeryEasy;  // Variable que indica el nivel actual de inteligencia artificial
        TileColor CurrentTileColor = TileColor.White;                       // Variable que indica el color actual de la ficha


        #region Variables para guardar datos

        StorageDevice device;
        string containerName = "MyGamesStorage";
        string filename = "mysave.sav";

        [Serializable]
        public struct SaveGame
        {
            public DifficultyLevel DifficultyLevelToSave;
            public TileColor TileColorToSave;
        }

        #endregion


        #region Variables for drawing table

        Texture2D WhiteSquare;                     // the white 64x64 pixels bitmap to draw with

        Vector2 MousePosition;                   // the current position of the mouse

        Rectangle DraggableSquareBorder;            // the boundaries of the draggable Cuadro

        bool[,] Table = new bool[32, 21];        // stores whether there is something in a square

        int SquareSize = 28;                   // how wide/tall the tiles are

        Texture2D OpponentTile;
        Texture2D FichaDePareja;

        //stores the previous and current states of the mouse
        //makes it possible to know if a button was just clicked
        //or whether it was up/down previously as well.
        MouseState PreviousMouseState, CurrentMouseState;

        #endregion



        #endregion

        #region Game Class Constructor and LoadContent Method

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //positions the top left corner of the board - change this to move the board
            TablePosition = new Vector2(95, 85);

            //positions the square to drag
            DraggableSquarePosition = new Vector2((graphics.PreferredBackBufferWidth) - 120, 608);

            // Add necessary component to read/write files
            //Components.Add(new GamerServicesComponent(this));

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            InitiateLoad();

            if (CurrentTileColor == TileColor.White)
                ColorToDraw = Color.White;
            if (CurrentTileColor == TileColor.Yellow)
                ColorToDraw = Color.Yellow;
            if (CurrentTileColor == TileColor.Blue)
                ColorToDraw = Color.Cyan;
            if (CurrentTileColor == TileColor.Red)
                ColorToDraw = Color.Red;
            if (CurrentTileColor == TileColor.Green)
                ColorToDraw = Color.Green;


            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Sobre resolucion de pantalla
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
          //  this.graphics.IsFullScreen = true;
            graphics.ApplyChanges();

            // Para Audio
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            // Start the soundtrack audio
            trackCue = soundBank.GetCue("track");
            trackCue.Play();

            // Sobre el Menu Principal
            btnPlay = new MenuButton(Content.Load<Texture2D>(@"Images\BotonJugar"), graphics.GraphicsDevice);
            btnPlay.SetPosition(new Vector2(871, 350));

            btnKeepPlaying = new MenuButton(Content.Load<Texture2D>(@"Images\BotonSeguirJugando"), graphics.GraphicsDevice);
            btnKeepPlaying.SetPosition(new Vector2(871, 350));

            btnOptions = new MenuButton(Content.Load<Texture2D>(@"Images\BotonOpciones"), graphics.GraphicsDevice);
            btnOptions.SetPosition(new Vector2(871, 420));

            btnCredits = new MenuButton(Content.Load<Texture2D>(@"Images\BotonSobre"), graphics.GraphicsDevice);
            btnCredits.SetPosition(new Vector2(871, 490));

            btnExit = new MenuButton(Content.Load<Texture2D>(@"Images\BotonSalir"), graphics.GraphicsDevice);
            btnExit.SetPosition(new Vector2(871, 560));

            btnMainMenuDoubleSix = new MenuButton(Content.Load<Texture2D>(@"Images\MainMenuDobleSix"), graphics.GraphicsDevice, new Vector2(0, 0));

            btnDifficultyLevel = new MenuButton(Content.Load<Texture2D>(@"Images\botonNivel"), graphics.GraphicsDevice);
            btnDifficultyLevel.SetPosition(new Vector2(842, 330));

            btnDominoTileColor = new MenuButton(Content.Load<Texture2D>(@"Images\botonColorDeDominos"), graphics.GraphicsDevice);
            btnDominoTileColor.SetPosition(new Vector2(842, 640));


            btnWhite = new MenuButton(Content.Load<Texture2D>(@"Images\botonBlanco"), graphics.GraphicsDevice);
            btnWhite.SetPosition(new Vector2(842, 370));


            btnYellow = new MenuButton(Content.Load<Texture2D>(@"Images\botonAmarillo"), graphics.GraphicsDevice);
            btnYellow.SetPosition(new Vector2(842, 470));


            btnBlue = new MenuButton(Content.Load<Texture2D>(@"Images\botonAzul"), graphics.GraphicsDevice);
            btnBlue.SetPosition(new Vector2(842, 520));


            btnRed = new MenuButton(Content.Load<Texture2D>(@"Images\botonRojo"), graphics.GraphicsDevice);
            btnRed.SetPosition(new Vector2(842, 570));


            btnGreen = new MenuButton(Content.Load<Texture2D>(@"Images\botonVerde"), graphics.GraphicsDevice);
            btnGreen.SetPosition(new Vector2(842, 420));


            btnVeryEasy = new MenuButton(Content.Load<Texture2D>(@"Images\botonMuyFacil"), graphics.GraphicsDevice);
            btnVeryEasy.SetPosition(new Vector2(842, 410));


            btnEasy = new MenuButton(Content.Load<Texture2D>(@"Images\botonFacil"), graphics.GraphicsDevice);
            btnEasy.SetPosition(new Vector2(842, 460));


            btnNormal = new MenuButton(Content.Load<Texture2D>(@"Images\botonNormal"), graphics.GraphicsDevice);
            btnNormal.SetPosition(new Vector2(842, 510));

            btnExpert = new MenuButton(Content.Load<Texture2D>(@"Images\botonExperto"), graphics.GraphicsDevice);
            btnExpert.SetPosition(new Vector2(842, 560));

            btnPassTurn = new MenuButton(Content.Load<Texture2D>(@"Images\botonDePasar"), graphics.GraphicsDevice, 1);
            btnPassTurn.SetPosition(new Vector2(700, 710));

            btnNextRound = new MenuButton(Content.Load<Texture2D>(@"Images\botonSiguienteRonda"), graphics.GraphicsDevice, 1);
            btnNextRound.SetPosition(new Vector2(700, 400));



            // Load images
            WhiteSquare = Content.Load<Texture2D>(@"Images\white_64x64");

            // Load font
            FontLetras = Content.Load<SpriteFont>(@"Font\Letras");
            FontCent = Content.Load<SpriteFont>(@"Font\Cent");

            // remembers the draggable squares position, so we can easily test for mouseclicks on it
            DraggableSquareBorder = new Rectangle((int)DraggableSquarePosition.X, (int)DraggableSquarePosition.Y, SquareSize, SquareSize);

            Background = Content.Load<Texture2D>(@"Images\MesaExtendida");

            EndOfRoundBackground = Content.Load<Texture2D>(@"Images\FondoDeFinDeRonda");

            Congratulations = Content.Load<Texture2D>(@"Images\Felicidades");
            SorryTryAgain = Content.Load<Texture2D>(@"Images\MalaSuerte");

            OpponentTile = Content.Load<Texture2D>(@"Images\FichaDeMiOponente");

            FichaDePareja = Content.Load<Texture2D>(@"Images\FichaDeMiFrente");


            #region All Tiles List

            //Load several different automated sprites into the list
            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/00"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 0, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/01"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 1, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/02"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 2, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/03"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 3, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/04"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 4, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/05"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 5, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/06"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/11"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 1, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/12"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 2, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/13"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 3, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/14"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 4, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/15"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 5, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/16"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/22"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 2, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/23"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 3, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/24"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 4, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/25"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 5, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/26"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/33"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 3, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/34"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 4, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/35"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 5, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/36"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/44"),
                10, Vector2.Zero, new Vector2(180, 150), 4, 4, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/45"),
                10, Vector2.Zero, new Vector2(150, 150), 4, 5, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/46"),
                10, Vector2.Zero, new Vector2(150, 150), 4, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/55"),
                10, Vector2.Zero, new Vector2(150, 150), 5, 5, true, true, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/56"),
                10, Vector2.Zero, new Vector2(150, 150), 5, 6, true, false, false));

            AllTilesList.Add(new Tile(Content.Load<Texture2D>(@"Imagenes/66"),
                10, Vector2.Zero, new Vector2(150, 150), 6, 6, true, true, false));

            #endregion

            // Add the 4 players to a list of players
            PlayersList.Add(player1);
            PlayersList.Add(player2);
            PlayersList.Add(player3);
            PlayersList.Add(player4);

            Table1 = new Table(1);

            base.LoadContent();

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        #endregion

        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            

            // Audio
            audioEngine.Update();

            // Mouse functions
            // get the current state of the mouse (position, buttons, etc.)
            CurrentMouseState = Mouse.GetState();

            // remember the mouse position for use in this Update and subsequent Draw
            MousePosition = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);


            // Menu principal
            switch (CurrentGameState)
            {
                case GameState.MainMenu:

                    if (btnPlay.isClicked == true)
                        CurrentGameState = GameState.Playing;

                    btnPlay.Update(CurrentMouseState);

                    if (btnKeepPlaying.isClicked == true)
                        CurrentGameState = GameState.Playing;

                    if (btnOptions.isClicked == true)
                        CurrentGameState = GameState.Options;

                    btnOptions.Update(CurrentMouseState);

                    if (btnCredits.isClicked == true)
                        CurrentGameState = GameState.About;

                    btnCredits.Update(CurrentMouseState);

                    if (btnExit.isClicked == true)
                    {
                        InitiateSave();
                        this.Exit();
                    }

                    btnExit.Update(CurrentMouseState);

                    btnPlay.Update(CurrentMouseState);
                    btnOptions.Update(CurrentMouseState);
                    btnCredits.Update(CurrentMouseState);
                    btnExit.Update(CurrentMouseState);
                    btnMainMenuDoubleSix.Update(CurrentMouseState);
                 


                    break;

                case GameState.Playing:

                    // TODO fix:
                    // If you hit Enter, go to the score menu (this just changes the state and doesnt work correctly because it forces a new round)
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        CurrentGameState = GameState.EndOfRound;


                    // Only deal the dominoes when StartOfNewGame or StartOfRound = true
                    if (StartOfNewGame || StartOfRound)
                    {

                        foreach (Player j in PlayersList)
                        {
                            j.PlayerTileList.Clear();
                        }

                        AllTilesListForDealing.AddRange(AllTilesList);
                        DealDominoTilesToPlayers(AllTilesListForDealing);

                        
                        // Whoever has the double 6 plays first
                        if (StartOfNewGame)
                        {
                            


                            for (int i = 0; i < PlayersList.Count; i++)
                            {
                                foreach (Tile f in PlayersList[i].PlayerTileList)
                                {
                                    if (f.FirstTileValue == 6 && f.SecondTileValue == 6)
                                    {
                                        PlayersList[i].MyTurn = true;
                                        Table1.PlayerInTurn = PlayersList[i];
                                    }
                                }
                            }

                            // If at the start of the round is the AI's turn, go ahead and play
                            if (!Table1.PlayerInTurn.IsHuman)
                            {
                                for (int i = 0; i < Table1.PlayerInTurn.PlayerTileList.Count; i++)
                                {
                                    if (StartOfNewGame)
                                    {
                                        if (Table1.PlayerInTurn.PlayerTileList[i].FirstTileValue == 6 && Table1.PlayerInTurn.PlayerTileList[i].SecondTileValue == 6)
                                        {
                                            Table1.PlayerInTurn.PlayerTileList[i].Position = new Vector2(TablePosition.X + 16 * SquareSize, TablePosition.Y + 10 * SquareSize);
                                            StartOfNewGame = false;
                                            TilePlacementLogic(Table1, Table1.PlayerInTurn.PlayerTileList[i], Table1.PlayerInTurn, i);

                                            break;

                                        }
                                    }

                                }

                                CalculateTurn(Table1.PlayerInTurn);

                            }
                        }


                        else if (StartOfRound)
                        {
                            

                            foreach (Player j in PlayersList)
                            {
                                if (j.MyTurn && !j.IsHuman)
                                {
                                    Table1.PlayerInTurn = j;
                                    j.PlayerTileList[0].Position = new Vector2(TablePosition.X + 16 * SquareSize, TablePosition.Y + 10 * SquareSize);
                                    
                                    TilePlacementLogic(Table1, j.PlayerTileList[0], j, 0);
                                    CalculateTurn(Table1.PlayerInTurn);
                                    break;

                                }



                            }

                            StartOfRound = false; 

                        }

                        


                    }

                    #region AI


                    // AI
                    switch (CurrentDifficultyLevel)
                    {
                        case DifficultyLevel.VeryEasy:
                            if (!Table1.PlayerInTurn.IsHuman && (!EndOfRound) && !StartOfNewGame && !StartOfRound)
                            {
                                DrawTilesForNonHumanPlayersLevel1(Table1.PlayerInTurn);
                                Thread.Sleep(2000);
                                CalculateTurn(Table1.PlayerInTurn);
                            }
                            break;

                        case DifficultyLevel.Easy:
                            if (!Table1.PlayerInTurn.IsHuman && (!EndOfRound) && !StartOfNewGame && !StartOfRound)
                            {
                                DrawTilesForNonHumanPlayersLevel1(Table1.PlayerInTurn);
                                Thread.Sleep(2000);
                                CalculateTurn(Table1.PlayerInTurn);
                            }
                            break;


                        case DifficultyLevel.Normal:
                            if (!Table1.PlayerInTurn.IsHuman && (!EndOfRound) && !StartOfNewGame && !StartOfRound)
                            {
                                DrawTilesForNonHumanPlayersLevel3(Table1.PlayerInTurn);
                                Thread.Sleep(2000);
                                CalculateTurn(Table1.PlayerInTurn);
                            }
                            break;

                        case DifficultyLevel.Expert:
                            if (!Table1.PlayerInTurn.IsHuman && (!EndOfRound) && !StartOfNewGame && !StartOfRound)
                            {
                                DrawTilesForNonHumanPlayersLevel3(Table1.PlayerInTurn);
                                Thread.Sleep(2000);
                                CalculateTurn(Table1.PlayerInTurn);
                            }
                            break;
                    }

                    if (Table1.PlayerInTurn.IsHuman)
                    {
                        FindOutIfPlayerPassesHisTurn(Table1.PlayerInTurn);
                    }


                    #endregion


                    #region Update to drag tiles
                    

                    foreach (Tile f in player1.PlayerTileList)
                    {

                        // if the user just clicked inside the draggable white square - set IsTileBeingDragged to true
                        if (PreviousMouseState.LeftButton == ButtonState.Released && CurrentMouseState.LeftButton == ButtonState.Pressed && f.TileEdge.Contains((int)MousePosition.X, (int)MousePosition.Y))
                        {
                            f.IsTileBeingDragged = true;
                            LastTileTaken = f;

                            trackCue = soundBank.GetCue("DominoPickUp");
                            trackCue.Play();
                        }

                        // if the user just released the mousebutton - set IsTileBeingDragged to false, and check if we should add the square to the board
                        if (PreviousMouseState.LeftButton == ButtonState.Pressed && CurrentMouseState.LeftButton == ButtonState.Released)
                        {
                            f.IsTileBeingDragged = false;


                            Vector2 Square = GetSquareFromMousePosition(MousePosition);
                            Rectangle rectFirstTileToPlay = new Rectangle((int)(TablePosition.X + 14 * SquareSize), (int)(TablePosition.Y + 8 * SquareSize), 5 * SquareSize, 5 * SquareSize);

                            if (Table1.TilesPlayedOnTableList.Count < 1)
                            {

                                // if the mousebutton was released inside the board
                                if (IsMouseInsideBoard() && (rectFirstTileToPlay.Contains((int)MousePosition.X, (int)MousePosition.Y)) &&
                                    Table1.PlayerInTurn.IsHuman)
                                {
                                    // find out which square the mouse is over

                                    // and set that square to true (has a piece)
                                    Table[(int)Square.X, (int)Square.Y] = true;

                                }
                            }
                            else
                            {
                                Rectangle RecFichaExtremoDerecho = new Rectangle((int)Table1.PositionOfRightHandSideEdge.X, (int)Table1.PositionOfRightHandSideEdge.Y, SquareSize, SquareSize);
                                Rectangle RecFichaExtremoIzquierdo = new Rectangle((int)Table1.PositionOfLeftHandSideEdge.X, (int)Table1.PositionOfLeftHandSideEdge.Y, SquareSize, SquareSize);
                                if (RecFichaExtremoDerecho.Contains((int)MousePosition.X, (int)MousePosition.Y) || RecFichaExtremoIzquierdo.Contains((int)MousePosition.X, (int)MousePosition.Y))
                                {
                                    // find out which square the mouse is over

                                    // and set that square to true (has a piece)
                                    try
                                    {
                                        Table[(int)Square.X, (int)Square.Y] = true;
                                    }
                                    catch (Exception e)
                                    {
                                        throw e;
                                    }
                                    

                                }
                            }
                        }
                    }

                    #endregion

                    btnPlay.Update(CurrentMouseState);

                    // If Escape is hit, go back to the main menu
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        GameStarted = true;
                        btnKeepPlaying.Update(CurrentMouseState);
                        CurrentGameState = GameState.MainMenu;
                    }

                    if (btnPassTurn.isClicked == true)
                    {
                        CalculateTurn(Table1.PlayerInTurn);
                    }
                    btnPassTurn.Update(CurrentMouseState);


                    VerifyEndOfRoundCondition();

                    break;


                case GameState.EndOfRound:

                    if (TeamOneTotalPoints>MaxScore)
                    {
                        PlayerHasWon = true;
                    }
                    else if (TeamTwoTotalPoints>MaxScore)
                    {
                        PlayerHasLost = true;
                    }
                    
                    if (btnNextRound.isClicked == true)
                    {
                        StartOfRound = true;                   
                        
                        Table1.TilesPlayedOnTableList.Clear();
                        foreach (Player j in PlayersList)
                        {
                            j.PlayerTileList.Clear();
                        }
                        EndOfRound = false;
                        GameLocked = false;
                        CurrentGameState = GameState.Playing;
                    }

                    btnNextRound.Update(CurrentMouseState);

                    break;

                case GameState.Options:

                    // If Escape is hit, go back to the main menu
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        CurrentGameState = GameState.MainMenu;

                    if (btnDifficultyLevel.isClicked == true)
                    {
                        CurrentGameState = GameState.DifficultyLevel;                        
                    }

                    if (btnDominoTileColor.isClicked == true)
                    {
                        CurrentGameState = GameState.TileColor;
                    }

                    btnDominoTileColor.Update(CurrentMouseState);
                    btnDominoTileColor.Update(CurrentMouseState);
                    btnDifficultyLevel.Update(CurrentMouseState);
                    btnDifficultyLevel.Update(CurrentMouseState);
                    
                   
                   

                    break;

                case GameState.About:

                    // If Escape is hit, go back to the main menu
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        CurrentGameState = GameState.MainMenu;

                    break;

                case GameState.DifficultyLevel:
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        CurrentGameState = GameState.Options;

                    if (btnVeryEasy.isClicked == true)
                    {
                        CurrentDifficultyLevel = DifficultyLevel.VeryEasy;
                    }

                    if (btnEasy.isClicked == true)
                    {
                        CurrentDifficultyLevel = DifficultyLevel.Easy;
                    }

                    if (btnNormal.isClicked == true)
                    {
                        CurrentDifficultyLevel = DifficultyLevel.Normal;
                    }

                    if (btnExpert.isClicked == true)
                    {
                        CurrentDifficultyLevel = DifficultyLevel.Expert;
                    }

                    btnVeryEasy.Update(CurrentMouseState);
                    btnVeryEasy.Update(CurrentMouseState);
                    btnEasy.Update(CurrentMouseState);
                    btnEasy.Update(CurrentMouseState);
                    btnNormal.Update(CurrentMouseState);
                    btnNormal.Update(CurrentMouseState);                    
                    btnExpert.Update(CurrentMouseState);
                    btnExpert.Update(CurrentMouseState);
                 
                    break;

                case GameState.TileColor:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        CurrentGameState = GameState.Options;

                    if (btnWhite.isClicked == true)
                    {
                        CurrentTileColor = TileColor.White;
                        ColorToDraw = Color.White;
                    }

                    if (btnYellow.isClicked == true)
                    {
                        CurrentTileColor = TileColor.Yellow;
                        ColorToDraw = Color.Yellow;
                    }

                    if (btnBlue.isClicked == true)
                    {
                        CurrentTileColor = TileColor.Blue;
                        ColorToDraw = Color.Cyan;
                    }

                    if (btnRed.isClicked == true)
                    {
                        CurrentTileColor = TileColor.Red;
                        ColorToDraw = Color.Red;
                    }

                    if (btnGreen.isClicked == true)
                    {
                        CurrentTileColor = TileColor.Green;
                        ColorToDraw = Color.DarkGreen;
                    }

                    btnWhite.Update(CurrentMouseState);
                    btnWhite.Update(CurrentMouseState);
                    btnYellow.Update(CurrentMouseState);
                    btnYellow.Update(CurrentMouseState);
                    btnBlue.Update(CurrentMouseState);
                    btnBlue.Update(CurrentMouseState);
                    btnRed.Update(CurrentMouseState);
                    btnRed.Update(CurrentMouseState);
                    btnGreen.Update(CurrentMouseState);
                    btnGreen.Update(CurrentMouseState);

                    break;

            }

            // store the current state of the mouse as the old
            PreviousMouseState = CurrentMouseState;
          
            base.Update(gameTime);
        }

        #endregion

        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Menu principal
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    //add a white background
                    GraphicsDevice.Clear(Color.White);
                    spriteBatch.Begin();

                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\MainMenu"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);
                    
                    // Replace new game button with Keep Playing button if the game has already started
                    if (!GameStarted)
                    {
                        btnPlay.Draw(spriteBatch);
                    }
                    else btnKeepPlaying.Draw(spriteBatch);

                    btnOptions.Draw(spriteBatch);
                    btnCredits.Draw(spriteBatch);
                    btnExit.Draw(spriteBatch);
                    btnMainMenuDoubleSix.Draw(spriteBatch);

                    // End drawing
                    spriteBatch.End();

                    break;

                case GameState.Playing:

                    // Add a background color
                    GraphicsDevice.Clear(Color.DarkGreen);

                    // Draw everything
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    
                    DrawText();                // Draw help text
                    DrawBoard();               // Draw the board
                    DrawTilesOnBoard();        // Draw tiles that have already been played
                    DrawDraggableSquare();     // Draw the draggable square, wherever it may be


                    foreach (Tile s in player1.PlayerTileList)       // Draw player's tiles except the one he's dragging
                        if (!s.IsTileBeingDragged)
                        {
                            spriteBatch.Draw(s.Image,
                                     s.Position, null,
                                     ColorToDraw, 0, Vector2.Zero,
                                     .07f, SpriteEffects.None, 1);
                        }



                    #region Draw other player's domino tiles

                    // Draw player 2's tiles (right hand side)
                    Vector2 Player2InitialPosition = new Vector2(1047, 270);

                    for (int i = 0; i < player2.PlayerTileList.Count; i++)
                    {
                        spriteBatch.Draw(OpponentTile, Player2InitialPosition, null,
                            ColorToDraw, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        Player2InitialPosition = new Vector2(Player2InitialPosition.X, Player2InitialPosition.Y + 28.8f);
                    }

                    // Draw player 3's tiles (teammate)
                    Vector2 Player3InitialPosition = new Vector2(449, 32);

                    for (int i = 0; i < player3.PlayerTileList.Count; i++)
                    {
                        spriteBatch.Draw(FichaDePareja, Player3InitialPosition, null,
                            ColorToDraw, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        Player3InitialPosition = new Vector2(Player3InitialPosition.X + 28.8f, Player3InitialPosition.Y);
                    }

                    // Draw player 4's tiles (left hand side)
                    Vector2 Player4InitialPosition = new Vector2(40, 272);

                    for (int i = 0; i < player4.PlayerTileList.Count; i++)
                    {
                        spriteBatch.Draw(OpponentTile, Player4InitialPosition, null,
                            ColorToDraw, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        Player4InitialPosition = new Vector2(Player4InitialPosition.X, Player4InitialPosition.Y + 28.8f);
                    }
                    #endregion

                    // Draw board's background
                    spriteBatch.Draw(Background,
                        new Rectangle(0, 0, Window.ClientBounds.Width,
                        Window.ClientBounds.Height), null,
                        Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.0f);
                  

                    if (Table1.PlayerInTurn.IsHuman && IsPlayerPassing && Table1.PlayerInTurn.PlayerTileList.Count>0)
                    {

                        btnPassTurn.Draw(spriteBatch, 1f);
                    }

                    // Print current player in turn
                    spriteBatch.DrawString(FontLetras, "Turn:", new Vector2(1162, 720), Color.White);

                    foreach (Player j in PlayersList)
                    {
                        if (j.MyTurn)
                            spriteBatch.DrawString(FontLetras, j.Name, new Vector2(1232, 720), Color.White);
                    }
                   
                    // End drawing
                    spriteBatch.End();


                    break;

                case GameState.EndOfRound:
                    
                    String TeamOnePoints = TeamOneTotalPoints.ToString();
                    String TeamTwoPoints = TeamTwoTotalPoints.ToString();


                    spriteBatch.Begin();

                    DelegateObject myDelegate = n => { string s = n ; spriteBatch.DrawString(FontLetras, s, new Vector2(570, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f); };
                    DelegateObject myDelegate2 = j => { string s = j; spriteBatch.DrawString(FontLetras, s, new Vector2(702, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f); };
                    
                    myDelegate(TeamOnePoints);
                    myDelegate2(TeamTwoPoints);


                    spriteBatch.Draw(EndOfRoundBackground, new Vector2(110, 60), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, .95f);

                    if (PlayerHasWon)
                    {
                        spriteBatch.Draw(Congratulations, new Vector2(110, 400), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, 1f);
                    }
                    if (PlayerHasLost)
                    {
                        spriteBatch.Draw(SorryTryAgain, new Vector2(110, 400), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, 1f);
                    }

                    if (GameLocked)
                    {
                        spriteBatch.DrawString(FontCent, "Game was locked by: " + PlayerWhoLastPlayed.Name, new Vector2(250, 100), Color.DarkGreen, 0f, Vector2.Zero, .7f, SpriteEffects.None, .99f);
                    }

                    else
                    {
                        spriteBatch.DrawString(FontCent, "END OF ROUND", new Vector2(220, 100), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    }
                        

                    spriteBatch.DrawString(FontCent, player1.Name, new Vector2(170, 190), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontCent, player2.Name, new Vector2(170, 250), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontCent, player3.Name, new Vector2(170, 310), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontCent, player4.Name, new Vector2(170, 370), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);


                    spriteBatch.DrawString(FontLetras, "SCORE", new Vector2(580, 200), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontLetras, "Team 1: ", new Vector2(530, 240), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontLetras, player1.Name, new Vector2(530, 260), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontLetras, player3.Name, new Vector2(530, 280), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    
                    spriteBatch.DrawString(FontLetras, TeamOnePoints, new Vector2(570, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    spriteBatch.DrawString(FontLetras, "Team 2: ", new Vector2(662, 240), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontLetras, player2.Name, new Vector2(662, 260), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(FontLetras, player4.Name, new Vector2(662, 280), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    spriteBatch.DrawString(FontLetras, TeamTwoPoints, new Vector2(702, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    

                    btnNextRound.Draw(spriteBatch, 1f);

                    Vector2 Player1EndOfRoundTilePosition = new Vector2(300, 180);
                    foreach (Tile s in player1.PlayerTileList)       // Draw player's tiles
                    {
                        s.LastOrientation = 1;
                        spriteBatch.Draw(s.Image,
                                    Player1EndOfRoundTilePosition, null,
                                    ColorToDraw, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);

                        Player1EndOfRoundTilePosition = new Vector2(Player1EndOfRoundTilePosition.X + 28.8f, Player1EndOfRoundTilePosition.Y);
                    }

                    Vector2 Player2EndOfRoundTilePosition = new Vector2(300, 240);
                    foreach (Tile s in player2.PlayerTileList)       // Draw player's tiles
                    {
                        s.LastOrientation = 1;
                        spriteBatch.Draw(s.Image,
                                    Player2EndOfRoundTilePosition, null,
                                    ColorToDraw, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        Player2EndOfRoundTilePosition = new Vector2(Player2EndOfRoundTilePosition.X + 28.8f, Player2EndOfRoundTilePosition.Y);
                    }

                    Vector2 Player3EndOfRoundTilePosition = new Vector2(300, 300);
                    foreach (Tile s in player3.PlayerTileList)       // Draw player's tiles
                    {
                        s.LastOrientation = 1;
                        spriteBatch.Draw(s.Image,
                                    Player3EndOfRoundTilePosition, null,
                                    ColorToDraw, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        Player3EndOfRoundTilePosition = new Vector2(Player3EndOfRoundTilePosition.X + 28.8f, Player3EndOfRoundTilePosition.Y);
                    }


                    Vector2 Player4EndOfRoundTilePosition = new Vector2(300, 360);
                    foreach (Tile s in player4.PlayerTileList)       // Draw player's tiles
                    {
                        s.LastOrientation = 1;
                        spriteBatch.Draw(s.Image,
                                    Player4EndOfRoundTilePosition, null,
                                    ColorToDraw, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        Player4EndOfRoundTilePosition = new Vector2(Player4EndOfRoundTilePosition.X + 28.8f, Player4EndOfRoundTilePosition.Y);
                    }

                    spriteBatch.End();


                    

                    break;

                case GameState.Options:

                    GraphicsDevice.Clear(Color.Red);
                    spriteBatch.Begin();
                   
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);
                    btnDifficultyLevel.Draw(spriteBatch);
                    btnDominoTileColor.Draw(spriteBatch);

                    spriteBatch.End();

                    break;

                case GameState.About:

                    spriteBatch.Begin();

                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\SobreDomino"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    spriteBatch.DrawString(FontCent, "About...", new Vector2(621, 500), Color.DarkGreen);
                    spriteBatch.DrawString(FontCent, "Domino Tropical v1.0", new Vector2(621, 540), Color.DarkGreen);
                    spriteBatch.DrawString(FontCent, "Developed by:  Leonardo Lopez & Anthony Ortiz", new Vector2(621, 580), Color.DarkGreen);
                    //spriteBatch.DrawString(FontCent, "Anthony Ortiz", new Vector2(741, 620), Color.DarkGreen);

                    spriteBatch.DrawString(FontLetras, "Press Escape to continue...", new Vector2(945, 700), Color.DarkGreen, 0, Vector2.Zero, .7f, SpriteEffects.None, 1);

                    spriteBatch.End();

                    break;

                case GameState.DifficultyLevel:

                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    if (CurrentDifficultyLevel==DifficultyLevel.VeryEasy)
                    {
                        btnVeryEasy.Draw(spriteBatch, Color.Black);
                        btnEasy.Draw(spriteBatch);
                        btnNormal.Draw(spriteBatch);
                        btnExpert.Draw(spriteBatch);
                    }

                    if (CurrentDifficultyLevel == DifficultyLevel.Easy)
                    {
                        btnEasy.Draw(spriteBatch, Color.Black);
                        btnVeryEasy.Draw(spriteBatch);
                        btnNormal.Draw(spriteBatch);
                        btnExpert.Draw(spriteBatch);
                    }

                    if (CurrentDifficultyLevel == DifficultyLevel.Normal)
                    {
                        btnNormal.Draw(spriteBatch, Color.Black);
                        btnVeryEasy.Draw(spriteBatch);
                        btnEasy.Draw(spriteBatch);
                        btnExpert.Draw(spriteBatch);
                    }

                    if (CurrentDifficultyLevel == DifficultyLevel.Expert)
                    {
                        btnExpert.Draw(spriteBatch, Color.Black);
                        btnVeryEasy.Draw(spriteBatch);
                        btnEasy.Draw(spriteBatch);
                        btnNormal.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                case GameState.TileColor:

                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Images\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    if (CurrentTileColor==TileColor.White)
                    {
                        btnWhite.Draw(spriteBatch,Color.Black);
                        btnYellow.Draw(spriteBatch);
                        btnBlue.Draw(spriteBatch);
                        btnRed.Draw(spriteBatch);
                        btnGreen.Draw(spriteBatch);
                    }

                    if (CurrentTileColor == TileColor.Yellow)
                    {
                        btnWhite.Draw(spriteBatch);
                        btnYellow.Draw(spriteBatch, Color.Black);
                        btnBlue.Draw(spriteBatch);
                        btnRed.Draw(spriteBatch);
                        btnGreen.Draw(spriteBatch);
                    }

                    if (CurrentTileColor == TileColor.Blue)
                    {
                        btnWhite.Draw(spriteBatch);
                        btnYellow.Draw(spriteBatch);
                        btnBlue.Draw(spriteBatch, Color.Black);
                        btnRed.Draw(spriteBatch);
                        btnGreen.Draw(spriteBatch);
                    }

                    if (CurrentTileColor == TileColor.Red)
                    {
                        btnWhite.Draw(spriteBatch);
                        btnYellow.Draw(spriteBatch);
                        btnBlue.Draw(spriteBatch);
                        btnRed.Draw(spriteBatch, Color.Black);
                        btnGreen.Draw(spriteBatch);
                    }

                    if (CurrentTileColor == TileColor.Green)
                    {
                        btnWhite.Draw(spriteBatch);
                        btnYellow.Draw(spriteBatch);
                        btnBlue.Draw(spriteBatch);
                        btnRed.Draw(spriteBatch);
                        btnGreen.Draw(spriteBatch, Color.Black);
                    }

                    spriteBatch.End();

                    break;
            }
            base.Draw(gameTime);
        }

        #endregion

        #region Draw Helper Methods

        // Dibuja el texto
        private void DrawText()
        {
            String PuntosDeEquipo1 = TeamOneTotalPoints.ToString();
            String PuntosDeEquipo2 = TeamTwoTotalPoints.ToString();
            spriteBatch.DrawString(FontLetras, "Domino Tropical", new Vector2(50, 20), Color.White);

            spriteBatch.DrawString(FontLetras, "SCORE", new Vector2(1150, 100), Color.White);
            spriteBatch.DrawString(FontLetras, "Team 1: "  , new Vector2(1100, 140), Color.White);
            spriteBatch.DrawString(FontLetras, player1.Name, new Vector2(1100, 160), Color.White);
            spriteBatch.DrawString(FontLetras,  player3.Name, new Vector2(1100, 180), Color.White);
            spriteBatch.DrawString(FontLetras, PuntosDeEquipo1, new Vector2(1140, 200), Color.White);

            spriteBatch.DrawString(FontLetras, "Team 2: ", new Vector2(1232, 140), Color.White);
            spriteBatch.DrawString(FontLetras, player2.Name, new Vector2(1232, 160), Color.White);
            spriteBatch.DrawString(FontLetras, player4.Name, new Vector2(1232, 180), Color.White);
            spriteBatch.DrawString(FontLetras, PuntosDeEquipo2, new Vector2(1272, 200), Color.White);

        }


        // Draws the draggable square either under the mouse, if it is currently being dragged, or in its default position
        private void DrawDraggableSquare()
        {
            foreach (Tile h in player1.PlayerTileList)
            {
                if (h.IsTileBeingDragged)
                {
                    spriteBatch.Draw(h.Image, new Rectangle((int)(MousePosition.X - WhiteSquare.Width / 4), (int)(MousePosition.Y - WhiteSquare.Height / 4), 30, 60), null, ColorToDraw, 0, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
        }


        // Draws the game board
        private void DrawBoard()
        {
            float Opacity = 1;                                      // how opaque/transparent to draw the square
            Color ColorToUse = Color.White;                         // background color to use
            Rectangle PositionToDrawSquare = new Rectangle();       // the square to draw (local variable to avoid creating a new variable per square)

            // for all columns
            for (int x = 0; x < Table.GetLength(0); x++)
            {
                // for all rows
                for (int y = 0; y < Table.GetLength(1); y++)
                {
                    // figure out where to draw the square
                    PositionToDrawSquare = new Rectangle((int)(x * SquareSize + TablePosition.X), (int)(y * SquareSize + TablePosition.Y), SquareSize, SquareSize);

                    // the code below will make the board checkered using only a single, white square:

                    // if we add the x and y value of the Cuadro
                    // and it is even, we make it one third opaque
                    if ((x + y) % 2 == 0)
                    {
                        Opacity = .15f;
                    }
                    else
                    {
                        // otherwise it is one tenth opaque
                        Opacity = .1f;
                    }

                    // make the square the mouse is over red
                    if (IsMouseInsideBoard() && IsMouseCoordinateWithinBoard(x, y))
                    {
                        ColorToUse = Color.Red;
                        Opacity = .5f;
                    }
                    else
                    {
                        ColorToUse = Color.White;
                    }

                    // draw the white square at the given position, offset by the x- and y-offset, in the opacity desired
                    spriteBatch.Draw(WhiteSquare, PositionToDrawSquare, null, ColorToUse * Opacity, 0, Vector2.Zero, SpriteEffects.None, 0);


                    //if the square has a rectangle - draw it
                    if (Table[x, y])
                    {
                        Rectangle recTileRightHandSide = new Rectangle((int)Table1.PositionOfRightHandSideEdge.X, (int)Table1.PositionOfRightHandSideEdge.Y, SquareSize, SquareSize);
                        Rectangle recTileLeftHandSide = new Rectangle((int)Table1.PositionOfLeftHandSideEdge.X, (int)Table1.PositionOfLeftHandSideEdge.Y, SquareSize, SquareSize);
                        if (((LastTileTaken.FirstTileValue == Table1.RightHandSide) || (LastTileTaken.SecondTileValue == Table1.RightHandSide)
                            || (StartOfNewGame) || (StartOfRound)) && (recTileRightHandSide.Contains((int)MousePosition.X, (int)MousePosition.Y)))
                        {
                            TileToUpdate = LastTileTaken;
                            TileToUpdate.Position = new Vector2((int)(x * SquareSize + TablePosition.X), (int)(y * SquareSize + TablePosition.Y));
                            UpdateTable(Table1, TileToUpdate);
                            Table[x, y] = false;
                        }
                        else if (((LastTileTaken.SecondTileValue == Table1.LeftHandSide)
                              || (LastTileTaken.FirstTileValue == Table1.LeftHandSide) || (StartOfNewGame) || (StartOfRound)) && (recTileLeftHandSide.Contains((int)MousePosition.X, (int)MousePosition.Y)))
                    	{
                            TileToUpdate = LastTileTaken;
                            TileToUpdate.Position = new Vector2((int)(x * SquareSize + TablePosition.X), (int)(y * SquareSize + TablePosition.Y));
                            UpdateTable(Table1, TileToUpdate);
                            Table[x, y] = false;
	                    }

                        else if(StartOfNewGame && LastTileTaken.FirstTileValue == 6 && LastTileTaken.SecondTileValue == 6)
                        {
                            TileToUpdate = LastTileTaken;
                            TileToUpdate.Position = new Vector2((int)(x * SquareSize + TablePosition.X), (int)(y * SquareSize + TablePosition.Y));
                            UpdateTable(Table1, TileToUpdate);
                            Table[x, y] = false;
                        }
                        else if (StartOfRound)
                        {
                            TileToUpdate = LastTileTaken;
                            TileToUpdate.Position = new Vector2((int)(x * SquareSize + TablePosition.X), (int)(y * SquareSize + TablePosition.Y));
                            UpdateTable(Table1, TileToUpdate);
                            Table[x, y] = false;
                        }
                        else
                        {
                            Table[x, y] = false;
                        }

                    }

                }
            }
        }

        private void DrawTilesOnBoard()
        {

            foreach (Tile h in Table1.TilesPlayedOnTableList)
            {
                if (h.LastOrientation == 1)
                    spriteBatch.Draw(h.Image, h.Position, null, ColorToDraw, 0, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.LastOrientation == 2)
                    spriteBatch.Draw(h.Image, new Vector2(h.Position.X + 28, h.Position.Y), null, ColorToDraw, (float)Math.PI / 2, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.LastOrientation == 3)
                    spriteBatch.Draw(h.Image, new Vector2(h.Position.X + 28, h.Position.Y + 28), null, ColorToDraw, (float)Math.PI, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.LastOrientation == 4)
                    spriteBatch.Draw(h.Image, new Vector2(h.Position.X, h.Position.Y + 28), null, ColorToDraw, (float)Math.PI * 3 / 2, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);
            }
        }

        #endregion

        #region Table and Mouse Behaviour Methods

        // Checks to see whether a given coordinate is within the board
        private bool IsMouseCoordinateWithinBoard(int x, int y)
        {
            // do an integerdivision (whole-number) of the coordinates relative to the board offset with the tilesize in mind
            return (int)(MousePosition.X - TablePosition.X) / SquareSize == x && (int)(MousePosition.Y - TablePosition.Y) / SquareSize == y;
        }

        // find out whether the mouse is inside the board
        bool IsMouseInsideBoard()
        {
            if (MousePosition.X >= TablePosition.X && MousePosition.X <= TablePosition.X + Table.GetLength(0) * SquareSize && MousePosition.Y >= TablePosition.Y && MousePosition.Y <= TablePosition.Y + Table.GetLength(1) * SquareSize)
            {
                return true;
            }
            else
            { return false; }
        }

        // get the column/row on the board for a given coordinate
        Vector2 GetSquareFromMousePosition(Vector2 position)
        {
            // adjust for the boards offset (TablePosition) and do an integer division
            return new Vector2((int)(MousePosition.X - TablePosition.X) / SquareSize, (int)(MousePosition.Y - TablePosition.Y) / SquareSize);
        }

        #endregion

        #region Game Methods

        /// <summary> 
        /// Deals seven dominoes to each player 
        /// </summary> 
        private void DealDominoTilesToPlayers(List<Tile> ListOfDominoTilesToDeal)
        {

            // Tiles are dealt
            // variable to hold the domino to remove 
            Tile dominoTileToRemove = null;
            if (ListOfDominoTilesToDeal.Count > 0)
            {
                // seven times per player
                for (int i = 0; i < 7; i++)
                {
                    // ...give each player a domino 
                    foreach (Player player in PlayersList)
                    {
                        // get a random position 
                        int RandomTilePosition = rand.Next(ListOfDominoTilesToDeal.Count);
                        // store the domino in a variable 
                        dominoTileToRemove = ListOfDominoTilesToDeal[RandomTilePosition];
                        // remove it from the list 
                        ListOfDominoTilesToDeal.RemoveAt(RandomTilePosition);
                        // add it to the player's dominoes 
                        player.PlayerTileList.Add(dominoTileToRemove);

                    }
                }

            }

            Vector2 InitialPlayerPosition = new Vector2(449, 680);

            for (int i = 0; i < player1.PlayerTileList.Count; i++)
            {
                player1.PlayerTileList[i].Position = InitialPlayerPosition;
                player1.PlayerTileList[i].TileEdge = new Rectangle((int)player1.PlayerTileList[i].Position.X, (int)player1.PlayerTileList[i].Position.Y, 28, 56);

                InitialPlayerPosition = new Vector2(InitialPlayerPosition.X + 28.8f, 680);
            }
        }


        private void UpdateTable(Table Table1, Tile TileToUpdate)
        {

            // ...give each player a domino 
            foreach (Player player in PlayersList)
            {
                for (int i = 0; i < player.PlayerTileList.Count; i++)
                {

                    if ((player.PlayerTileList[i].FirstTileValue == TileToUpdate.FirstTileValue) && (player.PlayerTileList[i].SecondTileValue == TileToUpdate.SecondTileValue)
                        && player.IsHuman && (Table1.PlayerInTurn == player))
                    {
                        if (StartOfNewGame)
                        {
                            if (player.PlayerTileList[i].FirstTileValue == 6 && player.PlayerTileList[i].SecondTileValue == 6)
                            {
                                TilePlacementLogic(Table1, TileToUpdate, player, i);
                                StartOfNewGame = false;
                            }

                        }

                        else if (StartOfRound)
                        {
                            TilePlacementLogic(Table1, TileToUpdate, player, i);
                            StartOfRound = false;
                        }

                        else if ((player.PlayerTileList[i].FirstTileValue == Table1.RightHandSide) || (player.PlayerTileList[i].FirstTileValue == Table1.LeftHandSide)
                            || (player.PlayerTileList[i].SecondTileValue == Table1.RightHandSide) || (player.PlayerTileList[i].SecondTileValue == Table1.LeftHandSide))
                        {
                            TilePlacementLogic(Table1, TileToUpdate, player, i);
                        }


                    }

                }

            }

        }


        /// <summary>
        /// This is very messy and needs some emergency refactoring
        /// </summary>
        /// <param name="Table1"></param>
        /// <param name="TileToUpdate"></param>
        /// <param name="Player"></param>
        /// <param name="PositionToRemove"></param>
        private void TilePlacementLogic(Table Table1, Tile TileToUpdate, Player Player, int PositionToRemove)
        {
            Vector2 ReferencePosition = TileToUpdate.Position;
            if (Table1.TilesPlayedOnTableList.Count < 1)
            {
                if (TileToUpdate.IsTileADouble)
                {
                    TileToUpdate.LastOrientation = 1;
                    TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - 14);
                    Table1.PositionOfRightHandSideEdge = new Vector2((int)(ReferencePosition.X + SquareSize), (int)(ReferencePosition.Y));
                    Table1.PositionOfLeftHandSideEdge = new Vector2((int)(ReferencePosition.X - SquareSize), (int)(ReferencePosition.Y));
                }
                else
                {
                    Table1.PositionOfRightHandSideEdge = new Vector2((int)(ReferencePosition.X + SquareSize), (int)(ReferencePosition.Y));
                    Table1.PositionOfLeftHandSideEdge = new Vector2((int)(ReferencePosition.X - 2 * SquareSize), (int)(ReferencePosition.Y));
                }

                Table1.RightHandSide = TileToUpdate.FirstTileValue;
                Table1.LeftHandSide = TileToUpdate.SecondTileValue;
                Table1.TileOnRightHandSide = TileToUpdate;
                Table1.TileOnLeftHandSide = TileToUpdate;
                Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                if (Table1.PlayerInTurn == player1)
                {
                    CalculateTurn(Table1.PlayerInTurn);
                }


            }

            else
            {
                if ((Table1.TileOnRightHandSide.LastOrientation == 1 || Table1.TileOnRightHandSide.LastOrientation == 3) && (TileToUpdate.IsTileADouble)
                    && (ReferencePosition.Y > Table1.PositionOfTileOnRightHandSide.Y) && (Table1.RightHandSide == TileToUpdate.FirstTileValue))
                {

                    TileToUpdate.LastOrientation = 2;
                    TileToUpdate.Position = new Vector2(TileToUpdate.Position.X + 14, TileToUpdate.Position.Y);
                    Table1.TileOnRightHandSide = TileToUpdate;
                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y + SquareSize);
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);


                }

                else if ((Table1.TileOnLeftHandSide.LastOrientation == 1 || Table1.TileOnLeftHandSide.LastOrientation == 3) && (TileToUpdate.IsTileADouble)
                     && (ReferencePosition.Y < Table1.PositionOfTileOnLeftHandSide.Y) && (Table1.LeftHandSide == TileToUpdate.FirstTileValue))
                {

                    TileToUpdate.LastOrientation = 2;
                    TileToUpdate.Position = new Vector2(TileToUpdate.Position.X + 14, TileToUpdate.Position.Y);
                    Table1.TileOnLeftHandSide = TileToUpdate;
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y - SquareSize);
                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);

                }
                else if ((Table1.TileOnLeftHandSide.LastOrientation == 2 || Table1.TileOnLeftHandSide.LastOrientation == 4) && (TileToUpdate.IsTileADouble)
                    && ReferencePosition.X < Table1.PositionOfTileOnLeftHandSide.X && Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                {

                    TileToUpdate.LastOrientation = 1;
                    TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - 14);
                    Table1.TileOnLeftHandSide = TileToUpdate;
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - SquareSize, ReferencePosition.Y);
                    if (ReferencePosition.X < (TablePosition.X + (4 * SquareSize)))
                    {
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y + 14);
                        TileToUpdate.LastOrientation = 2;
                        Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - SquareSize, ReferencePosition.Y - SquareSize);


                    }

                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);


                }

                else if ((Table1.TileOnRightHandSide.LastOrientation == 2 || Table1.TileOnRightHandSide.LastOrientation == 4) && (TileToUpdate.IsTileADouble)
                       && ReferencePosition.X > Table1.PositionOfTileOnRightHandSide.X && Table1.RightHandSide == TileToUpdate.FirstTileValue)
                {

                    TileToUpdate.LastOrientation = 1;
                    TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - 14);
                    Table1.TileOnRightHandSide = TileToUpdate;
                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + SquareSize, ReferencePosition.Y);
                    if (ReferencePosition.X > (TablePosition.X + (27 * SquareSize)))
                    {
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y + 14);
                        TileToUpdate.LastOrientation = 4;
                        Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + SquareSize, ReferencePosition.Y + SquareSize);


                    }
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);

                }


               // All well so far


                else if ((Table1.TileOnRightHandSide.LastOrientation == 1 || Table1.TileOnRightHandSide.LastOrientation == 3) && (!TileToUpdate.IsTileADouble)
                    && Table1.TileOnRightHandSide.IsTileADouble && ReferencePosition.X > Table1.TileOnRightHandSide.Position.X)
                {

                    // Mesa1.TileOnRightHandSide = FichaAActualizar;
                    if (Table1.RightHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 4;

                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.SecondTileValue;


                    }

                    else if (Table1.RightHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 2;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X + 28, TileToUpdate.Position.Y);
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.FirstTileValue;


                    }
                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + 2 * SquareSize, ReferencePosition.Y);
                    if (ReferencePosition.X > (TablePosition.X + (27 * SquareSize)))
                    {
                        Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + SquareSize, ReferencePosition.Y + SquareSize);


                    }
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);

                }
                else if ((Table1.TileOnLeftHandSide.LastOrientation == 1 || Table1.TileOnLeftHandSide.LastOrientation == 3)
                 && (!TileToUpdate.IsTileADouble) && Table1.TileOnLeftHandSide.IsTileADouble && (ReferencePosition.X < Table1.TileOnLeftHandSide.Position.X))
                {

                    if (Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 2;

                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.SecondTileValue;

                    }

                    else if (Table1.LeftHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 4;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X - SquareSize, ReferencePosition.Y);
                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.FirstTileValue;

                    }
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - 2 * SquareSize, ReferencePosition.Y);
                    if (ReferencePosition.X < (TablePosition.X + (4 * SquareSize)))
                    {
                        Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - SquareSize, ReferencePosition.Y - SquareSize);


                    }
                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }



                else if ((Table1.TileOnRightHandSide.LastOrientation == 2 || Table1.TileOnRightHandSide.LastOrientation == 4)
                     && (!TileToUpdate.IsTileADouble) && Table1.TileOnRightHandSide.IsTileADouble && ReferencePosition.Y > Table1.TileOnRightHandSide.Position.Y)
                {
                    if (Table1.RightHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.SecondTileValue;

                    }

                    else if (Table1.RightHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y + 28);
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.FirstTileValue;

                    }
                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y + 2 * SquareSize);
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }

                else if ((Table1.TileOnLeftHandSide.LastOrientation == 2 || Table1.TileOnLeftHandSide.LastOrientation == 4)
                     && (!TileToUpdate.IsTileADouble) && Table1.TileOnLeftHandSide.IsTileADouble && ReferencePosition.Y < Table1.TileOnLeftHandSide.Position.Y)
                {
                    if (Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;
                        Table1.LeftHandSide = TileToUpdate.SecondTileValue;
                        Table1.TileOnLeftHandSide = TileToUpdate;

                    }

                    else if (Table1.LeftHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - SquareSize);
                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.FirstTileValue;


                    }
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y - 2 * SquareSize);
                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);

                }

                    // Needs fixing
                else if ((Table1.TileOnRightHandSide.LastOrientation == 1 || Table1.TileOnRightHandSide.LastOrientation == 3)
                     && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnRightHandSide.IsTileADouble) && ReferencePosition.Y > Table1.TileOnRightHandSide.Position.Y)
                {
                    if (Table1.RightHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;

                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.RightHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y + SquareSize);
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.FirstTileValue;
                    }

                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y + 2 * SquareSize);
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }


                else if ((Table1.TileOnLeftHandSide.LastOrientation == 1 || Table1.TileOnLeftHandSide.LastOrientation == 3)
                    && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnLeftHandSide.IsTileADouble) && ReferencePosition.Y < Table1.TileOnLeftHandSide.Position.Y)
                {
                    if (Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;

                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.LeftHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - SquareSize);
                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.FirstTileValue;
                    }
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y - 2 * SquareSize);
                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }

                else if ((Table1.TileOnLeftHandSide.LastOrientation == 2 || Table1.TileOnLeftHandSide.LastOrientation == 4)
                    && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnLeftHandSide.IsTileADouble) && ReferencePosition.X < Table1.TileOnLeftHandSide.Position.X)
                {
                    if (Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 2;

                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.LeftHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 4;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X - SquareSize, TileToUpdate.Position.Y);
                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.FirstTileValue;
                    }
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - 2 * SquareSize, ReferencePosition.Y);
                    if (ReferencePosition.X < (TablePosition.X + (4 * SquareSize)))
                    {
                        Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X - SquareSize, ReferencePosition.Y - SquareSize);
                    }

                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }


                else if ((Table1.TileOnRightHandSide.LastOrientation == 2 || Table1.TileOnRightHandSide.LastOrientation == 4)
                    && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnRightHandSide.IsTileADouble) && ReferencePosition.X > Table1.TileOnRightHandSide.Position.X)
                {
                    if (Table1.RightHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 4;

                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.RightHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 2;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X + SquareSize, TileToUpdate.Position.Y);
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.FirstTileValue;

                    }

                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + 2 * SquareSize, ReferencePosition.Y);

                    if (ReferencePosition.X > (TablePosition.X + (27 * SquareSize)))
                    {
                        Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X + SquareSize, ReferencePosition.Y + SquareSize);
                    }

                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }

                else if ((Table1.TileOnRightHandSide.LastOrientation == 2 || Table1.TileOnRightHandSide.LastOrientation == 4)
                    && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnRightHandSide.IsTileADouble) && ReferencePosition.Y > Table1.TileOnRightHandSide.Position.Y)
                {
                    if (Table1.RightHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;

                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.RightHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y + SquareSize);
                        Table1.TileOnRightHandSide = TileToUpdate;
                        Table1.RightHandSide = TileToUpdate.FirstTileValue;

                    }
                    Table1.PositionOfRightHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y + 2 * SquareSize);
                    Table1.PositionOfTileOnRightHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }

                else if ((Table1.TileOnLeftHandSide.LastOrientation == 2 || Table1.TileOnLeftHandSide.LastOrientation == 4)
                    && (!TileToUpdate.IsTileADouble) && (!Table1.TileOnLeftHandSide.IsTileADouble) && ReferencePosition.Y < Table1.TileOnLeftHandSide.Position.Y)
                {
                    if (Table1.LeftHandSide == TileToUpdate.FirstTileValue)
                    {
                        TileToUpdate.LastOrientation = 3;

                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.SecondTileValue;
                    }

                    else if (Table1.LeftHandSide == TileToUpdate.SecondTileValue)
                    {
                        TileToUpdate.LastOrientation = 1;
                        TileToUpdate.Position = new Vector2(TileToUpdate.Position.X, TileToUpdate.Position.Y - SquareSize);
                        Table1.TileOnLeftHandSide = TileToUpdate;
                        Table1.LeftHandSide = TileToUpdate.FirstTileValue;

                    }
                    Table1.PositionOfLeftHandSideEdge = new Vector2(ReferencePosition.X, ReferencePosition.Y - 2 * SquareSize);
                    Table1.PositionOfTileOnLeftHandSide = ReferencePosition;
                    AddDominoTile(Table1, TileToUpdate, Player, PositionToRemove);
                }
                if (Table1.PlayerInTurn == player1)
                {
                    CalculateTurn(Table1.PlayerInTurn);
                }

            }


        }


        private void AddDominoTile(Table Table1, Tile DominoTileToUpdate, Player Player, int PositionToRemove)
        {
            // remove it from the list 
            Player.PlayerTileList.RemoveAt(PositionToRemove);
            // add it to the player's dominoes 
            Table1.TilesPlayedOnTableList.Add(DominoTileToUpdate);
           
            LastDominoTilePlayed = DominoTileToUpdate;
            PlayerWhoLastPlayed = Table1.PlayerInTurn;

            trackCue = soundBank.GetCue("DominoSetDown");
            trackCue.Play();


        }

        private void CalculateTurn(Player PlayerInTurn)
        {
            if (PlayerInTurn == player4)
            {
                Table1.PlayerInTurn = player1;
                player4.MyTurn = false;
                player1.MyTurn = true;
            }

            else if (PlayerInTurn == player1)
            {
                Table1.PlayerInTurn = player2;
                player1.MyTurn = false;
                player2.MyTurn = true;
            }

            else if (PlayerInTurn == player2)
            {
                Table1.PlayerInTurn = player3;
                player2.MyTurn = false;
                player3.MyTurn = true;
            }

            else if (PlayerInTurn == player3)
            {
                Table1.PlayerInTurn = player4;
                player3.MyTurn = false;
                player4.MyTurn = true;
            }


        }

        private void FindOutIfPlayerPassesHisTurn(Player player)
        {
            bool tempPlayerPassedTurn = true;
            foreach (Tile f in player.PlayerTileList)
            {   
                if (f.FirstTileValue == Table1.RightHandSide
                    ||f.FirstTileValue == Table1.LeftHandSide
                    ||f.SecondTileValue == Table1.RightHandSide
                    ||f.SecondTileValue == Table1.LeftHandSide)
                {
                    tempPlayerPassedTurn = false;
                }
            }
            IsPlayerPassing = tempPlayerPassedTurn;
        }

        /// <summary>
        /// Calculate the score of the winning team
        /// </summary>
        private void ScoreTheGame()
        {
            int pointsToAdd = 0;
            
            // If player 1 or 3 ran out of dominoes, they win, and we add pointsToAdd to the total of points from player 2 and 4
            if ((player1.PlayerTileList.Count == 0) || (player3.PlayerTileList.Count == 0))
            {
                for (int i = 0; i < player1.PlayerTileList.Count; i++)
			    {
                    pointsToAdd += player1.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player2.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player2.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player3.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player3.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player4.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player4.PlayerTileList[i].TotalPointsValue;
                }

                TeamOneTotalPoints += pointsToAdd;
                    
            }
            else if ((player2.PlayerTileList.Count == 0 )|| (player4.PlayerTileList.Count == 0))
            {
                for (int i = 0; i < player1.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player1.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player2.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player2.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player3.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player3.PlayerTileList[i].TotalPointsValue;
                }
                for (int i = 0; i < player4.PlayerTileList.Count; i++)
                {
                    pointsToAdd += player4.PlayerTileList[i].TotalPointsValue;
                }

                TeamTwoTotalPoints += pointsToAdd;
            }


            else if (GameLocked)
            {
                if (PlayerWhoLastPlayed.Name==player1.Name)
                {
                    int PointsFromPlayerWhoLockedGame=0, PointsOfPlayerToTheRightOfPlayerWhoLockedGame=0;
                    for (int i = 0; i < player1.PlayerTileList.Count; i++)
                    {
                        PointsFromPlayerWhoLockedGame += player1.PlayerTileList[i].TotalPointsValue;
                    }
                    for (int i = 0; i < player2.PlayerTileList.Count; i++)
                    {
                        PointsOfPlayerToTheRightOfPlayerWhoLockedGame += player2.PlayerTileList[i].TotalPointsValue;
                    }

                    // If the total of the dominoes from the player who locked game is less or equal to the player on his right, the player who locked game wins
                    if (PointsFromPlayerWhoLockedGame<PointsOfPlayerToTheRightOfPlayerWhoLockedGame || PointsFromPlayerWhoLockedGame==PointsOfPlayerToTheRightOfPlayerWhoLockedGame)
                    {
                        for (int i = 0; i < player1.PlayerTileList.Count; i++)
                        {
                            pointsToAdd += player1.PlayerTileList[i].TotalPointsValue;
                        }
                        for (int i = 0; i < player2.PlayerTileList.Count; i++)
                        {
                            pointsToAdd += player2.PlayerTileList[i].TotalPointsValue;
                        }
                        for (int i = 0; i < player3.PlayerTileList.Count; i++)
                        {
                            pointsToAdd += player3.PlayerTileList[i].TotalPointsValue;
                        }
                        for (int i = 0; i < player4.PlayerTileList.Count; i++)
                        {
                            pointsToAdd += player4.PlayerTileList[i].TotalPointsValue;
                        }
                        // Add points to the team who locked game
                        TeamOneTotalPoints += pointsToAdd;

                    }
                    // If the total of the dominoes from the player who locked the game is greater than the points of the player to his right, the player to his right wins
                    else if (PointsFromPlayerWhoLockedGame>PointsOfPlayerToTheRightOfPlayerWhoLockedGame)
                    {
                        for (int i = 0; i < player1.PlayerTileList.Count; i++)
                    {
                        pointsToAdd += player1.PlayerTileList[i].TotalPointsValue;
                    }
                    for (int i = 0; i < player2.PlayerTileList.Count; i++)
                    {
                        pointsToAdd += player2.PlayerTileList[i].TotalPointsValue;
                    }
                    for (int i = 0; i < player3.PlayerTileList.Count; i++)
                    {
                        pointsToAdd += player3.PlayerTileList[i].TotalPointsValue;
                    }
                    for (int i = 0; i < player4.PlayerTileList.Count; i++)
                    {
                        pointsToAdd += player4.PlayerTileList[i].TotalPointsValue;
                    }
                    // Add points to the team that did not lock the game
                    TeamTwoTotalPoints += pointsToAdd;
                    }
                }    
            }
        }


        private void DrawTilesForNonHumanPlayersLevel1(Player playerOnTurn)
        {
            for (int i = 0; i < playerOnTurn.PlayerTileList.Count; i++)
            {
                if ((Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue))
                {
                    playerOnTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                    TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[i], playerOnTurn, i);
                    break;
                }

                else if ((Table1.RightHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue))
                {
                    playerOnTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                    TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[i], playerOnTurn, i);
                    break;
                }
            }
        }

        private void DrawTilesForNonHumanPlayersLevel2(Player playerOnTurn)
        {
            int tempRight = 0;
            int tempLeft = 0;
            bool RightHandSideEdge = true;
            Tile tempTile = new Tile();
            tempTile.TotalPointsValue = 0;
            for (int i = 0; i < playerOnTurn.PlayerTileList.Count; i++)
            {
                if ((Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue))
                {
                    if (playerOnTurn.PlayerTileList[i].TotalPointsValue > tempTile.TotalPointsValue)
                    {
                        tempLeft = i;
                        tempTile = playerOnTurn.PlayerTileList[i];
                        RightHandSideEdge = false;
                    }



                }

                else if ((Table1.RightHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue))
                {
                    if (playerOnTurn.PlayerTileList[i].TotalPointsValue > tempTile.TotalPointsValue)
                    {
                        tempRight = i;
                        tempTile = playerOnTurn.PlayerTileList[i];
                        RightHandSideEdge = true;
                    }


                }

                if ((tempTile.TotalPointsValue != 0) && (RightHandSideEdge))
                {
                    playerOnTurn.PlayerTileList[tempRight].Position = Table1.PositionOfRightHandSideEdge;
                    TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[tempRight], playerOnTurn, tempRight);
                }

                else if ((tempTile.TotalPointsValue != 0) && (!RightHandSideEdge))
                {
                    playerOnTurn.PlayerTileList[tempLeft].Position = Table1.PositionOfLeftHandSideEdge;
                    TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[tempLeft], playerOnTurn, tempLeft);
                }


            }
        }



        private void DrawTilesForNonHumanPlayersLevel3(Player playerOnTurn)
        {

            List<int> TilesThatThePlayerIsAbleToPlayWith = new List<int>();
            int Doubles = 0;
            for (int i = 0; i < playerOnTurn.PlayerTileList.Count; i++)
            {

                if ((Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue)
                    || (Table1.RightHandSide == playerOnTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == playerOnTurn.PlayerTileList[i].SecondTileValue))
                {
                    TilesThatThePlayerIsAbleToPlayWith.Add(i);

                    if (playerOnTurn.PlayerTileList[i].IsTileADouble)
                    {
                        Doubles++;
                        int temp = 0;
                        foreach (Tile g in Table1.TilesPlayedOnTableList)
                        {
                            if (g.FirstTileValue == playerOnTurn.PlayerTileList[i].FirstTileValue || g.SecondTileValue == playerOnTurn.PlayerTileList[i].FirstTileValue)
                            {
                                temp++;
                            }

                        }
                        if (temp >= 3 && temp < 5)
                        {
                            playerOnTurn.PlayerTileList[i].Priority = 2;
                        }
                        else if (temp >= 5)
                        {
                            playerOnTurn.PlayerTileList[i].Priority = 3;
                        }

                    }
                }

                if (TilesThatThePlayerIsAbleToPlayWith.Count == 1)
                {
                    if ((Table1.LeftHandSide == playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].FirstTileValue) || (Table1.LeftHandSide == playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].SecondTileValue))
                    {
                        playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].Position = Table1.PositionOfLeftHandSideEdge;
                        TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]], playerOnTurn, TilesThatThePlayerIsAbleToPlayWith[0]);
                    }
                    else if ((Table1.RightHandSide == playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].FirstTileValue) || (Table1.RightHandSide == playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].SecondTileValue))
                    {
                        playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]].Position = Table1.PositionOfRightHandSideEdge;
                        TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[TilesThatThePlayerIsAbleToPlayWith[0]], playerOnTurn, TilesThatThePlayerIsAbleToPlayWith[0]);
                    }

                }
                else if (Doubles > 0)
                {
                    int itmp = new int();
                    for (int h = 0; h < playerOnTurn.PlayerTileList.Count; h++)
                    {
                        if (playerOnTurn.PlayerTileList[h].Priority > playerOnTurn.PlayerTileList[itmp].Priority)
                        {
                            itmp = h;
                        }

                    }
                    if ((Table1.LeftHandSide == playerOnTurn.PlayerTileList[itmp].FirstTileValue) || (Table1.LeftHandSide == playerOnTurn.PlayerTileList[itmp].SecondTileValue))
                    {
                        playerOnTurn.PlayerTileList[itmp].Position = Table1.PositionOfLeftHandSideEdge;
                        TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[itmp], playerOnTurn, itmp);

                    }
                    else if ((Table1.RightHandSide == playerOnTurn.PlayerTileList[itmp].FirstTileValue) || (Table1.RightHandSide == playerOnTurn.PlayerTileList[itmp].SecondTileValue))
                    {
                        playerOnTurn.PlayerTileList[itmp].Position = Table1.PositionOfRightHandSideEdge;
                        TilePlacementLogic(Table1, playerOnTurn.PlayerTileList[itmp], playerOnTurn, itmp);

                    }
                }
                else
                {
                    DrawTilesForNonHumanPlayersLevel1(playerOnTurn);
                }
            }
        }


        private void DrawTilesForNonHumanPlayersLevel4(Player PlayerInTurn)
        {
            if (PlayerInTurn == player1)
            {
                if ((player2.PositionOfTileLastPlayed != player3.PositionOfTileLastPlayed) && (player2.PositionOfTileLastPlayed != player4.PositionOfTileLastPlayed))
                {
                    if (player2.PositionOfTileLastPlayed == 2)
                    {
                        for (int i = 0; i < player1.PlayerTileList.Count; i++)
                        {
                            if ((Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }
                    else if (player2.PositionOfTileLastPlayed == 1)
                    {
                        for (int i = 0; i < player1.PlayerTileList.Count; i++)
                        {
                            if ((Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }

                    else
                    {
                        DrawTilesForNonHumanPlayersLevel1(PlayerInTurn);
                    }

                }

            }

            else if (PlayerInTurn == player2)
            {
                if ((player3.PositionOfTileLastPlayed != player4.PositionOfTileLastPlayed) && (player3.PositionOfTileLastPlayed != player1.PositionOfTileLastPlayed))
                {
                    if (player3.PositionOfTileLastPlayed == 2)
                    {
                        for (int i = 0; i < player2.PlayerTileList.Count; i++)
                        {
                            if ((Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }
                    else if (player3.PositionOfTileLastPlayed == 1)
                    {
                        for (int i = 0; i < player2.PlayerTileList.Count; i++)
                        {
                            if ((Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }

                    else
                    {
                        DrawTilesForNonHumanPlayersLevel1(PlayerInTurn);
                    }

                }

            }

            else if (PlayerInTurn == player3)
            {
                if ((player4.PositionOfTileLastPlayed != player1.PositionOfTileLastPlayed) && (player4.PositionOfTileLastPlayed != player2.PositionOfTileLastPlayed))
                {
                    if (player4.PositionOfTileLastPlayed == 2)
                    {
                        for (int i = 0; i < player3.PlayerTileList.Count; i++)
                        {
                            if ((Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }
                    else if (player4.PositionOfTileLastPlayed == 1)
                    {
                        for (int i = 0; i < player3.PlayerTileList.Count; i++)
                        {
                            if ((Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }

                    else
                    {
                        DrawTilesForNonHumanPlayersLevel1(PlayerInTurn);
                    }

                }
            }

            else if (PlayerInTurn == player4)
            {
                if ((player1.PositionOfTileLastPlayed != player2.PositionOfTileLastPlayed) && (player1.PositionOfTileLastPlayed != player3.PositionOfTileLastPlayed))
                {
                    if (player1.PositionOfTileLastPlayed == 2)
                    {
                        for (int i = 0; i < player4.PlayerTileList.Count; i++)
                        {
                            if ((Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }
                    else if (player1.PositionOfTileLastPlayed == 1)
                    {
                        for (int i = 0; i < player4.PlayerTileList.Count; i++)
                        {
                            if ((Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                            {
                                PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                                TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                            }
                        }

                    }

                    else
                    {
                        DrawTilesForNonHumanPlayersLevel1(PlayerInTurn);
                    }

                }
            }

            for (int i = 0; i < PlayerInTurn.PlayerTileList.Count; i++)
            {
                if ((Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.LeftHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                {
                    PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfLeftHandSideEdge;
                    TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                    break;
                }

                else if ((Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].FirstTileValue) || (Table1.RightHandSide == PlayerInTurn.PlayerTileList[i].SecondTileValue))
                {
                    PlayerInTurn.PlayerTileList[i].Position = Table1.PositionOfRightHandSideEdge;
                    TilePlacementLogic(Table1, PlayerInTurn.PlayerTileList[i], PlayerInTurn, i);
                    break;
                }


            }
        }

        private void VerifyEndOfRoundCondition()
        {

            // Determine end of round condition

            foreach (Player j in PlayersList)
            {
                // If a player ran out of tiles, or the turns repeat more than 5 times (it's locked), then it's the end of the round
                if ((j.PlayerTileList.Count == 0) && (!StartOfRound) && (!StartOfNewGame))
                {
                    EndOfRound = true;
                    ScoreTheGame();
                    PlayerWhoWonLastRound = j;
                    CurrentGameState = GameState.EndOfRound;
                    break;
                }
            }
            bool tempGameLocked = true;
            foreach (Player j in PlayersList)
            {
                foreach (Tile f in j.PlayerTileList)
                {
                    if (Table1.LeftHandSide == f.FirstTileValue || Table1.RightHandSide == f.FirstTileValue
                        || Table1.LeftHandSide == f.SecondTileValue || Table1.RightHandSide == f.SecondTileValue)
                    {
                        tempGameLocked = false;
                        break;
                    }
                }
                if (!tempGameLocked)
                {
                    break;
                }

            }
            GameLocked = tempGameLocked;

            if (GameLocked)
            {
                EndOfRound = true;
                ScoreTheGame();
                CurrentGameState = GameState.EndOfRound;
            }
        }


        #endregion

        #region Save Game & Save Data Methods

        public void InitiateSave()
        {
            if (!Guide.IsVisible)
            {
                try
                {
                    if (!Guide.IsVisible)
                    {
                        device = null;
                        StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
                    }
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    //Logger.error(“InvalidOperationException”);
                    StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
                }
            }
        }

        void SaveToDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                SaveGame SaveData = new SaveGame()
                {
                    DifficultyLevelToSave = CurrentDifficultyLevel,
                    TileColorToSave = CurrentTileColor

                };
                IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                serializer.Serialize(stream, SaveData);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
            }
        }

        public void InitiateLoad()
        {
            // This code doesn't run anymore, may need to port
            //if (!Guide.IsVisible)
            //{
            //    try
            //    {
            //        if (!Guide.IsVisible)
            //        {
            //            device = null;
            //            StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);
            //        }
            //    }
            //    catch (InvalidOperationException invalidOperationException)
            //    {
            //        //Logger.error(“InvalidOperationException”);
            //        StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);
            //    }

            //}
        }

        void LoadFromDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                SaveGame SaveData = (SaveGame)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
                // Update the game based on the save game file

                CurrentDifficultyLevel = SaveData.DifficultyLevelToSave;
                CurrentTileColor = SaveData.TileColorToSave;

            }
        }

        #endregion

    }
}
