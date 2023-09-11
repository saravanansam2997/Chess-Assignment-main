using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using Chess.Scripts.Core;
public sealed class ChessBoardPlacementHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _rowsArray;
    [SerializeField] private ChessPieceScriptableObject ChessPieceScriptableObject;
    [SerializeField] private GameObject _highlightPrefab;
    [SerializeField] private GameObject _chesspiecePrefab;
    [SerializeField] private GameObject _playerboardParent;
    [SerializeField] private string _currentPlayerName, _currentPlayerColor = "";
    [SerializeField] private int _totalRow, _totalColumn;
    [SerializeField] private TMP_Text _statusTxt;
    [SerializeField] private ChessPieceHandler[,] _chessPieceHandler;
    [SerializeField] private GameObject[,] _chessBoard;
    private string[] playerColor = { "Black", "White" };
    private int _playerPieceXBoard, _playerPieceYBoard;
    private bool IsWin, IsEnabled;
    internal static ChessBoardPlacementHandler Instance;

    private void Awake()
    {
        Instance = this;
        GenerateArray();
        GenerateChessPieces();
    }

    private void GenerateArray()
    {
        _chessBoard = new GameObject[_totalRow, _totalColumn];
        for (var i = 0; i < _totalRow; i++)
        {
            for (var j = 0; j < _totalColumn; j++)
            {
                _chessBoard[j, i] = _rowsArray[i].transform.GetChild(j).gameObject;
            }
        }
    }
    public void GenerateChessPieces()
    {
        _chessPieceHandler = new ChessPieceHandler[_totalRow, _totalColumn];
        foreach (var chesspiecedata in ChessPieceScriptableObject.ChessPieceInfos)
        {
            foreach (var piecedata in chesspiecedata.ChessPieceArrangePlaces)
            {
                GameObject _chesspieceobj = Instantiate(_chesspiecePrefab, _playerboardParent.transform.position, Quaternion.identity, _playerboardParent.transform);
                _chesspieceobj.GetComponent<ChessPieceHandler>().CreateChessPiece(chesspiecedata.ChessPieceName, chesspiecedata.ChessPieceColor, piecedata.xBoard, piecedata.yBoard, chesspiecedata.ChessPieceSprite);
                _chessPieceHandler[piecedata.xBoard, piecedata.yBoard] = _chesspieceobj.GetComponent<ChessPieceHandler>();
            }
        }
        for (var i = 0; i < _totalRow; i++)
        {
            for (var j = 0; j < _totalColumn; j++)
            {
                if (_chessPieceHandler[i, j] == null)
                {
                    GameObject _chesspieceobj = Instantiate(_chesspiecePrefab, _playerboardParent.transform.position, Quaternion.identity, _playerboardParent.transform);
                    _chesspieceobj.GetComponent<ChessPieceHandler>().CreateChessPiece("Empty", "None", i, j, null);
                    _chessPieceHandler[i, j] = _chesspieceobj.GetComponent<ChessPieceHandler>();
                }
            }
        }

    }
    internal GameObject GetTile(int row, int column)
    {
        try
        {
            return _chessBoard[row, column];
        }
        catch (Exception)
        {
            Debug.LogError("Invalid row or column.");
            return null;
        }
    }

    internal void Highlight(int row, int column, bool isEnemy = false)
    {
        var tile = GetTile(row, column).transform;
        if (tile == null)
        {
            Debug.LogError("Invalid row or column.");
            return;
        }
        if (_currentPlayerColor != _chessPieceHandler[row, column].GetChessPieceColor())
        {
            GameObject Highlightobj = Instantiate(_highlightPrefab, tile.transform.position, Quaternion.identity, tile.transform);
            if (isEnemy)
            {
                Highlightobj.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                Highlightobj.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    internal void ClearHighlights()
    {
        for (var i = 0; i < _totalRow; i++)
        {
            for (var j = 0; j < _totalColumn; j++)
            {
                var tile = GetTile(i, j);
                if (tile.transform.childCount <= 0) continue;
                foreach (Transform childTransform in tile.transform)
                {
                    Destroy(childTransform.gameObject);
                }
            }
        }
    }
    public void ChessPiecePressed(int xboard, int yboard)
    {
        if (IsEnabled)
        {
            if (_currentPlayerColor == _chessPieceHandler[xboard, yboard].GetChessPieceColor())
            {
                if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "King")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    ChessSurroundMove(xboard, yboard);
                }

                else if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "Queen")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    ChessPieceMovesHighlight(1, 0);
                    ChessPieceMovesHighlight(0, 1);
                    ChessPieceMovesHighlight(1, 1);
                    ChessPieceMovesHighlight(-1, 0);
                    ChessPieceMovesHighlight(0, -1);
                    ChessPieceMovesHighlight(-1, -1);
                    ChessPieceMovesHighlight(-1, 1);
                    ChessPieceMovesHighlight(1, -1);
                }
                else if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "Bishop")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    ChessPieceMovesHighlight(1, 1);
                    ChessPieceMovesHighlight(1, -1);
                    ChessPieceMovesHighlight(-1, 1);
                    ChessPieceMovesHighlight(-1, -1);
                }
                else if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "Rook")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    ChessPieceMovesHighlight(1, 0);
                    ChessPieceMovesHighlight(0, 1);
                    ChessPieceMovesHighlight(-1, 0);
                    ChessPieceMovesHighlight(0, -1);
                }
                else if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "Knight")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    ChessLMove(xboard, yboard);
                }
                else if (_chessPieceHandler[xboard, yboard].GetChessPieceName() == "Pawn")
                {
                    ClearHighlights();
                    _playerPieceXBoard = xboard;
                    _playerPieceYBoard = yboard;
                    if (_chessPieceHandler[xboard, yboard].GetChessPieceColor() == "Black")
                    {
                        ChessPawnMove(xboard, yboard - 1);
                    }
                    else if (_chessPieceHandler[xboard, yboard].GetChessPieceColor() == "White")
                    {
                        ChessPawnMove(xboard, yboard + 1);
                    }

                }

                else
                {
                    var tile = GetTile(xboard, yboard).gameObject;
                    if (tile.transform.childCount <= 0)
                    {
                        Debug.LogError("Invaild Move");
                        return;
                    }
                    ChessPieceMoves(xboard, yboard, IsEnemy(xboard, yboard));
                }
            }
            else
            {
                var tile = GetTile(xboard, yboard).gameObject;
                if (tile.transform.childCount <= 0)
                {
                    Debug.LogError("Invaild Move");
                    return;
                }
                ChessPieceMoves(xboard, yboard, IsEnemy(xboard, yboard));
            }
        }
    }
    public void ChessPieceMoves(int x, int y, bool isEnemy = false)
    {
        IsEnabled = false;
        if (_chessPieceHandler[x, y].GetChessPieceName() == "King")
        {
            IsWin = true;
        }
        _chessPieceHandler[x, y].SetChessPiece(_chessPieceHandler[_playerPieceXBoard, _playerPieceYBoard].GetChessPieceName(), _chessPieceHandler[_playerPieceXBoard, _playerPieceYBoard].GetChessPieceColor(), _chessPieceHandler[_playerPieceXBoard, _playerPieceYBoard].GetChessPieceSprite());
        _chessPieceHandler[_playerPieceXBoard, _playerPieceYBoard].SetChessPiece("Empty", "None", null);
        ClearHighlights();
        _playerPieceXBoard = _playerPieceYBoard = 0;
        if (IsWin)
        {
            StartCoroutine(GameReload());
            return;
        }
        IsEnabled = true;
        NextPlayerTurn();
    }
    public void ChessLMove(int x, int y)
    {
        ChessPiecePointMove(x + 1, y + 2);
        ChessPiecePointMove(x - 1, y + 2);
        ChessPiecePointMove(x + 2, y + 1);
        ChessPiecePointMove(x + 2, y - 1);
        ChessPiecePointMove(x + 1, y - 2);
        ChessPiecePointMove(x - 1, y - 2);
        ChessPiecePointMove(x - 2, y + 1);
        ChessPiecePointMove(x - 2, y - 1);
    }
    public void ChessSurroundMove(int x, int y)
    {
        ChessPiecePointMove(x, y + 1);
        ChessPiecePointMove(x, y - 1);
        ChessPiecePointMove(x - 1, y + 0);
        ChessPiecePointMove(x - 1, y - 1);
        ChessPiecePointMove(x - 1, y + 1);
        ChessPiecePointMove(x + 1, y + 0);
        ChessPiecePointMove(x + 1, y - 1);
        ChessPiecePointMove(x + 1, y + 1);
    }
    public void ChessPiecePointMove(int x, int y)
    {
        if (ChessPositionOnBoard(x, y))
        {
            if (_chessPieceHandler[x, y].GetChessPieceColor() == "None")
            {
                ChessMoveSpawn(x, y, false);
            }
            else if (_chessPieceHandler[x, y].GetChessPieceColor() != _currentPlayerColor
            && _chessPieceHandler[x, y].GetChessPieceColor() != "None")
            {
                ChessMoveSpawn(x, y, true);
            }
        }
    }
    public void ChessPieceMovesHighlight(int xIncrement, int yIncrement)
    {
        int x = _playerPieceXBoard + xIncrement;
        int y = _playerPieceYBoard + yIncrement;
        while (ChessPositionOnBoard(x, y) && _chessPieceHandler[x, y].GetChessPieceName() == "Empty")
        {
            ChessMoveSpawn(x, y, false);
            x += xIncrement;
            y += yIncrement;
        }

        if (ChessPositionOnBoard(x, y) && _chessPieceHandler[x, y].GetChessPieceColor() != _currentPlayerColor
       && _chessPieceHandler[x, y].GetChessPieceColor() != "None")
        {
            ChessMoveSpawn(x, y, true);
        }
    }
    public void ChessPawnMove(int x, int y)
    {
        if (ChessPositionOnBoard(x, y))
        {
            if (_chessPieceHandler[x, y].GetChessPieceColor() == "None")
            {
                ChessMoveSpawn(x, y, false);
            }

            if (ChessPositionOnBoard(x + 1, y) && _chessPieceHandler[x + 1, y].GetChessPieceColor() != _currentPlayerColor
            && _chessPieceHandler[x + 1, y].GetChessPieceColor() != "None")
            {
                ChessMoveSpawn(x + 1, y, true);
            }

            if (ChessPositionOnBoard(x - 1, y) && _chessPieceHandler[x - 1, y].GetChessPieceColor() != _currentPlayerColor
            && _chessPieceHandler[x - 1, y].GetChessPieceColor() != "None")
            {
                ChessMoveSpawn(x - 1, y, true);
            }
        }

    }

    public void ChessMoveSpawn(int matrixX, int matrixY, bool IsAttack = false)
    {
        Highlight(matrixX, matrixY, IsAttack);
    }
    public bool ChessPositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _chessPieceHandler.GetLength(0) || y >= _chessPieceHandler.GetLength(1)) return false;
        return true;
    }
    public bool IsEnemy(int row, int col)
    {
        bool isEnemy = false;
        if (_chessPieceHandler[row, col].GetChessPieceColor() != _currentPlayerName && _chessPieceHandler[row, col].GetChessPieceColor() != "None")
        {
            isEnemy = true;
        }
        return isEnemy;
    }
    private void Start()
    {
        StartCoroutine(PlayGameStart());
    }
    private IEnumerator PlayGameStart()
    {
        _playerPieceXBoard = 0;
        _playerPieceYBoard = 0;
        GetRandomChoosePlayerName();
        IsEnabled = false;
        ChessStatusShow(_currentPlayerColor + " Play Start");
        yield return new WaitForSeconds(1f);
        ChessStatusHide();
        IsEnabled = true;

    }
    private IEnumerator GameReload()
    {
        ChessStatusShow(_currentPlayerColor + " Win" + "....");
        yield return new WaitForSeconds(1f);
        ChessStatusHide();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
    private void ChessStatusShow(string txt)
    {
        _statusTxt.text = txt;
        _statusTxt.gameObject.SetActive(true);
    }
    private void ChessStatusHide()
    {
        _statusTxt.text = "";
        _statusTxt.gameObject.SetActive(false);
    }
    private void NextPlayerTurn()
    {
        if (_currentPlayerColor == "Black")
        {
            _currentPlayerColor = "White";
        }
        else if (_currentPlayerColor == "White")
        {
            _currentPlayerColor = "Black";
        }
    }
    private void GetRandomChoosePlayerName()
    {
        int Rnd = (int)Mathf.Round(Random.Range(0, playerColor.Length));
        _currentPlayerColor = playerColor[Rnd];
    }

}
