# RoomSense
Monitoring of Room Temperature and Air Quality

To efficiently monitor room conditions, rooms are equipped with sensors for temperature and carbon dioxide (CO2). These sensors continuously capture data, which is collected by an Arduino and transmitted to a central server using the MQTT protocol. The captured data is accessible through a user-friendly web application. This platform not only enables real-time monitoring, but also provides automated recommendations for optimal ventilation times, based on the current air quality and temperature. In addition to the web application, users can directly receive recommendations for the next ventilation time via a specially developed Alexa skill, accessed by voice command. This increases the comfort and accessibility of the data for all users.

Another feature is an alarm system that is activated when critical air quality values are reached. This alarm warns the occupants of potentially hazardous conditions and suggests immediate measures to improve the indoor air quality.
