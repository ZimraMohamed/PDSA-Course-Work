import React from 'react';

interface Props {
  players?: { name: string; position: number }[];
  boardSize?: number; // legacy prop name
  size?: number; // preferred prop name
}

const GameBoard: React.FC<Props> = ({ players = [], boardSize = 10, size }) => {
  const n = size ?? boardSize ?? 10;
  const cells = n * n;

  // Generate snake-style numbering: start at 1 on bottom-left, alternate direction each row.
  const rows: number[][] = [];
  for (let r = 0; r < n; r++) {
    // r = 0 is bottom row (logical)
    const start = r * n + 1;
    const rowNums = Array.from({ length: n }, (_, i) => start + i);
    if (r % 2 === 1) rowNums.reverse();
    rows.push(rowNums);
  }

  // We need to render rows top-to-bottom, so reverse the rows array for rendering
  const renderRows = rows.slice().reverse();
  const numbers = renderRows.flat();

  const gridStyle: React.CSSProperties = {
    display: 'grid',
    gridTemplateColumns: `repeat(${n}, 1fr)`,
    gap: 6,
  };

  const cellStyle: React.CSSProperties = {
    background: '#fff',
    border: '1px solid #e6eef8',
    borderRadius: 6,
    padding: 8,
    minHeight: 48,
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontWeight: 600,
    color: '#1f2937',
  };

  return (
    <div className="sal-gameboard" style={{ padding: 12, background: '#f8fafc', borderRadius: 8 }}>
      <h4 style={{ margin: 0, marginBottom: 8 }}>Game Board ({n} x {n})</h4>
      <div style={{ fontSize: 13, color: '#6b7280', marginBottom: 8 }}>Numbered squares (no snakes/ladders yet)</div>

      <div style={gridStyle}>
        {numbers.map((num) => (
          <div key={num} style={cellStyle}>{num}</div>
        ))}
      </div>

      {players.length > 0 && (
        <div style={{ marginTop: 12, fontSize: 13, color: '#374151' }}>
          <strong>Players</strong>
          <ul style={{ marginTop: 6 }}>
            {players.map((p) => (
              <li key={p.name}>{p.name}: {p.position}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
};

export default GameBoard;
