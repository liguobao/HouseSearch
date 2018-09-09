FROM node:10 AS build-env
WORKDIR /app
COPY ./package.json . 
RUN npm install
COPY . .
RUN npm run build

FROM nginx

WORKDIR /usr/share/nginx/html

ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

COPY --from=build-env /app/dist /usr/share/nginx/html/

EXPOSE 80

