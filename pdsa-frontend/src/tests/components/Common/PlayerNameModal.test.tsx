import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import PlayerNameModal from '../../../components/Common/PlayerNameModal';

describe('PlayerNameModal', () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        onSubmit: vi.fn(),
        gameName: 'Test Game',
    };

    beforeEach(() => {
        vi.clearAllMocks();
        localStorage.clear();
    });

    it('renders correctly when open', () => {
        render(<PlayerNameModal {...defaultProps} />);
        expect(screen.getByText('Welcome, Player!')).toBeInTheDocument();
        expect(screen.getByText('Test Game')).toBeInTheDocument();
        expect(screen.getByPlaceholderText('Enter your name...')).toBeInTheDocument();
    });

    it('does not render when closed', () => {
        const { container } = render(<PlayerNameModal {...defaultProps} isOpen={false} />);
        expect(container).toBeEmptyDOMElement();
    });

    it('loads saved name from localStorage', () => {
        localStorage.setItem('pdsa_player_name', 'Saved Player');
        render(<PlayerNameModal {...defaultProps} />);
        expect(screen.getByDisplayValue('Saved Player')).toBeInTheDocument();
    });

    it('updates input value when typing', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const input = screen.getByPlaceholderText('Enter your name...');
        fireEvent.change(input, { target: { value: 'New Player' } });
        expect(input).toHaveValue('New Player');
    });

    it('shows error for empty name submission', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const submitBtn = screen.getByText('Start Game');
        fireEvent.click(submitBtn);
        expect(screen.getByText('Please enter your name')).toBeInTheDocument();
        expect(defaultProps.onSubmit).not.toHaveBeenCalled();
    });

    it('shows error for short name', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const input = screen.getByPlaceholderText('Enter your name...');
        fireEvent.change(input, { target: { value: 'A' } });
        const submitBtn = screen.getByText('Start Game');
        fireEvent.click(submitBtn);
        expect(screen.getByText('Name must be at least 2 characters')).toBeInTheDocument();
        expect(defaultProps.onSubmit).not.toHaveBeenCalled();
    });

    it('submits valid name and saves to localStorage', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const input = screen.getByPlaceholderText('Enter your name...');
        fireEvent.change(input, { target: { value: 'Valid Player' } });
        const submitBtn = screen.getByText('Start Game');
        fireEvent.click(submitBtn);

        expect(defaultProps.onSubmit).toHaveBeenCalledWith('Valid Player');
        expect(localStorage.getItem('pdsa_player_name')).toBe('Valid Player');
    });

    it('calls onClose when cancel button is clicked', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const cancelBtn = screen.getByText('Cancel');
        fireEvent.click(cancelBtn);
        expect(defaultProps.onClose).toHaveBeenCalled();
    });

    it('calls onClose when close icon is clicked', () => {
        render(<PlayerNameModal {...defaultProps} />);
        const closeIcon = screen.getByLabelText('Close');
        fireEvent.click(closeIcon);
        expect(defaultProps.onClose).toHaveBeenCalled();
    });

    it('closes on Escape key press', () => {
        render(<PlayerNameModal {...defaultProps} />);
        fireEvent.keyDown(document, { key: 'Escape' });
        expect(defaultProps.onClose).toHaveBeenCalled();
    });
});
