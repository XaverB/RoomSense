#!/bin/bash

# Set the AWS region
AWS_REGION="eu-north-1a"

# Set the EC2 instance ID
INSTANCE_ID="i-0713779c24a6cdc00"

# Get the public IP address of the EC2 instance
PUBLIC_IP=$(aws ec2 describe-instances --region $AWS_REGION --instance-ids $INSTANCE_ID --query 'Reservations[].Instances[].PublicIpAddress' --output text)

echo "Public IP address: $PUBLIC_IP"

# Get the security group information
SECURITY_GROUPS=$(aws ec2 describe-instances --region $AWS_REGION --instance-ids $INSTANCE_ID --query 'Reservations[].Instances[].SecurityGroups[].GroupId' --output text)

echo "Security Groups: $SECURITY_GROUPS"

# Describe the security group rules
for GROUP_ID in $SECURITY_GROUPS; do
  echo "Security Group: $GROUP_ID"
  aws ec2 describe-security-group-rules --region $AWS_REGION --filter Name=group-id,Values=$GROUP_ID
done

# Get the Docker Compose file content
echo "Docker Compose file content:"
ssh -i "AmazonES2.pem" ec2-user@$PUBLIC_IP "cat /home/ec2-user/app/docker-compose.yml"

# Get the Docker container logs
echo "Docker container logs:"
ssh -i "AmazonES2.pem" ec2-user@$PUBLIC_IP "docker-compose -f /home/ec2-user/app/docker-compose.yml logs"

# Get the backend application configuration
echo "Backend application configuration:"
ssh -i "AmazonES2.pem" ec2-user@$PUBLIC_IP "cat /path/to/your/backend/config"