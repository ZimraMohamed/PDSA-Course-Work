import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import GameResultDialog from "../../Common/GameResultDialog";
import ConfirmDialog from "../../Common/ConfirmDialog";
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

  // Game progress tracking
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);

  // Result dialog state
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail'>('fail');
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);

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
    setShowResultDialog(false);
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

  const handleNextRound = () => {
    setShowResultDialog(false);
    createNewGame();
  };

  const handleBackToGames = () => {
    if (passedRounds > 0 || failedRounds > 0) {
      setShowConfirmDialog(true);
    } else {
      navigate('/');
    }
  };

  const handleConfirmLeave = () => {
    setShowConfirmDialog(false);
    navigate('/');
  };

  const handleCancelLeave = () => {
    setShowConfirmDialog(false);
  };

  const handleResultBackToGames = () => {
    navigate('/games');
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
      
      // Show result dialog
      const isCorrect = data.isCorrect || false;
      setCurrentResult(isCorrect ? 'pass' : 'fail');
      setShowResultDialog(true);
      
      // Update progress counters
      if (isCorrect) {
        setPassedRounds(prev => prev + 1);
      } else {
        setFailedRounds(prev => prev + 1);
      }
      
      setMessage(data.message || (isCorrect ? "Correct solution!" : "Incorrect solution."));
    } catch (err: any) {
      console.error(err);
      setCurrentResult('fail');
      setShowResultDialog(true);
      setFailedRounds(prev => prev + 1);
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
      
      // Scroll to solution info after a short delay
      setTimeout(() => {
        const solutionSection = document.querySelector('.eqp-solution-info');
        if (solutionSection) {
          solutionSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      }, 300);
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
        <button onClick={handleBackToGames} className="eqp-back-btn">
          ← Back to Games
        </button>
      </div>

      <div className="eqp-header">
        <h1>♟️ Eight Queens Puzzle</h1>
        <p>Place 8 queens so no two attack each other. Submit your solution or let the solver find all solutions.</p>
        
        <div className="eqp-game-stats">
          <div className="player-info">
            <span className="player-label">Player:</span>
            <span className="player-name">{playerName}</span>
          </div>
          
          <div className="game-progress">
            <div className="progress-item passed">
              <span className="progress-label">Passed:</span>
              <span className="progress-value">{passedRounds}</span>
            </div>
            <div className="progress-item failed">
              <span className="progress-label">Failed:</span>
              <span className="progress-value">{failedRounds}</span>
            </div>
          </div>
        </div>

        <div className="eqp-controls">
          <button onClick={solveAll} disabled={loading}>Run Solver (Sequential & Threaded)</button>
        </div>
      </div>

      {gameRound && (
        <>
          <div className="eqp-board-container">
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
            
            <div className="eqp-submit-section">
              <button className="eqp-submit-btn" onClick={submitPlayerSolution} disabled={loading}>
                Submit Solution
              </button>
            </div>
          </div>

          {solutionInfo && (
            <div className="eqp-solution-info">
              <h3>Solver Results</h3>
              <div className="solver-summary">
                <div className="summary-item">
                  <span className="summary-label">Sequential Found:</span>
                  <span className="summary-value">{solutionInfo.totalSolutionsSequential}</span>
                </div>
                <div className="summary-item">
                  <span className="summary-label">Threaded Found:</span>
                  <span className="summary-value">{solutionInfo.totalSolutionsThreaded}</span>
                </div>
              </div>
              <div className="eqp-alg-results">
                {solutionInfo.algorithmResults.map((r, i) => (
                  <div key={i} className="eqp-alg-card">
                    <h4>{r.algorithmName}</h4>
                    <p>Solutions: {r.solutionsFound}</p>
                    <p>Time: {r.executionTimeMs} ms</p>
                  </div>
                ))}
              </div>
            </div>
          )}
        </>
      )}

      <GameResultDialog
        isOpen={showResultDialog}
        result={currentResult}
        onNextRound={handleNextRound}
        onBackToGames={handleResultBackToGames}
        passedCount={passedRounds}
        failedCount={failedRounds}
      />

      <ConfirmDialog
        isOpen={showConfirmDialog}
        title="Leave Game?"
        message="Are you sure you want to leave? Your game progress will be lost."
        confirmText="Leave"
        cancelText="Stay"
        onConfirm={handleConfirmLeave}
        onCancel={handleCancelLeave}
      />

      <div className="eqp-footer">
        <small>Board size: {gameRound?.boardSize ?? "-"}</small>
      </div>
    </div>
  );
};

export default EQPGame;
