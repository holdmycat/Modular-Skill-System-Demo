#!/bin/bash

# Configuration
PROJECT_PATH="./BsonGenerator.csproj"
OUTPUT_DLL="./bin/Release/netstandard2.0/BsonGenerator.dll"
DESTINATION_DIR="../Assets/Plugins/SourceGenerators/BsonGenerator/"

# Check for msbuild
if command -v msbuild &> /dev/null; then
    BUILD_CMD="msbuild /p:Configuration=Release /restore"
elif [ -f "/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild" ]; then
    BUILD_CMD="/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild /p:Configuration=Release /restore"
else
    echo "Error: msbuild not found. Please install Mono or .NET SDK."
    exit 1
fi

echo "Building BsonGenerator..."
$BUILD_CMD $PROJECT_PATH

if [ $? -eq 0 ]; then
    echo "Build successful."
    
    # Ensure destination exists
    mkdir -p "$DESTINATION_DIR"
    
    echo "Copying DLL to Unity project..."
    cp "$OUTPUT_DLL" "$DESTINATION_DIR"
    
    echo "Done! Please switch to Unity and ensure the DLL has the 'RoslynAnalyzer' label."
else
    echo "Build failed."
    exit 1
fi
