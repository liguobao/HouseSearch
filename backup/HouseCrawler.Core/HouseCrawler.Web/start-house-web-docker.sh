#!/bin/sh
image_version=`date +%Y%m%d%H%M`;
echo $image_version;
cd ~/code/58HouseSearch/HouseCrawler.Core/HouseCrawler.Web;
git pull --rebase origin master;
docker stop house-web;
docker rm house-web;
docker build -t house-web:$image_version .;
docker images;
docker run -p 8080:80 -v ~/docker-data/house-web/appsettings.json:/app/appsettings.json -v ~/docker-data/house-web/NLogFile/:/app/NLogFile  --restart=always --name house-web -d house-web:$image_version;
docker logs house-web;