import React, { useEffect, useState } from "react";
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

  type PegState = number[][];
  const [pegs, setPegs] = useState<PegState>([]);

  const [moveErrorPopup, setMoveErrorPopup] = useState<string | null>(null);

  // selected disk = { pegIndex, size, fromIndex } where fromIndex is top index in peg array
  const [selectedDisk, setSelectedDisk] = useState<{
    pegIndex: number;
    size: number;
  } | null>(null);

  useEffect(() => {
    // keep disk count within range
    if (numDisks < 5) setNumDisks(5);
    if (numDisks > 10) setNumDisks(10);
  }, [numDisks]);

  // Initialize pegs whenever numPegs or numDisks changes
  useEffect(() => {
    const initialPegs: PegState = Array.from({ length: numPegs }, () => []);
    // Representation: peg array has elements ordered bottom -> top,
    // and the top (movable) disk is the last element: peg[peg.length - 1]
    const startingPeg: number[] = [];
    for (let d = numDisks; d >= 1; d--) startingPeg.push(d); // largest ... smallest (1 is smallest at the end)
    initialPegs[0] = startingPeg;
    setPegs(initialPegs);
    setSelectedDisk(null);
  }, [numPegs, numDisks]);

  const regenerateDisks = () => {
    const newCount = Math.floor(Math.random() * 6) + 5;
    setNumDisks(newCount);
    setResult(null);
    setUserMovesCount("");
    setUserSequence("");
    // effect above will reinitialize pegs
  };

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
      setResult(
        `Ooops! Your Answer Is ${userMoves}. Correct Answer is ${optimal}`
      );
    }
  };

  const handleNameSubmit = (e?: React.FormEvent) => {
    if (e) e.preventDefault();
    if (!playerName.trim()) return;
    console.log("TOHP: Player submitted name:", playerName);
    setNameSubmitted(true);
    setShowNamePopup(false);
    setResult(`Correct — ${playerName}'s Answer Saved!`);
  };

  const handleCancelName = () => {
    setShowNamePopup(false);
  };

  // Click-to-move: click a peg to pick top disk, click another peg to drop
  const handlePegClick = (pegIndex: number) => {
    const peg = pegs[pegIndex];

    // If nothing selected, pick top disk
    if (!selectedDisk) {
      if (peg.length === 0) return; // nothing to pick
      const disk = peg[peg.length - 1];
      setSelectedDisk({ pegIndex, size: disk });
      return;
    }

    // If clicking same peg -> deselect
    if (selectedDisk.pegIndex === pegIndex) {
      setSelectedDisk(null);
      return;
    }

    // Attempt move from selectedDisk.pegIndex -> pegIndex
    const movingDisk = selectedDisk.size;

    if (peg.length > 0) {
      const topDisk = peg[peg.length - 1];
      if (topDisk < movingDisk) {
        // invalid
        // we keep selection for convenience (or you can clear selection)
        setMoveErrorPopup(
          "Invalid move. Can't place larger disk on smaller disk."
        );
        return;
      }
    }

    // perform move (immutable update)
    setPegs((prev) => {
      const copy = prev.map((p) => [...p]);
      copy[selectedDisk.pegIndex].pop();
      copy[pegIndex].push(movingDisk);
      return copy;
    });

    setSelectedDisk(null);
  };

  // helpers for rendering
  const diskWidth = (diskSize: number) => {
    // diskSize ranges 1..numDisks, make width proportional
    // base width for smallest (1) and scale up
    const base = 60;
    const step = 12;
    return base + diskSize * step;
  };

  // constants for stacking (must match CSS heights)
  const DISK_HEIGHT = 24; // px
  const DISK_GAP = 6; // px

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
          </div>
        </form>
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

      {/* Unified board */}
      <div className="tohp-board">
        <div className="tohp-pegs-container">
          {pegs.map((peg, pegIndex) => (
            <div
              key={pegIndex}
              className="tohp-peg-wrapper"
              onClick={() => handlePegClick(pegIndex)}
            >
              <div className="tohp-peg-label">
                {String.fromCharCode(65 + pegIndex)}
              </div>
              <div
                className={`tohp-peg-slot ${
                  selectedDisk?.pegIndex === pegIndex ? "selected-slot" : ""
                }`}
              >
                <div className="tohp-peg"></div>
                <div
                  className="tohp-disk-stack"
                  style={{ position: "relative" }}
                >
                  {peg.map((diskSize, idx) => {
                    const bottom = idx * (DISK_HEIGHT + DISK_GAP);
                    const isTop = idx === peg.length - 1;
                    const isSelected =
                      selectedDisk?.pegIndex === pegIndex &&
                      selectedDisk.size === diskSize &&
                      isTop;
                    return (
                      <div
                        key={idx}
                        className={`tohp-disk ${isTop ? "top-disk" : ""} ${
                          isSelected ? "selected-disk" : ""
                        }`}
                        style={{
                          width: `${diskWidth(diskSize)}px`,
                          bottom: `${bottom}px`,
                          height: `${DISK_HEIGHT}px`,
                          lineHeight: `${DISK_HEIGHT}px`,
                          position: "absolute",
                        }}
                        title={`Disk ${diskSize}`}
                      >
                        {diskSize}
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default TOHPGame;
