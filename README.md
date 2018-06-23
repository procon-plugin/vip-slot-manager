Battlefield Procon Plugin:
# VIP Slot Manager 1.0.0.5  [BF3,  BF4,  BFH,  BC2]
This FREE Plugin gives you full control over reserved VIP Slots, with many customizations and features. It includes a time management control for each VIP player. This means you can add VIP players for a custom time period, whether it be 30 days, or longer. Expired VIP Slots will be disabled automatically.

The Plugin supports a web-based interface to manage a single Gameserver or many Gameservers with different VIP players. This means you can add, edit and remove VIP Slots via the admin website. It is highly recommended to use a website for administrative purposes! 

In addition, you can customize any in-game message based on player events. For example, a welcome message for valid VIPs only, such as '%player% your VIP Slot is still valid for: %time%'.

NEW: The Aggressive Join detection keeps you informed if a NON-VIP player got kicked to make room for a VIP on full server. If the kicked player rejoins, the Plugin sends him a customized message.



![img](img/empty.png)

# Installation
**IMPORTANT**: This Plugin requires a MySQL database with INNODB support.

**1.** Upload the **VipSlotManager.cs** file to your Procon Layer Server into the folder procon/Plugins/BF4 (procon/Plugins/BFHL OR procon/Plugins/BF3). Restart your Procon Layer.

**2.** Start your Procon PC Tool. Open the **VIP Slot Manager** Plugin settings. In the settings, you will find the section **'1. MySQL Details'**. There simply enter your MySQL details (host IP, port, database, username, password).

**3.** In the section **'2. Main Settings'**, you can choose your **'Gameserver Type'**.

**4.** The **'Server Group'** is an important setting, for when you have more than one Gameserver. If two or more Gameservers use the same MySQL database, then the VIP players are valid for all these Gameservers with the same **'Server Group'** ID. You can change the ID in order to manage the VIPs for each Gameserver separately.

**5.** Enable the Plugin.

**6.** Install the website (optional): In the downloaded ZIP file you find a free website template for this job. Before you upload the website replace your SQL details (SQL Server IP, dbName, dbUser, dbPW) in the 'config.php' file. The default login (user, pw) after the installation: admin , admin

After the first start the Plugin will connect to the MySQL database to automatically create the tables for the Plugin. After the table is created, it will sync all VIP players from the Gameserver to the MySQL database. All the imported VIP players will get a valid VIP Slot for 30 days by the default settings **'Import NEW VIPS from Gameserver to SQL' = yes (30 days first Plugin installation only)**. This means that all your VIPs will stay within the SQL database and on your Gameserver! This setting will be changed after the first Sync/Import is completed successfully.

![img](img/empty.png)

# Website (highly recommended)

The easiest way to manage reserved VIP Slots is a website with access to the MySQL database. In this way you can manage a single Gameserver or many Gameservers with different VIP players. It gives you full control. You can add, edit and remove VIP players via the website. After a few minutes, the Plugin on each Gameserver receives the updated information automatically and will do the rest.

It is highly recommended to use a website for administrative purposes! You can find a free website template for this purpose within the downloaded ZIP file from this Plugin. It requires a webspace with PHP support and access to the SQL database.



![img](img/empty.png)

# Sync Settings

All VIP informations are stored within the SQL database, in addition to the VIP Slot remaining time for each VIP player. The Plugin updates the Gameserver with the valid VIP Slots. Expired VIP Slots will be removed automatically.

### Server Groups
Based on the Plugin settings, **'Gameserver Type'** and **'Server Group'**, the VIPs are valid for one or more Gameserver. If two Gameservers use the same **'Server Group'** ID, then the VIP players are valid for both Gameservers. You can change the **'Server Group'** ID in order to manage the VIPs for each Gameserver separately.

**IMPORTANT**: If two or more Gameservers use the same **'Server Group'**, then the Plugin setting **'Import NEW VIPs from Gameserver to SQL'** must be set to **'no (remove)'**.

### Sync Update Interval
The Sync between the MySQL database and the Gameserver starts automatically every few minutes. You can change the Sync interval in the Plugin setting **'Sync Interval between SQL and Gameserver'**.

