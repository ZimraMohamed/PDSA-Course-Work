import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./EQPGame.css";

type Cell = {
  row: number;
  col: number;
  hasQueen: boolean;
};

type EQPGameRound = {
  gameId: string;
  boardSize: number;
};

type AlgorithmResult = {
  algorithmName: string;
  solutionsFound: number;
  executionTimeMs: number;
};

type EQPSolutionDto = {
  gameId: string;
  totalSolutionsSequential: number;
  totalSolutionsThreaded: number;
  algorithmResults: AlgorithmResult[];
};

const API_BASE = "http://localhost:5007";

const EQPGame: React.FC = () => {
  const navigate = useNavigate();
  const [gameRound, setGameRound] = useState<EQPGameRound | null>(null);
  const [board, setBoard] = useState<Cell[][]>([]);
  const [message, setMessage] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);
  const [solutionInfo, setSolutionInfo] = useState<EQPSolutionDto | null>(null);

  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  useEffect(() => {
    createNewGame();
  }, []);

  const createEmptyBoard = (n: number): Cell[][] => {
    return Array.from({ length: n }, (_, r) =>
      Array.from({ length: n }, (_, c) => ({ row: r, col: c, hasQueen: false }))
    );
  };

  const createNewGame = async () => {
    setLoading(true);
    setMessage("");
    try {
      const res = await fetch(`${API_BASE}/api/eqp/new-game`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
      });
      if (!res.ok) throw new Error("Failed to create new game");
      const data = await res.json();
      setGameRound(data);
      setBoard(createEmptyBoard(data.boardSize));
      setSolutionInfo(null);
    } catch (err) {
      console.error(err);
      setMessage("Failed to create new game.");
    } finally {
      setLoading(false);
    }
  };

  const toggleQueen = (r: number, c: number) => {
    setBoard(prev => {
      const next = prev.map(row => row.map(cell => ({ ...cell })));
      const cell = next[r][c];
      
      // Count current queens
      const currentQueenCount = next.flat().filter(c => c.hasQueen).length;
      
      // If trying to add a queen and already at max (8), don't allow it
      if (!cell.hasQueen && currentQueenCount >= 8) {
        return prev;
      }
      
      cell.hasQueen = !cell.hasQueen;
      return next;
    });
  };

  const canPlaceQueen = (r: number, c: number): boolean => {
    const cell = board[r]?.[c];
    if (!cell) return false;
    
    // If cell already has a queen, allow removal
    if (cell.hasQueen) return true;
    
    // Count current queens
    const currentQueenCount = board.flat().filter(c => c.hasQueen).length;
    
    // If trying to add a queen and already at max (8), don't allow it
    return currentQueenCount < 8;
  };

  const submitPlayerSolution = async () => {
    if (!gameRound) return;
    setLoading(true);
    setMessage("");
    // Build list of positions
    const queens = board.flat().filter(cell => cell.hasQueen).map(cell => ({ row: cell.row, col: cell.col }));
    try {
      const res = await fetch(`${API_BASE}/api/eqp/submit-solution`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          gameId: gameRound.gameId,
          playerName,
          queenPositions: queens
        }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data || "Server error");
      setMessage(data.message || (data.isCorrect ? "Correct solution!" : "Incorrect solution."));
    } catch (err: any) {
      console.error(err);
      setMessage(err?.message || "Failed to submit solution.");
    } finally {
      setLoading(false);
    }
  };

  const solveAll = async () => {
    if (!gameRound) return;
    setLoading(true);
    setMessage("");
    try {
      const res = await fetch(`${API_BASE}/api/eqp/solve`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ gameId: gameRound.gameId })
      });
      if (!res.ok) throw new Error("Failed to solve");
      const data = await res.json();
      setSolutionInfo(data);
      setMessage(`Found ${data.totalSolutionsSequential} solutions (sequential).`);
    } catch (err) {
      console.error(err);
      setMessage("Failed to run solver.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="eqp-game">
      <div className="eqp-nav">
        <button onClick={() => navigate('/')} className="eqp-back-btn">
          ← Back to Games
        </button>
      </div>

      <div className="eqp-header">
        <h1>♟️ Eight Queens Puzzle</h1>
        <p>Place 8 queens so no two attack each other. Submit your solution or let the solver find all solutions.</p>
        <div className="eqp-controls">
          <button onClick={createNewGame} disabled={loading}>New Game</button>
          <button onClick={solveAll} disabled={loading}>Run Solver (Sequential & Threaded)</button>
        </div>
      </div>

      {message && <div className="eqp-message">{message}</div>}

      {gameRound && (
        <div className="eqp-board-area">
          <div className="eqp-board">
            {board.map((row, r) => (
              <div key={r} className="eqp-row">
                {row.map((cell, c) => (
                  <button
                    key={c}
                    className={`eqp-cell ${cell.hasQueen ? "queen" : ""} ${(r + c) % 2 === 0 ? "light" : "dark"} ${!canPlaceQueen(r, c) ? "disabled" : ""}`}
                    onClick={() => toggleQueen(r, c)}
                  >
                    <span>{cell.hasQueen ? "♛" : "\u200B"}</span>
                  </button>
                ))}
              </div>
            ))}
          </div>

          <div className="eqp-actions">
            <div className="player-info">
              <span className="player-label">Player:</span>
              <span className="player-name">{playerName}</span>
            </div>
            <button onClick={submitPlayerSolution} disabled={loading}>Submit Solution</button>

            <div className="eqp-solution-info">
              {solutionInfo && (
                <>
                  <h3>Solver Results</h3>
                  <p>Sequential found: {solutionInfo.totalSolutionsSequential}</p>
                  <p>Threaded found: {solutionInfo.totalSolutionsThreaded}</p>
                  <div className="eqp-alg-results">
                    {solutionInfo.algorithmResults.map((r, i) => (
                      <div key={i} className="eqp-alg-card">
                        <h4>{r.algorithmName}</h4>
                        <p>Solutions: {r.solutionsFound}</p>
                        <p>Time: {r.executionTimeMs} ms</p>
                      </div>
                    ))}
                  </div>
                </>
              )}
            </div>
          </div>
        </div>
      )}

      <div className="eqp-footer">
        <small>Board size: {gameRound?.boardSize ?? "-"}</small>
      </div>
    </div>
  );
};

export default EQPGame;
