import React, { useState, useEffect } from 'react';
import './TSPGame.css';
import './TSPGame.css';

interface City {
  name: string;
  index: number;
}

interface TSPGameRound {
  gameId: string;
  homeCityName: string; // This will be a single character from backend, but JS treats it as string
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
  const [gameRound, setGameRound] = useState<TSPGameRound | null>(null);
  const [selectedCities, setSelectedCities] = useState<string[]>([]);
  const [solution, setSolution] = useState<TSPSolution | null>(null);
  const [userAnswer, setUserAnswer] = useState<string>('');
  const [gameStatus, setGameStatus] = useState<'setup' | 'playing' | 'solved' | 'validated'>('setup');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>('');
  const [validationResult, setValidationResult] = useState<any>(null);

  const API_BASE_URL = 'http://localhost:5007';
  const cities = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];

  useEffect(() => {
    createNewGame();
  }, []);

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
    } catch (err) {
      setError('Failed to solve TSP. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const validateAnswer = async () => {
    if (!solution || !userAnswer) {
      setError('Please enter your answer');
      return;
    }

    const userAnswerNum = parseInt(userAnswer);
    if (isNaN(userAnswerNum) || userAnswerNum <= 0) {
      setError('Please enter a valid positive number');
      return;
    }

    setLoading(true);
    setError('');
    
    try {
      const response = await fetch(`${API_BASE_URL}/api/tsp/validate-answer`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userAnswer: userAnswerNum,
          correctAnswer: solution.optimalDistance,
          tolerancePercentage: 5
        })
      });
      
      if (!response.ok) throw new Error('Failed to validate answer');
      
      const data = await response.json();
      setValidationResult(data);
      setGameStatus('validated');
      
      // TODO: Save result to database if correct
      if (data.isCorrect) {
        // Save player result to database
        console.log('Answer is correct! Saving to database...');
      }
    } catch (err) {
      setError('Failed to validate answer. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const getDistanceDisplay = (from: number, to: number): string => {
    if (!gameRound || from === to) return '--';
    return gameRound.distanceMatrix[from][to]?.toString() || '0';
  };

  const formatRoute = (route: string[]): string => {
    return route.join(' → ') + ' → ' + route[0];
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
        <button onClick={() => window.location.href = '/'} className="tsp-back-btn">
          ← Back to Games
        </button>
      </div>

      <div className="tsp-header">
        <h1>🗺️ Traveling Salesman Problem</h1>
        <p>Find the shortest route to visit selected cities and return home</p>
        <button onClick={createNewGame} className="tsp-new-game-btn">
          New Game
        </button>
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
                        className={`tsp-city-btn ${
                          selectedCities.includes(city) ? 'selected' : ''
                        } ${gameRound.homeCityName === city ? 'home' : ''}`}
                        onClick={() => toggleCitySelection(city)}
                        disabled={gameRound.homeCityName === city}
                      >
                        <span className="city-letter">City {city}</span>
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
          <div className="tsp-matrix-card">
            <div className="tsp-card-header">
              <h3>Distance Matrix (km)</h3>
              <p>Distances between selected cities</p>
            </div>
            <div className="tsp-card-content">
              <div className="tsp-distance-matrix">
                <table className="tsp-matrix-table">
                  <thead>
                    <tr>
                      <th></th>
                      {selectedCities.map(city => (
                        <th key={city}>{city}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {selectedCities.map(fromCity => (
                      <tr key={fromCity}>
                        <td className="tsp-matrix-header">{fromCity}</td>
                        {selectedCities.map(toCity => (
                          <td key={`${fromCity}-${toCity}`} className="tsp-matrix-cell">
                            {fromCity === toCity ? '--' : 
                             getDistanceDisplay(
                               gameRound.allCities.find(c => c.name === fromCity)?.index || 0,
                               gameRound.allCities.find(c => c.name === toCity)?.index || 0
                             )}
                          </td>
                        ))}
                      </tr>
                    ))}
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
                <p>What is the shortest total distance for this route?</p>
              </div>
              <div className="tsp-card-content">
                <div className="tsp-answer-input">
                  <label htmlFor="user-answer">Shortest Distance (km):</label>
                  <div className="tsp-input-group">
                    <input
                      id="user-answer"
                      type="number"
                      value={userAnswer}
                      onChange={(e: React.ChangeEvent<HTMLInputElement>) => setUserAnswer(e.target.value)}
                      placeholder="Enter distance in km"
                      min="1"
                    />
                    <button onClick={validateAnswer} disabled={loading}>
                      {loading ? 'Checking...' : 'Submit'}
                    </button>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Results */}
          {solution && gameStatus === 'validated' && (
            <div className="tsp-results">
              {validationResult && (
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
    </div>
  );
};

export default TSPGame;