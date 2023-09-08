using System;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPieceHandler : MonoBehaviour
    {
        [SerializeField] public int xBoard, yBoard;
        [SerializeField] public string ChessPieceName;
        [SerializeField] public string ChessPieceColor;

        public void CreateChessPiece(string chesspiecename, string chesspiececolor, int xboard, int yboard, Sprite sprite = null)
        {
            ChessPieceName = chesspiecename;
            ChessPieceColor = chesspiececolor;
            xBoard = xboard;
            yBoard = yboard;
            this.gameObject.transform.name = chesspiecename + "_" + chesspiececolor;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(xBoard, yBoard).transform.position;
        }
        public string GetChessPieceName()
        {
            return ChessPieceName;
        }
        public string GetChessPieceColor()
        {
            return ChessPieceColor;
        }
        public Sprite GetChessPieceSprite()
        {
            return this.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        public int GetXBoard()
        {
            return xBoard;
        }
        public int GetYBoard()
        {
            return yBoard;
        }
        public void SetChessPiece(string chesspiecename, string chesspiececolor, Sprite sprite)
        {
            ChessPieceName = chesspiecename;
            ChessPieceColor = chesspiececolor;
            this.gameObject.transform.name = chesspiecename + "_" + chesspiececolor;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }
        void OnMouseUp()
        {
            ChessBoardPlacementHandler.Instance.ChessPiecePressed(xBoard, yBoard);
        }
    }
}