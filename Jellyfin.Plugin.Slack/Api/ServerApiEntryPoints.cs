using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Plugin.Slack.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Jellyfin.Plugin.Slack.Api
{
    [Route("/Notification/Slack/Test/{UserID}", "POST", Summary = "Tests Slack")]
    public class TestNotification : IReturnVoid
    {
        [ApiMember(Name = "UserID", Description = "User Id", IsRequired = true, DataType = "string", ParameterType = "path", Verb = "GET")]
        public string UserID { get; set; }
    }

    public class ServerApiEndpoints : IService
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger<ServerApiEndpoints> _logger;
        private readonly IJsonSerializer _serializer;
        
        public ServerApiEndpoints(ILogger<ServerApiEndpoints> logger, IHttpClient httpClient, IJsonSerializer serializer)
        {
              _logger = logger;
              _httpClient = httpClient;
              _serializer = serializer;
        }

        private SlackConfiguration GetOptions(String userID)
        {
            return Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.JellyfinUserId, userID, StringComparison.OrdinalIgnoreCase));
        }

        public async Task Post(TestNotification request)
        {
            var options = GetOptions(request.UserID);

            var parameters = new Dictionary<string, string>
            {
                {"text", "This is a test notification from Jellyfin"}
            };
            if (!string.IsNullOrEmpty(options.Username))
            {
                parameters.Add("username", options.Username);
            }
            if (!string.IsNullOrEmpty(options.IconUrl))
            {
                parameters.Add("icon_url", options.IconUrl);
            }

            var httpRequest = new HttpRequestOptions
            {
                Url = options.WebHookUrl,
                RequestContent = _serializer.SerializeToString(parameters),
                RequestContentType = "application/json",
            };

            await _httpClient.Post(httpRequest).ConfigureAwait(false);
        }
    }
}
