const SlackConfigurationPageVar = {
    pluginId: '94FB77C3-55AD-4C50-BF4E-4E5497467B79'
};

function loadUserConfig(page, userId) {
    Dashboard.showLoadingMsg();
    ApiClient.getPluginConfiguration(SlackConfigurationPageVar.pluginId).then(function (config) {
        const slackConfig = config.Options.filter(function (c) {
            return userId === c.JellyfinUserId;
        })[0] || {};

        page.querySelector('#chkEnableSlack').checked = slackConfig.IsEnabled || false;
        page.querySelector('#txtSlackWebhookUrl').value = slackConfig.WebHookUrl || '';
        page.querySelector('#txtSlackWebhookUsername').value = slackConfig.Username || '';
        page.querySelector('#txtSlackWebhookIconUrl').value = slackConfig.IconUrl || '';
    });

    Dashboard.hideLoadingMsg();
}

export default function (view) {

    view.querySelector('#selectUser').addEventListener('change', function () {
        loadUserConfig(view, this.value);
    });

    view.querySelector('#testNotification').addEventListener('click', function () {
        Dashboard.showLoadingMsg();
        const onError = function () {
            Dashboard.alert('There was an error sending the test notification. Please check your notification settings and try again.');
            Dashboard.hideLoadingMsg();
        };

        ApiClient.getPluginConfiguration(SlackConfigurationPageVar.pluginId).then(function (config) {
            config.Options.map(function (c) {
                if (c.WebHookUrl === '') {
                    Dashboard.hideLoadingMsg();
                    Dashboard.alert('Please configure and save at least one notification account.');
                }

                ApiClient.ajax({
                    type: 'POST',
                    url: ApiClient.getUrl('Notification/Slack/Test/' + c.JellyfinUserId)
                }).then(function () {
                    Dashboard.hideLoadingMsg();
                }, onError);
            });
        });
    });

    view.querySelector('.SlackConfigurationForm').addEventListener('submit', function (e) {
        Dashboard.showLoadingMsg();
        const form = this;

        ApiClient.getPluginConfiguration(SlackConfigurationPageVar.pluginId).then(function (config) {
            const userId = form.querySelector('#selectUser').value;
            let slackConfig = config.Options.filter(function (c) {
                return userId === c.JellyfinUserId;
            })[0];

            if (!slackConfig) {
                slackConfig = {};
                config.Options.push(slackConfig);
            }

            slackConfig.JellyfinUserId = userId;
            slackConfig.IsEnabled = form.querySelector('#chkEnableSlack').checked;
            slackConfig.WebHookUrl = form.querySelector('#txtSlackWebhookUrl').value;
            slackConfig.Username = form.querySelector('#txtSlackWebhookUsername').value;
            slackConfig.IconUrl = form.querySelector('#txtSlackWebhookIconUrl').value;

            ApiClient.updatePluginConfiguration(SlackConfigurationPageVar.pluginId, config).then(function (result) {
                Dashboard.processPluginConfigurationUpdateResult(result);
            });
        });
        e.preventDefault();
        return false;
    });

    view.addEventListener('viewshow', function () {
        Dashboard.showLoadingMsg();
        const page = this;

        ApiClient.getUsers().then(function (users) {
            page.querySelector('#selectUser').innerHTML = users.map(function (user) {
                return '<option value="' + user.Id + '">' + user.Name + '</option>';
            });
        });

        Dashboard.hideLoadingMsg();
    });
}
