
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import TSPGame from '../../../../components/Games/TravelingSalesman/TSPGame';

// Mock dependencies
const mockNavigate = vi.fn();
// Mock scrollIntoView
window.HTMLElement.prototype.scrollIntoView = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

// Mock Common components
vi.mock('../../../../components/Common/GameResultDialog', () => ({
    default: ({ isOpen, result, onNextRound, onBackToGames }: any) => isOpen ? (
        <div data-testid="game-result-dialog">
            {result}
            <button onClick={onNextRound}>Next Round</button>
            <button onClick={onBackToGames}>Back to Games</button>
        </div>
    ) : null
}));

vi.mock('../../../../components/Common/ConfirmDialog', () => ({
    default: ({ isOpen, onConfirm, onCancel }: any) => isOpen ? (
        <div data-testid="confirm-dialog">
            <button onClick={onConfirm}>Confirm</button>
            <button onClick={onCancel}>Cancel</button>
        </div>
    ) : null
}));

describe('TSPGame', () => {
    const mockNewGame = {
        gameId: 'tsp-123',
        homeCityName: 'A',
        homeCityIndex: 0,
        distanceMatrix: [[0, 10], [10, 0]],
        allCities: [{ name: 'A', index: 0 }, { name: 'B', index: 1 }]
    };

    const mockSolution = {
        optimalDistance: 20,
        optimalRoute: ['A', 'B', 'A'],
        algorithmResults: [],
        distanceMatrixDisplay: ''
    };

    beforeEach(() => {
        vi.clearAllMocks();
        Storage.prototype.getItem = vi.fn(() => 'TestPlayer');

        globalThis.fetch = vi.fn((url) => {
            if (url.includes('new-game')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve(mockNewGame)
                });
            }
            if (url.includes('solve')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve(mockSolution)
                });
            }
            if (url.includes('validate-answer')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve({ isCorrect: true, difference: 0 })
                });
            }
            return Promise.reject(new Error('Unknown URL'));
        }) as any;
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('renders initial setup screen', async () => {
        render(
            <BrowserRouter>
                <TSPGame />
            </BrowserRouter>
        );

        expect(await screen.findByText(/Traveling Salesman Problem/i)).toBeInTheDocument();

        await waitFor(() => {
            expect(screen.getByText('Game Setup')).toBeInTheDocument();
            expect(screen.getByText(/City A/i)).toBeInTheDocument(); // Home city badge
        });
    });

    it('allows city selection and solving', async () => {
        render(
            <BrowserRouter>
                <TSPGame />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('B')).toBeInTheDocument(); // City button
        });

        // Select City B
        const cityBtn = screen.getByText('B').closest('button');
        fireEvent.click(cityBtn!);

        // Click Solve
        const solveBtn = screen.getByText('Solve TSP');
        expect(solveBtn).not.toBeDisabled();
        fireEvent.click(solveBtn);

        await waitFor(() => {
            expect(screen.getByText('Distance Matrix (km)')).toBeInTheDocument();
            expect(screen.getByText('Your Answer')).toBeInTheDocument();
        });
    });

    it('handles navigation back to games', async () => {
        render(
            <BrowserRouter>
                <TSPGame />
            </BrowserRouter>
        );

        // Wait for initial load
        await waitFor(() => {
            expect(screen.getByText('Game Setup')).toBeInTheDocument();
        });

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });
});
