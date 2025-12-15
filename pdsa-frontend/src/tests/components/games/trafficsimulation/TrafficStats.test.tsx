
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import TrafficStats from '../../../../components/Games/TrafficSimulation/TrafficStats';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('TrafficStats', () => {
    const mockLeaderboard = [
        { playerName: 'Player1', totalGames: 10, averageMaxFlow: 100, bestMaxFlow: 150, lastPlayed: '2023-01-01T12:00:00Z' },
        { playerName: 'Player2', totalGames: 8, averageMaxFlow: 90, bestMaxFlow: 140, lastPlayed: '2023-01-02T12:00:00Z' }
    ];

    const mockHistory = [
        {
            roundId: 1,
            correctMaxFlow: 100,
            datePlayed: '2023-01-01T12:00:00Z',
            algorithmTimes: [
                { algorithmName: 'Edmonds-Karp', timeTakenMs: 1.5 },
                { algorithmName: 'Dinic', timeTakenMs: 1.0 }
            ],
            capacities: []
        }
    ];

    beforeEach(() => {
        vi.clearAllMocks();
        // Mock sessionStorage
        Storage.prototype.getItem = vi.fn(() => 'Player1');

        // Mock fetch
        globalThis.fetch = vi.fn((url) => {
            if (url.includes('leaderboard')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve(mockLeaderboard)
                });
            }
            if (url.includes('player-history')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve(mockHistory)
                });
            }
            return Promise.reject(new Error('Unknown URL'));
        }) as any;
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('renders initial state (history tab)', async () => {
        render(
            <BrowserRouter>
                <TrafficStats />
            </BrowserRouter>
        );

        expect(screen.getByText('Traffic Simulation Statistics')).toBeInTheDocument();
        expect(screen.getByText('My History')).toHaveClass('active');

        // Wait for history data
        await waitFor(() => {
            expect(screen.getByText('Round #1')).toBeInTheDocument();
            expect(screen.getByText('Edmonds-Karp')).toBeInTheDocument();
        });
    });

    it('switches to Leaderboard tab and loads data', async () => {
        render(
            <BrowserRouter>
                <TrafficStats />
            </BrowserRouter>
        );

        const leaderboardTab = screen.getByText('Leaderboard');
        fireEvent.click(leaderboardTab);

        expect(leaderboardTab).toHaveClass('active');

        // Wait for leaderboard data
        await waitFor(() => {
            expect(screen.getByText('Player1 (You)')).toBeInTheDocument();
            expect(screen.getByText('Rank')).toBeInTheDocument();
        });
    });

    it('handles fetch errors gracefully', async () => {
        // Mock fetch error
        (globalThis.fetch as any).mockImplementation(() => Promise.resolve({ ok: false }));

        render(
            <BrowserRouter>
                <TrafficStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Failed to load data. Please try again.')).toBeInTheDocument();
        });
    });

    it('navigates back to games', () => {
        render(
            <BrowserRouter>
                <TrafficStats />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('navigates to play game', () => {
        render(
            <BrowserRouter>
                <TrafficStats />
            </BrowserRouter>
        );

        const playBtn = screen.getByText('Play Game');
        fireEvent.click(playBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/games/traffic-simulation');
    });
});
