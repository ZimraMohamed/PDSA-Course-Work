import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './TOHPStats.css';

interface AlgorithmTime {
  algorithmName: string;
  timeTakenMs: number;
}

interface RoundHistory {
  roundId: number;
  numDisks: number;
  numPegs: number;
  correctMoves: number;
  datePlayed: string;
}

interface LeaderboardEntry {
  playerName: string;
  totalGames: number;
  averageMoves: number;
  bestMoves: number;
  lastPlayed: string;
}

const TOHPStats: React.FC = () => {
  const navigate = useNavigate();
  const [playerHistory, setPlayerHistory] = useState<RoundHistory[]>([]);
  const [leaderboard, setLeaderboard] = useState<LeaderboardEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState<'history' | 'leaderboard'>('history');

  const API_BASE_URL = 'http://localhost:5007';
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
    const response = await fetch(`${API_BASE_URL}/api/TOHP/player-history/${encodeURIComponent(playerName)}`);

    if (!response.ok) throw new Error('Failed to load player history');

    const data = await response.json();
    setPlayerHistory(data.rounds || []);
  };

  const loadLeaderboard = async () => {
    const response = await fetch(`${API_BASE_URL}/api/TOHP/leaderboard?top=20`);

    if (!response.ok) throw new Error('Failed to load leaderboard');

    const data = await response.json();
    setLeaderboard(data);
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
  };

  return (
    <div className="tohp-stats">
      <div className="tohp-stats-nav">
        <button onClick={() => navigate('/')} className="tohp-stats-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/tohp')} className="tohp-stats-play-btn">
          Play Game
        </button>
      </div>

      <div className="tohp-stats-header">
        <h1>Tower of Hanoi Statistics</h1>
        <p>View game history and leaderboard</p>
      </div>

      <div className="tohp-stats-tabs">
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
        <div className="tohp-stats-error">
          <p>{error}</p>
        </div>
      )}

      {loading ? (
        <div className="tohp-stats-loading">
          <div className="spinner"></div>
          <p>Loading...</p>
        </div>
      ) : (
        <>
          {activeTab === 'history' && (
            <div className="tohp-stats-content">
              <div className="player-info-card">
                <h2>Player: {playerName}</h2>
                <p>Total Games Completed: {playerHistory.length}</p>
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
                          <span className="detail-label">Number of Disks:</span>
                          <span className="detail-value">{round.numDisks}</span>
                        </div>

                        <div className="detail-row">
                          <span className="detail-label">Number of Pegs:</span>
                          <span className="detail-value">{round.numPegs}</span>
                        </div>

                        <div className="detail-row highlight">
                          <span className="detail-label">Optimal Moves:</span>
                          <span className="detail-value moves">{round.correctMoves}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {activeTab === 'leaderboard' && (
            <div className="tohp-stats-content">
              {leaderboard.length === 0 ? (
                <div className="no-data">
                  <p>No leaderboard data available yet. Be the first to play!</p>
                </div>
              ) : (
                <div className="leaderboard-table">
                  <table>
                    <thead>
                      <tr>
                        <th>Rank</th>
                        <th>Player</th>
                        <th>Games</th>
                        <th>Avg Moves</th>
                        <th>Best Moves</th>
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
                          <td>{entry.averageMoves}</td>
                          <td className="best-moves">{entry.bestMoves}</td>
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

export default TOHPStats;
