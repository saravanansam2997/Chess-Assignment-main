using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ChessPieceData", menuName = "ScriptableObjects/ChessPieceScriptableObject")]
public class ChessPieceScriptableObject : ScriptableObject
{
   public List<ChessPieceInfo> ChessPieceInfos = new List<ChessPieceInfo>();
}
[System.Serializable]
public class ChessPieceInfo
{
   public string ChessPieceName;
   public Sprite ChessPieceSprite;
   public string ChessPieceColor;
   public List<ChessPieceArrangingPlace> ChessPieceArrangePlaces;
}
[System.Serializable]
public class ChessPieceArrangingPlace
{
   public int xBoard;
   public int yBoard;

}