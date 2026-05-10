#!/bin/bash
# Infrastructure Setup Script
set -e

echo "Setting up Policy RAG Infrastructure..."

# 1. Ensure infra/data directories exist
mkdir -p infra/data/qdrant
mkdir -p infra/data/redis
mkdir -p infra/data/ollama

# 2. Start containers
echo "Starting Docker containers..."
docker compose -f infra/docker-compose.infra.yml up -d

echo "Waiting for Ollama to be ready..."
until curl -s http://localhost:11434/api/tags > /dev/null; do
  echo -n "."
  sleep 2
done

echo -e "\nOllama is ready. Downloading LLM models..."

# 3. Pull models
echo "Downloading llama3 (Chat Model)..."
docker exec policy-rag-ollama ollama pull llama3

echo "Downloading nomic-embed-text (Embedding Model)..."
docker exec policy-rag-ollama ollama pull nomic-embed-text

echo -e "\nInfrastructure setup complete!"
