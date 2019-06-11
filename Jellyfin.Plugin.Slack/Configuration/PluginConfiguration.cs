using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Slack.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public SlackConfiguration[] Options { get; set; }

        public PluginConfiguration()
        {
            Options = new SlackConfiguration[] { };
        }

    }
}
