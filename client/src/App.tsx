import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';

function App() {
  const [whiteDisplay, setWhiteDisplay] = useState<boolean>(false);
  const [blackDisplay, setBlackDisplay] = useState<boolean>(false);
  const [boardArgs, setBoardArgs] = useState<BoardDisplay>();

  function LoadInitialBoard() {
    const api = new GeneratedAPI("http://localhost:4000");
    
    api.getBoard(0).then((board) => {
      setBoardArgs(board);
    });
  }

  function BoardSquareClick(col: number, row: number) {
    console.log(col, row);
  }

  useEffect(() => {
    LoadInitialBoard();
  }, []);

  return (
    <>
      <div className="pieceDisplay" style={{display: whiteDisplay ? "block" : "none"}} id="WhiteDisplay"></div>
      <Board board={boardArgs} clickFunc={BoardSquareClick} />
      <div className="pieceDisplay" style={{display: blackDisplay ? "block" : "none"}} id="BlackDisplay"></div>
    </>
  );
}

export default App;