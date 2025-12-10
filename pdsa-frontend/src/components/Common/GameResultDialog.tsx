import React from 'react';
import './GameResultDialog.css';

interface GameResultDialogProps {
  isOpen: boolean;
  result: 'pass' | 'fail' | 'draw';
  onNextRound: () => void;
  onBackToGames: () => void;
  passedCount: number;
  failedCount: number;
}

const GameResultDialog: React.FC<GameResultDialogProps> = ({
  isOpen,
  result,
  onNextRound,
  onBackToGames,
  passedCount,
  failedCount
}) => {
  if (!isOpen) return null;

  const getResultContent = () => {
    switch (result) {
      case 'pass':
        return {
          icon: '🎉',
          title: 'Congratulations!',
          message: 'You passed this round!',
          color: 'success'
        };
      case 'fail':
        return {
          icon: '😔',
          title: 'Round Failed',
          message: 'Better luck next time!',
          color: 'fail'
        };
      case 'draw':
        return {
          icon: '🤝',
          title: "It's a Draw!",
          message: 'Equal performance so far.',
          color: 'draw'
        };
      default:
        return {
          icon: '🎮',
          title: 'Round Complete',
          message: 'Continue playing!',
          color: 'neutral'
        };
    }
  };

  const content = getResultContent();

  return (
    <div className="game-result-overlay" onClick={(e) => e.stopPropagation()}>
      <div className={`game-result-content ${content.color}`}>
        <div className="game-result-icon">{content.icon}</div>
        <h2 className="game-result-title">{content.title}</h2>
        <p className="game-result-message">{content.message}</p>
        
        <div className="game-result-stats">
          <div className="result-stat success">
            <span className="stat-label">Passed</span>
            <span className="stat-value">{passedCount}</span>
          </div>
          <div className="result-stat fail">
            <span className="stat-label">Failed</span>
            <span className="stat-value">{failedCount}</span>
          </div>
        </div>

        <div className="game-result-actions">
          <button onClick={onBackToGames} className="result-btn-secondary">
            Back to Games
          </button>
          <button onClick={onNextRound} className="result-btn-primary">
            Next Round →
          </button>
        </div>
      </div>
    </div>
  );
};

export default GameResultDialog;
