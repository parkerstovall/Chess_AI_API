import { test, expect } from '@playwright/test'

test('Index has Title', async ({ page }) => {
  await page.goto('/')
  await expect(page).toHaveTitle('Chess AI API')
})

test('Can Start Game as White', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  let boardBtnCount = await page.locator('.boardBtn').count()
  expect(boardBtnCount).toBe(0)

  await page.locator('#ResetWhite').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  boardBtnCount = await page.locator('.boardBtn').count()
  expect(boardBtnCount).toBe(64)
})

test('Can Start Game as Black', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  const compMove = page.waitForResponse(
    'https://localhost:5000/api/v1/game/compMove*',
  )

  let boardBtnCount = await page.locator('.boardBtn').count()
  expect(boardBtnCount).toBe(0)

  await page.locator('#ResetBlack').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  boardBtnCount = await page.locator('.boardBtn').count()
  expect(boardBtnCount).toBe(64)

  await compMove.then((response) => {
    expect(response.status()).toBe(200)
  })
})

test('Clicking on a piece selects it', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  await page.locator('#ResetWhite').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  const clickResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )
  await page.locator('.whitePawn').nth(0).click()
  await clickResp.then((response) => {
    expect(response.status()).toBe(200)
  })

  const selectedSquares = await page.locator('.selected').count()
  expect(selectedSquares).toBe(1)
})

test('Clicking on a piece highlights its moves', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  await page.locator('#ResetWhite').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  const clickResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )
  await page.locator('.whitePawn').nth(0).click()
  await clickResp.then((response) => {
    expect(response.status()).toBe(200)
  })

  const highlightedSquares = await page.locator('.highlighted').count()
  expect(highlightedSquares).toBeGreaterThan(0)
})

test('Clicking on a highlighted square moves the piece', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  await page.locator('#ResetWhite').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  const clickResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )

  await page.locator('.whitePawn').nth(0).click()
  await clickResp.then((response) => {
    expect(response.status()).toBe(200)
  })

  const highlightedSquare = page.locator('.highlighted').nth(0)
  const highlightedId = await highlightedSquare.getAttribute('id')
  const moveResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )
  await highlightedSquare.click()
  await moveResp.then((response) => {
    expect(response.status()).toBe(200)
  })

  const selectedSquares = await page.locator('.selected').count()
  expect(selectedSquares).toBe(0)

  const highlightedSquares = await page.locator('.highlighted').count()
  expect(highlightedSquares).toBe(0)

  expect(await page.locator('.whitePawn').nth(0).getAttribute('id')).toBe(
    highlightedId,
  )
})

test('After moving a piece, the computer will move', async ({ page }) => {
  await page.goto('/')
  const startGame = page.waitForResponse(
    'https://localhost:5000/api/v1/game/startGame*',
  )

  await page.locator('#ResetWhite').click()
  await startGame.then((response) => {
    expect(response.status()).toBe(200)
  })

  const clickResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )

  await page.locator('.whitePawn').nth(0).click()
  await clickResp.then((response) => {
    expect(response.status()).toBe(200)
  })

  const highlightedSquare = page.locator('.highlighted').nth(0)
  const highlightedId = await highlightedSquare.getAttribute('id')
  const moveResp = page.waitForResponse(
    'https://localhost:5000/api/v1/game/click*',
  )

  const compMove = page.waitForResponse(
    'https://localhost:5000/api/v1/game/compMove*',
  )

  await highlightedSquare.click()
  let boardHtml = ''
  await moveResp.then(async (response) => {
    boardHtml = await page.locator('#GameBoard').innerHTML()
    expect(response.status()).toBe(200)
  })

  const selectedSquares = await page.locator('.selected').count()
  expect(selectedSquares).toBe(0)

  const highlightedSquares = await page.locator('.highlighted').count()
  expect(highlightedSquares).toBe(0)

  expect(await page.locator('.whitePawn').nth(0).getAttribute('id')).toBe(
    highlightedId,
  )

  await compMove.then(async (response) => {
    expect(response.status()).toBe(200)
    const newBoardHtml = await page.locator('#GameBoard').innerHTML()
    expect(newBoardHtml).not.toBe(boardHtml)
  })
})
