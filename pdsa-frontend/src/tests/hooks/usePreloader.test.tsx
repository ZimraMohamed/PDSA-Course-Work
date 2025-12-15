
import { renderHook, act } from '@testing-library/react';
import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest';
import { usePreloader } from '../../hooks/usePreloader';

describe('usePreloader', () => {
    beforeEach(() => {
        vi.useFakeTimers();
        sessionStorage.clear();
        vi.clearAllMocks();
    });

    afterEach(() => {
        vi.useRealTimers();
        sessionStorage.clear();
    });

    it('should initialize with loading true if not shown before', () => {
        const { result } = renderHook(() => usePreloader());
        expect(result.current.isLoading).toBe(true);
        expect(result.current.progress).toBe(0);
    });

    it('should initialize with loading false if already shown', () => {
        sessionStorage.setItem('preloaderShown', 'true');
        const { result } = renderHook(() => usePreloader());
        expect(result.current.isLoading).toBe(false);
    });

    it('should update progress over time and complete', () => {
        const { result } = renderHook(() => usePreloader());

        expect(result.current.isLoading).toBe(true);

        // Advance timers to simulate progress steps
        // The hook uses recursive setTimeout, total time is roughly sum of all delays
        // 300+400+600+500+400+350+300+200 = 3050ms
        // + initial 100ms
        // + final 400ms

        act(() => {
            vi.advanceTimersByTime(100); // Start
        });

        act(() => {
            vi.advanceTimersByTime(300); // Step 1 (15%)
        });
        expect(result.current.progress).toBeGreaterThanOrEqual(15);

        // Fast forward to end
        act(() => {
            vi.runAllTimers();
        });

        expect(result.current.progress).toBe(100);
        expect(result.current.isLoading).toBe(false);
        expect(sessionStorage.getItem('preloaderShown')).toBe('true');
    });

    it('should allow manual loading control', () => {
        const { result } = renderHook(() => usePreloader());

        act(() => {
            result.current.setLoading(false);
        });

        expect(result.current.isLoading).toBe(false);
        expect(result.current.progress).toBe(100);
        expect(sessionStorage.getItem('preloaderShown')).toBe('true');
    });
});
