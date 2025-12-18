# Build the Blazor WebAssembly app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Serve using nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx website
RUN rm -rf ./*

# Copy published wwwroot output
COPY --from=build /app/publish/wwwroot ./

# Fly expects 8080 by default
EXPOSE 8080
RUN sed -i 's/listen       80;/listen 8080;/' /etc/nginx/conf.d/default.conf