#!/bin/bash

docker-compose build

for img in $(docker-compose config | awk '{if ($1 == "image:") print $2;}'); do
  images="$images $img"
done

echo $images


docker image save $images | docker -H "ssh://user@serverIp" image load
docker-compose -H "ssh root@164.90.164.161" up --force-recreate -d
docker-compose -H "ssh root@164.90.164.161" logs -f
read -p "Press any key to continue... " -n1 -s