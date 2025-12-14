import React, { useState, useEffect } from 'react';
import './PlayerNameModal.css';
import avatarGif from '../../assets/avatar.gif';

interface PlayerNameModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (playerName: string) => void;
  gameName: string;
}

const PlayerNameModal: React.FC<PlayerNameModalProps> = ({ isOpen, onClose, onSubmit, gameName }) => {
  const [playerName, setPlayerName] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    // Load saved player name from localStorage
    const savedName = localStorage.getItem('pdsa_player_name');
    if (savedName) {
      setPlayerName(savedName);
    }
  }, [isOpen]);

  useEffect(() => {
    // Handle escape key to close modal
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === 'Escape' && isOpen) {
        onClose();
      }
    };

    document.addEventListener('keydown', handleEscape);
    return () => document.removeEventListener('keydown', handleEscape);
  }, [isOpen, onClose]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    const trimmedName = playerName.trim();
    
    if (!trimmedName) {
      setError('Please enter your name');
      return;
    }

    if (trimmedName.length < 2) {
      setError('Name must be at least 2 characters');
      return;
    }

    if (trimmedName.length > 50) {
      setError('Name must be less than 50 characters');
      return;
    }

    // Save player name to localStorage
    localStorage.setItem('pdsa_player_name', trimmedName);
    
    setError('');
    onSubmit(trimmedName);
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPlayerName(e.target.value);
    setError(''); // Clear error when user starts typing
  };

  if (!isOpen) return null;

  return (
    <div className="player-modal-overlay" onClick={onClose}>
      <div className="player-modal-content" onClick={(e) => e.stopPropagation()}>
        <button className="player-modal-close" onClick={onClose} aria-label="Close">
          ×
        </button>
        
        <div className="player-modal-header">
          {/* Avatar image above the title */}
          <img src={avatarGif} alt="Player Avatar" className="player-modal-avatar" />
          <h2>Welcome, Player!</h2>
          <p>Enter your name to start playing <strong>{gameName}</strong></p>
        </div>

        <form onSubmit={handleSubmit} className="player-modal-form">
          <div className="player-input-group">
            <label htmlFor="player-name">Your Name</label>
            <input
              id="player-name"
              type="text"
              value={playerName}
              onChange={handleInputChange}
              placeholder="Enter your name..."
              className={error ? 'error' : ''}
              autoFocus
              maxLength={50}
            />
            {error && <span className="player-error-message">{error}</span>}
          </div>

          <div className="player-modal-actions">
            <button type="button" onClick={onClose} className="player-btn-cancel">
              Cancel
            </button>
            <button type="submit" className="player-btn-submit">
              Start Game
            </button>
          </div>
        </form>

        <div className="player-modal-info">
          <small>💡 Your name will be saved for future games</small>
        </div>
      </div>
    </div>
  );
};

export default PlayerNameModal;
