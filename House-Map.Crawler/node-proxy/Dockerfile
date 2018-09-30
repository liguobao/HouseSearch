FROM node:10 AS build-env
WORKDIR /app
COPY package-lock.json package.json ./
RUN npm install
COPY . /app

EXPOSE 3000
CMD ["node","/app/app.js"]