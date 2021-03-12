<h1 align="center">Jellyfin Slack Plugin</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org">Jellyfin Project</a></h3>

<p align="center">
<img alt="Logo Banner" src="https://raw.githubusercontent.com/jellyfin/jellyfin-ux/master/branding/SVG/banner-logo-solid.svg?sanitize=true"/>
<br/>
<br/>
<a href="https://github.com/jellyfin/jellyfin-plugin-slack/actions?query=workflow%3A%22Test+Build+Plugin%22">
<img alt="GitHub Workflow Status" src="https://img.shields.io/github/workflow/status/jellyfin/jellyfin-plugin-slack/Test%20Build%20Plugin.svg">
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-slack">
<img alt="MIT License" src="https://img.shields.io/github/license/jellyfin/jellyfin-plugin-slack.svg"/>
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-slack/releases">
<img alt="Current Release" src="https://img.shields.io/github/release/jellyfin/jellyfin-plugin-slack.svg"/>
</a>
</p>

## About

This plugin can send notifications to a range of personal devices via [Slack](https://slack.com/) when certain events happen on your server.

## Build & Installation Process

1. Clone this repository

2. Ensure you have .NET Core SDK set up and installed

3. Build the plugin with your favorite IDE or the `dotnet` command.

```sh
dotnet publish --configuration Release --output bin
```

4. Place the resulting `Jellyfin.Plugin.Slack.dll` file in a folder called `plugins/` inside your Jellyfin data directory.
