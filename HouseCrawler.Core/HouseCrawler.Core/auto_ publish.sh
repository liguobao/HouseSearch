#!/bin/sh
image_version=`date +%Y%m%d%H%M`;
echo $image_version;
cd ~/code/58HouseSearch/HouseCrawler.Core/HouseCrawler.Core;
git pull --rebase origin master;
docker stop house-crawler;
docker rm house-crawler;
docker build -t house-crawler:$image_version .;
docker images;
docker ps;
docker run -p 8080:80 -v ~/docker-data/house-crawler/appsettings.json:/app/appsettings.json -v ~/docker-data/house-crawler/NLogFile/:/app/NLogFile --restart=always --name house-web house-crawler:$image_version;