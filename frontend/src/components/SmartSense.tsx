import { env } from 'process';
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const SmartSense: React.FC = () => {
  const SERVICE_URL = process.env.REACT_APP_SERVICE_URL;
  const [rooms, setRooms] = useState<Array<{ id: number, name: string, location: string }>>([]);
  const [alarms, setAlarms] = useState<Array<{ id: number, roomName: string, roomId: number, message: string, timestamp: string }>>([]);
  const [sensors, setSensors] = useState<Array<{ id: number, type: string, roomId: number, roomName: string }>>([]);
  const navigate = useNavigate();

  useEffect(() => {
    fetchRooms();
    fetchSensors();
    fetchAlarms();
    // Cleanup on component unmount
  }, []);

  const handleNavigation = (id: number) => {
    navigate(`/room/${id}`);
  };

  const fetchAlarms = async () => {
    const alarmsResponse = await fetch(`${SERVICE_URL}/api/Alarms`);
    if(alarmsResponse.ok) {
      const alarmsData = await alarmsResponse.json();
      setAlarms(alarmsData);
    } else {
      console.error('Failed to get alarms:', alarmsResponse.status, await alarmsResponse.text());
    }
  };

  const fetchRooms = async () => {
    const roomsResponse = await fetch(`${SERVICE_URL}/api/rooms`);
    if(roomsResponse.ok) {
      const roomsData = await roomsResponse.json();
      setRooms(roomsData);
    } else {
      console.error('Failed to get rooms:', roomsResponse.status, await roomsResponse.text());
    }
  };

  const fetchSensors = async () => {
    const sensorsResponse = await fetch(`${SERVICE_URL}/api/SensorData/sensors`);
    if(sensorsResponse.ok) {
      const sensorData: any[] = await sensorsResponse.json();
      setSensors(sensorData);
    } else {
      console.error('Failed to get rooms:', sensorsResponse.status, await sensorsResponse.text());
    }
  };

  return (
    <div className='pt-5 max-w-xl mx-auto my-10 p-5 text-left'>
      {alarms.length > 0 && (<label className='font-bold'>Alarms: </label>)}
            {alarms.map(({ id, message, timestamp, roomName }, idx) => (
              <div className={idx == alarms.length -1 ? "mx-auto p-5 mb-5" : "mx-auto p-5 border-b-2"}>
                <div key={id} className="grid grid-cols-4 text-left pb-3">
                  <div className='text-left font-bold col-span-3'>{roomName}</div>
                  <div className='text-right font-thin text-xs flex justify-end items-end'>{(new Intl.DateTimeFormat('en-US', { dateStyle: 'short' })).format(Date.parse(timestamp))}</div>
                </div>
                <div className='text-left'>{message}</div>
              </div>
            ))}

      <label className='font-bold'>Rooms: </label>
            {rooms.map(({ id, name, location }) => (
              <div key={id} className="max-w-xl mx-auto my-10 p-5 border rounded shadow-lg text-left">
                <div className='font-bold border-b-3 text-center pb-3'>{name}</div>
                <hr/>
                <p className='text-left pt-2'>{location}</p>
                <div className='pt-5 text-left'>
                    <label className='font-bold'>Sensors</label>
                    {sensors.filter(x => x.roomId === id).map(({ id, type}) => (
                      <div key={`Sensor-${id}`}>
                          {type}
                      </div>
                    ))}
                </div>
                <div className='text-right'>
                  <button onClick={() => handleNavigation(id)} className="bg-white hover:bg-gray-100 text-gray-800 font-semibold mt-3 py-2 px-4 border border-gray-400 rounded shadow">Show Room Details</button> 
                </div>
              </div>
            ))}
    </div>
  );
};

export default SmartSense;
