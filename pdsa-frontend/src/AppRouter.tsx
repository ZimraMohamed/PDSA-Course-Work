import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App';
import TSPGame from './components/Games/TravelingSalesman/TSPGame';
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
    path: '/games/snake-ladder',
    element: <SnakeAndLadder />,
  },
  {
    path: '/games/traffic-simulation',
    element: <ComingSoonPage title="Traffic Simulation Game" icon="🚦" />,
  },
  {
    path: '/games/tower-of-hanoi',
    element: <ComingSoonPage title="Tower of Hanoi Game" icon="🗼" />,
  },
  {
    path: '/games/eight-queens',
    element: <ComingSoonPage title="Eight Queens Game" icon="♛" />,
  },
]);

function AppRouter() {
  return <RouterProvider router={router} />;
}

export default AppRouter;