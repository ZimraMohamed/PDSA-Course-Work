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
        {/* Logo/Title */}
        <div className="preloader__logo">
          <h1 className="preloader__title">
            <span className="preloader__title-accent">PDSA</span>
            <span className="preloader__title-main">Gaming Platform</span>
          </h1>
          <p className="preloader__subtitle">Algorithm Challenge Arena</p>
        </div>

        {/* Data Structure Visualizations */}
        <div className="preloader__visualizations">
          {/* Binary Tree Animation */}
          <div className="visualization visualization--tree">
            <div className="tree-node tree-node--root">
              <div className="tree-node tree-node--left">
                <div className="tree-node tree-node--leaf"></div>
                <div className="tree-node tree-node--leaf"></div>
              </div>
              <div className="tree-node tree-node--right">
                <div className="tree-node tree-node--leaf"></div>
                <div className="tree-node tree-node--leaf"></div>
              </div>
            </div>
          </div>

          {/* Graph Network Animation */}
          <div className="visualization visualization--graph">
            <div className="graph-node graph-node--1"></div>
            <div className="graph-node graph-node--2"></div>
            <div className="graph-node graph-node--3"></div>
            <div className="graph-node graph-node--4"></div>
            <div className="graph-node graph-node--5"></div>
            <svg className="graph-edges">
              <line className="graph-edge graph-edge--1" x1="50%" y1="20%" x2="20%" y2="80%"></line>
              <line className="graph-edge graph-edge--2" x1="50%" y1="20%" x2="80%" y2="80%"></line>
              <line className="graph-edge graph-edge--3" x1="20%" y1="80%" x2="80%" y2="80%"></line>
              <line className="graph-edge graph-edge--4" x1="50%" y1="20%" x2="50%" y2="50%"></line>
            </svg>
          </div>

          {/* Array/Stack Animation */}
          <div className="visualization visualization--stack">
            <div className="stack-item stack-item--1"></div>
            <div className="stack-item stack-item--2"></div>
            <div className="stack-item stack-item--3"></div>
            <div className="stack-item stack-item--4"></div>
          </div>
        </div>

        {/* Progress Section */}
        <div className="preloader__progress-section">
          <div className="preloader__phase">
            <span className="preloader__phase-icon">{phases[currentPhase]?.icon}</span>
            <span className="preloader__phase-text">{phases[currentPhase]?.text}</span>
          </div>
          
          <div className="preloader__progress-bar">
            <div 
              className="preloader__progress-fill"
              data-progress={Math.round(progress / 5) * 5}
            ></div>
            <div className="preloader__progress-glow"></div>
          </div>
          
          <div className="preloader__percentage">
            {progress}%
          </div>
        </div>
      </div>
    </div>
  );
};

export default Preloader;