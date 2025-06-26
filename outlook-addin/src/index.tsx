import React from 'react';
import ReactDOM from 'react-dom/client';
import TaskPane from './components/TaskPane.tsx';
import './index.css';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  <React.StrictMode>
    <TaskPane />
  </React.StrictMode>
);