### Advanced Import
This feature is important for the first Plugin start and the first Sync to the SQL database. If the Plugin is found on the reserved slot list on the Gameserver a NEW VIP without an entry into the SQL database (or with the VIP status 'inactive / expired'), then you can configurate what the Plugin have to do with this NEW VIP. Based on the Plugin settings **'Import NEW VIPs from Gameserver to SQL'** you can select the following options:

- **yes (first Plugin installation)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days. This default setting will be changed to 'no (remove)' after the first Sync/Import is completed successfully. This setting is **recommended** for the first Plugin start and the first Sync to the SQL database.

- **no (ignore)** - The new VIP player will stay on the Gameserver without an entry in the SQL. It is not a valid VIP for the Plugin. The player will stay in the reserved slot list on the Gameserver. The player can not use VIP Commands.

- **no (remove)** - The new VIP player will be removed from the Gameserver reserved slot list. This default setting is **recommended** after the Plugin configuration and the first Sync to SQL is completed successfully.

- **yes (as inactive)** - The new VIP player will be added to the SQL database with the status 'inactive'. The player will be removed from the Gameserver. On the website with access to the SQL database, you can edit the VIP status to activate them.

- **yes (for 7/30/90/365 days)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days.

- **yes (permanent)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 7 years (permanent).

### Manual Force Sync
For a quick one time Sync you can use the **'Force Sync SQL and Gameserver NOW'** function in the settings. The proconrulz.ini file will also be updated (if this feature is enabled).



![img](img/empty.png)

# Notify & In-Game Messages
You can enable, disable and customize every single in-game message based on chat and player events. OnJoin, OnSpawn and OnChat are trigger events.

You do not have to use all the available textboxes for messages, leave it blank if you do not need it.

The 'Replacement Strings' below are available for any message:

REPLACEMENT STRING | EFFEKT
--- | ---
**%player%** | Will be replaced by the playername
**%time%** | Will be replaced by the VIP Slot remaining time (for valid VIPs only)
**%total%** | Will be replaced by the total number of all valid VIPs on this server
**%online%** | Will be replaced by the number of online VIPs

### Sample Message:
!VIP %player% valid for: %time%
!VIPs online: %online%/%total%


![img](img/empty.png)

# In-Game VIP Commands

These commands are for valid VIP Slot players only. Each command can be enabled or disabled separately.

IN-GAME VIP CMD | EFFEKT
--- | ---
**!lead** | take squad leader position
**!killme** | admin kill without death in scoreboard
**!switchme** | switch between teams



![img](img/empty.png)

# In-Game Admin Commands

These commands are for in-game admins only. Admins need the privilege **'Can Edit Reserved Slots List'**. You can enable or disable this function in the setting **'Enable In-Game Admin Commands'**.

**IMPORTANT**: Requires the <full playername> - this is case sensitive!

IN-GAME ADMIN CMD | SAMPLE | EFFECT
--- | --- | ---
**!addvip [full playername] [days]** | !addvip SniperBen 30 | This cmd adds and activates a player’s VIP status for the next 30 days. The time period of 30 days is fixed. When you enter this command more than once it has no effect on the time period.
**!addvip [full playername] +[days]** | !addvip SniperBen +7 | This cmd checks the VIP player’s remaining time (e.g. the VIP Slot is still valid for 5 days). Then the Plugin ADDS 7 days to the 'old' time period. For example: old time period (5 days) + new time period (7 days) = total time period (12 days). Now the VIP Slot is valid for 12 days.
**!removevip [full playername]** | !removevip SniperBen | This cmd will remove the VIP from the Gameserver. The player will stay in the SQL database and be marked as 'status inactive'.
**!checkvip   [full playername]** | !checkvip SniperBen | This cmd will display the remaining time
**!changevip [old playername] [new playername]** | !changevip SniperBen SniperBenni | This cmd will change the VIP Slot playername 



![img](img/empty.png)

# Other Plugin Support

Other Plugins such as ProconRulz/InsaneLimits can use special commands to remove or add a VIP Slot for a custom time period. Other Plugins can send commands as a 'hidden say' within the in-game chat. Other players will not see this in-game message, but the Plugin receives this information. In the Procon PC Tool you can enter the commands to (say, all players). You can enable or disable this function in the setting 'Enable Commands for other Plugins'.

