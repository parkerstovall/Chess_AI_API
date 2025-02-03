FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /Api

# Copy everything
COPY . .

RUN dotnet dev-certs https
RUN dotnet dev-certs https --trust
RUN dotnet restore
RUN dotnet publish -o out

CMD ["dotnet", "./out/ChessApi.dll"]
EXPOSE 3001

# Pushing new:
# docker build -t ghcr.io/parkerstovall/chess-ai_api .
# docker push ghcr.io/parkerstovall/chess-ai_api