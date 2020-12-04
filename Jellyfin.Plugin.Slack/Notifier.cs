using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Slack.Configuration;
using Jellyfin.Data.Entities;
using MediaBrowser.Common.Json;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Notifications;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Slack
{
    public class Notifier : INotificationService
    {
        private readonly ILogger<Notifier> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public Notifier(ILogger<Notifier> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _jsonSerializerOptions = JsonDefaults.GetOptions();
        }

        public bool IsEnabledForUser(User user)
        {
            var options = GetOptions(user);
            return options != null && IsValid(options) && options.IsEnabled;
        }

        private SlackConfiguration GetOptions(User user)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.JellyfinUserId, user.Id.ToString("N"), StringComparison.OrdinalIgnoreCase));
        }

        public string Name => Plugin.Instance.Name;

        public async Task SendNotification(UserNotification request, CancellationToken cancellationToken)
        {
            var options = GetOptions(request.User);

            var parameters = new Dictionary<string, string>
                {
                    {"text", $"{request.Name} \n {request.Description}"},
                };
            if (!string.IsNullOrEmpty(options.Username))
            {
                parameters.Add("username", options.Username);
            }
            if (!string.IsNullOrEmpty(options.IconUrl))
            {
                parameters.Add("icon_url", options.IconUrl);
            }

            _logger.LogDebug("Notification to Slack : {0} - {1}", options.WebHookUrl, request.Description);

            using var response = await _httpClientFactory.CreateClient(NamedClient.Default)
                .PostAsJsonAsync(options.WebHookUrl, parameters, _jsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);
        }

        private bool IsValid(SlackConfiguration options)
        {
            return !string.IsNullOrEmpty(options.WebHookUrl);
        }
    }
}
