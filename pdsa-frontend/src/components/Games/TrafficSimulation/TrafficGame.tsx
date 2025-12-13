import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./TrafficGame.css";
import axios from "axios";
import ConfirmDialog from "../../Common/ConfirmDialog";
import GameResultDialog from "../../Common/GameResultDialog";

// Type for capacities
type CapacityMap = Record<string, number>;

// Backend Response Type
interface ApiResponse {
  MaxFlow?: number;
  ExecutionTimeMs?: number;

  correctAnswer?: number;
  correct_answer?: number;

  playerAnswer?: number;
  player_answer?: number;
  player_guess?: number;

  status?: string;
}

// Normalized Result
interface TrafficResult {
  playerAnswer: number;
  correctAnswer: number;
  timeTaken: number;
  status: string;
}

const edges: [string, string][] = [
  ["A", "B"],
  ["A", "C"],
  ["A", "D"],
  ["B", "E"],
  ["B", "F"],
  ["C", "E"],
  ["C", "F"],
  ["D", "F"],
  ["E", "G"],
  ["E", "H"],
  ["F", "H"],
  ["G", "T"],
  ["H", "T"],
];

export default function TrafficGame() {
  const navigate = useNavigate();
  const [capacities, setCapacities] = useState<CapacityMap>({});
  const [playerAnswer, setPlayerAnswer] = useState("");
  const [result, setResult] = useState<TrafficResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [gameStarted, setGameStarted] = useState(false);
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail' | 'draw'>('pass');

  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  // Generate random capacities 5–15
  const generateRandomCapacities = () => {
    const cap: CapacityMap = {};
    edges.forEach(([u, v]) => {
      cap[`${u}-${v}`] = Math.floor(Math.random() * 11) + 5;
    });
    setCapacities(cap);
    setResult(null);
    setPlayerAnswer("");
    setGameStarted(true);
  };

  useEffect(() => {
    generateRandomCapacities();
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
    generateRandomCapacities();
  };

  const handleResultBackToGames = () => {
    setShowResultDialog(false);
    navigate('/');
  };

  // ------------ SUBMIT ANSWER (Updated PascalCase Payload) ------------
  const submitAnswer = async () => {
    if (!playerAnswer || playerAnswer.trim() === "") {
      alert("Please enter your answer.");
      return;
    }

    setLoading(true);
    try {
      const payload = {
        Edges: edges.map(([u, v]) => ({
          From: u,
          To: v,
          Capacity: capacities[`${u}-${v}`] ?? 0,
        })),
        PlayerAnswer: Number(playerAnswer),
        PlayerName: playerName,
      };

      const response = await axios.post<ApiResponse>(
        "http://localhost:5007/api/traffic/maxflow",
        payload
      );

      const d = response.data;

      const normalized: TrafficResult = {
        playerAnswer:
          d.playerAnswer ??
          d.player_answer ??
          d.player_guess ??
          Number(playerAnswer),

        correctAnswer:
          d.correctAnswer ??
          d.correct_answer ??
          d.MaxFlow ??
          0,

        timeTaken: d.ExecutionTimeMs ?? 0,

        status:
          d.status ??
          (Number(playerAnswer) ===
          (d.correctAnswer ?? d.MaxFlow ?? 0)
            ? "Correct"
            : "Wrong"),
      };

      setResult(normalized);

      // Update game progress
      const isCorrect = normalized.status === "Correct";
      if (isCorrect) {
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
      console.error("submitAnswer error:", err);
      alert("Backend error — open console for details.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="traffic-game">
      <div className="traffic-nav">
        <button onClick={handleBackToGames} className="traffic-back-btn">
          ← Back to Games
        </button>
      </div>

      <div className="traffic-header">
        <h1>🚦 Traffic Simulation – Maximum Flow</h1>
        <p>Find the maximum flow from node A to node T in the traffic network!</p>
        
        <div className="traffic-game-stats">
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

      <div className="traffic-content">
        {gameStarted && (
          <>
            <div className="traffic-main-section">
              <div className="traffic-left-panel">
                <div className="panel-header">
                  <h3>Traffic Network</h3>
                </div>
                <div className="graph-container">
                  {edges.map(([u, v]) => (
                    <div key={u + v} className="edge-card">
                      <div className="edge-nodes">
                        <span className="node-label">{u}</span>
                        <span className="edge-arrow">→</span>
                        <span className="node-label">{v}</span>
                      </div>
                      <span className="capacity-value">{capacities[`${u}-${v}`]}</span>
                    </div>
                  ))}
                </div>
              </div>

              <div className="traffic-right-panel">
                <div className="traffic-input-section">
                  <h3>Your Answer</h3>
                  <p className="input-prompt">Enter maximum flow (A → T):</p>
                  <div className="input-group">
                    <input
                      type="number"
                      value={playerAnswer}
                      onChange={(e) => setPlayerAnswer(e.target.value)}
                      placeholder="Enter flow..."
                      className="flow-input"
                      disabled={loading || result !== null}
                    />
                    <button 
                      onClick={submitAnswer} 
                      disabled={loading || !playerAnswer || result !== null}
                      className="submit-btn"
                    >
                      {loading ? 'Calculating...' : 'Submit Answer'}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </>
        )}

        {!gameStarted && (
          <div className="traffic-start-screen">
            <button onClick={generateRandomCapacities} className="start-game-btn" disabled={loading}>
              🎮 Start Game
            </button>
          </div>
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
        userAnswer={result?.playerAnswer}
        correctAnswer={result?.correctAnswer}
      />
    </div>
  );
}
