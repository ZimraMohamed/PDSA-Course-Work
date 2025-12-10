import React, { useState, useEffect } from "react";
import "./TOHPGame.css";

const TOHPGame: React.FC = () => {
  const [numPegs, setNumPegs] = useState<number>(3);
  const [numDisks, setNumDisks] = useState<number>(
    Math.floor(Math.random() * 6) + 5
  ); // 5-10
  const [userMovesCount, setUserMovesCount] = useState<string>("");
  const [userSequence, setUserSequence] = useState<string>("");
  const [result, setResult] = useState<string | null>(null);

  const [showNamePopup, setShowNamePopup] = useState<boolean>(false);
  const [playerName, setPlayerName] = useState<string>("");
  const [nameSubmitted, setNameSubmitted] = useState<boolean>(false);

  useEffect(() => {
    // ensure disk count stays in range if regenerated elsewhere
    if (numDisks < 5) setNumDisks(5);
    if (numDisks > 10) setNumDisks(10);
  }, [numDisks]);

  const regenerateDisks = () => {
    setNumDisks(Math.floor(Math.random() * 6) + 5);
    setResult(null);
    setUserMovesCount("");
    setUserSequence("");
  };

  // computeOptimalMoves: returns minimal moves for given pegs and disks
  const computeOptimalMoves = (pegsNum: number, disks: number): number => {
    if (disks <= 0) return 0;
    if (pegsNum <= 3) {
      return Math.pow(2, disks) - 1;
    }

    const M: number[] = new Array(disks + 1).fill(0);
    M[0] = 0;
    for (let i = 1; i <= disks; i++) {
      let best = Math.pow(2, i) - 1;
      for (let k = 1; k < i; k++) {
        const candidate = 2 * M[k] + Math.pow(2, i - k) - 1;
        if (candidate < best) best = candidate;
      }
      M[i] = best;
    }
    return M[disks];
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Parse user moves count
    const userMoves = parseInt(userMovesCount, 10);
    if (isNaN(userMoves)) {
      setResult("Please enter a valid number of moves.");
      return;
    }
    const optimal = computeOptimalMoves(numPegs, numDisks);
    if (userMoves === optimal) {
      setResult("Correct");
      setPlayerName("");
      setShowNamePopup(true);
    } else {
      setResult(`Ooops! Correct Answer is ${optimal}`);
    }
  };

  const handleNameSubmit = (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    if (!playerName.trim()) return;
    // TODO: send to leaderboard/backend if desired. For now store locally.
    console.log("TOHP: Player submitted name:", playerName);
    setNameSubmitted(true);
    setShowNamePopup(false);
    setResult(`Correct — ${playerName} submitted`);
  };

  const handleCancelName = () => {
    setShowNamePopup(false);
  };

  return (
    <div className="tohp-game">
      <div className="tohp-nav">
        <button
          onClick={() => (window.location.href = "/")}
          className="tohp-back-btn"
        >
          ← Back to Games
        </button>
      </div>

      <div className="tohp-header">
        <h1>🗼 Tower of Hanoi</h1>
        <p>
          Move the stack of disks from the left peg to the right peg following
          the rules: only one disk may be moved at a time, and a larger disk
          cannot be placed on top of a smaller disk.
        </p>
        <button className="tohp-new-game-btn" onClick={regenerateDisks}>
          New Game
        </button>
      </div>

      <div className="tohp-setup">
        <div className="tohp-card tsp-setup-card">
          <div className="tsp-card-header">
            <h3>Game Setup</h3>
          </div>
          <div className="tsp-card-content">
            <div className="tsp-input-row">
              <label htmlFor="num-pegs">Number of Pegs:</label>
              <select
                id="num-pegs"
                value={numPegs}
                onChange={(e) => setNumPegs(parseInt(e.target.value, 10))}
              >
                <option value={3}>3</option>
                <option value={4}>4</option>
              </select>
            </div>

            <div className="tsp-input-row">
              <label>Disk Count (random):</label>
              <div className="tohp-disk-count">{numDisks}</div>
            </div>
          </div>
        </div>

        <form
          className="tohp-card tsp-setup-card tohp-user-inputs"
          onSubmit={handleSubmit}
        >
          <div className="tsp-card-header">
            <h3>Your Answer</h3>
          </div>
          <div className="tsp-card-content">
            <div className="tsp-input-row">
              <label htmlFor="num-moves">Number of moves:</label>
              <input
                id="num-moves"
                type="number"
                min={1}
                value={userMovesCount}
                onChange={(e) => setUserMovesCount(e.target.value)}
              />
            </div>

            <div className="tsp-input-row">
              <label htmlFor="seq-moves">Sequence of moves:</label>
              <textarea
                id="seq-moves"
                placeholder="e.g., A→C, A→B, B→C"
                value={userSequence}
                onChange={(e) => setUserSequence(e.target.value)}
                rows={3}
              />
            </div>

            <div className="tsp-input-row">
              <button type="submit" className="tohp-start-btn tohp-check-btn">
                Check Answer
              </button>
            </div>

            {result && (
              <div
                className={`tohp-result ${
                  result.startsWith("Correct") ? "correct" : "incorrect"
                }`}
              >
                {result}
              </div>
            )}
          </div>
        </form>
      </div>

      {showNamePopup && (
        <div className="tohp-modal-overlay">
          <div className="tohp-modal">
            <h3>Congratulations! Wanna tell us your name?</h3>
            <form onSubmit={handleNameSubmit}>
              <input
                type="text"
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                placeholder="Your name"
                className="tohp-name-input"
              />
              <div className="tohp-modal-actions">
                <button
                  type="submit"
                  className="tohp-start-btn tohp-submit-btn"
                >
                  Submit
                </button>
                <button
                  type="button"
                  className="tohp-start-btn tohp-cancel-btn"
                  onClick={handleCancelName}
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default TOHPGame;
