using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public bool isWhitePiece;
    public bool isKing;

    public bool ValidateMove(Piece[,] board, int startX, int startY, int endX, int endY) {
        // check if trying to move on top of another piece
        if (board[endX, endY] != null)
        {
            return false;
        }

        int deltaMoveX = Mathf.Abs(startX - endX);
        int deltaMoveY = endY - startY;

        // White pieces
        if (isWhitePiece || isKing) {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == 1)
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2) {
                if (deltaMoveY == 2) {
                    Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
                    if(p != null && p.isWhitePiece != isWhitePiece){
                        return true;
                    }
                }
            }
        }

        // Black pieces
        if (!isWhitePiece || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(startX + endX) / 2, (startY + endY) / 2];
                    if (p != null && p.isWhitePiece != isWhitePiece)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
        // check if there is a required jump
        // check if not on the board
    }
    public bool IsForcedToMove(Piece[,] board, int x, int y) {
        if (isWhitePiece || isKing)
        {
            // check TL
            if (x >= 2 && y <= 5)
            {
                Piece p = board[x - 1, y + 1];
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // check TR
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        if(!isWhitePiece || isKing) {
            // check BL
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            // check BR
            if (x <= 5 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

