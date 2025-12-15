import React from 'react';
import { render, screen, act } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import Preloader from '../../../components/Common/Preloader';

describe('Preloader', () => {
    beforeEach(() => {
        vi.useFakeTimers();
    });

    afterEach(() => {
        vi.runOnlyPendingTimers();
        vi.useRealTimers();
    });

    it('renders with initial state', () => {
        render(<Preloader />);
        const loaderText = screen.getAllByText((content, element) => {
            return element?.tagName.toLowerCase() === 'span' &&
                content.length === 1 &&
                'Loading...'.includes(content);
        });
        expect(loaderText.length).toBeGreaterThan(0);
        expect(screen.getByText('Algorithm Challenge Arena')).toBeInTheDocument();
    });

    it('calls onComplete when progress reaches 100%', () => {
        const onComplete = vi.fn();
        const { rerender } = render(<Preloader progress={0} onComplete={onComplete} />);

        // Update progress to 100
        rerender(<Preloader progress={100} onComplete={onComplete} />);

        // Advance timers to trigger completion timeout
        act(() => {
            vi.advanceTimersByTime(300);
        });

        expect(onComplete).toHaveBeenCalledTimes(1);
    });

    it('adds complete class when finished', () => {
        const { container, rerender } = render(<Preloader progress={0} />);

        // Initially should not have complete class
        expect(container.firstChild).not.toHaveClass('preloader--complete');

        // Update progress to 100
        rerender(<Preloader progress={100} />);

        expect(container.firstChild).toHaveClass('preloader--complete');
    });

    it('displays footer information', () => {
        render(<Preloader />);
        expect(screen.getByText('Algorithm Challenge Arena')).toBeInTheDocument();
        expect(screen.getByText(/v1.0.0/)).toBeInTheDocument();
    });
});
