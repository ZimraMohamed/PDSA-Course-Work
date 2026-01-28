import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './TrafficStats.css';

interface AlgorithmTime {
  algorithmName: string;
  timeTakenMs: number;
}

interface Capacity {
  roadSegment: string;
  capacity: number;
}

interface RoundHistory {
  roundId: number;
  correctMaxFlow: number;
  datePlayed: string;
  algorithmTimes: AlgorithmTime[];
  capacities: Capacity[];
}

interface LeaderboardEntry {
  playerName: string;
  totalGames: number;
  averageMaxFlow: number;
  bestMaxFlow: number;
  lastPlayed: string;
}

const TrafficStats: React.FC = () => {
  const navigate = useNavigate();
  const [playerHistory, setPlayerHistory] = useState<RoundHistory[]>([]);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState<'history' | 'leaderboard'>('history');

  const API_BASE_URL = 'http://localhost:5007s';
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  useEffect(() => {
    loadData();
  }, [activeTab]);

  const loadData = async () => {
    setLoading(true);
    setError('');

    try {
      if (activeTab === 'history') {
        await loadPlayerHistory();
      } else {
        await loadLeaderboard();
      }
    } catch (err) {
      setError('Failed to load data. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const loadPlayerHistory = async () => {
    const response = await fetch(`${API_BASE_URL}/api/traffic/player-history/${encodeURIComponent(playerName)}`);

    if (!response.ok) throw new Error('Failed to load player history');

    const data = await response.json();
    setPlayerHistory(data);
  };

  const loadLeaderboard = async () => {
    const response = await fetch(`${API_BASE_URL}/api/traffic/leaderboard?top=20`);

    if (!response.ok) throw new Error('Failed to load leaderboard');

    const data = await response.json();
    setLeaderboard(data);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  };

  return (
    <div className="traffic-stats">
      <div className="traffic-stats-nav">
        <button onClick={() => navigate('/')} className="traffic-stats-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/traffic-simulation')} className="traffic-stats-play-btn">
          Play Game
        </button>
      </div>

      <div className="traffic-stats-header">
        <h1>Traffic Simulation Statistics</h1>
        <p>View game history and leaderboard</p>
      </div>

      <div className="traffic-stats-tabs">
        <button
          className={`tab-btn ${activeTab === 'history' ? 'active' : ''}`}
          onClick={() => setActiveTab('history')}
        >
          My History
        </button>
        <button
          className={`tab-btn ${activeTab === 'leaderboard' ? 'active' : ''}`}
          onClick={() => setActiveTab('leaderboard')}
        >
          Leaderboard
        </button>
      </div>

      {error && (
        <div className="traffic-stats-error">
          <p>{error}</p>
        </div>
      )}

      {loading ? (
        <div className="traffic-stats-loading">
          <div className="spinner"></div>
          <p>Loading...</p>
        </div>
      ) : (
        <>
          {activeTab === 'history' && (
            <div className="traffic-stats-content">
              <div className="player-info-card">
                <h2>Player: {playerName}</h2>
                <p>Total Games Played: {playerHistory.length}</p>
              </div>

              {playerHistory.length === 0 ? (
                <div className="no-data">
                  <p>No game history found. Play some games to see your statistics!</p>
                </div>
              ) : (
                <div className="history-list">
                  {playerHistory.map((round) => (
                    <div key={round.roundId} className="history-card">
                      <div className="history-header">
                        <h3>Round #{round.roundId}</h3>
                        <span className="history-date">{formatDate(round.datePlayed)}</span>
                      </div>

                      <div className="history-details">
                        <div className="detail-row highlight">
                          <span className="detail-label">Maximum Flow:</span>
                          <span className="detail-value max-flow">{round.correctMaxFlow} vehicles/min</span>
                        </div>

                        <div className="algorithm-times">
                          <h4>Algorithm Performance:</h4>
                          <div className="algo-grid">
                            {round.algorithmTimes.map((algo, idx) => (
                              <div key={idx} className="algo-item">
                                <span className="algo-name">{algo.algorithmName}</span>
                                <span className="algo-time">{algo.timeTakenMs.toFixed(2)}ms</span>
                              </div>
                            ))}
                          </div>
                        </div>

                        <div className="capacities-section">
                          <h4>Road Capacities:</h4>
                          <div className="capacities-grid">
                            {round.capacities.map((cap, idx) => (
                              <div key={idx} className="capacity-item">
                                <span className="road-segment">{cap.roadSegment}</span>
                                <span className="capacity-value">{cap.capacity} v/min</span>
                              </div>
                            ))}
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {activeTab === 'leaderboard' && (
            <div className="traffic-stats-content">
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
                        <th>Games</th>
                        <th>Avg Max Flow</th>
                        <th>Best Max Flow</th>
                        <th>Last Played</th>
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
                          <td>{entry.totalGames}</td>
                          <td>{entry.averageMaxFlow.toFixed(1)}</td>
                          <td className="best-flow">{entry.bestMaxFlow}</td>
                          <td>{formatDate(entry.lastPlayed)}</td>
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

export default TrafficStats;
