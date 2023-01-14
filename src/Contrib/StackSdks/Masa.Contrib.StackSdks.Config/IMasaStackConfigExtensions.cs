// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Config;

public static class IMasaStackConfigExtensions
{
    public static Dictionary<string, JsonObject> GetAllServer(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(masaStackConfig.GetValue(MasaStackConfigConst.MASA_SERVER)) ?? new();
    }

    public static Dictionary<string, JsonObject> GetAllUI(this IMasaStackConfig masaStackConfig)
    {
        return JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(masaStackConfig.GetValue(MasaStackConfigConst.MASA_UI)) ?? new();
    }

    public static bool HasAlert(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("alert");
    }

    public static bool HasTsc(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("tsc");
    }

    public static bool HasScheduler(this IMasaStackConfig masaStackConfig)
    {
        return GetAllServer(masaStackConfig).ContainsKey("scheduler");
    }

    public static string GetConnectionString(this IMasaStackConfig masaStackConfig, string datebaseName)
    {
        var connStr = masaStackConfig.GetValue(MasaStackConfigConst.CONNECTIONSTRING);
        var dbModel = JsonSerializer.Deserialize<DbModel>(connStr);

        return dbModel?.ToString(datebaseName) ?? "";
    }

    public static string GetServerDomain(this IMasaStackConfig masaStackConfig, string protocol, string project, string service)
    {
        var domain = "";
        GetAllServer(masaStackConfig).TryGetValue(project, out JsonObject? jsonObject);
        if (jsonObject != null)
        {
            var secondaryDomain = jsonObject[service]?.ToString();
            if (secondaryDomain != null)
            {
                domain = $"{protocol}://{secondaryDomain}.{masaStackConfig.Namespace}";
            }
        }
        return domain;
    }

    public static string GetUIDomain(this IMasaStackConfig masaStackConfig, string protocol, string project, string service)
    {
        var domain = "";
        GetAllUI(masaStackConfig).TryGetValue(project, out JsonObject? jsonObject);
        if (jsonObject != null)
        {
            var secondaryDomain = jsonObject[service]?.ToString();
            if (secondaryDomain != null)
            {
                domain = $"{protocol}://{secondaryDomain}.{masaStackConfig.DomainName.TrimStart('.')}";
            }
        }
        return domain;
    }

    public static string GetAuthServiceDomain(this IMasaStackConfig masaStackConfig)
    {

        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "auth", "server");
    }

    public static string GetPmServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "pm", "server");
    }

    public static string GetDccServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "dcc", "server");
    }

    public static string GetTscServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "tsc", "server");
    }

    public static string GetAlertServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "alert", "server");
    }

    public static string GetMcServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "mc", "server");
    }

    public static string GetSchedulerServiceDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "scheduler", "server");
    }

    public static string GetSchedulerWorkerDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetServerDomain(masaStackConfig, HttpProtocol.HTTP, "scheduler", "worker");
    }

    public static string GetSsoDomain(this IMasaStackConfig masaStackConfig)
    {
        return GetUIDomain(masaStackConfig, HttpProtocol.HTTPS, "auth", "sso");
    }

    public static IEnumerable<KeyValuePair<string, List<string>>> GetAllUINames(this IMasaStackConfig masaStackConfig)
    {
        foreach (var server in GetAllServer(masaStackConfig))
        {
            var uiName = server.Value["ui"]?.ToString();
            if (string.IsNullOrEmpty(uiName))
            {
                continue;
            }
            yield return new KeyValuePair<string, List<string>>(uiName, new List<string> {
                GetUIDomain(masaStackConfig,HttpProtocol.HTTP,server.Key,"ui"),
                GetUIDomain(masaStackConfig,HttpProtocol.HTTPS,server.Key,"ui")
            });
        }
    }

    public static string GetServiceId(this IMasaStackConfig masaStackConfig, string project, string service)
    {
        return masaStackConfig.GetAllServer()[project][service]?.ToString() ?? throw new KeyNotFoundException();
    }

    public static T GetDccMiniOptions<T>(this IMasaStackConfig masaStackConfig)
    {
        var dccServerAddress = GetDccServiceDomain(masaStackConfig);
        var redis = masaStackConfig.RedisModel ?? throw new Exception("redis options can not null");

        var stringBuilder = new System.Text.StringBuilder(@"{""ManageServiceAddress"":");
        stringBuilder.Append($"\"{dccServerAddress}\",");
        stringBuilder.Append(@"""RedisOptions"": {""Servers"": [{""Host"": ");
        stringBuilder.Append($"\"{redis.RedisHost}\",");
        stringBuilder.Append(@$"""Port"":{redis.RedisPort}");
        stringBuilder.Append("}],");
        stringBuilder.Append(@$"""DefaultDatabase"":{redis.RedisDb},");
        stringBuilder.Append(@$"""Password"": ""{redis.RedisPassword}""");
        stringBuilder.Append(@"}}");

        return JsonSerializer.Deserialize<T>(stringBuilder.ToString()) ?? throw new JsonException();
    }
}
