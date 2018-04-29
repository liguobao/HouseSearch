#!/bin/sh
image_version=`date +%Y%m%d%H%M`;
echo $image_version;
cd ~/code/58HouseSearch/HouseCrawler.Core/HouseCrawler.Core;
git pull --rebase origin master;
sudo docker ps;
sudo docker stop house-crawler;
sudo docker rm house-crawler;
sudo docker ps;
sudo docker build -t house-crawler:$image_version .;
sudo docker images;
sudo docker run -p 8080:80 -v ~/docker-data/house-crawler/appsettings.json:/app/appsettings.json -v ~/docker-data/house-crawler/NLogFile/:/app/NLogFile --restart=always --name house-crawler house-crawler:$image_version;