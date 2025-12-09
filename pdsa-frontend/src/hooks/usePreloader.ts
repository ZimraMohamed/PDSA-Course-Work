import { useState, useEffect } from 'react';

interface UsePreloaderReturn {
  isLoading: boolean;
  progress: number;
  setLoading: (loading: boolean) => void;
}

export const usePreloader = (): UsePreloaderReturn => {
  const hasShownPreloader = sessionStorage.getItem('preloaderShown') === 'true';
  const [isLoading, setIsLoading] = useState(!hasShownPreloader);
  const [progress, setProgress] = useState(0);

  const setLoading = (loading: boolean) => {
    setIsLoading(loading);
    if (!loading) {
      setProgress(100);
      sessionStorage.setItem('preloaderShown', 'true');
    }
  };

  useEffect(() => {
    if (!isLoading) return;

    const loadingSteps = [
      { progress: 15, delay: 300 },   
      { progress: 35, delay: 400 },   
      { progress: 50, delay: 600 },   
      { progress: 65, delay: 500 },   
      { progress: 75, delay: 400 },   
      { progress: 85, delay: 350 },   
      { progress: 95, delay: 300 },   
      { progress: 100, delay: 200 }   
    ];

    let currentStep = 0;
    
    const updateProgress = () => {
      if (currentStep < loadingSteps.length) {
        const step = loadingSteps[currentStep];
        setProgress(step.progress);
        
        if (step.progress >= 100) {
          setTimeout(() => {
            setIsLoading(false);
            sessionStorage.setItem('preloaderShown', 'true');
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