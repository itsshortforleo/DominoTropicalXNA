using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Domino_Beta_v0._1.Entidades
{
   public class Mesa 
    {
        #region Campos
        int _numeroMesa;
        int _extremoIzquierdo;
        int _extremoDerecho;
        Ficha _fichaExtremoDerecho;
        Ficha _fichaExtremoIzquierdo;
        Vector2 _posicionExtremoDerecho;
        Vector2 _posicionExtremoIzquierdo;
        Vector2 _posUltimoCuadroIzquierdo;
        Vector2 _posUltimoCuadroDerecho;
        Jugador _jugadorEnTurno;
        #endregion

        #region Propiedades
        public int NumeroMesa
        {
            get { return _numeroMesa; }
            set { _numeroMesa = value; }
        }

        public int ExtremoIzquierdo
        {
            get { return _extremoIzquierdo; }
            set { _extremoIzquierdo = value; }
        }

        public int ExtremoDerecho
        {
            get { return _extremoDerecho; }
            set { _extremoDerecho = value; }
        }

        public Ficha FichaExtremoDerecho
        {
            get { return _fichaExtremoDerecho; }
            set { _fichaExtremoDerecho = value; }
        }

        public Ficha FichaExtremoIzquierdo
        {
            get { return _fichaExtremoIzquierdo; }
            set { _fichaExtremoIzquierdo = value; }
        }

        public Vector2 PosicionExtremoDerecho
        {
            get { return _posicionExtremoDerecho; }
            set { _posicionExtremoDerecho = value; }
        }

        public Vector2 PosicionExtremoIzquierdo
        {
            get { return _posicionExtremoIzquierdo; }
            set { _posicionExtremoIzquierdo = value; }
        }


        public Vector2 PosUltimoCuadroIzquierdo
        {
            get { return _posUltimoCuadroIzquierdo; }
            set { _posUltimoCuadroIzquierdo = value; }
        }

        public Vector2 PosUltimoCuadroDerecho
        {
            get { return _posUltimoCuadroDerecho; }
            set { _posUltimoCuadroDerecho = value; }
        }
        
        public Jugador JugadorEnTurno
        {
            get { return _jugadorEnTurno; }
            set { _jugadorEnTurno = value; }
        }

        public List<Ficha> FichasEnMesa { get; set; } 
        #endregion

        #region Constructores
        public Mesa()
        {
        }

        public Mesa(int NumeroMesa)
            
        {
            this.NumeroMesa = NumeroMesa;
            this.FichaExtremoDerecho = FichaExtremoDerecho;
            this.FichaExtremoIzquierdo = FichaExtremoIzquierdo;
            this.ExtremoDerecho = new int();
            this.ExtremoIzquierdo = new int();
            this.PosicionExtremoDerecho = new Vector2();
            this.PosicionExtremoIzquierdo = new Vector2();
            this.PosUltimoCuadroDerecho = new Vector2();
            this.PosUltimoCuadroIzquierdo = new Vector2();
            this.JugadorEnTurno = new Jugador();
            this.FichasEnMesa = new List<Ficha>();

        }
        
        #endregion

        #region Metodos/Funciones
        

        #endregion

        #region Eventos
        
        #endregion
    }
}
