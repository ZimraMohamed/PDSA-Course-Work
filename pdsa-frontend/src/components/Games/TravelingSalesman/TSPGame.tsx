import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import ConfirmDialog from '../../Common/ConfirmDialog';
import GameResultDialog from '../../Common/GameResultDialog';
import './TSPGame.css';

interface City {
  name: string;
  index: number;
}

interface TSPGameRound {
  gameId: string;
  homeCityName: string;
  homeCityIndex: number;
  distanceMatrix: number[][];
  allCities: City[];
}

interface AlgorithmResult {
  algorithmName: string;
  distance: number;
  executionTimeMs: number;
  route: string[];
  complexity: number;
}

interface TSPSolution {
  gameId: string;
  optimalDistance: number;
  optimalRoute: string[];
  algorithmResults: AlgorithmResult[];
  complexityAnalysis?: any;
  distanceMatrixDisplay: string;
}

const TSPGame: React.FC = () => {
  const navigate = useNavigate();
  const [gameRound, setGameRound] = useState<TSPGameRound | null>(null);
  const [selectedCities, setSelectedCities] = useState<string[]>([]);
  const [solution, setSolution] = useState<TSPSolution | null>(null);
  const [userAnswer, setUserAnswer] = useState<string>('');
  const [userRoute, setUserRoute] = useState<string[]>([]);
  const [gameStatus, setGameStatus] = useState<'setup' | 'playing' | 'solved' | 'validated'>('setup');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [validationResult, setValidationResult] = useState<any>(null);

  // Game progress tracking
  const [passedRounds, setPassedRounds] = useState<number>(0);
  const [failedRounds, setFailedRounds] = useState<number>(0);
  const [showConfirmDialog, setShowConfirmDialog] = useState<boolean>(false);
  const [showResultDialog, setShowResultDialog] = useState<boolean>(false);
  const [currentResult, setCurrentResult] = useState<'pass' | 'fail' | 'draw'>('pass');

  const distanceMatrixRef = useRef<HTMLDivElement>(null);

  const API_BASE_URL = '';
  const cities = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

  // Get player name from sessionStorage
  const playerName = sessionStorage.getItem('currentPlayerName') || 'Anonymous';

  useEffect(() => {
    createNewGame();
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
    setUserRoute([]);
    setUserAnswer('');
    createNewGame();
  };

  const handleResultBackToGames = () => {
    setShowResultDialog(false);
    navigate('/');
  };

  const createNewGame = async () => {
    setLoading(true);
    setError('');
    try {
      const response = await fetch(`${API_BASE_URL}/api/tsp/new-game`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' }
      });

      if (!response.ok) throw new Error('Failed to create new game');

      const data = await response.json();
      setGameRound(data);
      setSelectedCities([data.homeCityName]);
      setGameStatus('setup');
      setSolution(null);
      setValidationResult(null);
      setUserAnswer('');
      setUserRoute([]);
    } catch (err) {
      setError('Failed to create new game. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const toggleCitySelection = (city: string) => {
    if (gameRound?.homeCityName === city) return; // Can't deselect home city

    setSelectedCities(prev => {
      if (prev.includes(city)) {
        return prev.filter(c => c !== city);
      } else {
        return [...prev, city];
      }
    });
  };

  const solveTSP = async () => {
    if (!gameRound || selectedCities.length < 2) {
      setError('Please select at least 2 cities (including home city)');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await fetch(`${API_BASE_URL}/api/tsp/solve`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          gameId: gameRound.gameId,
          homeCityName: gameRound.homeCityName,
          selectedCities: selectedCities.filter(c => c !== gameRound.homeCityName),
          distanceMatrix: gameRound.distanceMatrix
        })
      });

      if (!response.ok) throw new Error('Failed to solve TSP');

      const data = await response.json();
      setSolution(data);
      setGameStatus('solved');

      // Scroll to distance matrix section
      setTimeout(() => {
        if (distanceMatrixRef.current) {
          distanceMatrixRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      }, 100);
    } catch (err) {
      setError('Failed to solve TSP. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const validateAnswer = async () => {
    if (!solution || userRoute.length === 0) {
      setError('Please enter your route');
      return;
    }

    const nonHomeCities = selectedCities.filter(city => city !== gameRound?.homeCityName);
    if (userRoute.length !== nonHomeCities.length) {
      setError(`Route must include all ${nonHomeCities.length} selected cities (excluding home city)`);
      return;
    }

    setLoading(true);
    setError('');

    try {
      // Calculate user's route distance
      const userDistance = calculateRouteDistance(userRoute);
      const isRouteCorrect = JSON.stringify(userRoute) === JSON.stringify(solution.optimalRoute);

      const response = await fetch(`${API_BASE_URL}/api/tsp/validate-answer`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userAnswer: userDistance,
          correctAnswer: solution.optimalDistance,
          tolerancePercentage: 5
        })
      });

      if (!response.ok) throw new Error('Failed to validate answer');

      const data = await response.json();
      setValidationResult(data);
      setGameStatus('validated');

      // Store formatted route for display
      setUserAnswer(formatRoute(userRoute));

      // Update game progress and show result dialog
      if (data.isCorrect || isRouteCorrect) {
        const newPassed = passedRounds + 1;
        setPassedRounds(newPassed);

        // Determine result type
        if (newPassed === failedRounds) {
          setCurrentResult('draw');
        } else {
          setCurrentResult('pass');
        }

        // Save game to database only if answer is correct
        await saveGameToDatabase();
      } else {
        const newFailed = failedRounds + 1;
        setFailedRounds(newFailed);

        // Determine result type
        if (newFailed === passedRounds) {
          setCurrentResult('draw');
        } else {
          setCurrentResult('fail');
        }
      }

      // Show result dialog
      setShowResultDialog(true);
    } catch (err) {
      setError('Failed to validate answer. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const saveGameToDatabase = async () => {
    if (!solution || !gameRound) return;

    try {
      // Build distances array for selected cities
      const distances = [];
      for (let i = 0; i < selectedCities.length; i++) {
        for (let j = i + 1; j < selectedCities.length; j++) {
          const cityA = selectedCities[i];
          const cityB = selectedCities[j];
          const cityAIndex = gameRound.allCities.find(c => c.name === cityA)?.index || 0;
          const cityBIndex = gameRound.allCities.find(c => c.name === cityB)?.index || 0;
          const distance = gameRound.distanceMatrix[cityAIndex][cityBIndex];

          distances.push({
            cityA,
            cityB,
            distance
          });
        }
      }

      const saveRequest = {
        playerName,
        homeCityName: gameRound.homeCityName,
        selectedCities: selectedCities.filter(c => c !== gameRound.homeCityName),
        optimalRoute: solution.optimalRoute,
        optimalDistance: solution.optimalDistance,
        distances,
        algorithmResults: solution.algorithmResults
      };

      const response = await fetch(`${API_BASE_URL}/api/tsp/save-game`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(saveRequest)
      });

      if (response.ok) {
        const result = await response.json();
        console.log('Game saved successfully:', result);
      } else {
        console.error('Failed to save game to database');
      }
    } catch (err) {
      console.error('Error saving game to database:', err);
      // Don't show error to user, just log it
    }
  };

  const getDistanceDisplay = (from: number, to: number): string => {
    if (!gameRound || from === to) return '--';
    return gameRound.distanceMatrix[from][to]?.toString() || '0';
  };

  const formatRoute = (route: string[]): string => {
    return route.join(' → ') + ' → ' + route[0];
  };

  const calculateRouteDistance = (route: string[]): number => {
    if (!gameRound || route.length === 0) return 0;

    let totalDistance = 0;

    // Distance from home to first city
    const firstCityIndex = gameRound.allCities.find(c => c.name === route[0])?.index || 0;
    const homeIndex = gameRound.homeCityIndex;
    totalDistance += gameRound.distanceMatrix[homeIndex][firstCityIndex];

    // Distance between consecutive cities in route
    for (let i = 0; i < route.length - 1; i++) {
      const fromIndex = gameRound.allCities.find(c => c.name === route[i])?.index || 0;
      const toIndex = gameRound.allCities.find(c => c.name === route[i + 1])?.index || 0;
      totalDistance += gameRound.distanceMatrix[fromIndex][toIndex];
    }

    // Distance from last city back to home
    const lastCityIndex = gameRound.allCities.find(c => c.name === route[route.length - 1])?.index || 0;
    totalDistance += gameRound.distanceMatrix[lastCityIndex][homeIndex];

    return totalDistance;
  };

  if (loading && !gameRound) {
    return (
      <div className="tsp-loading">
        <div className="tsp-spinner"></div>
        <p>Creating new TSP game...</p>
      </div>
    );
  }

  return (
    <div className="tsp-game">
      <div className="tsp-nav">
        <button onClick={handleBackToGames} className="tsp-back-btn">
          ← Back to Games
        </button>
        <button onClick={() => navigate('/games/tsp/stats')} className="tsp-stats-btn">
          View Statistics
        </button>
      </div>

      <div className="tsp-header">
        <h1>🗺️ Traveling Salesman Problem</h1>
        <p>Find the shortest route to visit selected cities and return home</p>

        <div className="tsp-game-stats">
          <div className="player-info">
            <span className="player-label">Player:</span>
            <span className="player-name">{playerName}</span>
          </div>

          <div className="game-progress">
            <div className="progress-item passed">
              <span className="progress-label">Passed: </span>
              <span className="progress-value">{passedRounds}</span>
            </div>
            <div className="progress-item failed">
              <span className="progress-label">Failed: </span>
              <span className="progress-value">{failedRounds}</span>
            </div>
          </div>
        </div>
      </div>

      {error && (
        <div className="tsp-error">
          <p>{error}</p>
        </div>
      )}

      {gameRound && (
        <div className="tsp-content">
          {/* Game Setup */}
          <div className="tsp-setup-card">
            <div className="tsp-card-header">
              <h3>Game Setup</h3>
            </div>
            <div className="tsp-card-content">
              <div className="tsp-game-info">
                <div className="tsp-home-city">
                  <label>Home City:</label>
                  <span className="tsp-home-badge">
                    City {gameRound.homeCityName}
                  </span>
                </div>

                <div className="tsp-city-selection">
                  <label>Select Cities to Visit:</label>
                  <div className="tsp-city-grid">
                    {cities.map(city => (
                      <button
                        key={city}
                        className={`tsp-city-btn ${selectedCities.includes(city) ? 'selected' : ''
                          } ${gameRound.homeCityName === city ? 'home' : ''}`}
                        onClick={() => toggleCitySelection(city)}
                        disabled={gameRound.homeCityName === city}
                      >
                        <span className="city-letter">{city}</span>
                        {gameRound.homeCityName === city && <span className="home-indicator">(Home)</span>}
                      </button>
                    ))}
                  </div>
                  <p className="tsp-selection-info">
                    Selected: {selectedCities.length} cities |
                    Route: {selectedCities.join(' → ')} → {gameRound.homeCityName}
                  </p>
                </div>

                {gameStatus === 'setup' && (
                  <button
                    onClick={solveTSP}
                    disabled={selectedCities.length < 2 || loading}
                    className="tsp-solve-btn"
                  >
                    {loading ? 'Solving...' : 'Solve TSP'}
                  </button>
                )}
              </div>
            </div>
          </div>

          {/* Distance Matrix */}
          <div className="tsp-matrix-card" ref={distanceMatrixRef}>
            <div className="tsp-card-header">
              <h3>Distance Matrix (km)</h3>
              <p>Distances between selected cities</p>
            </div>
            <div className="tsp-card-content">
              <div className="tsp-distance-matrix">
                <table className="tsp-matrix-table">
                  <tbody>
                    {selectedCities.map((fromCity, fromIndex) => (
                      <tr key={fromCity}>
                        <td className="tsp-matrix-header">{fromCity}</td>
                        {selectedCities.map((toCity, toIndex) => {
                          if (toIndex < fromIndex) {
                            return (
                              <td key={`${fromCity}-${toCity}`} className="tsp-matrix-cell">
                                {getDistanceDisplay(
                                  gameRound.allCities.find(c => c.name === fromCity)?.index || 0,
                                  gameRound.allCities.find(c => c.name === toCity)?.index || 0
                                )}
                              </td>
                            );
                          }
                          if (toIndex === fromIndex) {
                            return (
                              <td key={`${fromCity}-${toCity}`} className="tsp-matrix-cell">
                                ---
                              </td>
                            );
                          }
                          return (
                            <td key={`${fromCity}-${toCity}`} className="tsp-matrix-cell tsp-matrix-empty">
                            </td>
                          );
                        })}
                      </tr>
                    ))}
                    <tr className="tsp-matrix-footer">
                      <td></td>
                      {selectedCities.map(city => (
                        <td key={city} className="tsp-matrix-footer-cell">{city}</td>
                      ))}
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          {/* User Answer Section */}
          {gameStatus === 'solved' && (
            <div className="tsp-answer-card">
              <div className="tsp-card-header">
                <h3>Your Answer</h3>
                <p>Build the shortest route starting from <strong>{gameRound?.homeCityName}</strong> visiting all selected cities exactly once and returning home</p>
              </div>
              <div className="tsp-card-content">
                <div className="tsp-answer-input">
                  <label>Click cities in order to build your route:</label>
                  <div className="tsp-route-input-container">
                    <div className="tsp-route-preview">
                      <span className="route-home">{gameRound?.homeCityName}</span>
                      {userRoute.length > 0 && (
                        <>
                          <span className="route-arrow">→</span>
                          {userRoute.map((city, index) => (
                            <React.Fragment key={index}>
                              <span
                                className="route-city clickable"
                                onClick={() => {
                                  const newRoute = userRoute.filter((_, i) => i !== index);
                                  setUserRoute(newRoute);
                                }}
                                title="Click to remove"
                              >
                                {city} ×
                              </span>
                              {index < userRoute.length - 1 && <span className="route-arrow">→</span>}
                            </React.Fragment>
                          ))}
                          <span className="route-arrow">→</span>
                          <span className="route-home">{gameRound?.homeCityName}</span>
                        </>
                      )}
                      {userRoute.length === 0 && (
                        <span className="route-placeholder">Click cities below to start building your route</span>
                      )}
                    </div>

                    <div className="tsp-route-builder">
                      <div className="route-cities-grid">
                        {selectedCities.filter(city => city !== gameRound?.homeCityName).map(city => {
                          const isInRoute = userRoute.includes(city);
                          const canAdd = !isInRoute;
                          return (
                            <button
                              key={city}
                              className={`route-city-btn ${isInRoute ? 'in-route' : ''} ${!canAdd ? 'disabled' : ''}`}
                              onClick={() => {
                                if (canAdd) {
                                  setUserRoute([...userRoute, city]);
                                }
                              }}
                              disabled={!canAdd}
                            >
                              <span className="city-letter-large">{city}</span>
                              {isInRoute && <span className="city-order">#{userRoute.indexOf(city) + 1}</span>}
                            </button>
                          );
                        })}
                      </div>
                    </div>

                    {userRoute.length > 0 && (
                      <div className="tsp-route-info">
                        <span className="info-label">Cities in your route:</span>
                        <span className="info-value">{userRoute.length} / {selectedCities.length - 1}</span>
                        <span className="info-separator">•</span>
                        <span className="info-label">Total distance:</span>
                        <span className="info-value">{calculateRouteDistance(userRoute)} km</span>
                        {userRoute.length < selectedCities.length - 1 && (
                          <>
                            <span className="info-separator">•</span>
                            <span className="info-warning">Add {selectedCities.length - 1 - userRoute.length} more city(s)</span>
                          </>
                        )}
                      </div>
                    )}

                    <div className="tsp-route-actions">
                      <button
                        className="clear-route-btn"
                        onClick={() => setUserRoute([])}
                        disabled={userRoute.length === 0}
                      >
                        Clear Route
                      </button>
                      <button
                        className="submit-route-btn"
                        onClick={validateAnswer}
                        disabled={loading || userRoute.length !== selectedCities.length - 1}
                      >
                        {loading ? 'Checking...' : 'Submit Route'}
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Results */}
          {solution && gameStatus === 'validated' && (
            <div className="tsp-results">
              {validationResult && (
                <>
                  <div className={`tsp-validation ${validationResult.isCorrect ? 'correct' : 'incorrect'}`}>
                    <p>
                      {validationResult.isCorrect ? (
                        <>
                          🎉 Correct! Your answer of {validationResult.userAnswer}km is within {validationResult.toleranceUsed}% of the optimal solution.
                        </>
                      ) : (
                        <>
                          ❌ Incorrect. Your answer: {validationResult.userAnswer}km,
                          Correct answer: {validationResult.correctAnswer}km
                          (Difference: {validationResult.difference}km)
                        </>
                      )}
                    </p>
                  </div>
                </>
              )}

              <div className="tsp-solution-card">
                <div className="tsp-card-header">
                  <h3>Algorithm Results</h3>
                </div>
                <div className="tsp-card-content">
                  <div className="tsp-algorithm-results">
                    {solution.algorithmResults.map((result, index) => (
                      <div key={index} className="tsp-algorithm-result">
                        <h4>{result.algorithmName}</h4>
                        <div className="tsp-result-details">
                          <p><strong>Distance:</strong> {result.distance}km</p>
                          <p><strong>Execution Time:</strong> {result.executionTimeMs}ms</p>
                          <p><strong>Route:</strong> {formatRoute(result.route)}</p>
                          <p><strong>Complexity:</strong> O({result.complexity})</p>
                        </div>
                      </div>
                    ))}
                  </div>

                  <div className="tsp-optimal-solution">
                    <h4>🏆 Optimal Solution</h4>
                    <p><strong>Shortest Distance:</strong> {solution.optimalDistance}km</p>
                    <p><strong>Optimal Route:</strong> {formatRoute(solution.optimalRoute)}</p>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      )}

      {/* Confirm Dialog */}
      <ConfirmDialog
        isOpen={showConfirmDialog}
        title="Leave Game?"
        message="Are you sure you want to leave? Your game progress will be lost."
        confirmText="Leave"
        cancelText="Stay"
        onConfirm={handleConfirmLeave}
        onCancel={handleCancelLeave}
      />

      {/* Game Result Dialog */}
      <GameResultDialog
        isOpen={showResultDialog}
        result={currentResult}
        onNextRound={handleNextRound}
        onBackToGames={handleResultBackToGames}
        passedCount={passedRounds}
        failedCount={failedRounds}
        userAnswer={userAnswer}
        correctAnswer={solution ? formatRoute(solution.optimalRoute) : ''}
      />
    </div>
  );
};

export default TSPGame;