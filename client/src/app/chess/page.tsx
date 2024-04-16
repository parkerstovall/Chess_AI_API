"use client";

import { BoardDisplay, ClickReturn, GeneratedAPI, SavedGameResult } from "src/GeneratedAPI";
import React, { useState, useEffect } from "react";
import Board from "src/components/chess/board";
import ResetButtons from "src/components/chess/resetbuttons";

export default function App() {
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameOver, setGameOver] = useState<boolean>(false);
  const [isCompTurn, setIsCompTurn] = useState<boolean>(false);
  const [isTwoPlayer, setIsTwoPlayer] = useState<boolean>(false);
  const [isWhite, setIsWhite] = useState<boolean>(true);

  function ComputerMove() {
    setIsCompTurn(true);
    new GeneratedAPI("").compMove().then((board: BoardDisplay) => {
      setBoard(board);
      setIsCompTurn(false);
    });
  }

  function BoardSquareClick(col: number, row: number) {
    if (gameOver || (isCompTurn && !isTwoPlayer)) {
      return;
    }

    new GeneratedAPI("").click(row, col).then((clickReturn: ClickReturn) => {
      setBoard(clickReturn.board);

      if (clickReturn.moved && !isTwoPlayer) {
        ComputerMove();
      }
    });
  }

  function Start(isWhite: boolean, isTwoPlayer: boolean) {
    setIsTwoPlayer(isTwoPlayer);
    new GeneratedAPI("").startGame(isWhite, isTwoPlayer).then((board: BoardDisplay) => {
      setIsWhite(isWhite);
      setBoard(board);

      if (!isWhite && !isTwoPlayer) {
        ComputerMove();
      }
    });
  }

  function tryGetSavedGame() {
    new GeneratedAPI("").tryGetSavedGame().then((resp: SavedGameResult) => {
      if (!resp.boardDisplay) {
        return;
      }

      setIsWhite(resp.isPlayerWhite ?? true);
      setBoard(resp.boardDisplay);
      setIsTwoPlayer(resp.isTwoPlayer ?? false);

      if (resp.isPlayerWhite && !resp.isTwoPlayer) {
        ComputerMove();
      }
    });
  }

  useEffect(() => {
    tryGetSavedGame();
  }, []);

  return (
    <>
      <Board board={board} isWhite={isWhite} clickFunc={BoardSquareClick} setGameOver={setGameOver} />
      <ResetButtons StartFunc={Start} />
    </>
  );
}
