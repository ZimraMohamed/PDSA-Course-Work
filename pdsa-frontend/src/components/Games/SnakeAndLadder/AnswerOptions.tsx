import React from 'react';

interface Props {
  options?: string[];
  onSelect?: (option: string) => void;
}

const AnswerOptions: React.FC<Props> = ({ options = ['Option 1', 'Option 2', 'Option 3'], onSelect }) => {
  return (
    <div className="sal-answer-options" style={{ padding: 12, background: '#fff', borderRadius: 8, border: '1px solid #e6eef8' }}>
      <h4 style={{ margin: 0, marginBottom: 8 }}>Options</h4>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginTop: 8 }}>
        {options.map((opt) => (
          <button
            key={opt}
            onClick={() => onSelect && onSelect(opt)}
            style={{ padding: '8px 12px', background: '#2563eb', color: 'white', borderRadius: 6, border: 'none', cursor: 'pointer' }}
          >
            {opt}
          </button>
        ))}
      </div>
    </div>
  );
};

export default AnswerOptions;
