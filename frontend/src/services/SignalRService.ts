import * as signalR from "@microsoft/signalr";
import { env } from 'process';
import { TaggedVm } from '../models/models';

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`https://localhost:43457/notificationHub`) // Update the URL to where your hub is hosted
    .configureLogging(signalR.LogLevel.Information)
    .build();


export const connect = async () => {
    try {
        if(connection.state === signalR.HubConnectionState.Disconnected){
            await connection.start();
        }
    } catch (err) {
        console.log(err);
        setTimeout(connect, 5000); // Retry connection
    }
};


export const stop = async () => {
    await connection.stop();
};

export const registerOnClassificationReceived = (callback: (msg: TaggedVm) => any) => {
    connection.on("Classification", callback);
};

export default connection;