CMD FOR OTHER PLUGINS | SAMPLE | EFFECT
--- | --- | ---
**/vsm-addvip [full playername] [days]** | /vsm-addvip SniperBen 30 | This cmd adds and activates a player’s VIP status for the next 30 days. The time period of 30 days is fixed. When you enter this command more than once it has no effect on the time period.
**/vsm-addvip [full playername] +[days]** | /vsm-addvip SniperBen +7 | This cmd checks the VIP player’s remaining time (e.g. the VIP Slot is still valid for 5 days). Then the Plugin ADDS 7 days to the 'old' time period. For example: old time period (5 days) + new time period (7 days) = total time period (12 days). Now the VIP Slot is valid for 12 days.
**/vsm-removevip [full playername]** | /vsm-removevip SniperBen | This cmd will remove the VIP from the Gameserver. The player will stay in the SQL database and be marked as 'status inactive'.
**vsm-changevip [old playername] [new playername]** | /vsm-changevip SniperBen SniperBenni | This cmd will change the VIP Slot playername 

### Sample Code for ProconRulz (perform 5 knife kills = VIP Slot for 7 days):
```
On Kill; Damage Melee; if %c% == 5; Say /vsm-addvip %p% +7
```



![img](img/empty.png)

# Advanced ProconRulz Support
The VIP Slot Manager Plugin can store a list of valid VIPs in the proconrulz.ini file. This file stays on your Procon Layer Server (path: CONFIGS/proconrulz_<ip>_<port>.ini). When you read this file within the Plugin ProconRulz (%ini_vipslotmanager_<playername>%) you will get the VIP timestamp in seconds. This means you can check the VIP status without any player protection for weapon rule punishment (kill, kick, ban). You can enable or disable this function within the setting **'On Round End write VIPs in proconrulz.ini file'**.

For a quick one time update to the proconrulz.ini file you can use the **'Force Sync SQL and Gameserver NOW'** function in the settings.

**IMPORTANT**: Requires Read+Write file permission in the directory /configs/ on your Procon Layer.

### Sample Code for ProconRulz (in-game command '!check' returns the VIP player status):
```
On Say; Text !check; if %ini_vipslotmanager_%p%% != 0; Say Yes, you are a VIP
On Say; Text !check; if %ini_vipslotmanager_%p%% == 0; Say No, you are NOT a VIP
```


![img](img/empty.png)

# Aggressive Join for VIPs
The 'Aggressive Join' is a server setting that allows VIPs to join a full server without waiting. A random NON-VIP player will be kicked to make room for a VIP.

The Plugin can detect this kind of kick and keeps you informed if a NON-VIP player got kicked to make room for a VIP on full server. If the kicked player rejoins, the Plugin sends him a customized message. You can enable, disable and customize this feature in the setting **'Private Message after NON-VIP got kicked and rejoins'**.

In addition, the Plugin can disable the 'Aggressive Join' close on round end to keep as many players as possible on the server. On the next round it will be enabled automatically. This feature works for the following game modes: ConquestLarge, ConquestSmall, TDM and Chainlink. You can enable or disable this function in the setting **'Temporary disable the Aggressive Join close on round end'**.



![img](img/empty.png)

# Advanced Settings

### Debug Level
1 - Errors will be displayed.
2 - will also show log entries for added and removed VIP players.
3 - will also show log entries when a VIP player joins the server.
4 - will also show log entries when a player uses the in-game commands (e.g. !lead, !killme).
5 - just for development and testing.

### Manual Force Sync
For a quick one time Sync you can use the **'Force Sync SQL and Gameserver NOW'** function in the settings. The proconrulz.ini file will also be updated (if this feature is enabled).

### Auto Database Cleaner
This feature reduces the Sync traffic between SQL and Gameserver. It is necessary because the Sync is limited (max. 800 active/expired VIPs for each Server Group). Each Sync includes a list of valid VIPs and expired VIPs. Expired VIPs will get a notify message on the next spawn event. But if the player does not join the server for long time period (60 days by default setting), then this feature will remove him from the Sync in order to reduce the traffic. It changes the player status from 'expired' to 'inactive' and the player will not recives the expired VIP Slot message.

