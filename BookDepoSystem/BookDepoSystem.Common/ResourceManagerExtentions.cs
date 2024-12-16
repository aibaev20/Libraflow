using System;
using System.Resources;

namespace BookDepoSystem.Common;

public static class ResourceManagerExtentions
{
    public static string GetStringOrDefault(this ResourceManager resourceManager, string resourceKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
            {
                return string.Empty;
            }

            return resourceManager.GetString(resourceKey) ?? string.Empty;
        }
        catch (Exception)
        {
            return resourceKey;
        }
    }
}