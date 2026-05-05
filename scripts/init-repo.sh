#!/bin/bash
# Run this once after cloning to set up your local environment

set -e

echo "Setting up policy-rag-chatbot..."

# 1. Copy env file
if [ ! -f infra/.env ]; then
  cp infra/.env.example infra/.env
  echo "Created infra/.env — fill in your API keys before running"
else
  echo "infra/.env already exists, skipping"
fi

# 2. Create data dirs for Docker volumes
mkdir -p infra/data/qdrant
mkdir -p infra/data/redis

# 3. Start dev infrastructure (Qdrant + Redis)
echo "Starting Qdrant and Redis..."
docker compose -f infra/docker-compose.dev.yml up -d

echo ""
echo "Setup complete!"
echo ""
echo "Next steps:"
echo "  1. Edit infra/.env and add your ANTHROPIC_API_KEY and OPENAI_API_KEY"
echo "  2. cd src/week01-fundamentals"
echo "  3. dotnet run"
echo ""
echo "Qdrant dashboard: http://localhost:6333/dashboard"
echo "Redis:            localhost:6379"
