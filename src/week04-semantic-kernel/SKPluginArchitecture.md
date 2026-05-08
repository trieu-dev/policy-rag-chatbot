# Semantic Kernel Plugin Architecture

## Overview
Semantic Kernel (SK) uses a plugin-based architecture to extend the capabilities of Large Language Models (LLMs). Plugins are collections of "functions" (either semantic prompts or native code) that the kernel can orchestrate.

## Native Plugins
In this project, we implemented a **Native Plugin** (`RagPlugin.cs`). Native plugins are C# classes where methods are decorated with the `[KernelFunction]` attribute.

### Key Components:
1. **[KernelFunction]**: This attribute marks a method as available to the Semantic Kernel.
2. **[Description]**: Descriptions are crucial! They are used by the LLM (when using automatic function calling) to understand *when* and *how* to use the function.
3. **Parameter Descriptions**: Likewise, describing parameters helps the LLM provide the correct arguments.

## Why use Plugins?
Instead of manually wiring every step (e.g., manually searching a database and then manually injecting it into a prompt), we can register these capabilities as plugins.

### Orchestration Flow:
- **SearchMemory**: The plugin searches the vector database (Kernel Memory).
- **BuildPrompt**: The kernel takes the output of the function and injects it into the prompt template.
- **Ask**: The final augmented prompt is sent to the LLM.

## Kernel Memory Integration
We used `Microsoft.KernelMemory` inside our plugin. While SK has its own memory abstractions, Kernel Memory (KM) provides a more robust, document-oriented approach with built-in support for citations, partitioning, and complex ingestion pipelines. Our plugin acts as the "bridge" between the Kernel's orchestration and Kernel Memory's retrieval.
