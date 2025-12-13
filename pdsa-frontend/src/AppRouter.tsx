import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import { TSPGame, TSPStats } from './components/Games/TravelingSalesman';
import { TrafficGame } from './components/Games/TrafficSimulation';
import { TOHPGame } from './components/Games/TowerOfHanoi';
import { EQPGame } from './components/Games/EightQueens';
import { SALGame } from './components/Games/SnakeAndLadder';
import './AppRouter.css';

const ComingSoonPage = ({ title, icon }: { title: string; icon: string }) => (
  <div className="coming-soon-page">
    <h2>{icon} {title}</h2>
    <p>Coming Soon...</p>
    <a href="/" className="back-link">← Back to Home</a>
  </div>
);

const router = createBrowserRouter([
  {
    path: '/',
    element: <App />,
  },
  {
    path: '/games/tsp',
    element: <TSPGame />,
  },
  {
    path: '/games/tsp/stats',
    element: <TSPStats />,
  },
  {
    path: '/games/eqp',
    element: <EQPGame />,
  },
  {
    path: '/games/snake-ladder',
    element: <SALGame />,
  },
  {
    path: '/games/traffic-simulation',
    element: <TrafficGame />,
  },
  {
    path: '/games/tower-of-hanoi',
    element: <TOHPGame />,
  },
  {
    path: '/games/eight-queens',
    element: <EQPGame />,
  },
]);

function AppRouter() {
  return <RouterProvider router={router} />;
}

export default AppRouter;