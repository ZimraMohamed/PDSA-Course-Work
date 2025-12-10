import { useState, useEffect } from "react";
import "./TrafficGame.css";
import axios from "axios";

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
  const [capacities, setCapacities] = useState<CapacityMap>({});
  const [playerAnswer, setPlayerAnswer] = useState("");
  const [result, setResult] = useState<TrafficResult | null>(null);
  const [loading, setLoading] = useState(false);

  // Generate random capacities 5–15
  const generateRandomCapacities = () => {
    const cap: CapacityMap = {};
    edges.forEach(([u, v]) => {
      cap[`${u}-${v}`] = Math.floor(Math.random() * 11) + 5;
    });
    setCapacities(cap);
  };

  useEffect(() => {
    generateRandomCapacities();
  }, []);

  // ------------ SUBMIT ANSWER (Updated PascalCase Payload) ------------
  const submitAnswer = async () => {
    setLoading(true);
    try {
      const payload = {
        Edges: edges.map(([u, v]) => ({
          From: u,
          To: v,
          Capacity: capacities[`${u}-${v}`] ?? 0,
        })),
        PlayerAnswer: Number(playerAnswer),
        PlayerName: "Player 1",
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
    } catch (err) {
      console.error("submitAnswer error:", err);
      alert("Backend error — open console for details.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="traffic-container">
      <h1>Traffic Simulation – Maximum Flow Game</h1>

      <button onClick={generateRandomCapacities}>New Game Round</button>

      <div className="graph-container">
        {edges.map(([u, v]) => (
          <div key={u + v} className="edge-box">
            {u} → {v} : <strong>{capacities[`${u}-${v}`]}</strong>
          </div>
        ))}
      </div>

      <div className="input-box">
        <label>Enter Maximum Flow from A to T:</label>
        <input
          type="number"
          value={playerAnswer}
          onChange={(e) => setPlayerAnswer(e.target.value)}
        />
        <button onClick={submitAnswer} disabled={loading}>
          Submit
        </button>
      </div>

      {result && (
        <div className="result-box">
          <h2>Results</h2>
          <p>Your Answer: {result.playerAnswer}</p>
          <p>Correct Maximum Flow: {result.correctAnswer}</p>
          <p>Time Taken (ms): {result.timeTaken}</p>
          <p>Status: {result.status}</p>
        </div>
      )}
    </div>
  );
}
