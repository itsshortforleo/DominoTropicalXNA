using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Domino.Entities
{
   public class Table 
    {
        #region Fields
        int _tableNumber;
        int _leftHandSide;
        int _rightHandSide;
        Tile _tileOnLeftHandSide;
        Tile _tileOnRightHandSide;
        Vector2 _positionOfLeftHandSideEdge;
        Vector2 _positionOfRightHandSideEdge;
        Vector2 _positionOfTileOnLeftHandSide;
        Vector2 _positionOfTileOnRightHandSide;
        Player _playerInTurn;
        #endregion

        #region Properties
        public int TableNumber
        {
            get { return _tableNumber; }
            set { _tableNumber = value; }
        }

        public int LeftHandSide
        {
            get { return _leftHandSide; }
            set { _leftHandSide = value; }
        }

        public int RightHandSide
        {
            get { return _rightHandSide; }
            set { _rightHandSide = value; }
        }

        public Tile TileOnRightHandSide
        {
            get { return _tileOnRightHandSide; }
            set { _tileOnRightHandSide = value; }
        }

        public Tile TileOnLeftHandSide
        {
            get { return _tileOnLeftHandSide; }
            set { _tileOnLeftHandSide = value; }
        }

        public Vector2 PositionOfRightHandSideEdge
        {
            get { return _positionOfRightHandSideEdge; }
            set { _positionOfRightHandSideEdge = value; }
        }

        public Vector2 PositionOfLeftHandSideEdge
        {
            get { return _positionOfLeftHandSideEdge; }
            set { _positionOfLeftHandSideEdge = value; }
        }


        public Vector2 PositionOfTileOnLeftHandSide
        {
            get { return _positionOfTileOnLeftHandSide; }
            set { _positionOfTileOnLeftHandSide = value; }
        }

        public Vector2 PositionOfTileOnRightHandSide
        {
            get { return _positionOfTileOnRightHandSide; }
            set { _positionOfTileOnRightHandSide = value; }
        }
        
        public Player PlayerInTurn
        {
            get { return _playerInTurn; }
            set { _playerInTurn = value; }
        }

        public List<Tile> TilesPlayedOnTableList { get; set; } 
        #endregion

        #region Constructors
        public Table()
        {
        }

        public Table(int TableNumber)
            
        {
            this.TableNumber = TableNumber;
            this.TileOnRightHandSide = TileOnRightHandSide;
            this.TileOnLeftHandSide = TileOnLeftHandSide;
            this.RightHandSide = new int();
            this.LeftHandSide = new int();
            this.PositionOfRightHandSideEdge = new Vector2();
            this.PositionOfLeftHandSideEdge = new Vector2();
            this.PositionOfTileOnRightHandSide = new Vector2();
            this.PositionOfTileOnLeftHandSide = new Vector2();
            this.PlayerInTurn = new Player();
            this.TilesPlayedOnTableList = new List<Tile>();

        }
        
        #endregion

        #region Methods/Functions
        

        #endregion

        #region Events
        
        #endregion
    }
}
