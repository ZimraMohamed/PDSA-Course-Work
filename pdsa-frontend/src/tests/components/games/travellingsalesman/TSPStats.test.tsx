
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import TSPStats from '../../../../components/Games/TravelingSalesman/TSPStats';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('TSPStats', () => {
    const mockLeaderboard = [
        { playerName: 'Player1', totalGames: 10, averageDistance: 50.5, bestDistance: 40, lastPlayed: '2023-01-01T12:00:00Z' },
        { playerName: 'Player2', totalGames: 8, averageDistance: 60.2, bestDistance: 45, lastPlayed: '2023-01-02T12:00:00Z' }
    ];

    const mockHistory = [
        {
            roundId: 1,
            homeCity: 'A',
            selectedCities: ['A', 'B', 'C'],
            shortestRoute: ['A', 'B', 'C', 'A'],
            shortestDistance: 100,
            datePlayed: '2023-01-01T12:00:00Z',
            algorithmTimes: [
                { algorithmName: 'Brute Force', timeTakenMs: 0.5 },
                { algorithmName: 'Held-Karp', timeTakenMs: 0.6 }
            ]
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
                <TSPStats />
            </BrowserRouter>
        );

        expect(screen.getByText('TSP Statistics')).toBeInTheDocument();
        expect(screen.getByText('My History')).toHaveClass('active');

        // Wait for history data
        await waitFor(() => {
            expect(screen.getByText('Round #1')).toBeInTheDocument();
            expect(screen.getByText('Brute Force')).toBeInTheDocument();
        });
    });

    it('switches to Leaderboard tab and loads data', async () => {
        render(
            <BrowserRouter>
                <TSPStats />
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
                <TSPStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Failed to load data. Please try again.')).toBeInTheDocument();
        });
    });

    it('navigates back to games', async () => {
        render(
            <BrowserRouter>
                <TSPStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
        });

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('navigates to play game', async () => {
        render(
            <BrowserRouter>
                <TSPStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.queryByText('Loading...')).not.toBeInTheDocument();
        });

        const playBtn = screen.getByText('Play Game');
        fireEvent.click(playBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/games/tsp');
    });
});
