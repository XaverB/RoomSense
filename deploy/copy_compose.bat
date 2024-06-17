scp -i "AmazonES2.pem" docker-compose.yml ec2-user@ec2-13-48-117-248.eu-north-1.compute.amazonaws.com:/home/ec2-user/app/
scp -i "AmazonES2.pem" xaverb.dev.cer ec2-user@ec2-13-48-117-248.eu-north-1.compute.amazonaws.com:/home/ec2-user/app/
scp -i "AmazonES2.pem" ./ingress/index.html ec2-user@ec2-13-48-117-248.eu-north-1.compute.amazonaws.com:/home/ec2-user/app/
scp -i "AmazonES2.pem" ./ingress/nginx.conf ec2-user@ec2-13-48-117-248.eu-north-1.compute.amazonaws.com:/home/ec2-user/app/

scp -i "AmazonES2.pem" ./xaverb.dev.key ec2-user@ec2-13-48-117-248.eu-north-1.compute.amazonaws.com:/home/ec2-user/app/
