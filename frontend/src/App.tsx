import React from 'react';
import logo from './logo.svg';
import SmartSense from './components/SmartSense';
import RoomSense from './components/RoomSense';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import './App.css';

function App() {
  return (
    <Router>
    <Routes>
      <Route path="/" element={<SmartSense />} />
      <Route path="/room/:id" element={<RoomSense />} />
    </Routes>
  </Router>
  );
}

export default App;
