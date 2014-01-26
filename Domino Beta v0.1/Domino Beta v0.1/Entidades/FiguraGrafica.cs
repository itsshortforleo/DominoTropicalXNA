using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Domino_Beta_v0._1.Entidades
{
    public class FiguraGrafica
    {
        #region Campos

        // Para dibujar la figura
        Texture2D _imagen;

        // Data para colisiones
        int _colision;

        // Datos de movimiento
        protected Vector2 _velocidad;
        protected Vector2 _posicion;
        //MouseState EstadoPrevioDeMouse;

        #endregion

        #region Propiedades


        public Texture2D Imagen
        {
            get { return _imagen; }
            set { _imagen = value; }
        }
        

        public int Colision
        {
            get { return _colision; }
            set { _colision = value; }
        }

        public Vector2 Velocidad
        {
            get { return _velocidad; }
            set { _velocidad = value; }
        }

        public Vector2 Posicion
        {
            get { return _posicion; }
            set { _posicion = value; }
        }

        #endregion

        #region Contructores

        public FiguraGrafica()
         
        { }

        public FiguraGrafica(Texture2D Imagen,
            int Colision, Vector2 Velocidad,
            Vector2 Posicion)
        {
            this.Imagen = Imagen;
            this.Colision = Colision;
            this.Velocidad = Velocidad;
            this.Posicion = Posicion;
           
        }
        
        #endregion

        #region Metodos/Funciones

        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the sprite
            spriteBatch.Draw(Imagen,
                Posicion, null,
                Color.White, 0, Vector2.Zero,
                .07f, SpriteEffects.None, 1);
        }

        #endregion

        #region Eventos
        
        #endregion

    }
}
