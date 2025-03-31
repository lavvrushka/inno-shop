FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm cache clean --force
RUN npm install --force
COPY . .
RUN npm run build
CMD ["npm", "start"]