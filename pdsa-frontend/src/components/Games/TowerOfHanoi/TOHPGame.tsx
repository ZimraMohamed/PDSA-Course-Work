import React, { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';
import ConfirmDialog from '../../Common/ConfirmDialog';
import GameResultDialog from '../../Common/GameResultDialog';
import "./TOHPGame.css";

const TOHPGame: React.FC = () => {
  const navigate = useNavigate();
  const [numPegs, setNumPegs] = useState<number>(3);
  const [numDisks, setNumDisks] = useState<number>(
    Math.floor(Math.random() * 6) + 5
  ); // 5-10
  const [userMovesCount, setUserMovesCount] = useState<string>("");
  const [userSequence, setUserSequence] = useState<string>("");
  const [userAnswer, setUserAnswer] = useState<string>("");
  const [correctAnswer, setCorrectAnswer] = useState<string>("");
  const [recordedMoves, setRecordedMoves] = useState<string[]>([]);

  // Game progress tracking
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail' | 'draw'>('pass');

  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  type PegState = number[][];
  const [pegs, setPegs] = useState<PegState>([]);

  const [moveErrorPopup, setMoveErrorPopup] = useState<string | null>(null);

  const [selectedDisk, setSelectedDisk] = useState<{
    pegIndex: number;
    size: number;
  } | null>(null);

  const [isAnimating, setIsAnimating] = useState<boolean>(false);

  // Initialize pegs whenever numPegs or numDisks changes
  useEffect(() => {
    const initialPegs: PegState = Array.from({ length: numPegs }, () => []);
    const startingPeg: number[] = [];
    for (let d = numDisks; d >= 1; d--) startingPeg.push(d);
    initialPegs[0] = startingPeg;
    setPegs(initialPegs);
    setSelectedDisk(null);

    console.log(`Game initialized with ${numPegs} pegs and ${numDisks} disks.`);
  }, [numPegs, numDisks]);

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
    regenerateDisks();
  };

  const handleResultBackToGames = () => {
    setShowResultDialog(false);
    navigate('/');
  };

  const regenerateDisks = () => {
    const newCount = Math.floor(Math.random() * 6) + 5;
    setNumDisks(newCount);
    setUserMovesCount("");
    setUserSequence("");
    setRecordedMoves([]);
    console.log(`New game generated with ${newCount} disks.`);
  };

  const incrementMoves = () => {
    const current = parseInt(userMovesCount) || 0;
    setUserMovesCount((current + 1).toString());
  };

  const decrementMoves = () => {
    const current = parseInt(userMovesCount) || 0;
    if (current > 0) {
      setUserMovesCount((current - 1).toString());
    }
  };

  const addMoveToSequence = (from: string, to: string) => {
    const move = `${from}→${to}`;
    const newMoves = [...recordedMoves, move];
    setRecordedMoves(newMoves);
    setUserSequence(newMoves.join(", "));
    incrementMoves();
  };

  const removeLastMove = () => {
    if (recordedMoves.length > 0) {
      const newMoves = recordedMoves.slice(0, -1);
      setRecordedMoves(newMoves);
      setUserSequence(newMoves.join(", "));
      decrementMoves();
    }
  };

  const clearMoves = () => {
    setRecordedMoves([]);
    setUserSequence("");
    setUserMovesCount("");
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate mandatory fields
    if (!userMovesCount.trim() || !userSequence.trim()) {
        alert("Please fill in both Number of Moves and Sequence of Moves.");
        return;
    }

    console.log("Submitting moves count:", userMovesCount);
    console.log("Submitting sequence:", userSequence);

    try {
        const res = await fetch("http://localhost:5007/api/TOHP/check-moves", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                numPegs,
                numDisks,
                userMovesCount: parseInt(userMovesCount, 10),
                userSequence
            })
        });

        if (!res.ok) {
            const errData = await res.json();
            console.error("Backend error:", errData.message || "Unknown error");
            alert(errData.message || "Backend returned an error.");
            return;
        }

        const data = await res.json();
        console.log("Backend response:", data);

        // Update game progress and show result dialog
        if (data.correctMoves && data.correctSequence) {
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

        // Prepare user answer and correct answer for dialog
        const userAnswerText = `Moves: ${userMovesCount}, Sequence: ${userSequence}`;
        const correctAnswerText = `Moves: ${data.optimalMoves}, Sequence: ${data.correctSequenceList || data.correctSequence}`;

        setUserAnswer(userAnswerText);
        setCorrectAnswer(correctAnswerText);
        setShowResultDialog(true);

    } catch (err) {
        console.error(err);
        alert("Error connecting to backend.");
    }
};



  const handlePegClick = (pegIndex: number) => {
    if (isAnimating) return; // Prevent clicks during animation
    
    console.log(`Peg clicked: ${pegIndex}`);
    const peg = pegs[pegIndex];

    if (!selectedDisk) {
      if (peg.length === 0) return;
      const disk = peg[peg.length - 1];
      setSelectedDisk({ pegIndex, size: disk });
      console.log(`Picked disk ${disk} from peg ${pegIndex}`);
      return;
    }

    if (selectedDisk.pegIndex === pegIndex) {
      setSelectedDisk(null);
      console.log(`Deselected disk from peg ${pegIndex}`);
      return;
    }

    const movingDisk = selectedDisk.size;
    if (peg.length > 0 && peg[peg.length - 1] < movingDisk) {
      setMoveErrorPopup(
        "Invalid move. Can't place larger disk on smaller disk."
      );
      console.log("Invalid move attempted.");
      return;
    }

    // Start animation
    setIsAnimating(true);

    // Update pegs after animation delay
    setTimeout(() => {
      setPegs((prev) => {
        const copy = prev.map((p) => [...p]);
        copy[selectedDisk.pegIndex].pop();
        copy[pegIndex].push(movingDisk);
        return copy;
      });

      console.log(
        `Moved disk ${movingDisk} from peg ${selectedDisk.pegIndex} to peg ${pegIndex}`
      );
      setSelectedDisk(null);
      setIsAnimating(false);
    }, 300); // Animation duration
  };

  const diskWidth = (diskSize: number) => {
    const base = 60;
    const step = 12;
    return base + diskSize * step;
  };

  const DISK_HEIGHT = 24;
  const DISK_GAP = 6;

  return (
    <div className="tohp-game">
      <div className="tohp-nav">
        <button
          onClick={handleBackToGames}
          className="tohp-back-btn"
        >
          ← Back to Games
        </button>
      </div>

      <div className="tohp-header">
        <h1>🗼 Tower of Hanoi</h1>
        <p>
          Move all disks to the rightmost peg. Only move one disk at a time, and never place a larger disk on a smaller one.
        </p>
        
        <div className="tsp-game-stats">
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

      {/* Game Setup Section - Full Width */}
      <div className="tohp-setup-full-width">
        <div className="tohp-card tsp-setup-card">
          <div className="tsp-card-header">
            <h3>Game Setup</h3>
            <p className="card-subtitle">Configure your puzzle</p>
          </div>
          <div className="tsp-card-content">
            <div className="setup-grid">
              <div className="setup-item">
                <label htmlFor="num-pegs" className="setup-label">
                  Number of Pegs
                </label>
                <select
                  id="num-pegs"
                  value={numPegs}
                  onChange={(e) => setNumPegs(parseInt(e.target.value, 10))}
                  className="modern-select"
                >
                  <option value={3}>3 Pegs</option>
                  <option value={4}>4 Pegs</option>
                </select>
              </div>

              <div className="setup-item">
                <label className="setup-label">
                  Disk Count
                </label>
                <div className="tohp-disk-count">{numDisks} Disks</div>
              </div>
            </div>

            <button className="tohp-new-game-btn" onClick={regenerateDisks}>
              Generate New Puzzle
            </button>
          </div>
        </div>
      </div>

      {/* Instructions Section */}
      <div className="tohp-instructions">
        <div className="instruction-content">
          <h3>How to Play</h3>
          <p>Click on the top disk of any peg to select it, then click on another peg to move it there.</p>
        </div>
      </div>

      {/* Play Area and Solution Container */}
      <div className="tohp-play-solution-container">
        {/* Play Area - Board */}
        <div className="tohp-board">
          <div className="tohp-pegs-container">
            {pegs.map((peg, pegIndex) => (
              <div
                key={pegIndex}
                className="tohp-peg-wrapper"
                onClick={() => handlePegClick(pegIndex)}
              >
                <div className="tohp-peg-slot">
                  <div className="tohp-peg" />
                  <div className="tohp-disk-stack">
                    {peg.map((diskSize, diskIndex) => {
                      const w = diskWidth(diskSize);
                      const bottomPos = diskIndex * (DISK_HEIGHT + DISK_GAP);
                      const isSelected =
                        selectedDisk?.pegIndex === pegIndex &&
                        selectedDisk?.size === diskSize &&
                        diskIndex === peg.length - 1;

                      return (
                        <div
                          key={diskIndex}
                          className={`tohp-disk ${isSelected ? "selected-disk" : ""}`}
                          style={{
                            "--disk-width": `${w}px`,
                            "--disk-bottom": `${bottomPos}px`,
                          } as React.CSSProperties}
                        >
                          {diskSize}
                        </div>
                      );
                    })}
                  </div>
                </div>
                <div className="tohp-peg-label">
                  {String.fromCharCode(65 + pegIndex)}
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Your Solution Section */}
        <form
          className="tohp-card tsp-setup-card tohp-user-inputs"
          onSubmit={handleSubmit}
        >
          <div className="tsp-card-header">
            <h3>Your Solution</h3>
            <p className="card-subtitle">Enter your answer below</p>
          </div>
          <div className="tsp-card-content">
            <div className="input-group">
              <label htmlFor="num-moves" className="input-label">
                Number of Moves
              </label>
              <div className="moves-counter">
                <button type="button" className="counter-btn" onClick={decrementMoves}>
                  −
                </button>
                <input
                  id="num-moves"
                  type="number"
                  value={userMovesCount}
                  onChange={(e) => setUserMovesCount(e.target.value)}
                  placeholder="0"
                  className="modern-input counter-input"
                  readOnly
                />
                <button type="button" className="counter-btn" onClick={incrementMoves}>
                  +
                </button>
              </div>
            </div>

            <div className="input-group">
              <label className="input-label">
                Build Move Sequence
              </label>
              <div className="move-builder">
                <div className="move-builder-grid">
                  {['A', 'B', 'C', ...(numPegs === 4 ? ['D'] : [])].map((from) =>
                    ['A', 'B', 'C', ...(numPegs === 4 ? ['D'] : [])]
                      .filter((to) => to !== from)
                      .map((to) => (
                        <button
                          key={`${from}-${to}`}
                          type="button"
                          className="move-btn"
                          onClick={() => addMoveToSequence(from, to)}
                        >
                          {from} → {to}
                        </button>
                      ))
                  )}
                </div>
                <div className="move-actions">
                  <button type="button" className="action-btn remove-btn" onClick={removeLastMove}>
                    ↶ Undo Last
                  </button>
                  <button type="button" className="action-btn clear-btn" onClick={clearMoves}>
                    ✕ Clear All
                  </button>
                </div>
              </div>
            </div>

            <div className="input-group">
              <label htmlFor="seq-moves" className="input-label">
                Move Sequence (Preview)
              </label>
              <textarea
                id="seq-moves"
                placeholder="Click buttons above to build sequence"
                value={userSequence}
                onChange={(e) => setUserSequence(e.target.value)}
                rows={3}
                className="modern-textarea"
              />
            </div>

            <button type="submit" className="tohp-check-btn">
              Check Answer
            </button>
          </div>
        </form>
      </div>

      {moveErrorPopup && (
        <div className="tohp-modal-overlay">
          <div className="tohp-modal">
            <h3>{moveErrorPopup}</h3>
            <div className="tohp-modal-actions">
              <button
                className="tohp-start-btn tohp-submit-btn"
                onClick={() => setMoveErrorPopup(null)}
              >
                OK
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Confirm Leave Dialog */}
      <ConfirmDialog
        isOpen={showConfirmDialog}
        title="Leave Game?"
        message="Are you sure you want to leave? Your current progress will be lost."
        onConfirm={handleConfirmLeave}
        onCancel={handleCancelLeave}
      />

      {/* Game Result Dialog */}
      <GameResultDialog
        isOpen={showResultDialog}
        result={currentResult}
        onNextRound={handleNextRound}
        onBackToGames={handleResultBackToGames}
        passedCount={passedRounds}
        failedCount={failedRounds}
        userAnswer={userAnswer}
        correctAnswer={correctAnswer}
      />
    </div>
  );
};

export default TOHPGame;
