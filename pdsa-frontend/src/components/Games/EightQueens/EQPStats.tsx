import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./EQPStats.css";

type PlayerSolution = {
  solutionID: number;
  dateFound: string;
  solution_Text: string;
};

type LeaderboardEntry = {
  playerName: string;
  solutionsFound: number;
  firstSolutionDate: string;
};

type AlgorithmTime = {
  algorithmType: string;
  timeTaken_ms: number;
  roundNumber: number;
  dateExecuted: string;
};

const API_BASE = "https://3.109.143.222";

const EQPStats: React.FC = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState<'history' | 'leaderboard'>('leaderboard');
  const [playerHistory, setPlayerHistory] = useState<PlayerSolution[]>([]);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [algorithmTimes, setAlgorithmTimes] = useState<AlgorithmTime[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>('');
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  useEffect(() => {
    if (activeTab === 'history') {
      loadPlayerHistory();
    } else if (activeTab === 'leaderboard') {
      loadLeaderboard();
    }
  }, [activeTab]);

  const loadPlayerHistory = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/api/eqp/player-solutions/${encodeURIComponent(playerName)}`);
      if (res.ok) {
        const data = await res.json();
        setPlayerHistory(data);
      } else {
        setError('Failed to load player history');
      }
    } catch (err) {
      setError('Failed to load player history');
      console.error("Failed to load player history:", err);
    } finally {
      setLoading(false);
    }
  };

  const loadLeaderboard = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/api/eqp/leaderboard?top=20`);
      if (res.ok) {
        const data = await res.json();
        setLeaderboard(data);
      } else {
        setError('Failed to load leaderboard');
      }
    } catch (err) {
      setError('Failed to load leaderboard');
      console.error("Failed to load leaderboard:", err);
    } finally {
      setLoading(false);
    }
  };

  const loadAlgorithmTimes = async () => {
    setLoading(true);
    setError('');
    try {
      const res = await fetch(`${API_BASE}/api/eqp/algorithm-times?count=20`);
      if (res.ok) {
        const data = await res.json();
        setAlgorithmTimes(data);
      } else {
        setError('Failed to load algorithm times');
      }
    } catch (err) {
      setError('Failed to load algorithm times');
      console.error("Failed to load algorithm times:", err);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  };

  const formatSolution = (solutionText: string) => {
    // Convert "0-0,1-4,2-7,..." to more readable format
    const positions = solutionText.split(',');
    return positions.map(pos => {
      const [row, col] = pos.split('-');
      return `R${parseInt(row) + 1}C${parseInt(col) + 1}`;
    }).join(', ');
  };

  return (
    <div className="eqp-stats">
      <div className="eqp-stats-nav">
        <button onClick={() => navigate('/')} className="eqp-stats-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/eqp')} className="eqp-stats-play-btn">
          Play Game
        </button>
      </div>

      <div className="eqp-stats-header">
        <h1>Eight Queens Statistics</h1>
        <p>View your solutions, leaderboard, and algorithm performance</p>
      </div>

      <div className="eqp-stats-tabs">
        <button
          className={`tab-btn ${activeTab === 'history' ? 'active' : ''}`}
          onClick={() => setActiveTab('history')}
        >
          My Solutions
        </button>
        <button
          className={`tab-btn ${activeTab === 'leaderboard' ? 'active' : ''}`}
          onClick={() => setActiveTab('leaderboard')}
        >
          Leaderboard
        </button>
      </div>

      {error && (
        <div className="eqp-stats-error">
          <p>{error}</p>
        </div>
      )}

      {loading ? (
        <div className="eqp-stats-loading">
          <div className="spinner"></div>
          <p>Loading...</p>
        </div>
      ) : (
        <>
          {activeTab === 'history' && (
            <div className="eqp-stats-content">
              <div className="player-info-card">
                <h2>Player: {playerName}</h2>
                <p>Total Solutions Found: {playerHistory.length}</p>
              </div>

              {playerHistory.length === 0 ? (
                <div className="no-data">
                  <p>No solutions found yet. Play the game to find solutions!</p>
                </div>
              ) : (
                <div className="history-list">
                  {playerHistory.map((solution) => (
                    <div key={solution.solutionID} className="history-card">
                      <div className="history-header">
                        <h3>Solution #{solution.solutionID}</h3>
                        <span className="history-date">{formatDate(solution.dateFound)}</span>
                      </div>
                      <div className="history-details">
                        <div className="detail-row">
                          <span className="detail-label">Queen Positions:</span>
                          <span className="detail-value route">{formatSolution(solution.solution_Text)}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {activeTab === 'leaderboard' && (
            <div className="eqp-stats-content">
              {leaderboard.length === 0 ? (
                <div className="no-data">
                  <p>No leaderboard data available yet.</p>
                </div>
              ) : (
                <div className="leaderboard-table">
                  <table>
                    <thead>
                      <tr>
                        <th>Rank</th>
                        <th>Player</th>
                        <th>Solutions Found</th>
                        <th>First Solution</th>
                      </tr>
                    </thead>
                    <tbody>
                      {leaderboard.map((entry, index) => (
                        <tr
                          key={index}
                          className={entry.playerName === playerName ? 'current-player' : ''}
                        >
                          <td className="rank">
                            {index === 0 && '🥇'}
                            {index === 1 && '🥈'}
                            {index === 2 && '🥉'}
                            {index > 2 && `#${index + 1}`}
                          </td>
                          <td className="player-name">
                            {entry.playerName}
                            {entry.playerName === playerName && ' (You)'}
                          </td>
                          <td className="best-distance">{entry.solutionsFound}</td>
                          <td>{formatDate(entry.firstSolutionDate)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          )}


        </>
      )}
    </div>
  );
};

export default EQPStats;
