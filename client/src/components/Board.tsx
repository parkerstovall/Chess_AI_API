import { MouseEventHandler } from 'react';
import { BoardDisplay } from '../GeneratedAPI.ts';


interface BoardProps {
    board: BoardDisplay | undefined;
    clickFunc: Function;
}

export default function Board(props: BoardProps)  {
    const { board, clickFunc } = props;
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