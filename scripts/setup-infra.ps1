# Infrastructure Setup Script for Windows
# This script starts Qdrant, Redis, and Ollama, and pulls the required models

$ErrorActionPreference = "Stop"

Write-Host "Setting up Policy RAG Infrastructure..." -ForegroundColor Cyan

# 1. Ensure infra/data directories exist
Write-Host "Creating data directories in infra/data/..." -ForegroundColor Gray
New-Item -ItemType Directory -Force -Path "infra/data/qdrant"
New-Item -ItemType Directory -Force -Path "infra/data/redis"
New-Item -ItemType Directory -Force -Path "infra/data/ollama"

# 2. Start containers (this will download images if missing)
Write-Host "Starting Docker containers..." -ForegroundColor Yellow
docker compose -f infra/docker-compose.infra.yml up -d

Write-Host "Waiting for Ollama to be ready..." -ForegroundColor Yellow
$maxRetries = 60 # Increased timeout for slow connections
$retryCount = 0
$ready = $false

while (-not $ready -and $retryCount -lt $maxRetries) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:11434/api/tags" -UseBasicParsing -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200) {
            $ready = $true
        }
    } catch { }
    
    if (-not $ready) {
        Start-Sleep -Seconds 2
        $retryCount++
        Write-Host "." -NoNewline
    }
}

if (-not $ready) {
    Write-Error "Ollama failed to start within the timeout period. Please check Docker Desktop."
}

Write-Host "`nOllama is ready. Downloading LLM models..." -ForegroundColor Cyan

# 3. Pull models
Write-Host "Downloading llama3 (Chat Model)..." -ForegroundColor Gray
docker exec policy-rag-ollama ollama pull llama3

Write-Host "Downloading nomic-embed-text (Embedding Model)..." -ForegroundColor Gray
docker exec policy-rag-ollama ollama pull nomic-embed-text

Write-Host "`nInfrastructure setup complete!" -ForegroundColor Green
Write-Host "Qdrant: http://localhost:6333"
Write-Host "Redis:  localhost:6379"
Write-Host "Ollama: http://localhost:11434"
