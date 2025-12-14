import React, { useEffect, useState } from 'react';
import './Preloader.css';

interface PreloaderProps {
  onComplete?: () => void;
  progress?: number;
}

const Preloader: React.FC<PreloaderProps> = ({ onComplete, progress = 0 }) => {
  const [currentPhase, setCurrentPhase] = useState(0);
  const [isComplete, setIsComplete] = useState(false);

  const phases = [
    { text: "Initializing Data Structures...", icon: "🔗" },
    { text: "Loading Algorithms...", icon: "⚡" },
    { text: "Building Game Engine...", icon: "🎮" },
    { text: "Optimizing Performance...", icon: "🚀" },
    { text: "Ready to Play!", icon: "✨" }
  ];

  useEffect(() => {
    // Update phase based on progress
    const phaseIndex = Math.floor((progress / 100) * phases.length);
    setCurrentPhase(Math.min(phaseIndex, phases.length - 1));
    
    if (progress >= 100) {
      setIsComplete(true);
      setTimeout(() => {
        onComplete?.();
      }, 300);
    }
  }, [progress, onComplete, phases.length]);

  return (
    <div className={`preloader ${isComplete ? 'preloader--complete' : ''}`}>
      <div className="preloader__background">
        <div className="preloader__grid"></div>
      </div>
      
      <div className="preloader__content">
        {/* Generating Loader */}
        <div className="preloader__generating">
          <div className="loader-wrapper">
            <span className="loader-letter">L</span>
            <span className="loader-letter">o</span>
            <span className="loader-letter">a</span>
            <span className="loader-letter">d</span>
            <span className="loader-letter">i</span>
            <span className="loader-letter">n</span>
            <span className="loader-letter">g</span>
            <span className="loader-letter">.</span>
            <span className="loader-letter">.</span>
            <span className="loader-letter">.</span>
            <div className="loader" />
          </div>
        </div>
      </div>

      {/* Footer */}
      <div className="preloader__footer">
        <p className="preloader__footer-text">
          Algorithm Challenge Arena
        </p>
        <p className="preloader__footer-version">
          v1.0.0 · © 2025
        </p>
      </div>
    </div>
  );
};

export default Preloader;