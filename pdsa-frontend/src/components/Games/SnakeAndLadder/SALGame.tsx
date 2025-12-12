import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import ConfirmDialog from '../../Common/ConfirmDialog';
import GameResultDialog from '../../Common/GameResultDialog';
import './SALGame.css';

interface Snake {
  head: number;
  tail: number;
}

interface Ladder {
  bottom: number;
  top: number;
}

interface SALGameRound {
  gameId: string;
  boardSize: number;
  snakes: Snake[];
  ladders: Ladder[];
}

interface AlgorithmResult {
  algorithmName: string;
  minimumThrows: number;
  executionTimeMs: number;
  path: number[];
}

interface SALSolution {
  gameId: string;
  minimumThrows: number;
  algorithmResults: AlgorithmResult[];
}

const SALGame: React.FC = () => {
  const navigate = useNavigate();
  const [boardSize, setBoardSize] = useState<number>(8);
  const [gameRound, setGameRound] = useState<SALGameRound | null>(null);
  const [solution, setSolution] = useState<SALSolution | null>(null);
  const [userAnswer, setUserAnswer] = useState<number | null>(null);
  const [answerChoices, setAnswerChoices] = useState<number[]>([]);
  const [gameStatus, setGameStatus] = useState<'setup' | 'playing' | 'solved' | 'answered'>('setup');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  
  // Game progress tracking
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail' | 'draw'>('pass');

  const API_BASE_URL = 'http://localhost:5007';
  
  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  useEffect(() => {
    // Don't auto-create game on mount, wait for user to set board size
  }, []);

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

  const handleNextRound = () => {
    setShowResultDialog(false);
    setUserAnswer(null);
    setAnswerChoices([]);
    createNewGame();
  };

  const handleResultBackToGames = () => {
    setShowResultDialog(false);
    navigate('/');
  };

  const createNewGame = async () => {
    setLoading(true);
    setError('');
    setGameStatus('setup');
    setUserAnswer(null);
    setSolution(null);
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/sal/new-game`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ boardSize })
      });

      if (!response.ok) throw new Error('Failed to create game');

      const data = await response.json();
      setGameRound(data);
      setGameStatus('playing');
    } catch (err) {
      setError('Failed to create new game. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const solvePuzzle = async () => {
    if (!gameRound) return;

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/sal/solve`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ gameId: gameRound.gameId })
      });

      if (!response.ok) throw new Error('Failed to solve puzzle');

      const data = await response.json();
      setSolution(data);
      
      // Generate answer choices (correct answer + 2 random options)
      const correct = data.minimumThrows;
      const choices = [correct];
      
      // Add incorrect options
      const option1 = correct + Math.floor(Math.random() * 5) + 1;
      const option2 = correct - Math.floor(Math.random() * 3) - 1;
      
      choices.push(option1);
      if (option2 > 0) {
        choices.push(option2);
      } else {
        choices.push(correct + Math.floor(Math.random() * 3) + 6);
      }
      
      // Shuffle choices
      setAnswerChoices(choices.sort(() => Math.random() - 0.5));
      setGameStatus('solved');
      
      // Scroll to answer section after a delay
      setTimeout(() => {
        const answerSection = document.querySelector('.sal-answer-section');
        if (answerSection) {
          answerSection.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      }, 300);
    } catch (err) {
      setError('Failed to solve puzzle. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const submitAnswer = async () => {
    if (!gameRound || !solution || userAnswer === null) return;

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/sal/validate-answer`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          gameId: gameRound.gameId,
          playerName: playerName,
          userAnswer: userAnswer,
          correctAnswer: solution.minimumThrows
        })
      });

      if (!response.ok) throw new Error('Failed to validate answer');

      const data = await response.json();
      setGameStatus('answered');

      // Update game progress
      if (data.isCorrect) {
        const newPassed = passedRounds + 1;
        setPassedRounds(newPassed);
        
        if (newPassed === failedRounds) {
          setCurrentResult('draw');
        } else {
          setCurrentResult('pass');
        }
      } else {
        const newFailed = failedRounds + 1;
        setFailedRounds(newFailed);
        
        if (newFailed === passedRounds) {
          setCurrentResult('draw');
        } else {
          setCurrentResult('fail');
        }
      }

      setShowResultDialog(true);
    } catch (err) {
      setError('Failed to validate answer. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const renderBoard = () => {
    if (!gameRound) return null;

    const cells = gameRound.boardSize * gameRound.boardSize;
    const rows = [];
    
    for (let row = gameRound.boardSize - 1; row >= 0; row--) {
      const cellsInRow = [];
      for (let col = 0; col < gameRound.boardSize; col++) {
        const cellNumber = row % 2 === (gameRound.boardSize - 1) % 2
          ? row * gameRound.boardSize + col + 1
          : row * gameRound.boardSize + (gameRound.boardSize - col);
        
        // Check for snake or ladder
        const snake = gameRound.snakes.find(s => s.head === cellNumber);
        const ladder = gameRound.ladders.find(l => l.bottom === cellNumber);
        
        let cellClass = 'sal-cell';
        let cellContent = cellNumber.toString();
        
        if (snake) {
          cellClass += ' snake-head';
          cellContent = `🐍 ${cellNumber}→${snake.tail}`;
        } else if (ladder) {
          cellClass += ' ladder-bottom';
          cellContent = `🪜 ${cellNumber}→${ladder.top}`;
        }
        
        if (cellNumber === 1) cellClass += ' start-cell';
        if (cellNumber === cells) cellClass += ' end-cell';
        
        cellsInRow.push(
          <div key={cellNumber} className={cellClass}>
            {cellContent}
          </div>
        );
      }
      rows.push(
        <div key={row} className="sal-row">
          {cellsInRow}
        </div>
      );
    }
    
    return <div className="sal-board">{rows}</div>;
  };

  return (
    <div className="sal-game">
      <div className="sal-nav">
        <button onClick={handleBackToGames} className="sal-back-btn">
          ← Back to Games
        </button>
      </div>

      <div className="sal-header">
        <h1>🎲 Snake and Ladder Game</h1>
        <p>Find the minimum number of dice throws required to reach the last cell!</p>
        
        <div className="sal-game-stats">
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
      </div>

      {error && <div className="sal-error">{error}</div>}

      <div className="sal-content">
        {gameStatus === 'setup' && (
        <div className="sal-setup">
          <div className="setup-card">
            <h2>Game Setup</h2>
            <div className="setup-form">
              <div className="form-group">
                <label htmlFor="board-size-select">Board Size (N × N):</label>
                <select 
                  id="board-size-select"
                  value={boardSize} 
                  onChange={(e) => setBoardSize(Number(e.target.value))}
                  disabled={loading}
                  aria-label="Select board size"
                >
                  <option value={6}>6 × 6</option>
                  <option value={7}>7 × 7</option>
                  <option value={8}>8 × 8</option>
                  <option value={9}>9 × 9</option>
                  <option value={10}>10 × 10</option>
                  <option value={11}>11 × 11</option>
                  <option value={12}>12 × 12</option>
                </select>
              </div>
              <div className="setup-info">
                <p>• Snakes: {boardSize - 2}</p>
                <p>• Ladders: {boardSize - 2}</p>
                <p>• Total Cells: {boardSize * boardSize}</p>
              </div>
              <button 
                onClick={createNewGame} 
                disabled={loading}
                className="start-game-btn"
              >
                {loading ? 'Creating Game...' : 'Start Game'}
              </button>
            </div>
          </div>
        </div>
      )}

      {gameStatus !== 'setup' && gameRound && (
        <>
          <div className="sal-game-info">
            <div className="info-card">
              <h3>Game Information</h3>
              <div className="info-grid">
                <div className="info-item">
                  <span className="info-label">Board Size:</span>
                  <span className="info-value">{gameRound.boardSize} × {gameRound.boardSize}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Total Cells:</span>
                  <span className="info-value">{gameRound.boardSize * gameRound.boardSize}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Snakes:</span>
                  <span className="info-value">{gameRound.snakes.length}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Ladders:</span>
                  <span className="info-value">{gameRound.ladders.length}</span>
                </div>
              </div>
              <button 
                onClick={solvePuzzle} 
                disabled={loading || gameStatus === 'solved' || gameStatus === 'answered'}
                className="solve-btn"
              >
                {loading ? 'Solving...' : gameStatus === 'solved' || gameStatus === 'answered' ? 'Already Solved' : 'Find Minimum Throws'}
              </button>
            </div>
          </div>

          <div className="sal-board-section">
            <h3>Game Board</h3>
            {renderBoard()}
            <div className="board-legend">
              <div className="legend-item">
                <span className="legend-icon start">1</span>
                <span>Start Cell</span>
              </div>
              <div className="legend-item">
                <span className="legend-icon snake">🐍</span>
                <span>Snake (Head → Tail)</span>
              </div>
              <div className="legend-item">
                <span className="legend-icon ladder">🪜</span>
                <span>Ladder (Bottom → Top)</span>
              </div>
              <div className="legend-item">
                <span className="legend-icon end">{gameRound.boardSize * gameRound.boardSize}</span>
                <span>End Cell</span>
              </div>
            </div>
          </div>

          {solution && (
            <>
              <div className="sal-answer-section">
                <h3>Your Answer</h3>
                <p className="answer-prompt">Select the minimum number of dice throws required:</p>
                <div className="answer-choices">
                  {answerChoices.map((choice, index) => (
                    <button
                      key={index}
                      className={`choice-btn ${userAnswer === choice ? 'selected' : ''}`}
                      onClick={() => setUserAnswer(choice)}
                      disabled={loading || gameStatus === 'answered'}
                    >
                      {choice} {choice === 1 ? 'throw' : 'throws'}
                    </button>
                  ))}
                </div>
                {userAnswer !== null && (
                  <button 
                    onClick={submitAnswer} 
                    disabled={loading || gameStatus === 'answered'}
                    className="submit-answer-btn"
                  >
                    {loading ? 'Submitting...' : 'Submit Answer'}
                  </button>
                )}
              </div>

              <div className="sal-solution-section">
                <h3>Algorithm Results</h3>
                <div className="algorithm-results">
                  {solution.algorithmResults.map((result, index) => (
                    <div key={index} className="algorithm-card">
                      <h4>{result.algorithmName}</h4>
                      <div className="result-details">
                        <div className="result-item">
                          <span className="result-label">Minimum Throws:</span>
                          <span className="result-value">{result.minimumThrows}</span>
                        </div>
                        <div className="result-item">
                          <span className="result-label">Execution Time:</span>
                          <span className="result-value">{result.executionTimeMs.toFixed(2)} ms</span>
                        </div>
                        <div className="result-item">
                          <span className="result-label">Path Length:</span>
                          <span className="result-value">{result.path.length} steps</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}
        </>
      )}
      </div>

      <ConfirmDialog
        isOpen={showConfirmDialog}
        title="Leave Game?"
        message="Are you sure you want to leave? Your game progress will be lost."
        confirmText="Leave"
        cancelText="Stay"
        onConfirm={handleConfirmLeave}
        onCancel={handleCancelLeave}
      />

      <GameResultDialog
        isOpen={showResultDialog}
        result={currentResult}
        onNextRound={handleNextRound}
        onBackToGames={handleResultBackToGames}
        passedCount={passedRounds}
        failedCount={failedRounds}
        userAnswer={userAnswer ?? undefined}
        correctAnswer={solution?.minimumThrows}
      />
    </div>
  );
};

export default SALGame;
