param (
    [Parameter(Mandatory=$true)]
    [string]$Version
)

# Build the Docker container
Set-Location ../backend/roomsense-backend
docker build -t xaverb/roomsense-backend:$Version .
docker push xaverb/roomsense-backend:$Version

# Connect to the EC2 instance
$pemFilePath = "path/to/your/AmazonES2.pem"
$ec2InstanceUrl = "ec2-user@ec2-13-48-13-136.eu-north-1.compute.amazonaws.com"

# SSH into the EC2 instance and execute the commands
ssh -i $pemFilePath $ec2InstanceUrl @"
    # Navigate to the app directory
    cd ~/app

    # Write the commands to a file
    cat > deploy_commands.sh <<EOL
    # Update the version in the docker-compose.yml file
    sed -i "s/xaverb\/roomsense-backend:.*/xaverb\/roomsense-backend:$Version/" docker-compose.yml

    # Stop the existing Docker containers
    docker-compose down

    # Pull the new version of the Docker image
    docker-compose pull

    # Start the Docker containers with the new version
    docker-compose up -d
EOL

    # Make the file executable
    chmod +x deploy_commands.sh

    # Execute the file
    ./deploy_commands.sh
"@