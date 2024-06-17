# Set the AWS region
$awsRegion = "eu-north-1"

# Set the EC2 instance ID
$instanceId = "i-0713779c24a6cdc00"

# Get the public IP address of the EC2 instance
$publicIp = (aws ec2 describe-instances --region $awsRegion --instance-ids $instanceId --query 'Reservations[].Instances[].PublicIpAddress' --output text)

Write-Host "Public IP address: $publicIp"

# Get the security group information
$securityGroups = (aws ec2 describe-instances --region $awsRegion --instance-ids $instanceId --query 'Reservations[].Instances[].SecurityGroups[].GroupId' --output text)

Write-Host "Security Groups: $securityGroups"

# Describe the security group rules
foreach ($groupId in $securityGroups) {
    Write-Host "Security Group: $groupId"
    aws ec2 describe-security-group-rules --region $awsRegion --filter Name=group-id,Values=$groupId
}

# Get the Docker Compose file content
Write-Host "Docker Compose file content:"
ssh -i "AmazonES2.pem" ec2-user@$publicIp "cat /home/ec2-user/app/docker-compose.yml"

# Get the Docker container logs
Write-Host "Docker container logs:"
ssh -i "AmazonES2.pem" ec2-user@$publicIp "docker-compose -f /home/ec2-user/app/docker-compose.yml logs"

# Get the backend application configuration
Write-Host "Backend application configuration:"
ssh -i "AmazonES2.pem" ec2-user@$publicIp "cat /path/to/your/backend/config"