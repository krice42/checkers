using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour {
    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private Piece selectedPiece;

    private List<Piece> forcedMovePieces;

    private bool isWhiteTurn;
    public bool isWhitePiece;
    private bool hasKilled;

    private void Start() {
        GenerateBoard();
        forcedMovePieces = new List<Piece>();
        isWhiteTurn = true;
    }

    private void Update() {
        UpdateMouseOver();

        if ((isWhitePiece) ? isWhiteTurn : !isWhiteTurn) {
            // check for turns
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }

            // select a piece with mouse button down
            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }

            // trying moving the piece when letting go of the mouse button
            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }
    private void UpdateMouseOver() {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
    private void UpdatePieceDrag(Piece p) {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board"))) {
            p.transform.position = hit.point + Vector3.up;

        }
    }

    private List<Piece> ScanForPossibleMoves() {
        forcedMovePieces = new List<Piece>();

        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (pieces[i, j] != null && pieces[i, j].isWhitePiece == isWhiteTurn) {
                    if (pieces[i, j].IsForcedToMove(pieces, i, j)) {
                        forcedMovePieces.Add(pieces[i, j]);
                    }
                }
            }
        }
        return forcedMovePieces;
    }
    private List<Piece> ScanForPossibleMoves(Piece p, int x, int y) {
        forcedMovePieces = new List<Piece>();

        if (pieces[x, y].IsForcedToMove(pieces, x, y)) {
            forcedMovePieces.Add(pieces[x, y]);
        }

        return forcedMovePieces;
    }
    private void SelectPiece(int x, int y) {
        // check if on board
        if (x < 0 || x >= 8 || y < 0 || y >= 8) {
            return;
        }
        Piece p = pieces[x, y];
        if (p != null && p.isWhitePiece == isWhitePiece) {
            if (forcedMovePieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else {
                if (forcedMovePieces.Find(fp => fp == p) == null) {
                    return;
                }

                selectedPiece = p;
                startDrag = mouseOver;
            }
        } 
    }
    private void TryMove(int startX, int startY, int endX, int endY) {
        forcedMovePieces = ScanForPossibleMoves();

        startDrag = new Vector2(startX, startY);
        endDrag = new Vector2(endX, endY);
        selectedPiece = pieces[startX, startY];

        // check out of bounds
        if (endX < 0 || endX >= 8 || endY < 0 || endY >= 8) {
            if (selectedPiece != null) {
                MovePiece(selectedPiece, startX, startY);
            }
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }
        // check if piece is selected
        if (selectedPiece != null) {
            if (endDrag == startDrag) {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
            // check if the move is valid
            if (selectedPiece.ValidateMove(pieces, startX, startY, endX, endY))
            {
                // check if we jump anything
                if (Mathf.Abs(endX - startX) == 2)
                {
                    Piece p = pieces[(startX + endX) / 2, (startY + endY) / 2];

                    if (p != null)
                    {
                        pieces[(startX + endX) / 2, (startY + endY) / 2] = null;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                if (forcedMovePieces.Count != 0 && !hasKilled) {
                    MovePiece(selectedPiece, startX, startY);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[endX, endY] = selectedPiece;
                pieces[startX, startY] = null;
                MovePiece(selectedPiece, endX, endY);

                EndTurn();
            }
            else {
                MovePiece(selectedPiece, startX, startY);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }
    private void EndTurn() {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        // Turn piece into King
        if (selectedPiece != null) {
            if (selectedPiece.isWhitePiece && !selectedPiece.isKing && y == 7) {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180f);
            }
            else if(!selectedPiece.isWhitePiece && !selectedPiece.isKing && y == 0){
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180f);
            }
        }

        selectedPiece = null;
        startDrag = Vector2.zero;

        if (ScanForPossibleMoves(selectedPiece, x, y).Count != 0 && hasKilled) {
            return;
        }

        isWhiteTurn = !isWhiteTurn;
        isWhitePiece = !isWhitePiece;
        hasKilled = false;
        CheckVictory();      
    }
    private void CheckVictory() {
        var ps = FindObjectsOfType<Piece>();
        bool hasWhite = false;
        bool hasBlack = false;

        for (int i = 0; i < ps.Length; i++) {
            if (ps[i].isWhitePiece)
            {
                hasWhite = true;
            }
            else {
                hasBlack = true;
            }
        }

        if (!hasWhite) {
            Victory(false);
        }
        if (!hasBlack) {
            Victory(true);
        }
    }
    private void Victory(bool isWhite) {
        if (isWhite)
        {
            Debug.Log("WHITE TEAMS WINS");
        }
        else {
            Debug.Log("BLACK TEAM HAS WON");
        }
    }
    private void GenerateBoard() {
        // White Team Generation
        for (int y = 0; y < 3; y++) {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x+=2) {
                GeneratePiece((oddRow) ? x : x+1, y);
            }
        }
        // Black Team Generation
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }
    private void GeneratePiece(int x, int y) {
        bool isPieceWhite = (y > 3) ? false : true;
        
        // Place White pieces
        if (isPieceWhite)
        {
            GameObject go = Instantiate(whitePiecePrefab) as GameObject;
            go.transform.SetParent(transform);
            Piece p = go.GetComponent<Piece>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
        else { // Place black pieces with proper rotation
            GameObject go = Instantiate(blackPiecePrefab, transform.position, transform.rotation * Quaternion.Euler(-90f, 0f, 180f)) as GameObject;
            go.transform.SetParent(transform);
            Piece p = go.GetComponent<Piece>();
            pieces[x, y] = p;
            MovePiece(p, x, y);
        }
    }
    private void MovePiece(Piece p, int x, int y) {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}
