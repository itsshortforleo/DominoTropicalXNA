using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Domino.Entities
{
    public class Tile : Sprite
    {
        #region Fields

        // Movement stuff
        int _firstTileValue;
        int _secondTileValue;
        bool _isTileVertical;
        bool _isTileADouble;
        bool _isTileBeingDragged;
        int _totalPointsValue;
        int _priority;
       

        #endregion

        #region Properties

        public int FirstTileValue
        {
            get { return _firstTileValue; }
            set { _firstTileValue = value; }
        }

        public int SecondTileValue
        {
            get { return _secondTileValue; }
            set { _secondTileValue = value; }
        }

        public bool IsTileVertical
        {
            get { return _isTileVertical; }
            set { _isTileVertical = value; }
        }

        public bool IsTileADouble
        {
            get { return _isTileADouble; }
            set { _isTileADouble = value; }
        }

        public bool IsTileBeingDragged
        {
            get { return _isTileBeingDragged; }
            set { _isTileBeingDragged = value; }
        }

        public int TotalPointsValue
        {
            get { return _totalPointsValue; }
            set { _totalPointsValue = value; }
        }

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public int LastOrientation
        {
            get;
            set;
        }

        public bool IsInsideTable
        {
            get;
            set;
        }

        
        public Rectangle TileEdge
        {
            get;
            set;
        }
        #endregion

        #region Constructor

        public Tile()
        {
        }

        public Tile(Texture2D Image, 
            int Collision, Vector2 Speed, 
            Vector2 Position,
            int FirstTileValue, int SecondTileValue, bool IsTileVertical, bool IsTileADouble, bool IsTileBeingDragged)
            : base( Image, Collision, Speed, 
             Position)
        {
            this.FirstTileValue = FirstTileValue;
            this.SecondTileValue = SecondTileValue;
            this.IsTileVertical = IsTileVertical;
            this.IsTileADouble = IsTileADouble;
            this.IsTileBeingDragged = IsTileBeingDragged;
            this.TileEdge = new Rectangle((int)Position.X, (int)Position.Y, 28, 56);
            this.IsInsideTable = false;
            this.LastOrientation = 2;
            this.TotalPointsValue = FirstTileValue + SecondTileValue;
            this.Priority = new int();
        } 

        #endregion

        #region Methods/Functions
        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            base.Update(gameTime, clientBounds);
        }
        
        #endregion

        #region Events

        #endregion

    }
}
