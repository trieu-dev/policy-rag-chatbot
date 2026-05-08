using System;
using DotNetEnv;

namespace Week04;

public static class Config
{
    public static void Load()
    {
        // Load .env from the infra folder
        string envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "infra", ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }
    }

    public static string GetEnv(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}
