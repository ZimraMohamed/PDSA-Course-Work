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

  type PegState = number[][];
  const [pegs, setPegs] = useState<PegState>([]);

  const [moveErrorPopup, setMoveErrorPopup] = useState<string | null>(null);

  const [selectedDisk, setSelectedDisk] = useState<{
    pegIndex: number;
    size: number;
  } | null>(null);

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

  const regenerateDisks = () => {
    const newCount = Math.floor(Math.random() * 6) + 5;
    setNumDisks(newCount);
    setResult(null);
    setUserMovesCount("");
    setUserSequence("");
    console.log(`New game generated with ${newCount} disks.`);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate mandatory fields
    if (!userMovesCount.trim() || !userSequence.trim()) {
        setResult("Please fill in both Number of Moves and Sequence of Moves.");
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
            setResult(errData.message || "Backend returned an error.");
            return;
        }

        const data = await res.json();
        console.log("Backend response:", data);

        // Handle all four situations
        let displayMessage = "";

        if (data.correctMoves && data.correctSequence) {
            displayMessage = "Correct! Both move count and sequence are correct.";
        } else if (!data.correctMoves && !data.correctSequence) {
            displayMessage = `Ooops! Move count and sequence are both wrong.\n`;
            displayMessage += `Your moves: ${userMovesCount}, Optimal: ${data.optimalMoves}\n`;
            displayMessage += `Correct sequence: ${data.correctSequenceList || data.correctSequence}`;
        } else if (!data.correctMoves && data.correctSequence) {
            displayMessage = `Ooops! Move count is wrong but sequence is correct.\n`;
            displayMessage += `Your moves: ${userMovesCount}, Optimal: ${data.optimalMoves}`;
        } else if (data.correctMoves && !data.correctSequence) {
            displayMessage = `Ooops! Move count is correct but sequence is wrong.\n`;
            displayMessage += `Correct sequence: ${data.correctSequenceList || data.correctSequence}`;
        }

        setResult(displayMessage);

    } catch (err) {
        console.error(err);
        setResult("Error connecting to backend.");
    }
};



  const handlePegClick = (pegIndex: number) => {
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
