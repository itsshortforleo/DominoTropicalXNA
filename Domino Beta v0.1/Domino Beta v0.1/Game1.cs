using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Domino_Beta_v0._1.Entidades;
using System.Threading;



namespace Domino_Beta_v0._1
{


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        

        #region Variables

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;                // SpriteBatch para dibujar las figuras graficas
        public delegate void Delegado(string s);  
        Color ColorADibujar;
        
        bool Gano;
        bool Perdio;
        int MaximaPuntuacion=2;   

        // Para la resolucion de la pantalla
        int screenWidth = 1366, screenHeight = 768;

        // Para Audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue trackCue;

        // Para el menu principal
        enum GameState
        {
            MenuPrincipal,
            Opciones,
            SobreDomino,
            Jugando,
            Barajando,
            Nivel,
            ColorDeFicha,
            FinDeRondaActual
        }

        GameState EstadoActualDeJuego = GameState.MenuPrincipal;
        
        
        BotonDeMenu BotonDeJugar;
        BotonDeMenu BotonDeOpciones;
        BotonDeMenu BotonDeCreditos;
        BotonDeMenu BotonDeSalida;
        BotonDeMenu BotonSeguirJugando;
        BotonDeMenu BotonDobleSeis;
        BotonDeMenu BotonNivel;
        BotonDeMenu BotonColorDeDominoes;
        BotonDeMenu Blanco;
        BotonDeMenu Amarillo;
        BotonDeMenu Azul;
        BotonDeMenu Rojo;
        BotonDeMenu Verde;
        BotonDeMenu Experto;
        BotonDeMenu Facil;
        BotonDeMenu MuyFacil;
        BotonDeMenu Normal;
        BotonDeMenu BotonDePasar;
        BotonDeMenu BotonSiguienteRonda;

        Texture2D Felicidades;
        Texture2D MalaSuerte;

        bool JuegoEmpezado = false;
        bool JuegoTrancado;
        bool InicioDePartida = true;                         
        bool InicioMano = false;

        Ficha UltimaFichaTomada = null;
        Jugador UltimoJugadorEnJugar;

        Vector2 PosicionDeTablero;                      // where to posicion the board
        Vector2 PosicionDeCuadroArrastrable;            // the draggable Cuadro


        Random aleatorio = new Random();                // Se crea una variable de tipo aleatorio (random)
        
        Texture2D Fondo;
        Texture2D FondoDeFinDeRonda;
        
        SpriteFont Letras;                              // Font to write info with
        SpriteFont Cent;


        // Se crean los cuatro jugadores
        Jugador jugador1 = new Jugador("Anthony", false, true);
        Jugador jugador2 = new Jugador("Jugador 2", false, false);
        Jugador jugador3 = new Jugador("Leonardo", false, false);
        Jugador jugador4 = new Jugador("Jugador 4", false, false);

        Jugador JugadorGanoUltimaRonda;

        bool JugadorPasa = false;
        bool FinDeRonda = false;

        int PuntosEquipo1 = 0;
        int PuntosEquipo2 = 0;
        



        public Mesa Mesa1;


        Ficha UltimaFichaJugada;

        Ficha FichaAActualizar = null;

        public List<Ficha> ListaCompletaDeFichas = new List<Ficha>();       // A sprite for the player and a list of automated sprites
        public List<Ficha> ListaCompletaDeFichasParaRepartir = new List<Ficha>(); 
 

        public List<Jugador> Jugadores = new List<Jugador>();               // Lista de jugadores

        
        Nivel NivelActual = Nivel.MuyFacil;                        // Variable que indica el nivel actual de inteligencia artificial
        ColorDeFicha ColorDeFichaActual = ColorDeFicha.Blanco;     // Variable que indica el color actual de la ficha


        #region Variables para guardar datos

        StorageDevice device;
        string containerName = "MyGamesStorage";
        string filename = "mysave.sav";

        [Serializable]
        public struct SaveGame
        {
            public Nivel NivelAGuardar;
            public ColorDeFicha ColorDeFichaAGuardar;
        }

        #endregion


        #region Variables para dibujar tablero

        Texture2D CuadroBlanco;                     // the white 64x64 pixels bitmap to draw with

        Vector2 PosicionDeMouse;                    // the current posicion of the mouse

        Rectangle BordeDeCuadroArrastrable;         // the boundaries of the draggable Cuadro

        bool[,] Tablero = new bool[32, 21];         //stores whether there is something in a square

        int TamanoDeCuadro = 28;                    //how wide/tall the tiles are

        Texture2D FichaDeOponente;
        Texture2D FichaDePareja;

        //stores the previous and current states of the mouse
        //makes it possible to know if a button was just clicked
        //or whether it was up/down previously as well.
        MouseState EstadoPrevioDeMouse, EstadoActualDeMouse;

        #endregion



        #endregion

        #region Constructor de Clase Game y Método de LoadContent (Cargar Contenido)

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //positions the top left corner of the board - change this to move the board
            PosicionDeTablero = new Vector2(95, 85);

            //positions the square to drag
            PosicionDeCuadroArrastrable = new Vector2((graphics.PreferredBackBufferWidth) - 120, 608);

            // Componente necesario para poder guardar/leer archivos
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

            if (ColorDeFichaActual == ColorDeFicha.Blanco)
                ColorADibujar = Color.White;
            if (ColorDeFichaActual == ColorDeFicha.Amarillo)
                ColorADibujar = Color.Yellow;
            if (ColorDeFichaActual == ColorDeFicha.Azul)
                ColorADibujar = Color.Cyan;
            if (ColorDeFichaActual == ColorDeFicha.Rojo)
                ColorADibujar = Color.Red;
            if (ColorDeFichaActual == ColorDeFicha.Verde)
                ColorADibujar = Color.Green;


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
            BotonDeJugar = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\BotonJugar"), graphics.GraphicsDevice);
            BotonDeJugar.EstablecerPosicion(new Vector2(871, 350));

            BotonSeguirJugando = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\BotonSeguirJugando"), graphics.GraphicsDevice);
            BotonSeguirJugando.EstablecerPosicion(new Vector2(871, 350));

            BotonDeOpciones = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\BotonOpciones"), graphics.GraphicsDevice);
            BotonDeOpciones.EstablecerPosicion(new Vector2(871, 420));

            BotonDeCreditos = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\BotonSobre"), graphics.GraphicsDevice);
            BotonDeCreditos.EstablecerPosicion(new Vector2(871, 490));

            BotonDeSalida = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\BotonSalir"), graphics.GraphicsDevice);
            BotonDeSalida.EstablecerPosicion(new Vector2(871, 560));

            BotonDobleSeis = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\MainMenuDobleSeis"), graphics.GraphicsDevice, new Vector2(0, 0));

            BotonNivel = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonNivel"), graphics.GraphicsDevice);
            BotonNivel.EstablecerPosicion(new Vector2(842, 330));

            BotonColorDeDominoes = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonColorDeDominos"), graphics.GraphicsDevice);
            BotonColorDeDominoes.EstablecerPosicion(new Vector2(842, 640));


            Blanco = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonBlanco"), graphics.GraphicsDevice);
            Blanco.EstablecerPosicion(new Vector2(842, 370));


            Amarillo = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonAmarillo"), graphics.GraphicsDevice);
            Amarillo.EstablecerPosicion(new Vector2(842, 470));


            Azul = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonAzul"), graphics.GraphicsDevice);
            Azul.EstablecerPosicion(new Vector2(842, 520));


            Rojo = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonRojo"), graphics.GraphicsDevice);
            Rojo.EstablecerPosicion(new Vector2(842, 570));


            Verde = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonVerde"), graphics.GraphicsDevice);
            Verde.EstablecerPosicion(new Vector2(842, 420));


            MuyFacil = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonMuyFacil"), graphics.GraphicsDevice);
            MuyFacil.EstablecerPosicion(new Vector2(842, 410));


            Facil = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonFacil"), graphics.GraphicsDevice);
            Facil.EstablecerPosicion(new Vector2(842, 460));


            Normal = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonNormal"), graphics.GraphicsDevice);
            Normal.EstablecerPosicion(new Vector2(842, 510));

            Experto = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonExperto"), graphics.GraphicsDevice);
            Experto.EstablecerPosicion(new Vector2(842, 560));

            BotonDePasar = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonDePasar"), graphics.GraphicsDevice, 1);
            BotonDePasar.EstablecerPosicion(new Vector2(700, 710));

            BotonSiguienteRonda = new BotonDeMenu(Content.Load<Texture2D>(@"Imagenes\botonSiguienteRonda"), graphics.GraphicsDevice, 1);
            BotonSiguienteRonda.EstablecerPosicion(new Vector2(700, 400));



            // Cargar las imagenes
            CuadroBlanco = Content.Load<Texture2D>(@"Imagenes\white_64x64");

            // Cargar la letra
            Letras = Content.Load<SpriteFont>(@"Letras\Letras");
            Cent = Content.Load<SpriteFont>(@"Letras\Cent");

            // remembers the draggable squares posicion, so we can easily test for mouseclicks on it
            BordeDeCuadroArrastrable = new Rectangle((int)PosicionDeCuadroArrastrable.X, (int)PosicionDeCuadroArrastrable.Y, TamanoDeCuadro, TamanoDeCuadro);

            Fondo = Content.Load<Texture2D>(@"Imagenes\MesaExtendida");

            FondoDeFinDeRonda = Content.Load<Texture2D>(@"Imagenes\FondoDeFinDeRonda");

            Felicidades = Content.Load<Texture2D>(@"Imagenes\Felicidades");
            MalaSuerte = Content.Load<Texture2D>(@"Imagenes\MalaSuerte");

            FichaDeOponente = Content.Load<Texture2D>(@"Imagenes\FichaDeMiOponente");

