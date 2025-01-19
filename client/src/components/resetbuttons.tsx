interface ResetButtonsParams {
  StartFunc: (isWhite: boolean, isTwoPlayer: boolean) => void
}

export default function ResetButtons(params: ResetButtonsParams) {
  const { StartFunc } = params

  return (
    <div className='buttonHolder'>
      <button
        onClick={() => {
          StartFunc(true, false)
        }}
        id='ResetWhite'
        className='resetButton'
      >
        Start Game as White
      </button>

      <button
        onClick={() => {
          StartFunc(false, false)
        }}
        id='ResetBlack'
        className='resetButton'
      >
        Start Game as Black
      </button>

      <button
        onClick={() => {
          StartFunc(true, true)
        }}
        id='ResetTwoPlayer'
        className='resetButton'
      >
        Two Players
      </button>
    </div>
  )
}
