
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import SALStats from '../../../../components/Games/SnakeAndLadder/SALStats';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('SALStats', () => {
    const mockLeaderboard = [
        { playerName: 'Player1', totalGames: 10, averageThrows: 5.5, bestThrows: 4, lastPlayed: '2023-01-01T12:00:00Z' },
        { playerName: 'Player2', totalGames: 8, averageThrows: 6.2, bestThrows: 5, lastPlayed: '2023-01-02T12:00:00Z' }
    ];

    const mockHistory = [
        {
            roundId: 1,
            boardSize: 10,
            numSnakes: 5,
            numLadders: 5,
            minimumThrows: 6,
            datePlayed: '2023-01-01T12:00:00Z',
            algorithmTimes: [
                { algorithmName: 'BFS', timeTakenMs: 0.5 },
                { algorithmName: 'Dijkstra', timeTakenMs: 0.6 }
            ],
            boardConfig: []
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
                <SALStats />
            </BrowserRouter>
        );

        expect(screen.getByText('Snake and Ladder Statistics')).toBeInTheDocument();
        expect(screen.getByText('My History')).toHaveClass('active');

        // Wait for history data
        await waitFor(() => {
            expect(screen.getByText('Round #1')).toBeInTheDocument();
            expect(screen.getByText('BFS')).toBeInTheDocument();
        });
    });

    it('switches to Leaderboard tab and loads data', async () => {
        render(
            <BrowserRouter>
                <SALStats />
            </BrowserRouter>
        );

        const leaderboardTab = screen.getByText('Leaderboard');
        fireEvent.click(leaderboardTab);

        expect(leaderboardTab).toHaveClass('active');

        // Wait for leaderboard data
        await waitFor(() => {
            expect(screen.getByText('Rank')).toBeInTheDocument();
        });
    });

    it('handles fetch errors gracefully', async () => {
        // Mock fetch error
        (globalThis.fetch as any).mockImplementation(() => Promise.resolve({ ok: false }));

        render(
            <BrowserRouter>
                <SALStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Failed to load data. Please try again.')).toBeInTheDocument();
        });
    });

    it('navigates back to games', () => {
        render(
            <BrowserRouter>
                <SALStats />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('navigates to play game', () => {
        render(
            <BrowserRouter>
                <SALStats />
            </BrowserRouter>
        );

        const playBtn = screen.getByText('Play Game');
        fireEvent.click(playBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/games/sal');
    });
});