            FichaDePareja = Content.Load<Texture2D>(@"Imagenes\FichaDeMiFrente");


            #region Lista Completa de Fichas

            //Load several different automated sprites into the list
            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/00"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 0, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/01"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 1, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/02"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 2, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/03"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 3, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/04"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 4, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/05"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 5, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/06"),
                10, Vector2.Zero, new Vector2(150, 150), 0, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/11"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 1, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/12"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 2, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/13"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 3, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/14"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 4, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/15"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 5, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/16"),
                10, Vector2.Zero, new Vector2(150, 150), 1, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/22"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 2, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/23"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 3, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/24"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 4, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/25"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 5, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/26"),
                10, Vector2.Zero, new Vector2(150, 150), 2, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/33"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 3, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/34"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 4, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/35"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 5, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/36"),
                10, Vector2.Zero, new Vector2(150, 150), 3, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/44"),
                10, Vector2.Zero, new Vector2(180, 150), 4, 4, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/45"),
                10, Vector2.Zero, new Vector2(150, 150), 4, 5, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/46"),
                10, Vector2.Zero, new Vector2(150, 150), 4, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/55"),
                10, Vector2.Zero, new Vector2(150, 150), 5, 5, true, true, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/56"),
                10, Vector2.Zero, new Vector2(150, 150), 5, 6, true, false, false));

            ListaCompletaDeFichas.Add(new Ficha(Content.Load<Texture2D>(@"Imagenes/66"),
                10, Vector2.Zero, new Vector2(150, 150), 6, 6, true, true, false));

            #endregion

            // Se agregan los cuatro jugadores a una lista de jugadores
            Jugadores.Add(jugador1);
            Jugadores.Add(jugador2);
            Jugadores.Add(jugador3);
            Jugadores.Add(jugador4);

            Mesa1 = new Mesa(1);

            base.LoadContent();

            // TODO: use this.Content to load your game content here
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

        #region Update (Actualizar)

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            

            // Para Audio
            audioEngine.Update();

            // Para funciones de Mouse
            // get the current state of the mouse (posicion, buttons, etc.)
            EstadoActualDeMouse = Mouse.GetState();

            //remember the mouseposition for use in this Update and subsequent Draw
            PosicionDeMouse = new Vector2(EstadoActualDeMouse.X, EstadoActualDeMouse.Y);


            // Menu principal
            switch (EstadoActualDeJuego)
            {
                case GameState.MenuPrincipal:

                    if (BotonDeJugar.seHizoClic == true)
                        EstadoActualDeJuego = GameState.Jugando;

                    BotonDeJugar.Update(EstadoActualDeMouse);

                    if (BotonSeguirJugando.seHizoClic == true)
                        EstadoActualDeJuego = GameState.Jugando;

                    if (BotonDeOpciones.seHizoClic == true)
                        EstadoActualDeJuego = GameState.Opciones;

                    BotonDeOpciones.Update(EstadoActualDeMouse);

                    if (BotonDeCreditos.seHizoClic == true)
                        EstadoActualDeJuego = GameState.SobreDomino;

                    BotonDeCreditos.Update(EstadoActualDeMouse);

                    if (BotonDeSalida.seHizoClic == true)
                    {
                        InitiateSave();
                        this.Exit();
                    }

                    BotonDeSalida.Update(EstadoActualDeMouse);

                    BotonDeJugar.Update(EstadoActualDeMouse);
                    BotonDeOpciones.Update(EstadoActualDeMouse);
                    BotonDeCreditos.Update(EstadoActualDeMouse);
                    BotonDeSalida.Update(EstadoActualDeMouse);
                    BotonDobleSeis.Update(EstadoActualDeMouse);
                 


                    break;

                case GameState.Jugando:

                    
                    // Si teclas Enter, ve al menu de Anotacion
                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        EstadoActualDeJuego = GameState.FinDeRondaActual;


                    // Solo reparte las fichas cuando InicioDePartida = true
                    if (InicioDePartida || InicioMano)
                    {

                        foreach (Jugador j in Jugadores)
                        {
                            j.FichasDeJugador.Clear();
                        }

                        ListaCompletaDeFichasParaRepartir.AddRange(ListaCompletaDeFichas);
                        RepartirFichas(ListaCompletaDeFichasParaRepartir);

                        
                        // Dale el primer turno al jugador que tenga el 6/6
                        if (InicioDePartida)
                        {
                            


                            for (int i = 0; i < Jugadores.Count; i++)
                            {
                                foreach (Ficha f in Jugadores[i].FichasDeJugador)
                                {
                                    if (f.PrimerValor == 6 && f.SegundoValor == 6)
                                    {
                                        Jugadores[i].MiTurno = true;
                                        Mesa1.JugadorEnTurno = Jugadores[i];
                                    }
                                }
                            }

                            // Si al inicio de partida, el turno es de una maquina, que juege
                            if (!Mesa1.JugadorEnTurno.EsHumano)
                            {
                                for (int i = 0; i < Mesa1.JugadorEnTurno.FichasDeJugador.Count; i++)
                                {
                                    if (InicioDePartida)
                                    {
                                        if (Mesa1.JugadorEnTurno.FichasDeJugador[i].PrimerValor == 6 && Mesa1.JugadorEnTurno.FichasDeJugador[i].SegundoValor == 6)
                                        {
                                            Mesa1.JugadorEnTurno.FichasDeJugador[i].Posicion = new Vector2(PosicionDeTablero.X + 16 * TamanoDeCuadro, PosicionDeTablero.Y + 10 * TamanoDeCuadro);
                                            InicioDePartida = false;
                                            RegularMesa(Mesa1, Mesa1.JugadorEnTurno.FichasDeJugador[i], Mesa1.JugadorEnTurno, i);

                                            break;

                                        }
                                    }

                                }

                                CalcularTurno(Mesa1.JugadorEnTurno);

                            }
                        }


                        else if (InicioMano)
                        {
                            

                            foreach (Jugador j in Jugadores)
                            {
                                if (j.MiTurno && !j.EsHumano)
                                {
                                    Mesa1.JugadorEnTurno = j;
                                    j.FichasDeJugador[0].Posicion = new Vector2(PosicionDeTablero.X + 16 * TamanoDeCuadro, PosicionDeTablero.Y + 10 * TamanoDeCuadro);
                                    
                                    RegularMesa(Mesa1, j.FichasDeJugador[0], j, 0);
                                    CalcularTurno(Mesa1.JugadorEnTurno);
                                    break;

                                }



                            }

                            InicioMano = false; 

                        }

                        


                    }

                    #region Inteligencia Artificial


                    // Inteligencia Artificial
                    switch (NivelActual)
                    {
                        case Nivel.MuyFacil:
                            if (!Mesa1.JugadorEnTurno.EsHumano && (!FinDeRonda) && !InicioDePartida && !InicioMano)
                            {
                                DibujarFichasJugadoresNoHumanosNivel1(Mesa1.JugadorEnTurno);
                                Thread.Sleep(2000);
                                CalcularTurno(Mesa1.JugadorEnTurno);
                            }
                            break;

                        case Nivel.Facil:
                            if (!Mesa1.JugadorEnTurno.EsHumano && (!FinDeRonda) && !InicioDePartida && !InicioMano)
                            {
                                DibujarFichasJugadoresNoHumanosNivel1(Mesa1.JugadorEnTurno);
                                Thread.Sleep(2000);
                                CalcularTurno(Mesa1.JugadorEnTurno);
                            }
                            break;


                        case Nivel.Normal:
                            if (!Mesa1.JugadorEnTurno.EsHumano && (!FinDeRonda) && !InicioDePartida && !InicioMano)
                            {
                                DibujarFichasJugadoresNoHumanosNivel3(Mesa1.JugadorEnTurno);
                                Thread.Sleep(2000);
                                CalcularTurno(Mesa1.JugadorEnTurno);
                            }
                            break;

                        case Nivel.Experto:
                            if (!Mesa1.JugadorEnTurno.EsHumano && (!FinDeRonda) && !InicioDePartida && !InicioMano)
                            {
                                DibujarFichasJugadoresNoHumanosNivel3(Mesa1.JugadorEnTurno);
                                Thread.Sleep(2000);
                                CalcularTurno(Mesa1.JugadorEnTurno);
                            }
                            break;
                    }

                    if (Mesa1.JugadorEnTurno.EsHumano)
                    {
                        DecideSiJugadorPasa(Mesa1.JugadorEnTurno);
                    }


                    #endregion


                    #region Update para arrastrar fichas
                    

                    foreach (Ficha f in jugador1.FichasDeJugador)
                    {

                        //if the user just clicked inside the draggable white square - set SeEstaArrastrando to true
                        if (EstadoPrevioDeMouse.LeftButton == ButtonState.Released && EstadoActualDeMouse.LeftButton == ButtonState.Pressed && f.BordeFicha.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y))
                        {
                            f.SeEstaArrastrando = true;
                            UltimaFichaTomada = f;

                            trackCue = soundBank.GetCue("DominoPickUp");
                            trackCue.Play();
                        }

                        //if the user just released the mousebutton - set SeEstaArrastrando to false, and check if we should add the Cuadro to the board
                        if (EstadoPrevioDeMouse.LeftButton == ButtonState.Pressed && EstadoActualDeMouse.LeftButton == ButtonState.Released)
                        {
                            f.SeEstaArrastrando = false;


                            Vector2 Cuadro = ObtenerCuadroAPartirDePosicion(PosicionDeMouse);
                            Rectangle RecPrimeraFichaAJugar = new Rectangle((int)(PosicionDeTablero.X + 14 * TamanoDeCuadro), (int)(PosicionDeTablero.Y + 8 * TamanoDeCuadro), 5 * TamanoDeCuadro, 5 * TamanoDeCuadro);

                            if (Mesa1.FichasEnMesa.Count < 1)
                            {

                                //if the mousebutton was released inside the board
                                if (MouseDentroDelTablero() && (RecPrimeraFichaAJugar.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y)) &&
                                    Mesa1.JugadorEnTurno.EsHumano)
                                {
                                    //find out which square the mouse is over

                                    //and set that square to true (has a piece)
                                    Tablero[(int)Cuadro.X, (int)Cuadro.Y] = true;

                                }
                            }
                            else
                            {
                                Rectangle RecFichaExtremoDerecho = new Rectangle((int)Mesa1.PosicionExtremoDerecho.X, (int)Mesa1.PosicionExtremoDerecho.Y, TamanoDeCuadro, TamanoDeCuadro);
                                Rectangle RecFichaExtremoIzquierdo = new Rectangle((int)Mesa1.PosicionExtremoIzquierdo.X, (int)Mesa1.PosicionExtremoIzquierdo.Y, TamanoDeCuadro, TamanoDeCuadro);
                                if (RecFichaExtremoDerecho.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y) || RecFichaExtremoIzquierdo.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y))
                                {
                                    //find out which square the mouse is over

                                    //and set that square to true (has a piece)
                                    try
                                    {
                                        Tablero[(int)Cuadro.X, (int)Cuadro.Y] = true;
                                    }
                                    catch (Exception)
                                    {
                                        
                                        throw;
                                    }
                                    

                                }
                            }
                        }
                    }






                    

                    #endregion


                    BotonDeJugar.Update(EstadoActualDeMouse);


                    
                    // Si teclas Escape, vuele al menu anterior
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        JuegoEmpezado = true;
                        BotonSeguirJugando.Update(EstadoActualDeMouse);
                        EstadoActualDeJuego = GameState.MenuPrincipal;
                    }

                    if (BotonDePasar.seHizoClic == true)
                    {
                        CalcularTurno(Mesa1.JugadorEnTurno);
                    }
                    BotonDePasar.Update(EstadoActualDeMouse);


                    VerificarCondicionDeFinDeRonda();

                    break;


                case GameState.FinDeRondaActual:

                    if (PuntosEquipo1>MaximaPuntuacion)
                    {
                        Gano = true;
                    }
                    else if (PuntosEquipo2>MaximaPuntuacion)
                    {
                        Perdio = true;
                    }
                    
                    if (BotonSiguienteRonda.seHizoClic == true)
                    {
                        InicioMano = true;                   
                        
                        Mesa1.FichasEnMesa.Clear();
                        foreach (Jugador j in Jugadores)
                        {
                            j.FichasDeJugador.Clear();
                        }
                        FinDeRonda = false;
                        JuegoTrancado = false;
                        EstadoActualDeJuego = GameState.Jugando;
                    }

                    BotonSiguienteRonda.Update(EstadoActualDeMouse);

                    break;

                case GameState.Opciones:

                    // Si teclas Escape, vuele al menu anterior
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        EstadoActualDeJuego = GameState.MenuPrincipal;

                    if (BotonNivel.seHizoClic == true)
                    {
                        EstadoActualDeJuego = GameState.Nivel;                        
                    }

                    if (BotonColorDeDominoes.seHizoClic == true)
                    {
                        EstadoActualDeJuego = GameState.ColorDeFicha;
                    }

                    BotonColorDeDominoes.Update(EstadoActualDeMouse);
                    BotonColorDeDominoes.Update(EstadoActualDeMouse);
                    BotonNivel.Update(EstadoActualDeMouse);
                    BotonNivel.Update(EstadoActualDeMouse);
                    
                   
                   

                    break;

                case GameState.SobreDomino:

                    // Si teclas Escape, vuele al menu anterior
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        EstadoActualDeJuego = GameState.MenuPrincipal;

                    break;

                case GameState.Nivel:
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        EstadoActualDeJuego = GameState.Opciones;

                    if (MuyFacil.seHizoClic == true)
                    {
                        NivelActual = Nivel.MuyFacil;
                    }

                    if (Facil.seHizoClic == true)
                    {
                        NivelActual = Nivel.Facil;
                    }

                    if (Normal.seHizoClic == true)
                    {
                        NivelActual = Nivel.Normal;
                    }

                    if (Experto.seHizoClic == true)
                    {
                        NivelActual = Nivel.Experto;
                    }

                    MuyFacil.Update(EstadoActualDeMouse);
                    MuyFacil.Update(EstadoActualDeMouse);
                    Facil.Update(EstadoActualDeMouse);
                    Facil.Update(EstadoActualDeMouse);
                    Normal.Update(EstadoActualDeMouse);
                    Normal.Update(EstadoActualDeMouse);                    
                    Experto.Update(EstadoActualDeMouse);
                    Experto.Update(EstadoActualDeMouse);
                 
                    break;

                case GameState.ColorDeFicha:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                        EstadoActualDeJuego = GameState.Opciones;

                    if (Blanco.seHizoClic == true)
                    {
                        ColorDeFichaActual = ColorDeFicha.Blanco;
                        ColorADibujar = Color.White;
                    }

                    if (Amarillo.seHizoClic == true)
                    {
                        ColorDeFichaActual = ColorDeFicha.Amarillo;
                        ColorADibujar = Color.Yellow;
                    }

                    if (Azul.seHizoClic == true)
                    {
                        ColorDeFichaActual = ColorDeFicha.Azul;
                        ColorADibujar = Color.Cyan;
                    }

                    if (Rojo.seHizoClic == true)
                    {
                        ColorDeFichaActual = ColorDeFicha.Rojo;
                        ColorADibujar = Color.Red;
                    }

                    if (Verde.seHizoClic == true)
                    {
                        ColorDeFichaActual = ColorDeFicha.Verde;
                        ColorADibujar = Color.DarkGreen;
                    }

                    Blanco.Update(EstadoActualDeMouse);
                    Blanco.Update(EstadoActualDeMouse);
                    Amarillo.Update(EstadoActualDeMouse);
                    Amarillo.Update(EstadoActualDeMouse);
                    Azul.Update(EstadoActualDeMouse);
                    Azul.Update(EstadoActualDeMouse);
                    Rojo.Update(EstadoActualDeMouse);
                    Rojo.Update(EstadoActualDeMouse);
                    Verde.Update(EstadoActualDeMouse);
                    Verde.Update(EstadoActualDeMouse);

                    break;





            }

            //store the current state of the mouse as the old
            EstadoPrevioDeMouse = EstadoActualDeMouse;
          
            base.Update(gameTime);
        }

        #endregion

        #region Draw (Dibujar)

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Menu principal
            switch (EstadoActualDeJuego)
            {
                case GameState.MenuPrincipal:
                    //add a white background
                    GraphicsDevice.Clear(Color.White);
                    spriteBatch.Begin();

                    spriteBatch.Draw(Content.Load<Texture2D>(@"Imagenes\MenuPrincipal"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);
                    
                    // Condicion que muestra el boton de "Seguir Jugando" si el juego ya ha sido comenzado
                    if (!JuegoEmpezado)
                    {
                        BotonDeJugar.Draw(spriteBatch);
                    }
                    else BotonSeguirJugando.Draw(spriteBatch);

                    BotonDeOpciones.Draw(spriteBatch);
                    BotonDeCreditos.Draw(spriteBatch);
                    BotonDeSalida.Draw(spriteBatch);
                    BotonDobleSeis.Draw(spriteBatch);

                    // End drawing
                    spriteBatch.End();

                    break;

                case GameState.Jugando:

                    // Agregar un fondo de color tal
                    GraphicsDevice.Clear(Color.DarkGreen);

                    // Empezar a dibujar
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    
                    DibujarTexto();                 //draw helptext
                    DibujarTablero();               //draw the board
                    DibujarFichasMesa();            // Dibujar las fichas que ya se han jugado
                    DibujarCuadroArrastrable();     // Draw the draggable Cuadro, wherever it may be


                    foreach (Ficha s in jugador1.FichasDeJugador)       // Dibujar fichas de jugador excepto la que se esta arrastrando
                        if (!s.SeEstaArrastrando)
                        {
                            spriteBatch.Draw(s.Imagen,
                                     s.Posicion, null,
                                     ColorADibujar, 0, Vector2.Zero,
                                     .07f, SpriteEffects.None, 1);
                        }



                    #region Dibujar Fichas de los otros jugadores
                    // Dibujar las fichas del jugador 2 (OPONENTE DERECHO)
                    Vector2 PosicionInicialDeJugador2 = new Vector2(1047, 270);

                    for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                    {
                        spriteBatch.Draw(FichaDeOponente, PosicionInicialDeJugador2, null,
                            ColorADibujar, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        PosicionInicialDeJugador2 = new Vector2(PosicionInicialDeJugador2.X, PosicionInicialDeJugador2.Y + 28.8f);
                    }

                    // Dibujar las fichas del jugador 3 (PAREJA)
                    Vector2 PosicionInicialDeJugador3 = new Vector2(449, 32);

                    for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                    {
                        spriteBatch.Draw(FichaDePareja, PosicionInicialDeJugador3, null,
                            ColorADibujar, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        PosicionInicialDeJugador3 = new Vector2(PosicionInicialDeJugador3.X + 28.8f, PosicionInicialDeJugador3.Y);
                    }

                    // Dibujar las fichas del jugador 4 (OPONENTE IZQUIERDO)
                    Vector2 PosicionInicialDeJugador4 = new Vector2(40, 272);

                    for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                    {
                        spriteBatch.Draw(FichaDeOponente, PosicionInicialDeJugador4, null,
                            ColorADibujar, 0, Vector2.Zero,
                            .07f, SpriteEffects.None, .5f);
                        PosicionInicialDeJugador4 = new Vector2(PosicionInicialDeJugador4.X, PosicionInicialDeJugador4.Y + 28.8f);
                    }
                    #endregion

                    //Dibuja el fondo de la mesa
                    spriteBatch.Draw(Fondo,
                        new Rectangle(0, 0, Window.ClientBounds.Width,
                        Window.ClientBounds.Height), null,
                        Color.White, 0, Vector2.Zero,
                        SpriteEffects.None, 0.0f);

                  

                  

                    if (Mesa1.JugadorEnTurno.EsHumano && JugadorPasa && Mesa1.JugadorEnTurno.FichasDeJugador.Count>0)
                    {

                        BotonDePasar.Draw(spriteBatch, 1f);
                    }

                    // Imprime el jugador en turno actual 
                    spriteBatch.DrawString(Letras, "Turno:", new Vector2(1162, 720), Color.White);

                    foreach (Jugador j in Jugadores)
                    {
                        if (j.MiTurno)
                            spriteBatch.DrawString(Letras, j.Nombre, new Vector2(1232, 720), Color.White);
                    }
                   
                    // End drawing
                    spriteBatch.End();


                    break;

                case GameState.FinDeRondaActual:
                    
                        
                    
    
 
                    String PuntosDeEquipo1 = PuntosEquipo1.ToString();
                    String PuntosDeEquipo2 = PuntosEquipo2.ToString();


                    spriteBatch.Begin();

                    Delegado miDelegado = n => { string s = n ; spriteBatch.DrawString(Letras, s, new Vector2(570, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f); };
                    Delegado miDelegado1 = j => { string s = j; spriteBatch.DrawString(Letras, s, new Vector2(702, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f); };
                    
                    miDelegado(PuntosDeEquipo1);
                    miDelegado1(PuntosDeEquipo2);


                   

                    spriteBatch.Draw(FondoDeFinDeRonda, new Vector2(110, 60), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, .95f);

                    if (Gano)
                    {
                        spriteBatch.Draw(Felicidades, new Vector2(110, 400), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, 1f);
                    }
                    if (Perdio)
                    {
                        spriteBatch.Draw(MalaSuerte, new Vector2(110, 400), null, Color.White, 0f, Vector2.Zero, .9f, SpriteEffects.None, 1f);
                    }

                    if (JuegoTrancado)
                    {
                        spriteBatch.DrawString(Cent, "Juego Trancado por: " + UltimoJugadorEnJugar.Nombre, new Vector2(250, 100), Color.DarkGreen, 0f, Vector2.Zero, .7f, SpriteEffects.None, .99f);
                    }

                    else
                    {
                        spriteBatch.DrawString(Cent, "FIN DE RONDA", new Vector2(220, 100), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    }
                        

                    spriteBatch.DrawString(Cent, jugador1.Nombre, new Vector2(170, 190), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Cent, jugador2.Nombre, new Vector2(170, 250), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Cent, jugador3.Nombre, new Vector2(170, 310), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Cent, jugador4.Nombre, new Vector2(170, 370), Color.DarkGreen, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);


                    spriteBatch.DrawString(Letras, "PUNTUACION", new Vector2(580, 200), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Letras, "Equipo de: ", new Vector2(530, 240), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Letras, jugador1.Nombre, new Vector2(530, 260), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Letras, jugador3.Nombre, new Vector2(530, 280), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    
                    spriteBatch.DrawString(Letras, PuntosDeEquipo1, new Vector2(570, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    spriteBatch.DrawString(Letras, "Equipo de: ", new Vector2(662, 240), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Letras, jugador2.Nombre, new Vector2(662, 260), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);
                    spriteBatch.DrawString(Letras, jugador4.Nombre, new Vector2(662, 280), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    spriteBatch.DrawString(Letras, PuntosDeEquipo2, new Vector2(702, 300), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, .99f);

                    

                    BotonSiguienteRonda.Draw(spriteBatch, 1f);

                    Vector2 PosicionInicialFichasJugador1 = new Vector2(300, 180);
                    foreach (Ficha s in jugador1.FichasDeJugador)       // Dibujar fichas de jugador excepto la que se esta arrastrando
                    {
                        s.UltimaOrientacion = 1;
                        spriteBatch.Draw(s.Imagen,
                                    PosicionInicialFichasJugador1, null,
                                    ColorADibujar, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);

                        PosicionInicialFichasJugador1 = new Vector2(PosicionInicialFichasJugador1.X + 28.8f, PosicionInicialFichasJugador1.Y);
                    }

                    Vector2 PosicionInicialFichasJugador2 = new Vector2(300, 240);
                    foreach (Ficha s in jugador2.FichasDeJugador)       // Dibujar fichas de jugador excepto la que se esta arrastrando
                    {
                        s.UltimaOrientacion = 1;
                        spriteBatch.Draw(s.Imagen,
                                    PosicionInicialFichasJugador2, null,
                                    ColorADibujar, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        PosicionInicialFichasJugador2 = new Vector2(PosicionInicialFichasJugador2.X + 28.8f, PosicionInicialFichasJugador2.Y);
                    }

                    Vector2 PosicionInicialFichasJugador3 = new Vector2(300, 300);
                    foreach (Ficha s in jugador3.FichasDeJugador)       // Dibujar fichas de jugador excepto la que se esta arrastrando
                    {
                        s.UltimaOrientacion = 1;
                        spriteBatch.Draw(s.Imagen,
                                    PosicionInicialFichasJugador3, null,
                                    ColorADibujar, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        PosicionInicialFichasJugador3 = new Vector2(PosicionInicialFichasJugador3.X + 28.8f, PosicionInicialFichasJugador3.Y);
                    }


                    Vector2 PosicionInicialFichasJugador4 = new Vector2(300, 360);
                    foreach (Ficha s in jugador4.FichasDeJugador)       // Dibujar fichas de jugador excepto la que se esta arrastrando
                    {
                        s.UltimaOrientacion = 1;
                        spriteBatch.Draw(s.Imagen,
                                    PosicionInicialFichasJugador4, null,
                                    ColorADibujar, 0, Vector2.Zero,
                                    .07f, SpriteEffects.None, 1);
                        PosicionInicialFichasJugador4 = new Vector2(PosicionInicialFichasJugador4.X + 28.8f, PosicionInicialFichasJugador4.Y);
                    }

                    spriteBatch.End();


                    

                    break;

                case GameState.Opciones:

                    GraphicsDevice.Clear(Color.Red);
                    spriteBatch.Begin();
                   
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Imagenes\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);
                    BotonNivel.Draw(spriteBatch);
                    BotonColorDeDominoes.Draw(spriteBatch);

                    spriteBatch.End();

                    break;

                case GameState.SobreDomino:

                    spriteBatch.Begin();

                    spriteBatch.Draw(Content.Load<Texture2D>(@"Imagenes\SobreDomino"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    spriteBatch.DrawString(Cent, "Sobre...", new Vector2(621, 500), Color.DarkGreen);
                    spriteBatch.DrawString(Cent, "Domino Tropical v1.0", new Vector2(621, 540), Color.DarkGreen);
                    spriteBatch.DrawString(Cent, "Desarrollado por:   Anthony Ortiz 2009-1407", new Vector2(621, 580), Color.DarkGreen);
                    spriteBatch.DrawString(Cent, "Leonardo Lopez 2008-2455", new Vector2(845, 620), Color.DarkGreen);

                    spriteBatch.DrawString(Letras, "Presione Esc para continuar...", new Vector2(945, 700), Color.DarkGreen, 0, Vector2.Zero, .7f, SpriteEffects.None, 1);

                    spriteBatch.End();

                    break;

                case GameState.Nivel:

                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Imagenes\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    if (NivelActual==Nivel.MuyFacil)
                    {
                        MuyFacil.Draw(spriteBatch, Color.Black);
                        Facil.Draw(spriteBatch);
                        Normal.Draw(spriteBatch);
                        Experto.Draw(spriteBatch);
                    }

                    if (NivelActual == Nivel.Facil)
                    {
                        Facil.Draw(spriteBatch, Color.Black);
                        MuyFacil.Draw(spriteBatch);
                        Normal.Draw(spriteBatch);
                        Experto.Draw(spriteBatch);
                    }

                    if (NivelActual == Nivel.Normal)
                    {
                        Normal.Draw(spriteBatch, Color.Black);
                        MuyFacil.Draw(spriteBatch);
                        Facil.Draw(spriteBatch);
                        Experto.Draw(spriteBatch);
                    }

                    if (NivelActual == Nivel.Experto)
                    {
                        Experto.Draw(spriteBatch, Color.Black);
                        MuyFacil.Draw(spriteBatch);
                        Facil.Draw(spriteBatch);
                        Normal.Draw(spriteBatch);
                    }

                    spriteBatch.End();

                    break;

                case GameState.ColorDeFicha:

                    spriteBatch.Begin();
                    spriteBatch.Draw(Content.Load<Texture2D>(@"Imagenes\Opciones"),
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        Color.White);

                    if (ColorDeFichaActual==ColorDeFicha.Blanco)
                    {
                        Blanco.Draw(spriteBatch,Color.Black);
                        Amarillo.Draw(spriteBatch);
                        Azul.Draw(spriteBatch);
                        Rojo.Draw(spriteBatch);
                        Verde.Draw(spriteBatch);
                    }

                    if (ColorDeFichaActual == ColorDeFicha.Amarillo)
                    {
                        Blanco.Draw(spriteBatch);
                        Amarillo.Draw(spriteBatch, Color.Black);
                        Azul.Draw(spriteBatch);
                        Rojo.Draw(spriteBatch);
                        Verde.Draw(spriteBatch);
                    }

                    if (ColorDeFichaActual == ColorDeFicha.Azul)
                    {
                        Blanco.Draw(spriteBatch);
                        Amarillo.Draw(spriteBatch);
                        Azul.Draw(spriteBatch, Color.Black);
                        Rojo.Draw(spriteBatch);
                        Verde.Draw(spriteBatch);
                    }

                    if (ColorDeFichaActual == ColorDeFicha.Rojo)
                    {
                        Blanco.Draw(spriteBatch);
                        Amarillo.Draw(spriteBatch);
                        Azul.Draw(spriteBatch);
                        Rojo.Draw(spriteBatch, Color.Black);
                        Verde.Draw(spriteBatch);
                    }

                    if (ColorDeFichaActual == ColorDeFicha.Verde)
                    {
                        Blanco.Draw(spriteBatch);
                        Amarillo.Draw(spriteBatch);
                        Azul.Draw(spriteBatch);
                        Rojo.Draw(spriteBatch);
                        Verde.Draw(spriteBatch, Color.Black);
                    }

                    spriteBatch.End();

                    break;
            }
            base.Draw(gameTime);
        }

        #endregion

        #region Draw Metodos para ayudar dibujar

        // Dibuja el texto
        private void DibujarTexto()
        {
            String PuntosDeEquipo1 = PuntosEquipo1.ToString();
            String PuntosDeEquipo2 = PuntosEquipo2.ToString();
            spriteBatch.DrawString(Letras, "Domino Tropical", new Vector2(50, 20), Color.White);

            spriteBatch.DrawString(Letras, "PUNTUACION", new Vector2(1150, 100), Color.White);
            spriteBatch.DrawString(Letras, "Equipo de: "  , new Vector2(1100, 140), Color.White);
            spriteBatch.DrawString(Letras, jugador1.Nombre, new Vector2(1100, 160), Color.White);
            spriteBatch.DrawString(Letras,  jugador3.Nombre, new Vector2(1100, 180), Color.White);
            spriteBatch.DrawString(Letras, PuntosDeEquipo1, new Vector2(1140, 200), Color.White);

            spriteBatch.DrawString(Letras, "Equipo de: ", new Vector2(1232, 140), Color.White);
            spriteBatch.DrawString(Letras, jugador2.Nombre, new Vector2(1232, 160), Color.White);
            spriteBatch.DrawString(Letras, jugador4.Nombre, new Vector2(1232, 180), Color.White);
            spriteBatch.DrawString(Letras, PuntosDeEquipo2, new Vector2(1272, 200), Color.White);

        }


        // Draws the draggable Cuadro either under the mouse, if it is currently being dragged, or in its default posicion
        private void DibujarCuadroArrastrable()
        {
            foreach (Ficha h in jugador1.FichasDeJugador)
            {
                if (h.SeEstaArrastrando)
                {
                    spriteBatch.Draw(h.Imagen, new Rectangle((int)(PosicionDeMouse.X - CuadroBlanco.Width / 4), (int)(PosicionDeMouse.Y - CuadroBlanco.Height / 4), 30, 60), null, ColorADibujar, 0, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
        }


        // Draws the game board
        private void DibujarTablero()
        {
            float Opacidad = 1;                                  //how opaque/transparent to draw the square
            Color ColorAUsar = Color.White;                     //background color to use
            Rectangle PosicionParaDibujarCuadro = new Rectangle();   //the square to draw (local variable to avoid creating a new variable per square)

            //for all columns
            for (int x = 0; x < Tablero.GetLength(0); x++)
            {
                //for all rows
                for (int y = 0; y < Tablero.GetLength(1); y++)
                {
                    //figure out where to draw the square
                    PosicionParaDibujarCuadro = new Rectangle((int)(x * TamanoDeCuadro + PosicionDeTablero.X), (int)(y * TamanoDeCuadro + PosicionDeTablero.Y), TamanoDeCuadro, TamanoDeCuadro);

                    //the code below will make the board checkered using only a single, white square:

                    //if we add the x and y value of the Cuadro
                    //and it is even, we make it one third opaque
                    if ((x + y) % 2 == 0)
                    {
                        Opacidad = .15f;
                    }
                    else
                    {
                        //otherwise it is one tenth opaque
                        Opacidad = .1f;
                    }

                    //make the square the mouse is over red
                    if (MouseDentroDelTablero() && MouseSobreCuadro(x, y))
                    {
                        ColorAUsar = Color.Red;
                        Opacidad = .5f;
                    }
                    else
                    {
                        ColorAUsar = Color.White;
                    }


                    //draw the white square at the given posicion, offset by the x- and y-offset, in the Opacidad desired
                    spriteBatch.Draw(CuadroBlanco, PosicionParaDibujarCuadro, null, ColorAUsar * Opacidad, 0, Vector2.Zero, SpriteEffects.None, 0);


                    //if the square has a Cuadro - draw it
                    if (Tablero[x, y])
                    {
                        Rectangle RecFichaExtremoDerecho = new Rectangle((int)Mesa1.PosicionExtremoDerecho.X, (int)Mesa1.PosicionExtremoDerecho.Y, TamanoDeCuadro, TamanoDeCuadro);
                        Rectangle RecFichaExtremoIzquierdo = new Rectangle((int)Mesa1.PosicionExtremoIzquierdo.X, (int)Mesa1.PosicionExtremoIzquierdo.Y, TamanoDeCuadro, TamanoDeCuadro);
                        if (((UltimaFichaTomada.PrimerValor == Mesa1.ExtremoDerecho) || (UltimaFichaTomada.SegundoValor == Mesa1.ExtremoDerecho)
                            || (InicioDePartida) || (InicioMano)) && (RecFichaExtremoDerecho.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y)))
                        {
                            FichaAActualizar = UltimaFichaTomada;
                            FichaAActualizar.Posicion = new Vector2((int)(x * TamanoDeCuadro + PosicionDeTablero.X), (int)(y * TamanoDeCuadro + PosicionDeTablero.Y));
                            ActualizarMesa(Mesa1, FichaAActualizar);
                            Tablero[x, y] = false;
                        }
                        else if (((UltimaFichaTomada.SegundoValor == Mesa1.ExtremoIzquierdo)
                              || (UltimaFichaTomada.PrimerValor == Mesa1.ExtremoIzquierdo) || (InicioDePartida) || (InicioMano)) && (RecFichaExtremoIzquierdo.Contains((int)PosicionDeMouse.X, (int)PosicionDeMouse.Y)))
                    	{
                            FichaAActualizar = UltimaFichaTomada;
                            FichaAActualizar.Posicion = new Vector2((int)(x * TamanoDeCuadro + PosicionDeTablero.X), (int)(y * TamanoDeCuadro + PosicionDeTablero.Y));
                            ActualizarMesa(Mesa1, FichaAActualizar);
                            Tablero[x, y] = false;
	                    }

                        else if(InicioDePartida && UltimaFichaTomada.PrimerValor == 6 && UltimaFichaTomada.SegundoValor == 6)
                        {
                            FichaAActualizar = UltimaFichaTomada;
                            FichaAActualizar.Posicion = new Vector2((int)(x * TamanoDeCuadro + PosicionDeTablero.X), (int)(y * TamanoDeCuadro + PosicionDeTablero.Y));
                            ActualizarMesa(Mesa1, FichaAActualizar);
                            Tablero[x, y] = false;
                        }
                        else if (InicioMano)
                        {
                            FichaAActualizar = UltimaFichaTomada;
                            FichaAActualizar.Posicion = new Vector2((int)(x * TamanoDeCuadro + PosicionDeTablero.X), (int)(y * TamanoDeCuadro + PosicionDeTablero.Y));
                            ActualizarMesa(Mesa1, FichaAActualizar);
                            Tablero[x, y] = false;
                        }
                        else
                        {
                            Tablero[x, y] = false;
                        }

                    }

                }
            }
        }

        private void DibujarFichasMesa()
        {

            foreach (Ficha h in Mesa1.FichasEnMesa)
            {
                if (h.UltimaOrientacion == 1)
                    spriteBatch.Draw(h.Imagen, h.Posicion, null, ColorADibujar, 0, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.UltimaOrientacion == 2)
                    spriteBatch.Draw(h.Imagen, new Vector2(h.Posicion.X + 28, h.Posicion.Y), null, ColorADibujar, (float)Math.PI / 2, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.UltimaOrientacion == 3)
                    spriteBatch.Draw(h.Imagen, new Vector2(h.Posicion.X + 28, h.Posicion.Y + 28), null, ColorADibujar, (float)Math.PI, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);

                if (h.UltimaOrientacion == 4)
                    spriteBatch.Draw(h.Imagen, new Vector2(h.Posicion.X, h.Posicion.Y + 28), null, ColorADibujar, (float)Math.PI * 3 / 2, Vector2.Zero, 0.07f, SpriteEffects.None, .9f);
            }
        }

        #endregion

        #region Metodos de comportamiento de Mouse y Tablero

        // Checks to see whether a given coordinate is within the board
        private bool MouseSobreCuadro(int x, int y)
        {
            //do an integerdivision (whole-number) of the coordinates relative to the board offset with the tilesize in mind
            return (int)(PosicionDeMouse.X - PosicionDeTablero.X) / TamanoDeCuadro == x && (int)(PosicionDeMouse.Y - PosicionDeTablero.Y) / TamanoDeCuadro == y;
        }

        //find out whether the mouse is inside the board
        bool MouseDentroDelTablero()
        {
            if (PosicionDeMouse.X >= PosicionDeTablero.X && PosicionDeMouse.X <= PosicionDeTablero.X + Tablero.GetLength(0) * TamanoDeCuadro && PosicionDeMouse.Y >= PosicionDeTablero.Y && PosicionDeMouse.Y <= PosicionDeTablero.Y + Tablero.GetLength(1) * TamanoDeCuadro)
            {
                return true;
            }
            else
            { return false; }
        }

        //get the column/row on the board for a given coordinate
        Vector2 ObtenerCuadroAPartirDePosicion(Vector2 posicion)
        {
            //adjust for the boards offset (PosicionDeTablero) and do an integerdivision
            return new Vector2((int)(PosicionDeMouse.X - PosicionDeTablero.X) / TamanoDeCuadro, (int)(PosicionDeMouse.Y - PosicionDeTablero.Y) / TamanoDeCuadro);
        }

        #endregion

        #region Metodos de Juego

        /// <summary> 
        /// Deals seven dominoes to each player 
        /// </summary> 
        private void RepartirFichas(List<Ficha> ListaCompletaDeFichasParaRepartir)
        {

            // Con esta condicion "if", se reparen las fichas
            // variable to hold the Domino to remove 
            Ficha fichaARemover = null;
            if (ListaCompletaDeFichasParaRepartir.Count > 0)
            {
                //seven times 
                for (int i = 0; i < 7; i++)
                {
                    //...give each player a domino 
                    foreach (Jugador jugador in Jugadores)
                    {
                        //get a aleatorio posicion 
                        int PosicionDeFichaAleatoria = aleatorio.Next(ListaCompletaDeFichasParaRepartir.Count);
                        //store the domino in a variable 
                        fichaARemover = ListaCompletaDeFichasParaRepartir[PosicionDeFichaAleatoria];
                        //remove it from the list 
                        ListaCompletaDeFichasParaRepartir.RemoveAt(PosicionDeFichaAleatoria);
                        //add it to the player's dominoes 
                        jugador.FichasDeJugador.Add(fichaARemover);

                    }
                }

            }

            Vector2 PosicionInicialDeJugador = new Vector2(449, 680);

            for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
            {
                jugador1.FichasDeJugador[i].Posicion = PosicionInicialDeJugador;
                jugador1.FichasDeJugador[i].BordeFicha = new Rectangle((int)jugador1.FichasDeJugador[i].Posicion.X, (int)jugador1.FichasDeJugador[i].Posicion.Y, 28, 56);

                PosicionInicialDeJugador = new Vector2(PosicionInicialDeJugador.X + 28.8f, 680);
            }
        }


        private void ActualizarMesa(Mesa Mesa1, Ficha FichaAActualizar)
        {

            //...give each player a domino 
            foreach (Jugador jugador in Jugadores)
            {
                for (int i = 0; i < jugador.FichasDeJugador.Count; i++)
                {

                    if ((jugador.FichasDeJugador[i].PrimerValor == FichaAActualizar.PrimerValor) && (jugador.FichasDeJugador[i].SegundoValor == FichaAActualizar.SegundoValor)
                        && jugador.EsHumano && (Mesa1.JugadorEnTurno == jugador))
                    {
                        if (InicioDePartida)
                        {
                            if (jugador.FichasDeJugador[i].PrimerValor == 6 && jugador.FichasDeJugador[i].SegundoValor == 6)
                            {
                                RegularMesa(Mesa1, FichaAActualizar, jugador, i);
                                InicioDePartida = false;
                            }

                        }

                        else if (InicioMano)
                        {
                            RegularMesa(Mesa1, FichaAActualizar, jugador, i);
                            InicioMano = false;
                        }

                        else if ((jugador.FichasDeJugador[i].PrimerValor == Mesa1.ExtremoDerecho) || (jugador.FichasDeJugador[i].PrimerValor == Mesa1.ExtremoIzquierdo)
                            || (jugador.FichasDeJugador[i].SegundoValor == Mesa1.ExtremoDerecho) || (jugador.FichasDeJugador[i].SegundoValor == Mesa1.ExtremoIzquierdo))
                        {
                            RegularMesa(Mesa1, FichaAActualizar, jugador, i);
                        }


                    }

                }

            }

        }



        private void RegularMesa(Mesa Mesa1, Ficha FichaAActualizar, Jugador Jugador, int PosicionARemover)
        {
            Vector2 PosicionDeReferencia = FichaAActualizar.Posicion;
            if (Mesa1.FichasEnMesa.Count < 1)
            {
                if (FichaAActualizar.Doble)
                {
                    FichaAActualizar.UltimaOrientacion = 1;
                    FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - 14);
                    Mesa1.PosicionExtremoDerecho = new Vector2((int)(PosicionDeReferencia.X + TamanoDeCuadro), (int)(PosicionDeReferencia.Y));
                    Mesa1.PosicionExtremoIzquierdo = new Vector2((int)(PosicionDeReferencia.X - TamanoDeCuadro), (int)(PosicionDeReferencia.Y));
                }
                else
                {
                    Mesa1.PosicionExtremoDerecho = new Vector2((int)(PosicionDeReferencia.X + TamanoDeCuadro), (int)(PosicionDeReferencia.Y));
                    Mesa1.PosicionExtremoIzquierdo = new Vector2((int)(PosicionDeReferencia.X - 2 * TamanoDeCuadro), (int)(PosicionDeReferencia.Y));
                }

                Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;
                Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;
                Mesa1.FichaExtremoDerecho = FichaAActualizar;
                Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                if (Mesa1.JugadorEnTurno == jugador1)
                {
                    CalcularTurno(Mesa1.JugadorEnTurno);
                }


            }

            else
            {
                if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 1 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 3) && (FichaAActualizar.Doble)
                    && (PosicionDeReferencia.Y > Mesa1.PosUltimoCuadroDerecho.Y) && (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor))
                {

                    FichaAActualizar.UltimaOrientacion = 2;
                    FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X + 14, FichaAActualizar.Posicion.Y);
                    Mesa1.FichaExtremoDerecho = FichaAActualizar;
                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y + TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);


                }

                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 1 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 3) && (FichaAActualizar.Doble)
                     && (PosicionDeReferencia.Y < Mesa1.PosUltimoCuadroIzquierdo.Y) && (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor))
                {

                    FichaAActualizar.UltimaOrientacion = 2;
                    FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X + 14, FichaAActualizar.Posicion.Y);
                    Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y - TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);

                }
                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 2 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 4) && (FichaAActualizar.Doble)
                    && PosicionDeReferencia.X < Mesa1.PosUltimoCuadroIzquierdo.X && Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                {

                    FichaAActualizar.UltimaOrientacion = 1;
                    FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - 14);
                    Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - TamanoDeCuadro, PosicionDeReferencia.Y);
                    if (PosicionDeReferencia.X < (PosicionDeTablero.X + (4 * TamanoDeCuadro)))
                    {
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y + 14);
                        FichaAActualizar.UltimaOrientacion = 2;
                        Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - TamanoDeCuadro, PosicionDeReferencia.Y - TamanoDeCuadro);


                    }

                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);


                }

                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 2 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 4) && (FichaAActualizar.Doble)
                       && PosicionDeReferencia.X > Mesa1.PosUltimoCuadroDerecho.X && Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                {

                    FichaAActualizar.UltimaOrientacion = 1;
                    FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - 14);
                    Mesa1.FichaExtremoDerecho = FichaAActualizar;
                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + TamanoDeCuadro, PosicionDeReferencia.Y);
                    if (PosicionDeReferencia.X > (PosicionDeTablero.X + (27 * TamanoDeCuadro)))
                    {
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y + 14);
                        FichaAActualizar.UltimaOrientacion = 4;
                        Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + TamanoDeCuadro, PosicionDeReferencia.Y + TamanoDeCuadro);


                    }
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);

                }


               // Hasta aquí todo bn


                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 1 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 3) && (!FichaAActualizar.Doble)
                    && Mesa1.FichaExtremoDerecho.Doble && PosicionDeReferencia.X > Mesa1.FichaExtremoDerecho.Posicion.X)
                {

                    // Mesa1.FichaExtremoDerecho = FichaAActualizar;
                    if (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 4;

                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.SegundoValor;


                    }

                    else if (Mesa1.ExtremoDerecho == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 2;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X + 28, FichaAActualizar.Posicion.Y);
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;


                    }
                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + 2 * TamanoDeCuadro, PosicionDeReferencia.Y);
                    if (PosicionDeReferencia.X > (PosicionDeTablero.X + (27 * TamanoDeCuadro)))
                    {
                        Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + TamanoDeCuadro, PosicionDeReferencia.Y + TamanoDeCuadro);


                    }
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);

                }
                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 1 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 3)
                 && (!FichaAActualizar.Doble) && Mesa1.FichaExtremoIzquierdo.Doble && (PosicionDeReferencia.X < Mesa1.FichaExtremoIzquierdo.Posicion.X))
                {

                    if (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 2;

                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;

                    }

                    else if (Mesa1.ExtremoIzquierdo == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 4;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X - TamanoDeCuadro, PosicionDeReferencia.Y);
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.PrimerValor;

                    }
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - 2 * TamanoDeCuadro, PosicionDeReferencia.Y);
                    if (PosicionDeReferencia.X < (PosicionDeTablero.X + (4 * TamanoDeCuadro)))
                    {
                        Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - TamanoDeCuadro, PosicionDeReferencia.Y - TamanoDeCuadro);


                    }
                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }



                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 2 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 4)
                     && (!FichaAActualizar.Doble) && Mesa1.FichaExtremoDerecho.Doble && PosicionDeReferencia.Y > Mesa1.FichaExtremoDerecho.Posicion.Y)
                {
                    if (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.SegundoValor;

                    }

                    else if (Mesa1.ExtremoDerecho == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y + 28);
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;

                    }
                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y + 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }

                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 2 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 4)
                     && (!FichaAActualizar.Doble) && Mesa1.FichaExtremoIzquierdo.Doble && PosicionDeReferencia.Y < Mesa1.FichaExtremoIzquierdo.Posicion.Y)
                {
                    if (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;

                    }

                    else if (Mesa1.ExtremoIzquierdo == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - TamanoDeCuadro);
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.PrimerValor;


                    }
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y - 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);

                }

                    //Aqui arreglar algo
                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 1 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 3)
                     && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoDerecho.Doble) && PosicionDeReferencia.Y > Mesa1.FichaExtremoDerecho.Posicion.Y)
                {
                    if (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;

                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoDerecho == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y + TamanoDeCuadro);
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;


                    }



                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y + 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }


                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 1 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 3)
                    && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoIzquierdo.Doble) && PosicionDeReferencia.Y < Mesa1.FichaExtremoIzquierdo.Posicion.Y)
                {
                    if (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;

                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoIzquierdo == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - TamanoDeCuadro);
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.PrimerValor;


                    }
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y - 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }

                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 2 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 4)
                    && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoIzquierdo.Doble) && PosicionDeReferencia.X < Mesa1.FichaExtremoIzquierdo.Posicion.X)
                {
                    if (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 2;

                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoIzquierdo == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 4;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X - TamanoDeCuadro, FichaAActualizar.Posicion.Y);
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.PrimerValor;


                    }
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - 2 * TamanoDeCuadro, PosicionDeReferencia.Y);
                    if (PosicionDeReferencia.X < (PosicionDeTablero.X + (4 * TamanoDeCuadro)))
                    {
                        Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X - TamanoDeCuadro, PosicionDeReferencia.Y - TamanoDeCuadro);


                    }

                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }


                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 2 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 4)
                    && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoDerecho.Doble) && PosicionDeReferencia.X > Mesa1.FichaExtremoDerecho.Posicion.X)
                {
                    if (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 4;

                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoDerecho == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 2;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X + TamanoDeCuadro, FichaAActualizar.Posicion.Y);
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;

                    }

                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + 2 * TamanoDeCuadro, PosicionDeReferencia.Y);

                    if (PosicionDeReferencia.X > (PosicionDeTablero.X + (27 * TamanoDeCuadro)))
                    {
                        Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X + TamanoDeCuadro, PosicionDeReferencia.Y + TamanoDeCuadro);


                    }

                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }

                else if ((Mesa1.FichaExtremoDerecho.UltimaOrientacion == 2 || Mesa1.FichaExtremoDerecho.UltimaOrientacion == 4)
                    && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoDerecho.Doble) && PosicionDeReferencia.Y > Mesa1.FichaExtremoDerecho.Posicion.Y)
                {
                    if (Mesa1.ExtremoDerecho == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;

                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoDerecho == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y + TamanoDeCuadro);
                        Mesa1.FichaExtremoDerecho = FichaAActualizar;
                        Mesa1.ExtremoDerecho = FichaAActualizar.PrimerValor;

                    }
                    Mesa1.PosicionExtremoDerecho = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y + 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroDerecho = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }

                else if ((Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 2 || Mesa1.FichaExtremoIzquierdo.UltimaOrientacion == 4)
                    && (!FichaAActualizar.Doble) && (!Mesa1.FichaExtremoIzquierdo.Doble) && PosicionDeReferencia.Y < Mesa1.FichaExtremoIzquierdo.Posicion.Y)
                {
                    if (Mesa1.ExtremoIzquierdo == FichaAActualizar.PrimerValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 3;

                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.SegundoValor;
                    }

                    else if (Mesa1.ExtremoIzquierdo == FichaAActualizar.SegundoValor)
                    {
                        FichaAActualizar.UltimaOrientacion = 1;
                        FichaAActualizar.Posicion = new Vector2(FichaAActualizar.Posicion.X, FichaAActualizar.Posicion.Y - TamanoDeCuadro);
                        Mesa1.FichaExtremoIzquierdo = FichaAActualizar;
                        Mesa1.ExtremoIzquierdo = FichaAActualizar.PrimerValor;

                    }
                    Mesa1.PosicionExtremoIzquierdo = new Vector2(PosicionDeReferencia.X, PosicionDeReferencia.Y - 2 * TamanoDeCuadro);
                    Mesa1.PosUltimoCuadroIzquierdo = PosicionDeReferencia;
                    AnadirFicha(Mesa1, FichaAActualizar, Jugador, PosicionARemover);
                }
                if (Mesa1.JugadorEnTurno == jugador1)
                {
                    CalcularTurno(Mesa1.JugadorEnTurno);
                }

            }


        }


        private void AnadirFicha(Mesa Mesa1, Ficha FichaAActualizar, Jugador Jugador, int PosicionARemover)
        {
            //remove it from the list 
            Jugador.FichasDeJugador.RemoveAt(PosicionARemover);
            //add it to the player's dominoes 
            Mesa1.FichasEnMesa.Add(FichaAActualizar);
           
            UltimaFichaJugada = FichaAActualizar;
            UltimoJugadorEnJugar = Mesa1.JugadorEnTurno;
            trackCue = soundBank.GetCue("DominoSetDown");
            trackCue.Play();


        }

        private void CalcularTurno(Jugador JugadorEnTurno)
        {
            if (JugadorEnTurno == jugador4)
            {
                Mesa1.JugadorEnTurno = jugador1;
                jugador4.MiTurno = false;
                jugador1.MiTurno = true;
            }

            else if (JugadorEnTurno == jugador1)
            {
                Mesa1.JugadorEnTurno = jugador2;
                jugador1.MiTurno = false;
                jugador2.MiTurno = true;
            }

            else if (JugadorEnTurno == jugador2)
            {
                Mesa1.JugadorEnTurno = jugador3;
                jugador2.MiTurno = false;
                jugador3.MiTurno = true;
            }

            else if (JugadorEnTurno == jugador3)
            {
                Mesa1.JugadorEnTurno = jugador4;
                jugador3.MiTurno = false;
                jugador4.MiTurno = true;
            }


        }

        private void DecideSiJugadorPasa(Jugador jugador)
        {
            bool tmpPaso = true;
            foreach (Ficha f in jugador.FichasDeJugador)
            {   
                if (f.PrimerValor == Mesa1.ExtremoDerecho
                    ||f.PrimerValor == Mesa1.ExtremoIzquierdo
                    ||f.SegundoValor == Mesa1.ExtremoDerecho
                    ||f.SegundoValor == Mesa1.ExtremoIzquierdo)
                {
                    tmpPaso = false;
                }
            }
            JugadorPasa = tmpPaso;
        }

        /// <summary>
        /// Calcula la puntuacion del equipo ganador
        /// </summary>
        private void CalcularPuntuaciones()
        {
            int puntosASumar = 0;
                
            // Si al jugador 1 o el jugador 3 se le acaban las fichas, este equipo gana y se le suma a puntosASumar el total de puntos de fichas de jugador 2 y del 4.
            if ((jugador1.FichasDeJugador.Count == 0) || (jugador3.FichasDeJugador.Count == 0))
            {
                for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
			    {
                    puntosASumar += jugador1.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador2.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador3.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador4.FichasDeJugador[i].ValorTotal;
                }

                PuntosEquipo1 += puntosASumar;
                    
            }
            else if ((jugador2.FichasDeJugador.Count == 0 )|| (jugador4.FichasDeJugador.Count == 0))
            {
                for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador1.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador2.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador3.FichasDeJugador[i].ValorTotal;
                }
                for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                {
                    puntosASumar += jugador4.FichasDeJugador[i].ValorTotal;
                }

                PuntosEquipo2 += puntosASumar;
            }


            else if (JuegoTrancado )
            {
                if (UltimoJugadorEnJugar.Nombre==jugador1.Nombre)
                {
                    int PuntosDeJugadorQueTranco=0, PuntosDeJugadorALaDerechaDelQueTranco=0;
                    for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                    {
                        PuntosDeJugadorQueTranco += jugador1.FichasDeJugador[i].ValorTotal;
                    }
                    for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                    {
                        PuntosDeJugadorALaDerechaDelQueTranco += jugador2.FichasDeJugador[i].ValorTotal;
                    }

                    // Si el total de las fichas del jugador que tranco es menor o igual al jugador de su derecha, gana la ronda
                    if (PuntosDeJugadorQueTranco<PuntosDeJugadorALaDerechaDelQueTranco || PuntosDeJugadorQueTranco==PuntosDeJugadorALaDerechaDelQueTranco)
                    {
                        for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                        {
                            puntosASumar += jugador1.FichasDeJugador[i].ValorTotal;
                        }
                        for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                        {
                            puntosASumar += jugador2.FichasDeJugador[i].ValorTotal;
                        }
                        for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                        {
                            puntosASumar += jugador3.FichasDeJugador[i].ValorTotal;
                        }
                        for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                        {
                            puntosASumar += jugador4.FichasDeJugador[i].ValorTotal;
                        }
                        // Se le suman los puntos al equipo que trancó
                        PuntosEquipo1 += puntosASumar;

                    }
                    // Si el total de las fichas del jugador que tranco es mayor al jugador de su derecha, pierde la ronda
                    else if (PuntosDeJugadorQueTranco>PuntosDeJugadorALaDerechaDelQueTranco)
                    {
                        for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                    {
                        puntosASumar += jugador1.FichasDeJugador[i].ValorTotal;
                    }
                    for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                    {
                        puntosASumar += jugador2.FichasDeJugador[i].ValorTotal;
                    }
                    for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                    {
                        puntosASumar += jugador3.FichasDeJugador[i].ValorTotal;
                    }
                    for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                    {
                        puntosASumar += jugador4.FichasDeJugador[i].ValorTotal;
                    }
                    // Se le suman los puntos al equipo que no trancó
                    PuntosEquipo2 += puntosASumar;
                    }
                }    
            }
        }


        private void DibujarFichasJugadoresNoHumanosNivel1(Jugador JugadorAJugar)
        {
            for (int i = 0; i < JugadorAJugar.FichasDeJugador.Count; i++)
            {
                if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                    break;
                }

                else if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                    break;
                }


            }
        }


        private void DibujarFichasJugadoresNoHumanosNivel2(Jugador JugadorAJugar)
        {
            int tmpder = 0;
            int tmpizq = 0;
            bool ExtremoDerecho = true;
            Ficha FichaTemporal = new Ficha();
            FichaTemporal.ValorTotal = 0;
            for (int i = 0; i < JugadorAJugar.FichasDeJugador.Count; i++)
            {

                if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    if (JugadorAJugar.FichasDeJugador[i].ValorTotal > FichaTemporal.ValorTotal)
                    {
                        tmpizq = i;
                        FichaTemporal = JugadorAJugar.FichasDeJugador[i];
                        ExtremoDerecho = false;
                    }



                }

                else if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    if (JugadorAJugar.FichasDeJugador[i].ValorTotal > FichaTemporal.ValorTotal)
                    {
                        tmpder = i;
                        FichaTemporal = JugadorAJugar.FichasDeJugador[i];
                        ExtremoDerecho = true;
                    }


                }

                if ((FichaTemporal.ValorTotal != 0) && (ExtremoDerecho))
                {
                    JugadorAJugar.FichasDeJugador[tmpder].Posicion = Mesa1.PosicionExtremoDerecho;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[tmpder], JugadorAJugar, tmpder);
                }

                else if ((FichaTemporal.ValorTotal != 0) && (!ExtremoDerecho))
                {
                    JugadorAJugar.FichasDeJugador[tmpizq].Posicion = Mesa1.PosicionExtremoIzquierdo;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[tmpizq], JugadorAJugar, tmpizq);
                }


            }
        }



        private void DibujarFichasJugadoresNoHumanosNivel3(Jugador JugadorAJugar)
        {

            List<int> FichasConLasQuePuedeJugar = new List<int>();
            int dobles = 0;
            for (int i = 0; i < JugadorAJugar.FichasDeJugador.Count; i++)
            {

                if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor)
                    || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    FichasConLasQuePuedeJugar.Add(i);

                    if (JugadorAJugar.FichasDeJugador[i].Doble)
                    {
                        dobles++;
                        int temporal = 0;
                        foreach (Ficha g in Mesa1.FichasEnMesa)
                        {
                            if (g.PrimerValor == JugadorAJugar.FichasDeJugador[i].PrimerValor || g.SegundoValor == JugadorAJugar.FichasDeJugador[i].PrimerValor)
                            {
                                temporal++;
                            }

                        }
                        if (temporal >= 3 && temporal < 5)
                        {
                            JugadorAJugar.FichasDeJugador[i].Prioridad = 2;
                        }
                        else if (temporal >= 5)
                        {
                            JugadorAJugar.FichasDeJugador[i].Prioridad = 3;
                        }

                    }
                }

                if (FichasConLasQuePuedeJugar.Count == 1)
                {
                    if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].SegundoValor))
                    {
                        JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].Posicion = Mesa1.PosicionExtremoIzquierdo;
                        RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]], JugadorAJugar, FichasConLasQuePuedeJugar[0]);
                    }
                    else if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].SegundoValor))
                    {
                        JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]].Posicion = Mesa1.PosicionExtremoDerecho;
                        RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[FichasConLasQuePuedeJugar[0]], JugadorAJugar, FichasConLasQuePuedeJugar[0]);
                    }

                }
                else if (dobles > 0)
                {
                    int itmp = new int();
                    for (int h = 0; h < JugadorAJugar.FichasDeJugador.Count; h++)
                    {
                        if (JugadorAJugar.FichasDeJugador[h].Prioridad > JugadorAJugar.FichasDeJugador[itmp].Prioridad)
                        {
                            itmp = h;
                        }

                    }
                    if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[itmp].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[itmp].SegundoValor))
                    {
                        JugadorAJugar.FichasDeJugador[itmp].Posicion = Mesa1.PosicionExtremoIzquierdo;
                        RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[itmp], JugadorAJugar, itmp);

                    }
                    else if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[itmp].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[itmp].SegundoValor))
                    {
                        JugadorAJugar.FichasDeJugador[itmp].Posicion = Mesa1.PosicionExtremoDerecho;
                        RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[itmp], JugadorAJugar, itmp);

                    }


                }
                else
                {
                    DibujarFichasJugadoresNoHumanosNivel1(JugadorAJugar);
                }


            }
        }


        private void DibujarFichasJugadoresNoHumanosNivel4(Jugador JugadorAJugar)
        {

            if (JugadorAJugar == jugador1)
            {
                if ((jugador2.ExtremoUltimaJugada != jugador3.ExtremoUltimaJugada) && (jugador2.ExtremoUltimaJugada != jugador4.ExtremoUltimaJugada))
                {
                    if (jugador2.ExtremoUltimaJugada == 2)
                    {
                        for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }
                    else if (jugador2.ExtremoUltimaJugada == 1)
                    {
                        for (int i = 0; i < jugador1.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }

                    else
                    {
                        DibujarFichasJugadoresNoHumanosNivel1(JugadorAJugar);
                    }

                }

            }

            else if (JugadorAJugar == jugador2)
            {
                if ((jugador3.ExtremoUltimaJugada != jugador4.ExtremoUltimaJugada) && (jugador3.ExtremoUltimaJugada != jugador1.ExtremoUltimaJugada))
                {
                    if (jugador3.ExtremoUltimaJugada == 2)
                    {
                        for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }
                    else if (jugador3.ExtremoUltimaJugada == 1)
                    {
                        for (int i = 0; i < jugador2.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }

                    else
                    {
                        DibujarFichasJugadoresNoHumanosNivel1(JugadorAJugar);
                    }

                }

            }

            else if (JugadorAJugar == jugador3)
            {
                if ((jugador4.ExtremoUltimaJugada != jugador1.ExtremoUltimaJugada) && (jugador4.ExtremoUltimaJugada != jugador2.ExtremoUltimaJugada))
                {
                    if (jugador4.ExtremoUltimaJugada == 2)
                    {
                        for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }
                    else if (jugador4.ExtremoUltimaJugada == 1)
                    {
                        for (int i = 0; i < jugador3.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }

                    else
                    {
                        DibujarFichasJugadoresNoHumanosNivel1(JugadorAJugar);
                    }

                }
            }

            else if (JugadorAJugar == jugador4)
            {
                if ((jugador1.ExtremoUltimaJugada != jugador2.ExtremoUltimaJugada) && (jugador1.ExtremoUltimaJugada != jugador3.ExtremoUltimaJugada))
                {
                    if (jugador1.ExtremoUltimaJugada == 2)
                    {
                        for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }
                    else if (jugador1.ExtremoUltimaJugada == 1)
                    {
                        for (int i = 0; i < jugador4.FichasDeJugador.Count; i++)
                        {
                            if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                            {
                                JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                                RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                            }
                        }

                    }

                    else
                    {
                        DibujarFichasJugadoresNoHumanosNivel1(JugadorAJugar);
                    }

                }
            }







            for (int i = 0; i < JugadorAJugar.FichasDeJugador.Count; i++)
            {
                if ((Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoIzquierdo == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoIzquierdo;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                    break;
                }

                else if ((Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].PrimerValor) || (Mesa1.ExtremoDerecho == JugadorAJugar.FichasDeJugador[i].SegundoValor))
                {
                    JugadorAJugar.FichasDeJugador[i].Posicion = Mesa1.PosicionExtremoDerecho;
                    RegularMesa(Mesa1, JugadorAJugar.FichasDeJugador[i], JugadorAJugar, i);
                    break;
                }


            }
        }

        private void VerificarCondicionDeFinDeRonda()
        {

            // Determina condicion de fin de ronda

            foreach (Jugador j in Jugadores)
            {
                // Si un jugador no tiene mas fichas, o se recorren todos los jugadores mas de 5 veces (se tranca), es fin de ronda
                if ((j.FichasDeJugador.Count == 0) && (!InicioMano) && (!InicioDePartida))
                {
                    FinDeRonda = true;
                    CalcularPuntuaciones();
                    JugadorGanoUltimaRonda = j;
                    EstadoActualDeJuego = GameState.FinDeRondaActual;
                    break;
                }
            }
            bool tempJuegoTrancado = true;
            foreach (Jugador j in Jugadores)
            {
                foreach (Ficha f in j.FichasDeJugador)
                {
                    if (Mesa1.ExtremoIzquierdo == f.PrimerValor || Mesa1.ExtremoDerecho == f.PrimerValor
                        || Mesa1.ExtremoIzquierdo == f.SegundoValor || Mesa1.ExtremoDerecho == f.SegundoValor)
                    {
                        tempJuegoTrancado = false;
                        break;
                    }
                }
                if (!tempJuegoTrancado)
                {
                    break;
                }

            }
            JuegoTrancado = tempJuegoTrancado;

            if (JuegoTrancado)
            {
                FinDeRonda = true;
                CalcularPuntuaciones();
                EstadoActualDeJuego = GameState.FinDeRondaActual;
            }
        }


        #endregion

        #region Metodos para guardar datos

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
                    NivelAGuardar = NivelActual,
                    ColorDeFichaAGuardar = ColorDeFichaActual

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
                //Update the game based on the save game file

                NivelActual = SaveData.NivelAGuardar;
                ColorDeFichaActual = SaveData.ColorDeFichaAGuardar;

            }
        }

        #endregion

    }
}
