
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import TOHPStats from '../../../../components/Games/TowerOfHanoi/TOHPStats';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('TOHPStats', () => {
    const mockLeaderboard = [
        { playerName: 'Player1', totalGames: 10, averageMoves: 20, bestMoves: 15, lastPlayed: '2023-01-01T12:00:00Z' },
        { playerName: 'Player2', totalGames: 8, averageMoves: 25, bestMoves: 18, lastPlayed: '2023-01-02T12:00:00Z' }
    ];

    const mockHistory = {
        rounds: [
            {
                roundId: 1,
                numDisks: 3,
                numPegs: 3,
                correctMoves: 7,
                datePlayed: '2023-01-01T12:00:00Z'
            }
        ]
    };

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
                <TOHPStats />
            </BrowserRouter>
        );

        expect(screen.getByText('Tower of Hanoi Statistics')).toBeInTheDocument();
        expect(screen.getByText('My History')).toHaveClass('active');

        // Wait for history data
        await waitFor(() => {
            expect(screen.getByText('Round #1')).toBeInTheDocument();
            expect(screen.getByText('Number of Disks:')).toBeInTheDocument();
        });
    });

    it('switches to Leaderboard tab and loads data', async () => {
        render(
            <BrowserRouter>
                <TOHPStats />
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
                <TOHPStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Failed to load data. Please try again.')).toBeInTheDocument();
        });
    });

    it('navigates back to games', () => {
        render(
            <BrowserRouter>
                <TOHPStats />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('navigates to play game', () => {
        render(
            <BrowserRouter>
                <TOHPStats />
            </BrowserRouter>
        );

        const playBtn = screen.getByText('Play Game');
        fireEvent.click(playBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/games/tohp');
    });
});
