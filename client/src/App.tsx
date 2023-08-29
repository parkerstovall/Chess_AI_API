import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';

function App() {
  const [whiteDisplay, setWhiteDisplay] = useState<boolean>(false);
  const [blackDisplay, setBlackDisplay] = useState<boolean>(false);
  const [boardArgs, setBoardArgs] = useState<BoardDisplay>();
  const [gameID, setGameID] = useState<number>(-1);

  function LoadInitialBoard() {
    const api = new GeneratedAPI("http://localhost:4000");
    
    api.startGame().then((lGameID) => {
      setGameID(lGameID);

      api.getBoard(lGameID).then((board) => {
        setBoardArgs(board);
        setWhiteDisplay(false);
        setBlackDisplay(false);
      });
      
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