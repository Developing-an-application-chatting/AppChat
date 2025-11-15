#!/bin/bash
# start.sh - dùng để build & run app .NET trên Railway

# 1. Chuyển vào thư mục project
cd AppChat

# 2. Build project
dotnet build

# 3. Chạy project, lắng nghe trên port do Railway cấp
# Railway tự đặt biến môi trường $PORT
dotnet run --urls "http://0.0.0.0:$PORT"