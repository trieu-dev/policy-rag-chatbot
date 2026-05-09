using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Week05;

public interface IHistoryStore
{
    ChatHistory GetHistory(string chatId);
    void SaveHistory(string chatId, ChatHistory history);
    void DeleteHistory(string chatId);
    IEnumerable<string> GetChatIds();
}

public class HistoryStore : IHistoryStore
{
    private readonly ConcurrentDictionary<string, ChatHistory> _store = new();
    private readonly string _storagePath = "data/history";

    public HistoryStore()
    {
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
        LoadFromDisk();
    }

    public ChatHistory GetHistory(string chatId)
    {
        return _store.GetOrAdd(chatId, _ => new ChatHistory("You are a helpful assistant answering policy questions. Always cite your sources."));
    }

    public void SaveHistory(string chatId, ChatHistory history)
    {
        _store[chatId] = history;
        SaveToDisk(chatId, history);
    }

    public void DeleteHistory(string chatId)
    {
        _store.TryRemove(chatId, out _);
        var path = Path.Combine(_storagePath, $"{chatId}.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public IEnumerable<string> GetChatIds() => _store.Keys;

    private void SaveToDisk(string chatId, ChatHistory history)
    {
        var path = Path.Combine(_storagePath, $"{chatId}.json");
        var json = JsonSerializer.Serialize(history);
        File.WriteAllText(path, json);
    }

    private void LoadFromDisk()
    {
        foreach (var file in Directory.GetFiles(_storagePath, "*.json"))
        {
            var chatId = Path.GetFileNameWithoutExtension(file);
            var json = File.ReadAllText(file);
            var history = JsonSerializer.Deserialize<ChatHistory>(json);
            if (history != null)
            {
                _store[chatId] = history;
            }
        }
    }
}
