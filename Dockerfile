# Chọn .NET 9 SDK chính thức
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Thư mục làm việc trong container
WORKDIR /app

# Copy solution và project
COPY AppChat.sln ./
COPY AppChat/ ./AppChat/

# Restore và build project
RUN dotnet restore
RUN dotnet build --configuration Release

# Chuyển vào thư mục project
WORKDIR /app/AppChat

# Lệnh chạy app, dùng port Railway cấp
CMD ["dotnet", "run", "--urls", "http://0.0.0.0:$PORT"]
