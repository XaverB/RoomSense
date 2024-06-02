import { env } from 'process';
import React, { useState, useEffect } from 'react';
import { VmWrapper, Url, TaggedVm } from '../models/models';
import * as signalRService from '../services/SignalRService';

const SmartSense: React.FC = () => {
  const SERVICE_URL = process.env.REACT_APP_SERVICE_URL;
  const [rooms, setRooms] = useState<Array<{ id: number, name: string, location: string }>>([]);
  const [sensors, setSensors] = useState<Array<{ id: number, type: string, roomId: number, roomName: string }>>([]);

  useEffect(() => {
    // signalRService.connect();
    fetchRooms();
    fetchSensors();
    // signalRService.registerOnClassificationReceived((message: TaggedVm) => {
    //   if(message?.imageId === imageId){
    //     setClassification(message?.classification);
    //   }
    // });

    // Cleanup on component unmount
  }, []);

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
      <label className='font-bold'>Rooms: </label>
            {rooms.map(({ id, name, location }) => (
              <div key={id} className="max-w-xl mx-auto my-10 p-5 border rounded shadow-lg hover:bg-gray-50 hover:cursor-pointer text-center">
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
              </div>
            ))}
    </div>
  );
};

export default SmartSense;
