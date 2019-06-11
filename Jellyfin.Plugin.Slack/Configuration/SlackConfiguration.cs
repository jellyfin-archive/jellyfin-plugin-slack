using System;
using System.Collections.Generic;
using System.Text;

namespace Jellyfin.Plugin.Slack.Configuration
{
    public class SlackConfiguration
    {

        public string WebHookUrl { get; set; }
        public bool IsEnabled { get; set; }
        public string MediaBrowserUserId { get; set; }
    }
}
