using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.Slack.Api
{
    [ApiController]
    [Route("Notification/Slack")]
    [Produces(MediaTypeNames.Application.Json)]
    public class NotificationsController : ControllerBase
    {
        private readonly IHttpClient _httpClient;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IJsonSerializer _serializer;
        
        public NotificationsController(ILogger<NotificationsController> logger, IHttpClient httpClient, IJsonSerializer serializer)
        {
              _logger = logger;
              _httpClient = httpClient;
              _serializer = serializer;
        }

        /// <summary>
        /// Sends a slack notification to test the configuration.
        /// </summary>
        /// <param name="userId">The user id of the jellyfin user.</param>
        /// <response code="204">Notification sent successfully.</response>
        /// <returns>A <see cref="NoContentResult"/> indicating success.</returns>
        [HttpPost("Test/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> SendTestNotification([FromRoute] string userId)
        {
            var options = Plugin.Instance.Configuration.Options
                .FirstOrDefault(i => string.Equals(i.JellyfinUserId, userId, StringComparison.OrdinalIgnoreCase));

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

            _logger.LogInformation("Slack test notification sent.");

            return NoContent();
        }
    }
}