### VIP EA GUID Tracking (optional)
If a VIP changes his playername then his VIP Slot will be updated to the new playername automatically. After a VIP joins the server, the Plugin links his playername to his EA GUID. If he joins again with a new/changed playername then his VIP Slot will be updated to the new playername for all **Server Groups** on current Gameserver Type in SQL database (e.g. for all BF4 Groups 1-99). After the VIP Slot has expired the EA GUID will be unlinked. You can enable or disable the tracking function in the setting **'EA GUID Tracking'**.



![img](img/empty.png)

# How to add, edit and remove VIPs

### Website (highly recommended)
The easiest way to manage reserved VIP Slots is a website with access to the MySQL database. You can find a free website template in the downloaded ZIP file from this Plugin. It is highly recommended that you use it!

### In-Game Admin Commands
As an in-game admin you can use the commands: !addvip, !checkvip, and !removevip for the current Gameserver (Server Group).

### Procon PC Tool
In the Plugin settings you can use the **'Mini Manager - Print VIP list'** to display the current VIP list with players remaining time on your Procon PC Tool Chat tab.

You can also use the commands from the 'Other Plugin Support' function to add and remove VIP Slots. You can enter the commands in the Procon PC Tool chat as a hidden admin say (e.g. /vsm-addvip SniperBen +7). Nobody will see the commands within the in-game chat.



![img](img/empty.png)

# FAQ

### Do I need a new MySQL database?
No. For this Plugin, it is NOT necessary to create a new MySQL database. You can use the same MySQL database as the Statslogger Plugin.

### How to manage two or more Gameservers?
With the web-based interface you can manage a single Gameserver or many Gameservers with different VIP players. If two Gameservers use the same **'Server Group'** ID (Plugin settings), then the VIP players are valid for both Gameservers. You can change the **'Server Group'** ID in order to manage the VIPs for each Gameserver separately. It is recommended that all Gameservers use the same MySQL database. If two or more Gameservers use the same **'Server Group'**, then the Plugin setting **'Import NEW VIPs from Gameserver to SQL'** must be set to **no (remove)**.

### AdKats Plugin
It is possible to use the VIP Slot Manager and the AdKats Plugin. Please make sure that Adkats do not manage the reserved VIP Slots. This is disabled by default. Open the settings from Adkats Plugin, then go to Adkats > A16. Orchestration Settings > Feed Server Reserved Slots > False

### What is the diffenence between 'add 30' and 'add +30'?
The command with + checks the VIP players remaining time (e.g. the VIP Slot is still valid for 5 days), then the Plugin ADDS 30 days to the 'old' time period. For example: old time period (5 days) + new time period (30 days) = total time period (35 days). Now the VIP Slot is valid for 35 days.

### How to clean up the database?
With the website you can clean up the database to remove all old VIPs with the status 'inactive'. Go to the website. Type 'inactive' into the search box. Mark all entires (click on the first VIP and then hold down the SHIFT key on your keyboard and click on the last VIP). Then open the drop down menu and click on 'DELETE' to delete the marked entries.

### Witch Games are supported?
The Plugin works fine for BF3, BF4, BFH and BFBC2. The support for other Games are still not tested.



![img](img/empty.png)

# Changelog
### 1.0.0.5 (26.01.2018)
- Add: VIP EA Guid Tracking to update playername changes automatically (optional)
- Add: Command !changevip to change VIP Slot playername
- Modification: In-Game VIP Command !lead (optional VIP protection)
- Modification: Small code improvements
- Fix: SQL Credentials after server restart
- Fix: BC2 Procon compatibility
- Fix: Website compatibility to php 5.6 / 7.0 / 7.1 / 7.2. New features and filters for better workflow

### 1.0.0.4 (04.10.2017)
- Add: Aggressive Join features

### 1.0.0.3 (12.08.2017)
- Add: Auto Correction for case sensitive difference in playername
- Add: Auto Database Cleaner
- Fix: Website (add days button)

### 1.0.0.2 (09.05.2017)
- Fix: Website blank site

### 1.0.0.1 (02.05.2017)
- Fix: In-Game VIP Commands

