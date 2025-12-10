import React from 'react';

interface Props {
  gameResult?: 'win' | 'lose' | 'draw' | string | null;
}

const GameStatus: React.FC<Props> = ({ gameResult = null }) => {
  if (!gameResult) return null;

  const key = String(gameResult).toLowerCase();

  if (key === 'win') {
    return <div style={{ color: '#065f46', fontWeight: 700 }}>you won!</div>;
  }

  if (key === 'lose' || key === 'lost') {
    return <div style={{ color: '#9f1239', fontWeight: 700 }}>you lost!</div>;
  }

  if (key === 'draw') {
    return <div style={{ color: '#92400e', fontWeight: 700 }}>draw</div>;
  }

  // fallback: print the raw result
  return <div style={{ fontWeight: 700 }}>{gameResult}</div>;
};

export default GameStatus;
