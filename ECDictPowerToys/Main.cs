using Wox.Plugin;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;
using System.Windows;
using ManagedCommon;

namespace Community.PowerToys.Run.Plugin.ECDict;

public class Main : IPlugin, IDelayedExecutionPlugin
{
    public string Name => "ECDict";
    public static string PluginID => "C5BF1E05CD25451A8CC02B216A44C3F7";
    public string Description => "一个离线的英中词典";
    private string _queryCommandText =
@"SELECT * FROM Dictionary
WHERE word COLLATE NOCASE LIKE '% ' || $word || ' %'
OR word COLLATE NOCASE LIKE '% ' || $word
OR word COLLATE NOCASE LIKE $word || ' %'
OR word COLLATE NOCASE = $word
ORDER BY 
  CASE 
    WHEN word COLLATE NOCASE = $word THEN 0
    ELSE 1
  END,
  word;
";
    private SQLiteConnection? _dbConnection;
    private string _icoPath = null!;
    private PluginInitContext _context = null!;

    public void Init(PluginInitContext context)
    {
        context.API.ThemeChanged += ThemeChanged;
        UpdateIconPath(context.API.GetCurrentTheme());
        _context = context;
        var dbPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new Exception("Failed to get plugin directory");
        dbPath = Path.Combine(dbPath, "ecdict.db");
        _dbConnection = new SQLiteConnection($"Data Source={dbPath};Version=3;") ?? throw new Exception("Failed to create database connection");
        _dbConnection.Open();
    }

    private void ThemeChanged(Theme oldTheme, Theme newTheme)
    {
        UpdateIconPath(newTheme);
    }

    public List<Result> Query(Query query)
    {
        return new();
    }

    public List<Result> Query(Query query, bool delayedExecution)
    {
        if (_dbConnection is null)
        {
            throw new Exception("Failed to connect to database");
        }
        var searchWord = query.Search;
        var cmd = _dbConnection.CreateCommand();
        cmd.CommandText = _queryCommandText;
        cmd.Parameters.AddWithValue("$word", searchWord.Trim());
        var reader = cmd.ExecuteReader();
        var results = new List<Result>();
        while (reader.Read())
        {
            var word = reader["word"] as string;
            var translation = reader["translation"] as string;
            translation = translation?.Replace("\\n", Environment.NewLine);
            var entry = new Dictionary<string, object>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                entry.Add(reader.GetName(i), reader.GetValue(i));
            }
            var result = new Result
            {
                Title = word,
                SubTitle = translation,
                IcoPath = _icoPath,
                Action = _ =>
                {

                    new WordWindow(entry, IsThemeDark(_context.API.GetCurrentTheme())).Show();
                    return true;
                }
            };
            results.Add(result);
        }
        return results;
    }

    private void UpdateIconPath(Theme theme)
    {
        if (!IsThemeDark(theme))
        {
            _icoPath = "Images/icon-light.png";
        }
        else
        {
            _icoPath = "Images/icon-dark.png";
        }
    }

    private bool IsThemeDark(Theme theme)
        => theme == Theme.Dark || theme == Theme.HighContrastBlack;
}
