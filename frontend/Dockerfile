# Use an official Node environment as a parent image
FROM node:16

# Set the working directory in the container
WORKDIR /usr/src/app

# Copies package.json and package-lock.json to Docker environment
COPY package*.json ./

# Install any needed packages specified in package.json
RUN npm install

# Bundle app source inside Docker image
COPY . .

# Bind node_modules to a named volume
VOLUME /usr/src/app/node_modules

# Make port 3000 available for the app
EXPOSE 3000

# Set environment variable for Chokidar polling
ENV CHOKIDAR_USEPOLLING=true
ENV CHOKIDAR_INTERVAL=1000

# Command to run the app
CMD ["npm", "start"]
