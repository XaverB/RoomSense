param (
    [Parameter(Mandatory=$true)]
    [string]$Version
)

# Build the Docker container
Set-Location ../backend/roomsense-backend
docker build -t xaverb/roomsense-backend:$Version .
docker push xaverb/roomsense-backend:$Version

# Connect to the EC2 instance
$pemFilePath = "./AmazonES2.pem"
$ec2InstanceUrl = "ec2-user@ec2-13-48-13-136.eu-north-1.compute.amazonaws.com"

# SSH into the EC2 instance
ssh -i $pemFilePath $ec2InstanceUrl '
    # Navigate to the app directory
    cd ~/app

    # Update the version in the docker-compose.yml file
    sed -i "s/xaverb\/roomsense-backend:.*/xaverb\/roomsense-backend:$Version/" docker-compose.yml

    # Stop the existing Docker containers
    docker-compose down

    # Pull the new version of the Docker image
    docker-compose pull

    # Start the Docker containers with the new version
    docker-compose up -d
'