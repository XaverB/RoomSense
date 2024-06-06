import { env } from 'process';
import React, { useState, useEffect } from 'react';
import { VmWrapper, Url, TaggedVm } from '../models/models';
import * as signalRService from '../services/SignalRService';
import { useParams } from 'react-router-dom';

const RoomSense: React.FC = () => {
  const SERVICE_URL = process.env.REACT_APP_SERVICE_URL;
  const { id } = useParams();
  const [rooms, setRooms] = useState<Array<{ id: number, value: number, timestamp: string, type: string }>>([]);
  const [recommendations, setRecommendations] = useState<Array<{ id: number, message: string, timestamp: string }>>([]);

  useEffect(() => {
    // signalRService.connect();
    fetchRoom(parseInt(id ?? "0"));
    fetchRecommendations(parseInt(id ?? "0"));
    // signalRService.registerOnClassificationReceived((message: TaggedVm) => {
    //   if(message?.imageId === imageId){
    //     setClassification(message?.classification);
    //   }
    // });

    // Cleanup on component unmount
  }, []);

  const fetchRoom = async (id: number) => {
    const sensorTypes = ["humidity", "temperature", "co2"];

    await sensorTypes.map(async (x) => {
      const roomsResponse = await fetch(`${SERVICE_URL}/api/SensorData/room/${id}?type=${x}`);
      if(roomsResponse.ok) {
        const roomsData: any[] = await roomsResponse.json();
        roomsData.forEach(y => y.type = x);
        setRooms(rooms.concat(roomsData));
      } else {
        console.error('Failed to get sensor values:', roomsResponse.status, await roomsResponse.text());
      }
    });
    
  };

  const fetchRecommendations = async (id: number) => {
    const recommendationsResponse = await fetch(`${SERVICE_URL}/api/Recommendations/room/${id}`);
    if(recommendationsResponse.ok) {
      const recommendationData: any[] = await recommendationsResponse.json();
      setRecommendations(recommendationData);
    } else {
      console.error('Failed to get recommendations:', recommendationsResponse.status, await recommendationsResponse.text());
    }
  };

  return (
    <div className='pt-5 max-w-xl mx-auto my-10 p-5 text-left'>
      {recommendations.length > 0 && (<label className='font-bold'>Recommendations: </label>)}
            {recommendations.map(({ id, message, timestamp }) => (
              <div key={id} className="mx-auto p-5 border-b-2 grid grid-cols-5 text-left">
                <div className='text-left col-span-4'>{message}</div>
                <div className='text-right font-thin text-xs flex justify-end items-end'>{(new Intl.DateTimeFormat('en-US', { dateStyle: 'short' })).format(Date.parse(timestamp))}</div>
              </div>
            ))}
      <br/>
      <label className='font-bold'>Sensor Values: </label>
            {rooms.map(({ id, value, timestamp, type }) => (
              <div key={id} className="mx-auto p-5 border-b-2 grid grid-cols-5 text-center">
                <div className='text-left font-bold col-span-4'>{type}</div>
                <div className='text-right font-thin text-xs flex justify-end items-end'>{(new Intl.DateTimeFormat('en-US', { dateStyle: 'short' })).format(Date.parse(timestamp))}</div>
                <div className='text-left col-span-5'>{value}</div>
              </div>
            ))}
    </div>
  );
};

export default RoomSense;
