import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import GameResultDialog from '../../../components/Common/GameResultDialog';

describe('GameResultDialog', () => {
    const defaultProps = {
        isOpen: true,
        result: 'pass' as const,
        onNextRound: vi.fn(),
        onBackToGames: vi.fn(),
        passedCount: 5,
        failedCount: 2,
    };

    it('renders pass result correctly', () => {
        render(<GameResultDialog {...defaultProps} />);
        expect(screen.getByText('Congratulations!')).toBeInTheDocument();
        expect(screen.getByText('You passed this round!')).toBeInTheDocument();
        expect(screen.getByText('🎉')).toBeInTheDocument();
    });

    it('renders fail result correctly', () => {
        render(<GameResultDialog {...defaultProps} result="fail" />);
        expect(screen.getByText('Round Failed')).toBeInTheDocument();
        expect(screen.getByText('Better luck next time!')).toBeInTheDocument();
        expect(screen.getByText('😔')).toBeInTheDocument();
    });

    it('renders draw result correctly', () => {
        render(<GameResultDialog {...defaultProps} result="draw" />);
        expect(screen.getByText("It's a Draw!")).toBeInTheDocument();
        expect(screen.getByText('Equal performance so far.')).toBeInTheDocument();
        expect(screen.getByText('🤝')).toBeInTheDocument();
    });

    it('displays passed and failed counts', () => {
        render(<GameResultDialog {...defaultProps} />);
        expect(screen.getByText('5')).toBeInTheDocument();
        expect(screen.getByText('2')).toBeInTheDocument();
    });

    it('calls onNextRound when Next Round button is clicked', () => {
        render(<GameResultDialog {...defaultProps} />);
        const nextRoundBtn = screen.getByText('Next Round →');
        fireEvent.click(nextRoundBtn);
        expect(defaultProps.onNextRound).toHaveBeenCalled();
    });

    it('calls onBackToGames when Back to Games button is clicked', () => {
        render(<GameResultDialog {...defaultProps} />);
        const backBtn = screen.getByText('Back to Games');
        fireEvent.click(backBtn);
        expect(defaultProps.onBackToGames).toHaveBeenCalled();
    });

    it('does not render when isOpen is false', () => {
        const { container } = render(<GameResultDialog {...defaultProps} isOpen={false} />);
        expect(container).toBeEmptyDOMElement();
    });

    it('displays user answer and correct answer on failure', () => {
        render(
            <GameResultDialog
                {...defaultProps}
                result="fail"
                userAnswer="My Answer"
                correctAnswer="Real Answer"
            />
        );
        expect(screen.getByText('Your Answer:')).toBeInTheDocument();
        expect(screen.getByText('My Answer')).toBeInTheDocument();
        expect(screen.getByText('Correct Answer:')).toBeInTheDocument();
        expect(screen.getByText('Real Answer')).toBeInTheDocument();
    });

    it('does not display comparison for pass result', () => {
        render(
            <GameResultDialog
                {...defaultProps}
                result="pass"
                userAnswer="My Answer"
                correctAnswer="Real Answer"
            />
        );
        expect(screen.queryByText('Your Answer:')).not.toBeInTheDocument();
    });
});
