import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import { TSPGame, TSPStats } from './components/Games/TravelingSalesman';
import { TrafficGame, TrafficStats } from './components/Games/TrafficSimulation';
import { TOHPGame, TOHPStats } from './components/Games/TowerOfHanoi';
import { EQPGame, EQPStats } from './components/Games/EightQueens';
import { SALGame, SALStats } from './components/Games/SnakeAndLadder';
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
    path: '/games/eqp/stats',
    element: <EQPStats />,
  },
  {
    path: '/games/snake-ladder',
    element: <SALGame />,
  },
  {
    path: '/games/sal',
    element: <SALGame />,
  },
  {
    path: '/games/sal/stats',
    element: <SALStats />,
  },
  {
    path: '/games/traffic-simulation',
    element: <TrafficGame />,
  },
  {
    path: '/games/traffic-simulation/stats',
    element: <TrafficStats />,
  },
  {
    path: '/games/tower-of-hanoi',
    element: <TOHPGame />,
  },
  {
    path: '/games/tohp',
    element: <TOHPGame />,
  },
  {
    path: '/games/tohp/stats',
    element: <TOHPStats />,
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