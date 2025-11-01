using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;

namespace BoreGame;

public class ProfileManager
{
    private readonly string _profilesPath;

    public static Bitmap? FromBase64(string base64String)
    {
        try
        {
            // Удаляем data URI схему если есть (например: "data:image/png;base64,...")
            if (base64String.Contains(","))
            {
                base64String = base64String.Split(',')[1];
            }

            // Конвертируем base64 в byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            
            // Создаем MemoryStream
            using var ms = new MemoryStream(imageBytes);
            
            // Возвращаем Bitmap который можно использовать напрямую в Source
            return new Bitmap(ms);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return null;
        }
    }
    
    // Конструктор с валидацией пути
    public ProfileManager(string profilesPath)
    {
        if (string.IsNullOrWhiteSpace(profilesPath))
            throw new ArgumentException("Путь не может быть пустым", nameof(profilesPath));

        _profilesPath = profilesPath;
    }

    // Основной метод для получения профилей
    public List<string> GetProfiles()
    {
        try
        {
            if (!Directory.Exists(_profilesPath))
            {
                Console.WriteLine($"Папка не существует: {_profilesPath}");
                return new List<string>();
            }

            return Directory.GetFiles(_profilesPath, "*.json").ToList();
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Нет доступа к папке: {ex.Message}");
            return new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении профилей: {ex.Message}");
            return new List<string>();
        }
    }

    // Получить только имена файлов без расширения
    public List<string> GetProfileNames()
    {
        return GetProfiles()
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();
    }

    // Получить только имена файлов с расширением
    public List<string> GetProfileFileNames()
    {
        return GetProfiles()
            .Select(Path.GetFileName)
            .ToList();
    }

    // Проверить существование конкретного профиля
    public bool ProfileExists(string profileName)
    {
        if (!profileName.EndsWith(".json"))
            profileName += ".json";

        string fullPath = Path.Combine(_profilesPath, profileName);
        return File.Exists(fullPath);
    }

    // Получить полный путь к профилю
    public string GetProfilePath(string profileName)
    {
        if (!profileName.EndsWith(".json"))
            profileName += ".json";

        return Path.Combine(_profilesPath, profileName);
    }

    // Получить количество профилей
    public int GetProfileCount()
    {
        return GetProfiles().Count;
    }
}