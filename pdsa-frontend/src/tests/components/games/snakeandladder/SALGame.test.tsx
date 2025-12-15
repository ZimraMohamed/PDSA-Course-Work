
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import SALGame from '../../../../components/Games/SnakeAndLadder/SALGame';

// Mock dependencies
const mockNavigate = vi.fn();

vi.mock('react-router-dom', async () => {
    const actual = await vi.importActual('react-router-dom');
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    };
});

// Mock Konva to avoid canvas issues in jsdom
vi.mock('react-konva', () => ({
    Stage: ({ children }: any) => <div data-testid="konva-stage">{children}</div>,
    Layer: ({ children }: any) => <div data-testid="konva-layer">{children}</div>,
    Line: () => <div data-testid="konva-line" />,
    Circle: () => <div data-testid="konva-circle" />,
    Group: ({ children }: any) => <div data-testid="konva-group">{children}</div>,
}));

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

describe('SALGame', () => {
    const mockGameRound = {
        gameId: 'test-sal-game',
        boardSize: 8,
        snakes: [{ head: 20, tail: 5 }],
        ladders: [{ bottom: 3, top: 15 }]
    };

    const mockSolution = {
        gameId: 'test-sal-game',
        minimumThrows: 4,
        algorithmResults: []
    };

    const mockValidation = {
        isCorrect: true,
        message: 'Correct!'
    };

    beforeEach(() => {
        vi.clearAllMocks();
        Storage.prototype.getItem = vi.fn(() => 'TestPlayer');

        // Mock fetch
        globalThis.fetch = vi.fn((url) => {
            if (url.includes('new-game')) {
                return Promise.resolve({
                    ok: true,
                    json: () => Promise.resolve(mockGameRound)
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
                    json: () => Promise.resolve(mockValidation)
                });
            }
            return Promise.reject(new Error('Unknown URL'));
        }) as any;
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('renders setup screen initially', () => {
        render(
            <BrowserRouter>
                <SALGame />
            </BrowserRouter>
        );

        expect(screen.getByText(/Snake and Ladder Game/i)).toBeInTheDocument();
        expect(screen.getByText('Game Setup')).toBeInTheDocument();
        expect(screen.getByText('Start Game')).toBeInTheDocument();
    });

    it('starts a new game', async () => {
        render(
            <BrowserRouter>
                <SALGame />
            </BrowserRouter>
        );

        const startBtn = screen.getByText('Start Game');
        fireEvent.click(startBtn);

        await waitFor(() => {
            expect(screen.getByText('Game Information')).toBeInTheDocument();
            expect(screen.getByText('Submit Minimum Rolls')).toBeInTheDocument();
        });
    });

    it('completes the game loop successfully', async () => {
        render(
            <BrowserRouter>
                <SALGame />
            </BrowserRouter>
        );

        // Start Game
        fireEvent.click(screen.getByText('Start Game'));

        await waitFor(() => {
            expect(screen.getByText('Submit Minimum Rolls')).toBeInTheDocument();
        });

        // Click Solve/Submit
        const solveBtn = screen.getByText('Submit Minimum Rolls');
        fireEvent.click(solveBtn);

        // Check if popup appears with choices
        await waitFor(() => {
            expect(screen.getByText('Minimum Throws Required')).toBeInTheDocument();
        });

        // Find the correct choice (mocked as 4) and click it
        // Note: The component generates random incorrect choices, but one will be 4.
        const correctChoice = screen.getByText(/4 throws/i); // "4 throws" or "4 throw"
        fireEvent.click(correctChoice);

        // Submit answer
        const submitAnswerBtn = screen.getByText('Submit Answer');
        fireEvent.click(submitAnswerBtn);

        // Check result dialog
        await waitFor(() => {
            const dialog = screen.getByTestId('game-result-dialog');
            expect(dialog).toBeInTheDocument();
            expect(dialog).toHaveTextContent('pass'); // mockValidation isCorrect: true
        });
    });

    it('handles navigation back to games', () => {
        render(
            <BrowserRouter>
                <SALGame />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });
});
