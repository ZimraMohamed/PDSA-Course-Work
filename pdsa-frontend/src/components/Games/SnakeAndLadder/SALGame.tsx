import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Stage, Layer, Line, Circle, Group } from 'react-konva';
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
  const [previewSnakes, setPreviewSnakes] = useState<Snake[]>([]);
  const [previewLadders, setPreviewLadders] = useState<Ladder[]>([]);
  const [gameRound, setGameRound] = useState<SALGameRound | null>(null);
  const [solution, setSolution] = useState<SALSolution | null>(null);
  const [userAnswer, setUserAnswer] = useState<number | null>(null);
  const [answerChoices, setAnswerChoices] = useState<number[]>([]);
  const [gameStatus, setGameStatus] = useState<'setup' | 'playing' | 'solved' | 'answered'>('setup');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const boardRef = useRef<HTMLDivElement>(null);
  const [boardDimensions, setBoardDimensions] = useState({ width: 0, height: 0 });
  
  // Game progress tracking
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [showAnswerPopup, setShowAnswerPopup] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail' | 'draw'>('pass');

  const API_BASE_URL = '';
  
  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  // Generate random snakes and ladders for preview
  const generateRandomSnakesAndLadders = (size: number) => {
    const count = size - 2;
    const totalCells = size * size;
    const usedCells = new Set<number>([1, totalCells]); // Reserve start and end
    
    const snakes: Snake[] = [];
    const ladders: Ladder[] = [];
    
    // Generate snakes
    for (let i = 0; i < count; i++) {
      let head, tail;
      do {
        head = Math.floor(Math.random() * (totalCells - size)) + size + 1; // Snake head in upper portion
        tail = Math.floor(Math.random() * (head - 1)) + 1; // Tail below head
      } while (usedCells.has(head) || usedCells.has(tail) || head === totalCells); // No snake on finish cell
      
      usedCells.add(head);
      usedCells.add(tail);
      snakes.push({ head, tail });
    }
    
    // Generate ladders
    for (let i = 0; i < count; i++) {
      let bottom, top;
      do {
        bottom = Math.floor(Math.random() * (totalCells - size)) + 1; // Ladder bottom in lower portion
        top = Math.floor(Math.random() * (totalCells - bottom)) + bottom + 1; // Top above bottom
      } while (usedCells.has(bottom) || usedCells.has(top));
      
      usedCells.add(bottom);
      usedCells.add(top);
      ladders.push({ bottom, top });
    }
    
    return { snakes, ladders };
  };

  useEffect(() => {
    // Generate preview board when board size changes
    const { snakes, ladders } = generateRandomSnakesAndLadders(boardSize);
    setPreviewSnakes(snakes);
    setPreviewLadders(ladders);
  }, [boardSize]);

  // Track board dimensions for Konva canvas
  useEffect(() => {
    const updateDimensions = () => {
      if (boardRef.current) {
        const rect = boardRef.current.getBoundingClientRect();
        setBoardDimensions({ width: rect.width, height: rect.height });
      }
    };
    
    updateDimensions();
    window.addEventListener('resize', updateDimensions);
    return () => window.removeEventListener('resize', updateDimensions);
  }, [gameStatus, boardSize]);

  // Helper function to get cell position (row, col) from cell number
  const getCellPosition = (cellNumber: number, boardSize: number): { row: number; col: number } => {
    const row = Math.floor((cellNumber - 1) / boardSize);
    const col = (cellNumber - 1) % boardSize;
    
    // Check if row should be reversed (snake pattern)
    const isReversedRow = row % 2 === 1;
    const actualCol = isReversedRow ? boardSize - 1 - col : col;
    
    // Convert to visual row (bottom to top)
    const visualRow = boardSize - 1 - row;
    
    return { row: visualRow, col: actualCol };
  };

  // Render snake with Konva for better visuals
  const renderSnake = (snake: Snake, size: number, cellSize: number, index: number) => {
    const startPos = getCellPosition(snake.head, size);
    const endPos = getCellPosition(snake.tail, size);
    
    const x1 = (startPos.col + 0.5) * cellSize;
    const y1 = (startPos.row + 0.5) * cellSize;
    const x2 = (endPos.col + 0.5) * cellSize;
    const y2 = (endPos.row + 0.5) * cellSize;
    
    // Create smooth S-curve control points
    const dx = x2 - x1;
    const dy = y2 - y1;
    const distance = Math.sqrt(dx * dx + dy * dy);
    
    // Calculate perpendicular offset for curve
    const perpX = -dy / distance;
    const perpY = dx / distance;
    const curveOffset = distance * 0.2;
    
    // Control points for bezier curve
    const cp1x = x1 + dx * 0.25 + perpX * curveOffset * (index % 2 === 0 ? 1 : -1);
    const cp1y = y1 + dy * 0.25 + perpY * curveOffset * (index % 2 === 0 ? 1 : -1);
    const cp2x = x1 + dx * 0.75 + perpX * curveOffset * (index % 2 === 0 ? -1 : 1);
    const cp2y = y1 + dy * 0.75 + perpY * curveOffset * (index % 2 === 0 ? -1 : 1);
    
    // Generate points along the curve for the snake body
    const segments = 50;
    const points: number[] = [];
    for (let i = 0; i <= segments; i++) {
      const t = i / segments;
      const x = Math.pow(1-t, 3) * x1 + 
                3 * Math.pow(1-t, 2) * t * cp1x + 
                3 * (1-t) * Math.pow(t, 2) * cp2x + 
                Math.pow(t, 3) * x2;
      const y = Math.pow(1-t, 3) * y1 + 
                3 * Math.pow(1-t, 2) * t * cp1y + 
                3 * (1-t) * Math.pow(t, 2) * cp2y + 
                Math.pow(t, 3) * y2;
      points.push(x, y);
    }
    
    const bodyWidth = cellSize * 0.15;
    
    return (
      <Group key={`snake-${index}`}>
        {/* Snake body shadow */}
        <Line
          points={points}
          stroke="rgba(0, 0, 0, 0.2)"
          strokeWidth={bodyWidth + 4}
          lineCap="round"
          lineJoin="round"
          shadowBlur={10}
          shadowOffset={{ x: 2, y: 2 }}
          shadowOpacity={0.3}
        />
        
        {/* Snake body gradient (outer) */}
        <Line
          points={points}
          stroke="#dc2626"
          strokeWidth={bodyWidth}
          lineCap="round"
          lineJoin="round"
        />
        
        {/* Snake body gradient (inner) */}
        <Line
          points={points}
          stroke="#ef4444"
          strokeWidth={bodyWidth * 0.7}
          lineCap="round"
          lineJoin="round"
        />
        
        {/* Snake scales pattern */}
        {Array.from({ length: Math.floor(distance / (cellSize * 0.15)) }).map((_, i) => {
          const t = (i + 0.5) / Math.floor(distance / (cellSize * 0.15));
          const segX = Math.pow(1-t, 3) * x1 + 
                      3 * Math.pow(1-t, 2) * t * cp1x + 
                      3 * (1-t) * Math.pow(t, 2) * cp2x + 
                      Math.pow(t, 3) * x2;
          const segY = Math.pow(1-t, 3) * y1 + 
                      3 * Math.pow(1-t, 2) * t * cp1y + 
                      3 * (1-t) * Math.pow(t, 2) * cp2y + 
                      Math.pow(t, 3) * y2;
          
          return (
            <Circle
              key={i}
              x={segX}
              y={segY}
              radius={bodyWidth * 0.25}
              fill="rgba(220, 38, 38, 0.5)"
            />
          );
        })}
        
        {/* Snake head base (darker outer layer) */}
        <Circle
          x={x1}
          y={y1}
          radius={bodyWidth * 1.5}
          fill="#7f1d1d"
          shadowBlur={10}
          shadowOpacity={0.5}
        />
        
        {/* Snake head middle layer */}
        <Circle
          x={x1}
          y={y1}
          radius={bodyWidth * 1.3}
          fill="#991b1b"
        />
        
        {/* Snake head top layer */}
        <Circle
          x={x1}
          y={y1}
          radius={bodyWidth * 1.1}
          fill="#dc2626"
        />
        
        {/* Snake snout highlight */}
        <Circle
          x={x1}
          y={y1 + bodyWidth * 0.4}
          radius={bodyWidth * 0.6}
          fill="#ef4444"
          opacity={0.8}
        />
        
        {/* Snake eye sockets (subtle shadow) */}
        <Circle
          x={x1 - bodyWidth * 0.6}
          y={y1 - bodyWidth * 0.3}
          radius={bodyWidth * 0.35}
          fill="rgba(0, 0, 0, 0.2)"
        />
        <Circle
          x={x1 + bodyWidth * 0.6}
          y={y1 - bodyWidth * 0.3}
          radius={bodyWidth * 0.35}
          fill="rgba(0, 0, 0, 0.2)"
        />
        
        {/* Snake eyes (yellow with black slit pupils) */}
        <Circle
          x={x1 - bodyWidth * 0.6}
          y={y1 - bodyWidth * 0.3}
          radius={bodyWidth * 0.32}
          fill="#fbbf24"
        />
        <Circle
          x={x1 + bodyWidth * 0.6}
          y={y1 - bodyWidth * 0.3}
          radius={bodyWidth * 0.32}
          fill="#fbbf24"
        />
        
        {/* Snake eye shine */}
        <Circle
          x={x1 - bodyWidth * 0.7}
          y={y1 - bodyWidth * 0.4}
          radius={bodyWidth * 0.1}
          fill="white"
          opacity={0.9}
        />
        <Circle
          x={x1 + bodyWidth * 0.5}
          y={y1 - bodyWidth * 0.4}
          radius={bodyWidth * 0.1}
          fill="white"
          opacity={0.9}
        />
        
        {/* Vertical slit pupils */}
        <Line
          points={[
            x1 - bodyWidth * 0.6, y1 - bodyWidth * 0.5,
            x1 - bodyWidth * 0.6, y1 - bodyWidth * 0.1
          ]}
          stroke="black"
          strokeWidth={bodyWidth * 0.15}
          lineCap="round"
        />
        <Line
          points={[
            x1 + bodyWidth * 0.6, y1 - bodyWidth * 0.5,
            x1 + bodyWidth * 0.6, y1 - bodyWidth * 0.1
          ]}
          stroke="black"
          strokeWidth={bodyWidth * 0.15}
          lineCap="round"
        />
        
        {/* Nostrils */}
        <Circle
          x={x1 - bodyWidth * 0.15}
          y={y1 + bodyWidth * 0.6}
          radius={bodyWidth * 0.08}
          fill="#7f1d1d"
        />
        <Circle
          x={x1 + bodyWidth * 0.15}
          y={y1 + bodyWidth * 0.6}
          radius={bodyWidth * 0.08}
          fill="#7f1d1d"
        />
        
        {/* Snake tongue (forked) */}
        <Line
          points={[
            x1, y1 + bodyWidth * 0.8,
            x1, y1 + bodyWidth * 1.3,
            x1 - bodyWidth * 0.25, y1 + bodyWidth * 1.6,
          ]}
          stroke="#ef4444"
          strokeWidth={bodyWidth * 0.1}
          lineCap="round"
        />
        <Line
          points={[
            x1, y1 + bodyWidth * 1.3,
            x1 + bodyWidth * 0.25, y1 + bodyWidth * 1.6,
          ]}
          stroke="#ef4444"
          strokeWidth={bodyWidth * 0.1}
          lineCap="round"
        />
        
        {/* Snake tail with gradient taper */}
        <Circle
          x={x2}
          y={y2}
          radius={bodyWidth * 0.9}
          fill="#ef4444"
          shadowBlur={5}
          shadowOpacity={0.3}
        />
        <Circle
          x={x2}
          y={y2}
          radius={bodyWidth * 0.7}
          fill="#f87171"
        />
        <Circle
          x={x2}
          y={y2}
          radius={bodyWidth * 0.5}
          fill="#fca5a5"
        />
        <Circle
          x={x2}
          y={y2}
          radius={bodyWidth * 0.3}
          fill="#fecaca"
        />
        
        {/* Tail tip */}
        <Circle
          x={x2}
          y={y2}
          radius={bodyWidth * 0.15}
          fill="#fee2e2"
        />
      </Group>
    );
  };

  // Render board with Konva overlays for snakes and ladders
  const renderBoardWithConnections = (snakes: Snake[], ladders: Ladder[], size: number) => {
    const cells = size * size;
    const rows = [];
    
    for (let row = 0; row < size; row++) {
      const cellsInRow = [];
      for (let col = 0; col < size; col++) {
        // Calculate cell number: bottom row (row=0) goes left to right (1,2,3...),
        // next row goes right to left, alternating snake pattern
        const cellNumber = row % 2 === 0
          ? row * size + col + 1        // Even rows: left to right
          : row * size + (size - col);  // Odd rows: right to left
        
        const isStart = cellNumber === 1;
        const isEnd = cellNumber === cells;
        
        let cellClass = 'sal-cell';
        let cellContent;
        
        if (isStart) {
          cellClass += ' start-cell';
          cellContent = (
            <>
              <span className="cell-icon">🎯</span>
              <span className="cell-number">START</span>
            </>
          );
        } else if (isEnd) {
          cellClass += ' end-cell';
          cellContent = (
            <>
              <span className="cell-icon">🏁</span>
              <span className="cell-number">END</span>
            </>
          );
        } else {
          cellContent = <span className="cell-number">{cellNumber}</span>;
        }
        
        cellsInRow.push(
          <div key={cellNumber} className={cellClass} data-cell={cellNumber}>
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
    
    // Reverse rows so cell 1 appears at the bottom
    rows.reverse();
    
    const cellSize = boardDimensions.width > 0 ? (boardDimensions.width - 40) / size : 60;
    
    return (
      <div className="sal-board-container" ref={boardRef}>
        <div className="sal-board">{rows}</div>
        {boardDimensions.width > 0 && (
          <Stage
            width={boardDimensions.width - 40}
            height={boardDimensions.height - 40}
            className="sal-board-overlay-konva"
          >
            <Layer>
              {/* Draw snakes with Konva */}
              {snakes.map((snake, index) => renderSnake(snake, size, cellSize, index))}
              
              {/* Draw ladders */}
              {ladders.map((ladder, index) => {
                const startPos = getCellPosition(ladder.bottom, size);
                const endPos = getCellPosition(ladder.top, size);
                
                const x1 = (startPos.col + 0.5) * cellSize;
                const y1 = (startPos.row + 0.5) * cellSize;
                const x2 = (endPos.col + 0.5) * cellSize;
                const y2 = (endPos.row + 0.5) * cellSize;
                
                // Calculate ladder sides
                const dx = x2 - x1;
                const dy = y2 - y1;
                const length = Math.sqrt(dx * dx + dy * dy);
                const perpX = -dy / length * (cellSize * 0.08);
                const perpY = dx / length * (cellSize * 0.08);
                
                // Ladder rungs count
                const rungs = Math.floor(length / (cellSize * 0.3));
                
                return (
                  <Group key={`ladder-${index}`}>
                    {/* Ladder sides */}
                    <Line
                      points={[
                        x1 + perpX, y1 + perpY,
                        x2 + perpX, y2 + perpY
                      ]}
                      stroke="#8b5cf6"
                      strokeWidth={cellSize * 0.08}
                      lineCap="round"
                      shadowBlur={5}
                      shadowOpacity={0.3}
                    />
                    <Line
                      points={[
                        x1 - perpX, y1 - perpY,
                        x2 - perpX, y2 - perpY
                      ]}
                      stroke="#8b5cf6"
                      strokeWidth={cellSize * 0.08}
                      lineCap="round"
                      shadowBlur={5}
                      shadowOpacity={0.3}
                    />
                    
                    {/* Ladder rungs */}
                    {Array.from({ length: rungs }).map((_, i) => {
                      const t = (i + 1) / (rungs + 1);
                      const rx = x1 + dx * t;
                      const ry = y1 + dy * t;
                      return (
                        <Line
                          key={i}
                          points={[
                            rx + perpX, ry + perpY,
                            rx - perpX, ry - perpY
                          ]}
                          stroke="#a78bfa"
                          strokeWidth={cellSize * 0.06}
                          lineCap="round"
                        />
                      );
                    })}
                  </Group>
                );
              })}
            </Layer>
          </Stage>
        )}
      </div>
    );
  };

  const renderBoard = () => {
    if (!gameRound) return null;
    return renderBoardWithConnections(gameRound.snakes, gameRound.ladders, gameRound.boardSize);
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

  const handleCloseAnswerPopup = () => {
    setShowAnswerPopup(false);
    setUserAnswer(null);
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
        body: JSON.stringify({
          gameId: gameRound.gameId,
          boardSize: gameRound.boardSize,
          snakes: gameRound.snakes,
          ladders: gameRound.ladders
        })
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
      
      // Show answer popup
      setShowAnswerPopup(true);
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
          correctAnswer: solution.minimumThrows,
          boardSize: gameRound.boardSize,
          numSnakes: gameRound.snakes.length,
          numLadders: gameRound.ladders.length,
          snakes: gameRound.snakes,
          ladders: gameRound.ladders,
          algorithmResults: solution.algorithmResults
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

  return (
    <div className="sal-game">
      <div className="sal-nav">
        <button onClick={handleBackToGames} className="sal-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/sal/stats')} className="sal-stats-btn">
          View Statistics
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
          
          <div className="setup-board-preview">
            <h3>Board Preview</h3>
            {renderBoardWithConnections(previewSnakes, previewLadders, boardSize)}
          </div>
        </div>
        )}

      {gameStatus !== 'setup' && gameRound && (
        <>
          <div className="sal-game-board-container">
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
                  disabled={loading || gameStatus === 'answered'}
                  className="solve-btn"
                >
                  {loading ? 'Solving...' : gameStatus === 'answered' ? 'Challenge Completed' : 'Submit Minimum Rolls'}
                </button>
              </div>
            </div>

            <div className="sal-board-section">
              <h3>Game Board</h3>
              {renderBoard()}
            </div>
          </div>
        </>
      )}
      </div>

      {/* Answer Popup */}
      {showAnswerPopup && solution && (
        <div className="sal-answer-popup-overlay" onClick={handleCloseAnswerPopup}>
          <div className="sal-answer-popup" onClick={(e) => e.stopPropagation()}>
            <div className="popup-header">
              <h3>Minimum Throws Required</h3>
              <button className="popup-close" onClick={handleCloseAnswerPopup}>×</button>
            </div>
            <div className="popup-content">
              <p className="answer-prompt">Select the minimum number of dice throws required to reach the end:</p>
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
                  onClick={() => {
                    submitAnswer();
                    setShowAnswerPopup(false);
                  }} 
                  disabled={loading || gameStatus === 'answered'}
                  className="submit-answer-btn"
                >
                  {loading ? 'Submitting...' : 'Submit Answer'}
                </button>
              )}
            </div>
          </div>
        </div>
      )}

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
