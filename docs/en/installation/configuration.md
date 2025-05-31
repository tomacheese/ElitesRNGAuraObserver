# Application Settings

This page explains the configurable items in Elite's RNG Aura Observer and what you can check on the settings screen.

![Settings screen of Elite's RNG Aura Observer](/docs/assets/installation/settings-ui.png)

To open the settings screen, double-click the application icon in the taskbar or right-click and select `Settings`.

## Monitored Log File Path

In `Monitoring Information` > `Monitoring Log File`, you can check the path of the log file used to detect whether an Aura has been obtained.

## Discord Webhook URL

By entering the Discord Webhook URL in `Discord Settings` > `Discord Webhook URL`, you can set the notification destination.

For how to obtain the Discord Webhook URL, please see the following page:

- [How to get Discord Webhook URL](get-discord-webhook-url.md)

## Enable Windows Toast Notification

By checking the `Enable Toast notification` checkbox in `Application Settings`, you will receive a Windows toast notification when an Aura is obtained.

The Windows toast notification will be displayed as shown below:

![Example of Windows toast notification for Aura acquisition](/docs/assets/installation/unlocked-new-aura-toast.png)

## Start on Windows Startup

By checking the `Start when Windows starts` checkbox in `Application Settings`, Elite's RNG Aura Observer will start automatically when Windows starts.

## Change Config File Save Location

By entering a folder path in `Config File Directory` in `Application Settings`, you can change where the config file is saved. If nothing is entered, it will be saved in `%USERPROFILE%\AppData\Local\tomacheese\ElitesRNGAuraObserver\`.

The config file name is `config.json` and cannot be changed in the settings.

## Other Information

In `About`, you can check the following information:

- `Application version`: Version of Elite's RNG Aura Observer
- `Aura data version`: Version of Aura information for Elite's RNG Land
