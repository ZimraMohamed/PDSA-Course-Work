import Preloader from './components/Common/Preloader'
import { usePreloader } from './hooks/usePreloader'
import brainGif from './assets/brain.gif'
import './App.css'

function App() {
  const { isLoading, progress } = usePreloader();

  const handlePreloaderComplete = () => {
    console.log('Preloader completed, app is ready!');
  };

  const scrollToGames = () => {
    const gamesSection = document.getElementById('games-section');
    gamesSection?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  };

  const navigateToGame = (gameRoute: string) => {
    window.location.href = gameRoute;
  };

  if (isLoading) {
    return <Preloader onComplete={handlePreloaderComplete} progress={progress} />;
  }

  return (
    <div className="app">
      {/* Animated Background */}
      <div className="app-background">
        <div className="floating-shapes">
          <div className="shape shape-1"></div>
          <div className="shape shape-2"></div>
          <div className="shape shape-3"></div>
          <div className="shape shape-4"></div>
          <div className="shape shape-5"></div>
        </div>
      </div>

      <header className="app-header">
        <div className="header-content">
          <h1>🎮 PDSA Gaming Platform</h1>
          <p>Algorithm Challenge Arena</p>
          <button className="scroll-to-games-btn" onClick={scrollToGames}>
            <span>Explore Games</span>
            <span className="scroll-arrow">↓</span>
          </button>
        </div>
      </header>
      
      <main className="app-main">
        <section className="intro-section">
          <h2>Master Data Structures & Algorithms Through Gaming</h2>
          <p>Challenge yourself with interactive algorithmic puzzles designed to enhance your problem-solving skills</p>
          <div className="brain-animation-container">
            <img src={brainGif} alt="Brain thinking animation" className="brain-gif" />
          </div>
        </section>

        <div id="games-section" className="games-section-wrapper">
          <div className="section-header">
            <h2>🎮 Choose Your Algorithm Challenge</h2>
            <p>Select a game to start your algorithmic adventure</p>
          </div>
          
          <div className="game-grid">
          <div className="game-card">
            <div className="game-icon">🐍</div>
            <h3>Snake & Ladder</h3>
            <p>Minimum dice throws calculation using BFS and dynamic programming techniques</p>
            <div className="game-tags">
              <span className="tag">BFS</span>
              <span className="tag">Dynamic Programming</span>
            </div>
            <button className="game-button" onClick={() => navigateToGame('/games/snake-ladder')}>Play Now</button>
          </div>
          
          <div className="game-card">
            <div className="game-icon">🚦</div>
            <h3>Traffic Simulation</h3>
            <p>Maximum flow algorithms to optimize traffic routing systems</p>
            <div className="game-tags">
              <span className="tag">Graph Theory</span>
              <span className="tag">Max Flow</span>
            </div>
            <button className="game-button" onClick={() => navigateToGame('/games/traffic-simulation')}>Play Now</button>
          </div>
          
          <div className="game-card">
            <div className="game-icon">🗺️</div>
            <h3>Traveling Salesman</h3>
            <p>Shortest route optimization using advanced heuristic approaches</p>
            <div className="game-tags">
              <span className="tag">Optimization</span>
              <span className="tag">NP-Hard</span>
            </div>
            <button className="game-button" onClick={() => navigateToGame('/games/tsp')}>Play Now</button>
          </div>
          
          <div className="game-card">
            <div className="game-icon">🗼</div>
            <h3>Tower of Hanoi</h3>
            <p>Classic recursive puzzle demonstrating divide and conquer strategy</p>
            <div className="game-tags">
              <span className="tag">Recursion</span>
              <span className="tag">Stack</span>
            </div>
            <button className="game-button" onClick={() => navigateToGame('/games/tower-of-hanoi')}>Play Now</button>
          </div>
          
          <div className="game-card">
            <div className="game-icon">♛</div>
            <h3>Eight Queens</h3>
            <p>Chess placement challenge using backtracking algorithms</p>
            <div className="game-tags">
              <span className="tag">Backtracking</span>
              <span className="tag">Constraint Satisfaction</span>
            </div>
            <button className="game-button" onClick={() => navigateToGame('/games/eight-queens')}>Play Now</button>
          </div>
        </div>
        </div>
      </main>

      <footer className="app-footer">
        <div className="footer-content">
          <div className="footer-section">
            <h4>About PDSA</h4>
            <p>Interactive platform for mastering Programming, Data Structures & Algorithms through gamification</p>
          </div>
          <div className="footer-section">
            <h4>Technologies</h4>
            <div className="tech-list">
              <span className="tech-item">React</span>
              <span className="tech-item">.NET Core</span>
              <span className="tech-item">TypeScript</span>
              <span className="tech-item">SQLite</span>
            </div>
          </div>
          <div className="footer-section">
            <h4>Learning Goals</h4>
            <ul>
              <li>Algorithm Analysis</li>
              <li>Time Complexity</li>
              <li>Space Optimization</li>
              <li>Problem Solving</li>
            </ul>
          </div>
        </div>
        <div className="footer-bottom">
          <p>&copy; 2025 PDSA Gaming Platform. Built for educational purposes.</p>
        </div>
      </footer>
    </div>
  )
}

export default App
