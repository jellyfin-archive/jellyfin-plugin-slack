using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.Slack.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Notifications;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Jellyfin.Plugin.Slack
{
    public class Notifier : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _serializer;

        public Notifier(ILogger logger, IHttpClient httpClient, IJsonSerializer serializer)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serializer = serializer;
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

            _logger.LogDebug("Notification to Slack : {0} - {1}", options.WebHookUrl, request.Description);
            var httpRequest = new HttpRequestOptions
            {
                Url = options.WebHookUrl,
                RequestContent = _serializer.SerializeToString(parameters),
            };

            await _httpClient.Post(httpRequest).ConfigureAwait(false);
        }

        private bool IsValid(SlackConfiguration options)
        {
            return !string.IsNullOrEmpty(options.WebHookUrl);
        }
    }
}
