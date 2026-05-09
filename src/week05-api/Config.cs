using System;
using DotNetEnv;

namespace Week05;

public static class Config
{
    public static void Load()
    {
        Env.Load();
    }

    public static string GetEnv(string key, string defaultValue = "")
    {
        return Environment.GetEnvironmentVariable(key) ?? defaultValue;
    }
}
