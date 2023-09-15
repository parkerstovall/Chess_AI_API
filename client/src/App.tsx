import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';

const api = new GeneratedAPI("");

function App() {
  const [whiteDisplay, setWhiteDisplay] = useState<boolean>(false);
  const [blackDisplay, setBlackDisplay] = useState<boolean>(false);
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameID, setGameID] = useState<number>(-1);
  const [gameOver, setGameOver] = useState<boolean>(false);

  function BoardSquareClick(col: number, row: number) {
    if(gameOver) {
      return;
    }

    document.getElementById("Root")?.classList.add("loading");
    
    api.click(gameID, col, row).then((board) => {
      document.getElementById("Root")?.classList.remove("loading");
      setBoard(board);
    });
  }

  function LoadBoard() {
    
    setTimeout(() => {
      api.startGame().then((gameStart) => {
        setWhiteDisplay(false);
        setBlackDisplay(false);
        setGameID(gameStart.gameID);
        setBoard(gameStart.board);
      }).catch((err) => { 
        console.error("Failed to communicate with server", err);
      });
    }, 100);
  }

  useEffect(() => {
    LoadBoard();
  }, []);

  return (
    <>
      <button onClick={() => {LoadBoard()}} id="ResetButton">Reset Game</button>
      <div className="pieceDisplay" style={{display: whiteDisplay ? "block" : "none"}} id="WhiteDisplay"></div>
      <Board board={board} clickFunc={BoardSquareClick} setGameOver={setGameOver} />
      <div className="pieceDisplay" style={{display: blackDisplay ? "block" : "none"}} id="BlackDisplay"></div>
    </>
  );
}

export default App;