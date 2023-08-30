import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';

const api = new GeneratedAPI("http://localhost:4000");

function App() {
  const [whiteDisplay, setWhiteDisplay] = useState<boolean>(false);
  const [blackDisplay, setBlackDisplay] = useState<boolean>(false);
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameID, setGameID] = useState<number>(-1);

  function BoardSquareClick(col: number, row: number) {
    document.getElementById("Root")?.classList.add("loading");
    
    api.click(gameID, col, row).then((board) => {
      document.getElementById("Root")?.classList.remove("loading");
      setBoard(board);
    });
  }

  function LoadInitialBoard() {
    setWhiteDisplay(false);
    setBlackDisplay(false);

    api.startGame().then((gameStart) => {
      setGameID(gameStart.gameID);
      setBoard(gameStart.board);
    });

  }

  useEffect(() => {
    LoadInitialBoard();
  }, []);

  return (
    <>
      <div className="pieceDisplay" style={{display: whiteDisplay ? "block" : "none"}} id="WhiteDisplay"></div>
      <Board board={board} clickFunc={BoardSquareClick} />
      <div className="pieceDisplay" style={{display: blackDisplay ? "block" : "none"}} id="BlackDisplay"></div>
    </>
  );
}

export default App;