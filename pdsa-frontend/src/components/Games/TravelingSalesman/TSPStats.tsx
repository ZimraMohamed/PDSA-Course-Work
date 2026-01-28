import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './TSPStats.css';

interface AlgorithmTime {
  algorithmName: string;
  timeTakenMs: number;
}

interface RoundHistory {
  roundId: number;
  homeCity: string;
  selectedCities: string[];
  shortestRoute: string[];
  shortestDistance: number;
  datePlayed: string;
  algorithmTimes: AlgorithmTime[];
}

interface LeaderboardEntry {
  playerName: string;
  totalGames: number;
  averageDistance: number;
  bestDistance: number;
  lastPlayed: string;
}

const TSPStats: React.FC = () => {
  const navigate = useNavigate();
  const [playerHistory, setPlayerHistory] = useState<RoundHistory[]>([]);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState<'history' | 'leaderboard'>('history');

  const API_BASE_URL = '';
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
    const response = await fetch(`${API_BASE_URL}/api/tsp/player-history/${encodeURIComponent(playerName)}`);

    if (!response.ok) throw new Error('Failed to load player history');

    const data = await response.json();
    setPlayerHistory(data);
  };

  const loadLeaderboard = async () => {
    const response = await fetch(`${API_BASE_URL}/api/tsp/leaderboard?top=20`);

    if (!response.ok) throw new Error('Failed to load leaderboard');

    const data = await response.json();
    setLeaderboard(data);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  };

  const formatRoute = (route: string[], homeCity?: string) => {
    if (homeCity) {
      // Route already includes home city at start, just add it at the end
      return route.join(' → ') + ' → ' + homeCity;
    }
    return route.join(' → ');
  };

  return (
    <div className="tsp-stats">
      <div className="tsp-stats-nav">
        <button onClick={() => navigate('/')} className="tsp-stats-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/tsp')} className="tsp-stats-play-btn">
          Play Game
        </button>
      </div>

      <div className="tsp-stats-header">
        <h1>TSP Statistics</h1>
        <p>View game history and leaderboard</p>
      </div>

      <div className="tsp-stats-tabs">
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
        <div className="tsp-stats-error">
          <p>{error}</p>
        </div>
      )}

      {loading ? (
        <div className="tsp-stats-loading">
          <div className="spinner"></div>
          <p>Loading...</p>
        </div>
      ) : (
        <>
          {activeTab === 'history' && (
            <div className="tsp-stats-content">
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
                        <div className="detail-row">
                          <span className="detail-label">Home City:</span>
                          <span className="detail-value">{round.homeCity}</span>
                        </div>

                        <div className="detail-row">
                          <span className="detail-label">Selected Cities:</span>
                          <span className="detail-value">{round.selectedCities.join(', ')}</span>
                        </div>

                        <div className="detail-row">
                          <span className="detail-label">Optimal Route:</span>
                          <span className="detail-value route">{formatRoute(round.shortestRoute, round.homeCity)}</span>
                        </div>

                        <div className="detail-row highlight">
                          <span className="detail-label">Shortest Distance:</span>
                          <span className="detail-value distance">{round.shortestDistance} km</span>
                        </div>

                        <div className="algorithm-times">
                          <h4>Algorithm Performance:</h4>
                          <div className="algo-grid">
                            {round.algorithmTimes.map((algo, idx) => (
                              <div key={idx} className="algo-item">
                                <span className="algo-name">{algo.algorithmName}</span>
                                <span className="algo-time">{algo.timeTakenMs}ms</span>
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
            <div className="tsp-stats-content">
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
                        <th>Avg Distance</th>
                        <th>Best Distance</th>
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
                          <td>{entry.averageDistance} km</td>
                          <td className="best-distance">{entry.bestDistance} km</td>
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

export default TSPStats;
