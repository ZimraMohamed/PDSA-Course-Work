import React, { useEffect, useState } from "react";
import { useNavigate } from 'react-router-dom';
import ConfirmDialog from '../../Common/ConfirmDialog';
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

  // Update move count based on sequence
  useEffect(() => {
    const moves = userSequence.split(',').map(s => s.trim()).filter(s => s.length > 0);
    setUserMovesCount(String(moves.length));
  }, [userSequence]);

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

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Scroll to bottom to show result section
    setTimeout(() => {
      window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    }, 300);

    console.log("Submitting moves count:", userMovesCount);
    console.log("Submitting sequence:", userSequence);

    try {
      const res = await fetch("/api/TOHP/submit-answer", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          numPegs,
          numDisks,
          userMovesCount: parseInt(userMovesCount, 10),
          userSequence,
          playerName
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

    // Get peg labels (A, B, C, D)
    const fromPegLabel = String.fromCharCode(65 + selectedDisk.pegIndex);
    const toPegLabel = String.fromCharCode(65 + pegIndex);

    // Update pegs after animation delay
    setTimeout(() => {
      setPegs((prev) => {
        const copy = prev.map((p) => [...p]);
        copy[selectedDisk.pegIndex].pop();
        copy[pegIndex].push(movingDisk);
        return copy;
      });

      // Automatically update sequence
      setUserSequence((prevSequence) => {
        const move = `${fromPegLabel}→${toPegLabel}`;
        return prevSequence ? `${prevSequence}, ${move}` : move;
      });

      setRecordedMoves((prev) => [...prev, `${fromPegLabel}→${toPegLabel}`]);

      console.log(
        `Moved disk ${movingDisk} from peg ${selectedDisk.pegIndex} (${fromPegLabel}) to peg ${pegIndex} (${toPegLabel})`
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
        <button onClick={() => navigate('/games/tohp/stats')} className="tohp-stats-btn">
          View Statistics
        </button>
      </div>

      <div className="tohp-header">
        <h1>Tower of Hanoi</h1>
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
                Number of Moves (Auto-tracked)
              </label>
              <input
                id="num-moves"
                type="number"
                value={userMovesCount}
                placeholder="0"
                className="modern-input"
                readOnly
                style={{ textAlign: 'center', fontSize: '1.1rem', fontWeight: '600' }}
              />
            </div>

            <div className="input-group">
              <label htmlFor="seq-moves" className="input-label">
                Move Sequence
              </label>
              <textarea
                id="seq-moves"
                placeholder="Move disks between pegs or enter sequence manually (e.g., A→C, B→A)"
                value={userSequence}
                onChange={(e) => setUserSequence(e.target.value.replace(/\s*-\s*/g, ' → '))}
                rows={3}
                className="modern-textarea"
              />
              <small className="input-hint">Accepted formats: A - C, B - A or A → C, B → A</small>
            </div>

            <button
              type="submit"
              className="tohp-check-btn"
              disabled={!userMovesCount.trim() || !userSequence.trim()}
            >
              Check Answer
            </button>
          </div>
        </form>
      </div>

      {/* Result Section */}
      {showResultDialog && (
        <div className="tohp-result-section">
          <div className={`result-card result-${currentResult}`}>
            <div className="result-header">
              <div className="result-icon">
                {currentResult === 'pass' ? '✓' : currentResult === 'fail' ? '✕' : '='}
              </div>
              <h3 className="result-title">
                {currentResult === 'pass' ? 'Correct!' : currentResult === 'fail' ? 'Incorrect' : 'Draw'}
              </h3>
            </div>

            <div className="result-answers">
              <div className="answer-section">
                <h4 className="answer-label">Your Answer</h4>
                <p className="answer-text">{userAnswer}</p>
              </div>
              <div className="answer-section">
                <h4 className="answer-label">Correct Answer</h4>
                <p className="answer-text correct">{correctAnswer}</p>
              </div>
            </div>

            <div className="result-actions">
              <button className="result-btn next-round-btn" onClick={handleNextRound}>
                Next Round
              </button>
              <button className="result-btn back-btn" onClick={handleResultBackToGames}>
                Back to Games
              </button>
            </div>
          </div>
        </div>
      )}

      {moveErrorPopup && (
        <div className="tohp-modal-overlay" onClick={() => setMoveErrorPopup(null)}>
          <div className="tohp-error-modal" onClick={(e) => e.stopPropagation()}>
            <div className="error-icon">⚠️</div>
            <h3 className="error-title">Invalid Move</h3>
            <p className="error-message">{moveErrorPopup}</p>
            <button
              className="error-ok-btn"
              onClick={() => setMoveErrorPopup(null)}
            >
              Got it!
            </button>
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

    </div>
  );
};

export default TOHPGame;
