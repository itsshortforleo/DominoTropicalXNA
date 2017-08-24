using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domino.Entities
{
    public class Jugador
    {
        #region Fields

        string _strNombre;
        bool _miTurno;
        bool _esHumano;
        //Si es 1 jugo por extremo derecho, si es 2 jugo por extremo izquierdo
        int _extremoUltimaJugada;

        #endregion

        #region Properties

        public string Nombre
        {
            get { return _strNombre; }
            set { _strNombre = value; }
        }

        public bool MiTurno
        {
            get { return _miTurno; }
            set { _miTurno = value; }
        }

        public bool EsHumano
        {
            get { return _esHumano; }
            set { _esHumano = value; }
        }

        public int ExtremoUltimaJugada
        {
            get { return _extremoUltimaJugada; }
            set { _extremoUltimaJugada = value; }
        }

        public List<Tile> FichasDeJugador { get; set; } 
      
        #endregion

        #region Constructores

        public Jugador()
        {
        }

        public Jugador(string Nombre, bool MiTurno, bool EsHumano)
        {
            this.Nombre = Nombre;

            this.FichasDeJugador = new List<Tile>();

            this.MiTurno = MiTurno;
            this.EsHumano = EsHumano;
            this.ExtremoUltimaJugada = new int();
        }
        #endregion

        #region Methods/Functions
        
        #endregion

        #region Events
        
        #endregion

    }
}
