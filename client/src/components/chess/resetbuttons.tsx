interface ResetButtonsParams {
  StartFunc: (isWhite: boolean) => void;
}

export default function ResetButtons(params: ResetButtonsParams) {
  const { StartFunc } = params;

  return (
    <div className="buttonHolder">
      <button
        onClick={() => {
          StartFunc(true);
        }}
        id="ResetWhite"
        className="resetButton"
      >
        Start Game as White
      </button>
      <button
        onClick={() => {
          StartFunc(false);
        }}
        id="ResetBlack"
        className="resetButton"
      >
        Start Game as Black
      </button>
    </div>
  );
}
