Battlefield Procon Plugin:
# VIP Slot Manager 1.0.0.5  [BF3,  BF4,  BFH,  BC2]
This FREE Plugin gives you full control over reserved VIP Slots, with many customizations and features. It includes a time management control for each VIP player. This means you can add VIP players for a custom time period, whether it be 30 days, or longer. Expired VIP Slots will be disabled automatically.

The Plugin supports a web-based interface to manage a single Gameserver or many Gameservers with different VIP players. This means you can add, edit and remove VIP Slots via the admin website. It is highly recommended to use a website for administrative purposes! 

In addition, you can customize any in-game message based on player events. For example, a welcome message for valid VIPs only, such as '%player% your VIP Slot is still valid for: %time%'.

NEW: The Aggressive Join detection keeps you informed if a NON-VIP player got kicked to make room for a VIP on full server. If the kicked player rejoins, the Plugin sends him a customized message.

# Installation
**IMPORTANT**: This Plugin requires a MySQL database with INNODB support.

**1.** Upload the **VipSlotManager.cs** file to your Procon Layer Server into the folder procon/Plugins/BF4 (procon/Plugins/BFHL OR procon/Plugins/BF3). Restart your Procon Layer.

**2.** Start your Procon PC Tool. Open the **VIP Slot Manager** Plugin settings. In the settings, you will find the section **'1. MySQL Details'**. There simply enter your MySQL details (host IP, port, database, username, password).

**3.** In the section **'2. Main Settings'**, you can choose your **'Gameserver Type'**.

**4.** The **'Server Group'** is an important setting, for when you have more than one Gameserver. If two or more Gameservers use the same MySQL database, then the VIP players are valid for all these Gameservers with the same **'Server Group'** ID. You can change the ID in order to manage the VIPs for each Gameserver separately.

**5.** Enable the Plugin.

**6.** Install the website (optional): In the downloaded ZIP file you find a free website template for this job. Before you upload the website replace your SQL details (SQL Server IP, dbName, dbUser, dbPW) in the 'config.php' file. The default login (user, pw) after the installation: admin , admin

After the first start the Plugin will connect to the MySQL database to automatically create the tables for the Plugin. After the table is created, it will sync all VIP players from the Gameserver to the MySQL database. All the imported VIP players will get a valid VIP Slot for 30 days by the default settings **'Import NEW VIPS from Gameserver to SQL' = yes (30 days first Plugin installation only)**. This means that all your VIPs will stay within the SQL database and on your Gameserver! This setting will be changed after the first Sync/Import is completed successfully.


# Website (highly recommended)

The easiest way to manage reserved VIP Slots is a website with access to the MySQL database. In this way you can manage a single Gameserver or many Gameservers with different VIP players. It gives you full control. You can add, edit and remove VIP players via the website. After a few minutes, the Plugin on each Gameserver receives the updated information automatically and will do the rest.

It is highly recommended to use a website for administrative purposes! You can find a free website template for this purpose within the downloaded ZIP file from this Plugin. It requires a webspace with PHP support and access to the SQL database.


# Sync Settings

All VIP informations are stored within the SQL database, in addition to the VIP Slot remaining time for each VIP player. The Plugin updates the Gameserver with the valid VIP Slots. Expired VIP Slots will be removed automatically.

**Server Groups**
Based on the Plugin settings, **'Gameserver Type'** and **'Server Group'**, the VIPs are valid for one or more Gameserver. If two Gameservers use the same **'Server Group'** ID, then the VIP players are valid for both Gameservers. You can change the **'Server Group'** ID in order to manage the VIPs for each Gameserver separately.

**IMPORTANT**: If two or more Gameservers use the same **'Server Group'**, then the Plugin setting **'Import NEW VIPs from Gameserver to SQL'** must be set to **'no (remove)'**.

**Sync Update Interval**
The Sync between the MySQL database and the Gameserver starts automatically every few minutes. You can change the Sync interval in the Plugin setting **'Sync Interval between SQL and Gameserver'**.

**Advanced Import**
This feature is important for the first Plugin start and the first Sync to the SQL database. If the Plugin is found on the reserved slot list on the Gameserver a NEW VIP without an entry into the SQL database (or with the VIP status 'inactive / expired'), then you can configurate what the Plugin have to do with this NEW VIP. Based on the Plugin settings **'Import NEW VIPs from Gameserver to SQL'** you can select the following options:

- **yes (first Plugin installation)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days. This default setting will be changed to 'no (remove)' after the first Sync/Import is completed successfully. This setting is **recommended** for the first Plugin start and the first Sync to the SQL database.

- **no (ignore)** - The new VIP player will stay on the Gameserver without an entry in the SQL. It is not a valid VIP for the Plugin. The player will stay in the reserved slot list on the Gameserver. The player can not use VIP Commands.

- **no (remove)** - The new VIP player will be removed from the Gameserver reserved slot list. This default setting is **recommended** after the Plugin configuration and the first Sync to SQL is completed successfully.

- **yes (as inactive)** - The new VIP player will be added to the SQL database with the status 'inactive'. The player will be removed from the Gameserver. On the website with access to the SQL database, you can edit the VIP status to activate them.

- **yes (for 7/30/90/365 days)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days.

- **yes (permanent)** - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 7 years (permanent).

**Manual Force Sync**
For a quick one time Sync you can use the **'Force Sync SQL and Gameserver NOW'** function in the settings. The proconrulz.ini file will also be updated (if this feature is enabled).


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

######Sample Message:######
!VIP %player% valid for: %time%
!VIPs online: %online%/%total%
