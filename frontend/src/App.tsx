import React, { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend } from 'recharts';

interface Room {
  id: number;
  name: string;
}

interface SensorReading {
  timestamp: string;
  value: number;
}

interface Alarm {
  id: number;
  roomName: string;
  message: string;
  timestamp: string;
}

interface Recommendation {
  id: number;
  roomName: string;
  message: string;
  timestamp: string;
}

const App: React.FC = () => {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [selectedRoom, setSelectedRoom] = useState<number | null>(null);
  const [sensorData, setSensorData] = useState<SensorReading[]>([]);
  const [alarms, setAlarms] = useState<Alarm[]>([]);
  const [recommendations, setRecommendations] = useState<Recommendation[]>([]);

  useEffect(() => {
    fetch('/api/Rooms')
      .then(response => response.json())
      .then(data => setRooms(data));

    const pollData = () => {
      fetch('/api/Alarms')
        .then(response => response.json())
        .then(data => setAlarms(data));

      fetch('/api/Recommendations')
        .then(response => response.json())
        .then(data => setRecommendations(data));
    };

    pollData();
    const interval = setInterval(pollData, 5000);

    return () => {
      clearInterval(interval);
    };
  }, []);

  const handleRoomChange = (roomId: number) => {
    setSelectedRoom(roomId);
    fetch(`/api/SensorData/room/${roomId}`)
      .then(response => response.json())
      .then(data => setSensorData(data));
  };

  return (
    <div className="container px-4 py-8 mx-auto">
      <h1 className="mb-6 text-4xl font-bold text-center text-indigo-600">Room Sensor Dashboard</h1>
      <div className="mb-8">
        <label htmlFor="room-select" className="block mb-2 font-bold text-gray-700">
          Select a room:
        </label>
        <select
          id="room-select"
          className="block w-full px-4 py-2 pr-8 leading-tight bg-white border border-gray-400 rounded shadow appearance-none hover:border-gray-500 focus:outline-none focus:shadow-outline"
          onChange={(e) => handleRoomChange(Number(e.target.value))}
        >
          <option value="">Select a room</option>
          {rooms.map(room => (
            <option key={room.id} value={room.id}>{room.name}</option>
          ))}
        </select>
      </div>

      {selectedRoom && (
        <div className="mb-8">
          <h2 className="mb-4 text-2xl font-bold text-gray-800">Sensor Values for Room {selectedRoom}</h2>
          <LineChart width={600} height={300} data={sensorData} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
            <XAxis dataKey="timestamp" />
            <YAxis />
            <CartesianGrid strokeDasharray="3 3" />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="value" stroke="#8884d8" activeDot={{ r: 8 }} />
          </LineChart>
        </div>
      )}

      <div className="grid grid-cols-1 gap-8 md:grid-cols-2">
        <div>
          <h2 className="mb-4 text-2xl font-bold text-gray-800">Alarms</h2>
          {alarms && alarms.length > 0 ? (
            <ul className="space-y-4">
              {alarms.map(alarm => (
                <li key={alarm.id} className="p-4 text-red-700 bg-red-100 border border-red-400 rounded shadow">
                  <p><strong>Room:</strong> {alarm.roomName}</p>
                  <p><strong>Message:</strong> {alarm.message}</p>
                  <p><strong>Timestamp:</strong> {alarm.timestamp}</p>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-600">No alarms</p>
          )}
        </div>

        <div>
          <h2 className="mb-4 text-2xl font-bold text-gray-800">Recommendations</h2>
          {recommendations && recommendations.length > 0 ? (
            <ul className="space-y-4">
              {recommendations.map(recommendation => (
                <li key={recommendation.id} className="p-4 text-blue-700 bg-blue-100 border border-blue-400 rounded shadow">
                  <p><strong>Room:</strong> {recommendation.roomName}</p>
                  <p><strong>Message:</strong> {recommendation.message}</p>
                  <p><strong>Timestamp:</strong> {recommendation.timestamp}</p>
                </li>
              ))}
            </ul>
          ) : (
            <p className="text-gray-600">No recommendations</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default App;