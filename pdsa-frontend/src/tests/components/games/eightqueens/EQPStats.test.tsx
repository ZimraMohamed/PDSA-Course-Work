import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import EQPStats from '../../../../components/Games/EightQueens/EQPStats';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

describe('EQPStats', () => {
    const mockLeaderboard = [
        { playerName: 'Player1', solutionsFound: 10, firstSolutionDate: '2023-01-01T12:00:00Z' },
        { playerName: 'Player2', solutionsFound: 8, firstSolutionDate: '2023-01-02T12:00:00Z' }
    ];

    const mockHistory = [
        { solutionID: 1, dateFound: '2023-01-01T12:00:00Z', solution_Text: '0-0,1-4,2-7,3-5,4-2,5-6,6-1,7-3' }
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
            if (url.includes('player-solutions')) {
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

    it('renders initial state (leaderboard tab)', async () => {
        render(
            <BrowserRouter>
                <EQPStats />
            </BrowserRouter>
        );

        expect(screen.getByText('Eight Queens Statistics')).toBeInTheDocument();
        expect(screen.getByText('Leaderboard')).toHaveClass('active');

        // Wait for leaderboard data
        await waitFor(() => {
            expect(screen.getByText('Player1 (You)')).toBeInTheDocument();
            expect(screen.getByText('Player2')).toBeInTheDocument();
        });
    });

    it('switches to My Solutions tab and loads history', async () => {
        render(
            <BrowserRouter>
                <EQPStats />
            </BrowserRouter>
        );

        const historyTab = screen.getByText('My Solutions');
        fireEvent.click(historyTab);

        expect(historyTab).toHaveClass('active');

        // Wait for history data
        await waitFor(() => {
            expect(screen.getByText('Solution #1')).toBeInTheDocument();
        });

        // Verify formatted solution text
        expect(screen.getByText(/R1C1, R2C5/)).toBeInTheDocument();
    });

    it('handles fetch errors gracefully', async () => {
        // Mock fetch error
        (globalThis.fetch as any).mockImplementation(() => Promise.resolve({ ok: false }));

        render(
            <BrowserRouter>
                <EQPStats />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Failed to load leaderboard')).toBeInTheDocument();
        });
    });

    it('navigates back to games', () => {
        render(
            <BrowserRouter>
                <EQPStats />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });

    it('navigates to play game', () => {
        render(
            <BrowserRouter>
                <EQPStats />
            </BrowserRouter>
        );

        const playBtn = screen.getByText('Play Game');
        fireEvent.click(playBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/games/eqp');
    });
});
