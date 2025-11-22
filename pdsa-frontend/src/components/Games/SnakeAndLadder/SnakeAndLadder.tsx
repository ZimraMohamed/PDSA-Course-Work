import React, { useState, useEffect } from 'react';
import './SnakeAndLadder.css';

interface Player {
  name: string;
  position: number;
}

type ResultState = 'idle' | 'playing' | 'win' | 'lose' | 'draw';

const DEFAULT_PLAYERS: Player[] = [
  { name: 'Player 1', position: 0 },
  { name: 'Player 2', position: 0 },
];

const LOCAL_STORAGE_WINNERS_KEY = 'sal_winners_v1';

const SnakeAndLadder: React.FC = () => {
  const [players, setPlayers] = useState<Player[]>(DEFAULT_PLAYERS);
  const [currentPlayerIndex, setCurrentPlayerIndex] = useState<number>(0);
  const [lastRoll, setLastRoll] = useState<number | null>(null);
  const [message, setMessage] = useState<string>('');

  // Setup options
  const [boardSize, setBoardSize] = useState<number>(10); // N x N
  const [maxThrows, setMaxThrows] = useState<number>(100);
  const [throwCounts, setThrowCounts] = useState<number[]>(() => DEFAULT_PLAYERS.map(() => 0));
  const [gameState, setGameState] = useState<ResultState>('idle');
  const [gameStarted, setGameStarted] = useState<boolean>(false);
  const [winnerIndex, setWinnerIndex] = useState<number | null>(null);
  const [saveNameInput, setSaveNameInput] = useState<string>('');

  // Example snakes and ladders. Keys = start, Values = destination.
  const snakes = new Map<number, number>([[16, 6], [48, 30], [62, 19], [88, 24]]);
  const ladders = new Map<number, number>([[3, 22], [15, 44], [52, 72], [68, 90]]);

  const cells = boardSize * boardSize;

  useEffect(() => {
    // Reset any message when setup changes
    setMessage('');
  }, [boardSize, maxThrows]);

  const startGame = () => {
    setPlayers(DEFAULT_PLAYERS.map((p) => ({ ...p, position: 0 })));
    setThrowCounts(DEFAULT_PLAYERS.map(() => 0));
    setCurrentPlayerIndex(0);
    setLastRoll(null);
    setGameState('playing');
    setGameStarted(true);
    setWinnerIndex(null);
    setSaveNameInput('');
    setMessage(`Game started: ${boardSize}x${boardSize} board, max ${maxThrows} throws each`);
  };

  const resetGame = () => {
    setPlayers(DEFAULT_PLAYERS.map((p) => ({ ...p, position: 0 })));
    setThrowCounts(DEFAULT_PLAYERS.map(() => 0));
    setCurrentPlayerIndex(0);
    setLastRoll(null);
    setGameState('idle');
    setGameStarted(false);
    setWinnerIndex(null);
    setSaveNameInput('');
    setMessage('Game reset');
  };

  const rollDice = () => {
    if (gameState !== 'playing') return;

    const playerThrows = throwCounts[currentPlayerIndex];
    if (playerThrows >= maxThrows) {
      setMessage(`${players[currentPlayerIndex].name} has no throws left.`);
      // advance turn
      setCurrentPlayerIndex((i) => (i + 1) % players.length);
      return;
    }

    const roll = Math.floor(Math.random() * 6) + 1;
    setLastRoll(roll);
    setThrowCounts((prev) => {
      const next = [...prev];
      next[currentPlayerIndex] = next[currentPlayerIndex] + 1;
      return next;
    });

    setPlayers((prev) => {
      const next = prev.map((p) => ({ ...p }));
      let pos = next[currentPlayerIndex].position + roll;

      // require exact finish
      if (pos > cells) {
        setMessage(`${next[currentPlayerIndex].name} rolled ${roll} (overshoot), stays at ${next[currentPlayerIndex].position}`);
        // keep position
      } else {
        if (snakes.has(pos)) {
          const dest = snakes.get(pos)!;
          setMessage(`${next[currentPlayerIndex].name} rolled ${roll} and hit a snake! Down to ${dest}`);
          pos = dest;
        } else if (ladders.has(pos)) {
          const dest = ladders.get(pos)!;
          setMessage(`${next[currentPlayerIndex].name} rolled ${roll} and climbed a ladder to ${dest}`);
          pos = dest;
        } else {
          setMessage(`${next[currentPlayerIndex].name} rolled ${roll} and moved to ${pos}`);
        }

        next[currentPlayerIndex].position = pos;
      }

      return next;
    });

    // After move, check for win/draw
    setTimeout(() => {
      // We can't rely on players state immediately; compute winner using latest players array instead
      setPlayers((currPlayers) => {
        const currPos = currPlayers[currentPlayerIndex].position;
        if (currPos === cells) {
          // winner
          setGameState('win');
          setWinnerIndex(currentPlayerIndex);
          setSaveNameInput(currPlayers[currentPlayerIndex].name);
        } else {
          // check draw condition: all players used up throws
          const allUsed = throwCounts.every((t, idx) => t >= maxThrows || (idx === currentPlayerIndex ? t + 1 >= maxThrows : t >= maxThrows));
          if (allUsed) {
            setGameState('draw');
          }
        }
        return currPlayers;
      });
    }, 20);

    // Advance to next player (unless someone won)
    setCurrentPlayerIndex((i) => (i + 1) % players.length);
  };

  const saveWinnerName = () => {
    if (winnerIndex === null) return;
    const winnersJson = localStorage.getItem(LOCAL_STORAGE_WINNERS_KEY);
    const winners = winnersJson ? JSON.parse(winnersJson) : [];
    winners.push({ name: saveNameInput || players[winnerIndex].name, date: new Date().toISOString(), boardSize, cells });
    localStorage.setItem(LOCAL_STORAGE_WINNERS_KEY, JSON.stringify(winners));
    setMessage('Winner saved ✓');
  };

  const getResultLabel = (): string => {
    if (gameState === 'win') return 'Win (correct)';
    if (gameState === 'lose') return 'Lose (wrong)';
    if (gameState === 'draw') return 'Draw';
    return 'Playing';
  };

  return (
    <div className="sal-game">
      <div className="sal-header">
        <h2>Snake & Ladder</h2>
        <p>Configure the board and play. Save player name if they win.</p>
      </div>

      <div className="sal-setup">
        <div className="sal-setup-field">
          <label>Board size (N x N):</label>
          <input
            type="number"
            min={3}
            max={12}
            value={boardSize}
            onChange={(e) => setBoardSize(Math.max(3, Math.min(12, Number(e.target.value) || 10)))}
            disabled={gameStarted}
          />
        </div>

        <div className="sal-setup-field">
          <label>Max dice throws per player:</label>
          <select value={maxThrows} onChange={(e) => setMaxThrows(Number(e.target.value))} disabled={gameStarted}>
            <option value={10}>10</option>
            <option value={20}>20</option>
            <option value={50}>50</option>
            <option value={100}>100</option>
          </select>
        </div>

        <div className="sal-setup-actions">
          <button onClick={startGame} disabled={gameStarted}>Start Game</button>
          <button onClick={resetGame}>Reset</button>
        </div>
      </div>

      <div className="sal-controls">
        <button onClick={rollDice} disabled={!gameStarted || gameState !== 'playing'}>Roll Dice</button>
        <div className="sal-last-roll">Last roll: {lastRoll ?? '-'}</div>
      </div>

      <div className="sal-board-summary">
        <h4>Players</h4>
        <ul>
          {players.map((p, idx) => (
            <li key={p.name} className={idx === currentPlayerIndex ? 'current' : ''}>
              <strong>{p.name}</strong>: {p.position} &nbsp;(<span className="muted">throws: {throwCounts[idx]}</span>)
            </li>
          ))}
        </ul>
      </div>

      <div className={`sal-message ${gameState}`}>{message || getResultLabel()}</div>

      {gameState === 'win' && winnerIndex !== null && (
        <div className="sal-result-card">
          <h3>🎉 {players[winnerIndex].name} wins!</h3>
          <p>Result: <strong>Win (correct)</strong></p>
          <div className="sal-save-winner">
            <label>Save winner name:</label>
            <input value={saveNameInput} onChange={(e) => setSaveNameInput(e.target.value)} />
            <button onClick={saveWinnerName}>Save</button>
          </div>
        </div>
      )}

      {gameState === 'draw' && (
        <div className="sal-result-card">
          <h3>🤝 Draw</h3>
          <p>Both players used their throws and no one reached the end.</p>
        </div>
      )}

      {gameState === 'lose' && (
        <div className="sal-result-card">
          <h3>☹️ Game Over</h3>
          <p>Result: <strong>Lose (wrong)</strong></p>
        </div>
      )}

      <div className="sal-board-note">
        <em>Note:</em> This is a minimal UI. The visual board grid is not rendered here — replace or extend as needed.
      </div>
    </div>
  );
};

export default SnakeAndLadder;
