import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import { TSPGame } from './components/Games/TravelingSalesman';
import { TrafficGame } from './components/Games/TrafficSimulation';
import { TOHPGame } from './components/Games/TowerOfHanoi';
import { EQPGame } from './components/Games/EightQueens';
import './AppRouter.css';
import SnakeAndLadder from './components/Games/SnakeAndLadder/SnakeAndLadder';

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
    path: '/games/eqp',
    element: <EQPGame />,
  },
  {
    path: '/games/snake-ladder',
    element: <SnakeAndLadder />,
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