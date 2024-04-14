import { useEffect } from "react";
import { BoardDisplay } from "../../GeneratedAPI";

interface BoardProps {
  board: BoardDisplay | undefined;
  isWhite: boolean;
  clickFunc: Function;
  setGameOver: Function;
}

export default function Board(props: BoardProps) {
  const { board, isWhite, clickFunc, setGameOver } = props;

  useEffect(() => {
    let breakLoop = false;
    for (const row of board?.rows ?? []) {
      for (const square of row.squares ?? []) {
        if (square.cssClass?.includes("inCheckMate")) {
          let color = "White";

          if (square.cssClass.includes("whiteKing")) {
            color = "Black";
          }

          alert(`Checkmate, ${color} wins!`);
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
                clickFunc(square.col, square.row);
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
