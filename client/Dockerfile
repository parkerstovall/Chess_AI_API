# syntax=docker/dockerfile:1

FROM node:lts-alpine
WORKDIR /client
COPY package.json .
RUN npm install
COPY . .
RUN npm run build
CMD ["npx", "vite", "--host"]
EXPOSE 3000

# Pushing new:
# docker build -t ghcr.io/parkerstovall/chess-ai_client .
# docker push ghcr.io/parkerstovall/chess-ai_client