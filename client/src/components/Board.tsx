import { useEffect } from 'react';
import { BoardDisplay } from '../GeneratedAPI.ts';


interface BoardProps {
    board: BoardDisplay | undefined;
    clickFunc: Function;
    setGameOver: Function;
}

export default function Board(props: BoardProps)  {

    const { board, clickFunc, setGameOver } = props;
    
    useEffect(() => {
        let breakLoop = false;
        for(const row of board?.rows ?? []) {
            for (const square of row.squares ?? []) {
                if(square.cssClass?.includes("inCheckMate")) {
                    let color = "White";

                    if(square.cssClass.includes("whiteKing")) {
                        color = "Black";
                    }

                    alert(`Checkmate, ${color} wins!`);
                    setGameOver(true);
                    breakLoop = true;
                    break;
                }
            }

            if(breakLoop) {
                break;
            }
        }
    }, [board, setGameOver]);

    return (
        <div id="GameBoard" className="GameBoard">
            {
                board?.rows?.map((row) => {
                    return row.squares?.map((square) => {
                        const id = `square-${square.col}-${square.row}`;
                        return <div onClick={() => {clickFunc(square.col, square.row)}} key={id} className={square.cssClass} id={id}></div>
                    });
                })
            }
        </div>
    )
}