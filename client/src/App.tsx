import { useEffect, useState } from 'react';
import { BoardDisplay, GeneratedAPI } from './GeneratedAPI.ts';
import Board from './components/Board.tsx';
import './App.css';

function App() {
  const [whiteDisplay, setWhiteDisplay] = useState<boolean>(false);
  const [blackDisplay, setBlackDisplay] = useState<boolean>(false);
  const [board, setBoard] = useState<BoardDisplay>();
  const [gameID, setGameID] = useState<number>(-1);
  const api = new GeneratedAPI("http://localhost:4000");

  function LoadInitialBoard() {
    
    api.startGame().then((lGameID) => {
      setGameID(lGameID);

      api.getBoard(lGameID).then((board) => {
        setBoard(board);
        setWhiteDisplay(false);
        setBlackDisplay(false);
      });
      
    });

  }

  function GetMoves(col: number, row: number) {
    api.getMoves(gameID, col, row).then((moves) => {
      moves.map((move) => {
        let square = document.getElementById(`square-${move[0]}-${move[1]}`);

        if (square !== null) {
          square.classList.add("highlighted");
        }
      });
    });
  }
  
  function MovePiece(col: number, row: number) {
    api.movePiece(gameID, col, row).then((message) => {
      alert(message);
    });
  }

  function BoardSquareClick(col: number, row: number) {

    const square = document.getElementById(`square-${col}-${row}`);
    let action: Function | null = null;

    if (square !== null) {
      if(square.classList.contains("highlighted")) {
        action = MovePiece;
      }
      else {
        action = GetMoves;
      }
    }

    document.querySelectorAll('.highlighted').forEach((el) => {
      el.classList.remove('highlighted');
    });

    if (action !== null) {
      action(col, row);
    }
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