namespace Jellyfin.Plugin.Slack.Configuration
{
    public class SlackConfiguration
    {
        public string WebHookUrl { get; set; }
        public bool IsEnabled { get; set; }
        public string JellyfinUserId { get; set; }
    }
}
