using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Domino_Beta_v0._1.Entidades
{
    public class Ficha : FiguraGrafica
    {
        #region Campos

        // Movement stuff
        int _primerValor;
        int _segundoValor;
        bool _vertical;
        bool _doble;
        bool _seEstaArrastrando;
        int _valorTotal;
        int _prioridad;
       

        #endregion

        #region Propiedades

        public int PrimerValor
        {
            get { return _primerValor; }
            set { _primerValor = value; }
        }

        public int SegundoValor
        {
            get { return _segundoValor; }
            set { _segundoValor = value; }
        }

        public bool Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        public bool Doble
        {
            get { return _doble; }
            set { _doble = value; }
        }

        public bool SeEstaArrastrando
        {
            get { return _seEstaArrastrando; }
            set { _seEstaArrastrando = value; }
        }

        public int ValorTotal
        {
            get { return _valorTotal; }
            set { _valorTotal = value; }
        }

        public int Prioridad
        {
            get { return _prioridad; }
            set { _prioridad = value; }
        }

        public int UltimaOrientacion
        {
            get;
            set;
        }

        public bool DentroDeMesa
        {
            get;
            set;
        }

        
        public Rectangle BordeFicha
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public Ficha()
        {
        }

        public Ficha(Texture2D Imagen, 
            int Colision, Vector2 Velocidad, 
            Vector2 Posicion,
            int PrimerValor, int SegundoValor, bool Vertical, bool Doble, bool SeEstaArrastrando)
            : base( Imagen, Colision, Velocidad, 
             Posicion)
        {
            this.PrimerValor = PrimerValor;
            this.SegundoValor = SegundoValor;
            this.Vertical = Vertical;
            this.Doble = Doble;
            this.SeEstaArrastrando = SeEstaArrastrando;
            this.BordeFicha = new Rectangle((int)Posicion.X, (int)Posicion.Y, 28, 56);
            this.DentroDeMesa = false;
            this.UltimaOrientacion = 2;
            this.ValorTotal = PrimerValor + SegundoValor;
            this.Prioridad = new int();
        } 

        #endregion

        #region Metodos/Funciones
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            base.Update(gameTime, clientBounds);
        }
        
        #endregion

        #region Eventos

        #endregion

    }
}
