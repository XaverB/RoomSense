@echo off

REM Check if the frontend directory exists
if not exist "../../frontend" (
    echo Frontend directory not found.
    exit /b 1
)

REM Navigate to the frontend directory
cd ..
cd ..
cd frontend

REM Run the frontend build script
echo Running frontend build script...
call build.bat

REM Check if the frontend build script was successful
if %errorlevel% neq 0 (
    echo Frontend build script failed.
REM    exit /b 1
)

REM Navigate back to the ingress directory
cd ..
cd deploy
cd ingress

REM Build the Docker image
echo Building Docker image...
docker build -t xaverb/roomsense-ingress:v1 .

REM Check if the Docker build was successful
if %errorlevel% neq 0 (
    echo Docker build failed.
    exit /b 1
)

REM Push the Docker image
echo Pushing Docker image...
docker push xaverb/roomsense-ingress:v1

REM Check if the Docker push was successful
if %errorlevel% neq 0 (
    echo Docker push failed.
    exit /b 1
)

echo Build and push completed successfully.