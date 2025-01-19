import { useEffect } from "react";
import { BoardDisplay } from "../api/GeneratedAPI";

interface BoardProps {
  board: BoardDisplay | undefined;
  isWhite: boolean;
  clickFunc: (col: number, row: number) => void;
  setGameOver: (isGameOver: boolean) => void;
}

export default function Board(props: BoardProps) {
  const { board, isWhite, clickFunc, setGameOver } = props;

  useEffect(() => {
    let breakLoop = false;
    for (const row of board?.rows ?? []) {
      for (const square of row.squares ?? []) {
        if (square.cssClass?.includes("inCheckMate")) {
          setGameOver(true);
          breakLoop = true;
          break;
        }
      }

      if (breakLoop) {
        break;
      }
    }
  }, [board, setGameOver]);

  let className = "GameBoard";
  if (!isWhite) {
    className += " rotated";
  }

  return (
    <div id="GameBoard" className={className}>
      {board?.rows?.map((row) => {
        return row.squares?.map((square) => {
          const id = `square-${square.col}-${square.row}`;
          return (
            <div
              onClick={() => {
                if (square.col !== undefined && square.row !== undefined) {
                  clickFunc(square.col, square.row);
                }
              }}
              key={id}
              className={square.cssClass}
              id={id}
            ></div>
          );
        });
      })}
    </div>
  );
}
