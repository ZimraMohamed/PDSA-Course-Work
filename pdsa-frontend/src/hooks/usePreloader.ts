import { useState, useEffect } from 'react';

interface UsePreloaderReturn {
  isLoading: boolean;
  progress: number;
  setLoading: (loading: boolean) => void;
}

export const usePreloader = (): UsePreloaderReturn => {
  const [isLoading, setIsLoading] = useState(true);
  const [progress, setProgress] = useState(0);

  const setLoading = (loading: boolean) => {
    setIsLoading(loading);
    if (!loading) {
      setProgress(100);
    }
  };

  useEffect(() => {
    if (!isLoading) return;

    // Define realistic loading steps with variable timing
    const loadingSteps = [
      { progress: 15, delay: 300 },   // Fast initial load
      { progress: 35, delay: 400 },   // Moderate speed
      { progress: 50, delay: 600 },   // Slower processing
      { progress: 65, delay: 500 },   // Steady progress
      { progress: 75, delay: 400 },   // Faster again
      { progress: 85, delay: 350 },   // Almost there
      { progress: 95, delay: 300 },   // Final processing
      { progress: 100, delay: 200 }   // Complete
    ];

    let currentStep = 0;
    
    const updateProgress = () => {
      if (currentStep < loadingSteps.length) {
        const step = loadingSteps[currentStep];
        setProgress(step.progress);
        
        if (step.progress >= 100) {
          setTimeout(() => {
            setIsLoading(false);
          }, 400); // Small delay after reaching 100%
        } else {
          setTimeout(() => {
            currentStep++;
            updateProgress();
          }, step.delay);
        }
      }
    };

    // Start loading sequence
    setTimeout(updateProgress, 100);
  }, [isLoading]);

  return {
    isLoading,
    progress,
    setLoading
  };
};