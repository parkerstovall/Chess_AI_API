"use client";

import { BoardDisplay, ClickReturn } from "src/GeneratedAPI";
import React, { useState } from "react";
import Board from "src/components/chess/board";
import ResetButtons from "src/components/chess/resetbuttons";

//let api = new GeneratedAPI(process.env.NEXT_PUBLIC_API_URL);

export default function App() {
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameOver, setGameOver] = useState<boolean>(false);
  const [isCompTurn, setIsCompTurn] = useState<boolean>(false);
  const [isWhite, setIsWhite] = useState<boolean>(true);

  function BoardSquareClick(col: number, row: number) {
    if (gameOver || isCompTurn) {
      return;
    }

    fetch(`http://localhost:5000/api/v1/game/click?row=${row}&col=${col}`, {
      mode: "cors",
      method: "POST",
      credentials: "include",
      headers: {
        "Access-Control-Allow-Origin": "*",
      },
    })
      .then((res) => {
        if (res.ok) {
          return res.json();
        }
      })
      .then((clickReturn: ClickReturn) => {
        setBoard(clickReturn.board);

        if (clickReturn.moved) {
          setIsCompTurn(true);
          fetch(`http://localhost:5000/api/v1/game/compMove`, {
            mode: "cors",
            method: "POST",
            credentials: "include",
            headers: {
              "Access-Control-Allow-Origin": "*",
            },
          })
            .then((res) => {
              if (res.ok) {
                return res.json();
              }
            })
            .then((board: BoardDisplay) => {
              setBoard(board);
              setIsCompTurn(false);
            });
        }
      });

    // api.click(col, row).then((clickReturn) => {
    //   setBoard(clickReturn.board);

    //   if (clickReturn.moved) {
    //     setIsCompTurn(true);
    //     api.compMove().then((board) => {
    //       setBoard(board);
    //       setIsCompTurn(false);
    //     });
    //   }
    // });
  }

  function Start(isWhite: boolean) {
    fetch(`http://localhost:5000/api/v1/game/startGame?isWhite=${isWhite}`, {
      mode: "cors",
      method: "POST",
      credentials: "include",
      headers: {
        "Access-Control-Allow-Origin": "*",
      },
    })
      .then((res) => {
        if (res.ok) {
          return res.json();
        }
      })
      .then((board: BoardDisplay) => {
        setIsWhite(isWhite);
        setBoard(board);

        if (!isWhite) {
          setIsCompTurn(true);
          fetch(`http://localhost:5000/api/v1/game/compMove`, {
            mode: "cors",
            method: "POST",
            credentials: "include",
            headers: {
              "Access-Control-Allow-Origin": "*",
            },
          })
            .then((res) => {
              if (res.ok) {
                return res.json();
              }
            })
            .then((board: BoardDisplay) => {
              setBoard(board);
              setIsCompTurn(false);
            });
        }
      });

    // api
    //   .startGame(false)
    //   .then((board) => {
    //     setBoard(board);
    //   })
    //   .catch((ex) => {
    //     console.log(ex);
    //   });
  }

  return (
    <>
      <Board
        board={board}
        isWhite={isWhite}
        clickFunc={BoardSquareClick}
        setGameOver={setGameOver}
      />
      <ResetButtons StartFunc={Start} />
    </>
  );
}
