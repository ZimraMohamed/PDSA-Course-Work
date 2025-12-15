
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { BrowserRouter } from 'react-router-dom';
import TrafficGame from '../../../../components/Games/TrafficSimulation/TrafficGame';
import axios from 'axios';

// Mock dependencies
const mockNavigate = vi.fn();
vi.mock('axios');

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

describe('TrafficGame', () => {
    beforeEach(() => {
        vi.clearAllMocks();
        Storage.prototype.getItem = vi.fn(() => 'TestPlayer');
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    it('starts game and renders network immediately', async () => {
        render(
            <BrowserRouter>
                <TrafficGame />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByText('Traffic Network')).toBeInTheDocument();
            expect(screen.getByText('Your Answer')).toBeInTheDocument();
        });
    });

    it('handles answer submission success', async () => {
        (axios.post as any).mockResolvedValue({
            data: {
                correctAnswer: 50,
                playerAnswer: 50,
                edmondsKarpTime: 1.0,
                dinicTime: 0.8,
                status: 'Correct',
                message: 'Good job!'
            }
        });

        render(
            <BrowserRouter>
                <TrafficGame />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Enter flow...')).toBeInTheDocument();
        });

        // Enter Answer
        const input = screen.getByPlaceholderText('Enter flow...');
        fireEvent.change(input, { target: { value: '50' } });

        // Submit
        const submitBtn = screen.getByText('Submit Answer');
        fireEvent.click(submitBtn);

        // Verify result dialog
        await waitFor(() => {
            const dialog = screen.getByTestId('game-result-dialog');
            expect(dialog).toBeInTheDocument();
            expect(dialog).toHaveTextContent('pass'); // derived from status 'Correct'
        });
    });

    it('handles answer submission failure', async () => {
        (axios.post as any).mockResolvedValue({
            data: {
                correctAnswer: 50,
                playerAnswer: 40,
                status: 'Incorrect',
                message: 'Wrong answer'
            }
        });

        render(
            <BrowserRouter>
                <TrafficGame />
            </BrowserRouter>
        );

        await waitFor(() => {
            expect(screen.getByPlaceholderText('Enter flow...')).toBeInTheDocument();
        });

        // Enter Answer
        const input = screen.getByPlaceholderText('Enter flow...');
        fireEvent.change(input, { target: { value: '40' } });

        // Submit
        const submitBtn = screen.getByText('Submit Answer');
        fireEvent.click(submitBtn);

        // Verify result dialog
        await waitFor(() => {
            const dialog = screen.getByTestId('game-result-dialog');
            expect(dialog).toBeInTheDocument();
            expect(dialog).toHaveTextContent('fail'); // derived from status 'Incorrect'
        });
    });

    it('handles navigation back to games', () => {
        render(
            <BrowserRouter>
                <TrafficGame />
            </BrowserRouter>
        );

        const backBtn = screen.getByText(/Back to Games/i);
        fireEvent.click(backBtn);

        expect(mockNavigate).toHaveBeenCalledWith('/');
    });
});
