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
  correctAnswer?: number;
  playerAnswer?: number;
  edmondsKarpTime?: number;
  dinicTime?: number;
  status?: string;
  message?: string;
}

// Normalized Result
interface TrafficResult {
  playerAnswer: number;
  correctAnswer: number;
  edmondsKarpTime: number;
  dinicTime: number;
  status: string;
  message?: string;
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

  // ------------ SUBMIT ANSWER (Updated to use submit-answer endpoint) ------------
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
        "http://localhost:5007/api/traffic/submit-answer",
        payload
      );

      const d = response.data;

      const normalized: TrafficResult = {
        playerAnswer: d.playerAnswer ?? Number(playerAnswer),
        correctAnswer: d.correctAnswer ?? 0,
        edmondsKarpTime: d.edmondsKarpTime ?? 0,
        dinicTime: d.dinicTime ?? 0,
        status: d.status ?? "Unknown",
        message: d.message
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
        <button onClick={() => navigate('/games/traffic-simulation/stats')} className="traffic-stats-btn">
          View Statistics
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
                  <div className="header-legend">
                    <div className="legend-item">
                      <div className="legend-icon node-icon source"></div>
                      <span>Source/Target</span>
                    </div>
                    <div className="legend-item">
                      <div className="legend-icon node-icon intermediate"></div>
                      <span>Intermediate Node</span>
                    </div>
                    <div className="legend-item">
                      <div className="legend-icon capacity-icon"></div>
                      <span>Edge Capacity</span>
                    </div>
                  </div>
                </div>
                <div className="network-visualization">
                  <svg viewBox="0 0 900 550" className="network-svg" preserveAspectRatio="xMidYMid meet">
                    {/* Define arrow markers */}
                    <defs>
                      <marker id="arrowhead" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto">
                        <polygon points="0 0, 10 3, 0 6" fill="#06b6d4" />
                      </marker>
                    </defs>
                    
                    {/* Node positions */}
                    {(() => {
                      const nodePositions: Record<string, [number, number]> = {
                        'A': [80, 325],
                        'B': [250, 150],
                        'C': [250, 325],
                        'D': [250, 500],
                        'E': [450, 200],
                        'F': [450, 450],
                        'G': [650, 250],
                        'H': [650, 400],
                        'T': [820, 325]
                      };
                      
                      return (
                        <>
                          {/* Draw edges */}
                          {edges.map(([u, v]) => {
                            const [x1, y1] = nodePositions[u];
                            const [x2, y2] = nodePositions[v];
                            const midX = (x1 + x2) / 2;
                            const midY = (y1 + y2) / 2;
                            
                            return (
                              <g key={`${u}-${v}`}>
                                <line
                                  x1={x1}
                                  y1={y1}
                                  x2={x2}
                                  y2={y2}
                                  stroke="#06b6d4"
                                  strokeWidth="3"
                                  markerEnd="url(#arrowhead)"
                                  className="network-edge"
                                />
                                <circle
                                  cx={midX}
                                  cy={midY}
                                  r="18"
                                  fill="#f97316"
                                  className="capacity-circle"
                                />
                                <text
                                  x={midX}
                                  y={midY + 5}
                                  textAnchor="middle"
                                  fill="white"
                                  fontSize="14"
                                  fontWeight="bold"
                                  className="capacity-text"
                                >
                                  {capacities[`${u}-${v}`]}
                                </text>
                              </g>
                            );
                          })}
                          
                          {/* Draw nodes */}
                          {Object.entries(nodePositions).map(([node, [x, y]]) => (
                            <g key={node}>
                              <circle
                                cx={x}
                                cy={y}
                                r="35"
                                fill={node === 'A' || node === 'T' ? '#667eea' : '#ffffff'}
                                stroke={node === 'A' || node === 'T' ? '#764ba2' : '#06b6d4'}
                                strokeWidth="3"
                                className="network-node"
                              />
                              <text
                                x={x}
                                y={y + 6}
                                textAnchor="middle"
                                fill={node === 'A' || node === 'T' ? '#ffffff' : '#1f2937'}
                                fontSize="20"
                                fontWeight="bold"
                              >
                                {node}
                              </text>
                              {node === 'A' && (
                                <text x={x} y={y + 55} textAnchor="middle" fontSize="12" fill="#667eea" fontWeight="600">Source</text>
                              )}
                              {node === 'T' && (
                                <text x={x} y={y + 55} textAnchor="middle" fontSize="12" fill="#667eea" fontWeight="600">Target</text>
                              )}
                            </g>
                          ))}
                        </>
                      );
                    })()}
                  </svg>
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
