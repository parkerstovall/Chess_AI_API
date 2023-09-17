import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';



let api = new GeneratedAPI("");

if(window.location.href.includes("localhost")) {
  api = new GeneratedAPI("//localhost:5000");
}

function App() {
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameID, setGameID] = useState<string>("");
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
    
    const interval = setInterval(() => {
      api.startGame().then((gameStart) => {
        clearInterval(interval);
        setGameID(gameStart.gameID);
        setBoard(gameStart.board);
        return gameStart.gameID;
      }).catch((err) => { 
        console.error("Failed to communicate with server", err);
      });
    }, 250);
  }

  function StartPing(lGameID : string) {
    const interval = setInterval(() => {
        api.ping(lGameID).then((isValid) => {
          if(!isValid) {
            clearInterval(interval);
          }
        }).catch((err) => { 
          console.error("Failed to communicate with server", err);
        });
      }, 1000);
  }

  useEffect(() => {
    document.title = "Chess";
    LoadBoard();
  }, []);

  useEffect(() => {
    if(gameID !== "") {
      StartPing(gameID); 
    }
  }, [gameID])

  return (
    <>
      <Board board={board} clickFunc={BoardSquareClick} setGameOver={setGameOver} />
      <button onClick={() => {LoadBoard()}} id="ResetButton">Reset Game</button>
    </>
  );
}

export default App;