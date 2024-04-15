"use client";

import { BoardDisplay, ClickReturn, SavedGameResult } from "src/GeneratedAPI";
import React, { useState, useEffect } from "react";
import Board from "src/components/chess/board";
import ResetButtons from "src/components/chess/resetbuttons";

//let api = new GeneratedAPI(process.env.NEXT_PUBLIC_API_URL);

export default function App() {
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameOver, setGameOver] = useState<boolean>(false);
  const [isCompTurn, setIsCompTurn] = useState<boolean>(false);
  const [isTwoPlayer, setIsTwoPlayer] = useState<boolean>(false);
  const [isWhite, setIsWhite] = useState<boolean>(true);

  function BoardSquareClick(col: number, row: number) {
    if (gameOver || (isCompTurn && !isTwoPlayer)) {
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

        if (clickReturn.moved && !isTwoPlayer) {
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

  function Start(isWhite: boolean, isTwoPlayer: boolean) {
    setIsTwoPlayer(isTwoPlayer);
    fetch(
      `http://localhost:5000/api/v1/game/startGame?isWhite=${isWhite}&isTwoPlayer=${isTwoPlayer}`,
      {
        mode: "cors",
        method: "POST",
        credentials: "include",
        headers: {
          "Access-Control-Allow-Origin": "*",
        },
      },
    )
      .then((res) => {
        if (res.ok) {
          return res.json();
        }
      })
      .then((board: BoardDisplay) => {
        setIsWhite(isWhite);
        setBoard(board);

        if (!isWhite && !isTwoPlayer) {
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

  function tryGetSavedGame() {
    fetch(`http://localhost:5000/api/v1/game/tryGetSavedGame`, {
      mode: "cors",
      method: "POST",
      credentials: "include",
      headers: {
        "Access-Control-Allow-Origin": "*",
      },
    })
      .then(async (res) => {
        if (res.ok) {
          return res.text();
        }
      })
      .then((strResp: string | undefined) => {
        if (!strResp || strResp.length === 0) {
          return;
        }

        const resp: SavedGameResult = JSON.parse(strResp);
        setIsWhite(resp.isPlayerWhite ?? true);
        setBoard(resp.boardDisplay);
        setIsTwoPlayer(resp.isTwoPlayer ?? false);

        if (resp.isPlayerWhite && !resp.isTwoPlayer) {
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
  }

  useEffect(() => {
    tryGetSavedGame();
  }, []);

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
