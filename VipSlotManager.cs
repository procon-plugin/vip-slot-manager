/*	VipSlotManager.cs -  Procon Plugin [BF3, BF4, BFH, BC2]

	Version: 1.0.0.6

	Code Credit:
	PapaCharlie9 (forum.myrcon.com)  -  Basic Plugin Template Part (BasicPlugin.cs)
	[GWC]XpKiller (forum.myrcon.com) -  MySQL Main Functions (CChatGUIDStatsLogger.inc)
	[VdSk]LmaA-aD  -  Sponsoring BF3 & BFH Gameserver for testing Plugin

	Development by maxdralle@gmx.com

	This plugin file is part of PRoCon Frostbite.

	This plugin is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This plugin is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with PRoCon Frostbite.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Web;
using System.Data;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

using PRoCon.Core;
using PRoCon.Core.Plugin;
using PRoCon.Core.Plugin.Commands;
using PRoCon.Core.Players;
using PRoCon.Core.Players.Items;
using PRoCon.Core.Battlemap;
using PRoCon.Core.Maps;

using MySql.Data.MySqlClient;

namespace PRoConEvents {
// Aliases
using EventType = PRoCon.Core.Events.EventType;
using CapturableEvent = PRoCon.Core.Events.CapturableEvents;

public class VipSlotManager : PRoConPluginAPI, IPRoConPluginInterface {
/* Inherited:
	this.PunkbusterPlayerInfoList = new Dictionary<String, CPunkbusterInfo>();
	this.FrostbitePlayerInfoList = new Dictionary<String, CPlayerInfo>();
*/

private bool fIsEnabled;
private bool gotVipsGS;
private bool firstCheck;
private bool AggressiveJoin;
private bool isForceSync;
private bool SqlTableExist;
private bool AdkatsRunning;
private int fDebugLevel;
private int SyncCounter;
private int DBCleanerCounter = (new Random().Next(960, 990));
private int vipsCurrentlyOnline;
private int start_delay = (new Random().Next(40, 100));
private double ticketLoserTeam;
private string CurrentGameMode;
private string ServerIP;
private string ServerPort;
private string NewLiner;
private string GetGameType;
private DateTime LayerStartingTime = DateTime.UtcNow;
private DateTime CurrentRoundEndTime = DateTime.UtcNow;
private DateTime LastSync = DateTime.UtcNow;
private DateTime LastAggressiveJoinKickTime = DateTime.UtcNow.AddMinutes(-5);

private List<string> vipsGS = new List<string>();
private List<string> vipmsg = new List<string>();
private List<string> vipsExpired = new List<string>();
private List<string> SqlVipsActiveNamesOnly = new List<string>();
private List<string> kickedForVip = new List<string>();
private List<string> GetPlayerGuid = new List<string>();
private List<string> RoundTempVips = new List<string>();
private Dictionary<String, int> SqlVipsActive = new Dictionary<string, int>();
private Dictionary<String, int> playerTeamID = new Dictionary<string, int>();
private Dictionary<String, int> playerSquadID = new Dictionary<string, int>();
private Dictionary<String, String> SquadLederList = new Dictionary<string, string>();
private Dictionary<String, String> Guid2Check = new Dictionary<string, string>();
private Dictionary<String, DateTime> onJoinSpammer = new Dictionary<string, DateTime>();
private Dictionary<String, String> NameGuidList = new Dictionary<string, string>();
private Dictionary<String, int> AggressiveVips = new Dictionary<string, int>();

private MySql.Data.MySqlClient.MySqlTransaction MySqlTrans;

private string SettingStrSqlHostname;
private string SettingStrSqlPort;
private string SettingStrSqlDatabase;
private string SettingStrSqlUsername;
private string SettingStrSqlPassword;
private string SettingGameType;
private string SettingStrSqlServerGroup;
private int SettingSyncInterval;
private string SettingSyncGs2Sql;
private enumBoolYesNo SettingProconRulzIni;
private enumBoolYesNo SettingAdminCmd;
private enumBoolYesNo SettingPluginCmd;
private enumBoolYesNo SettingVipCmd;
private enumBoolYesNo SettingLeadCmd;
private enumBoolYesNo SettingIgnoreVipLeader;
private enumBoolYesNo SettingLeadByCRose;
private enumBoolYesNo SettingKillmeCmd;
private enumBoolYesNo SettingSwitchmeCmd;
private enumBoolYesNo SettingChatReq;
private string SettingInfoCommands;
private string SettingInfoCmdRegMatch;
private string SettingInfoVip1;
private string SettingInfoVip2;
private string SettingInfoVip3;
private enumBoolYesNo SettingInfoSay;
private enumBoolYesNo SettingVipJoin;
private string SettingVipJoinMsg;
private enumBoolYesNo SettingNonVipJoin;
private string SettingNonVipJoinMsg;
private enumBoolYesNo SettingVipSpawn;
private string SettingVipSpawnMsg;
private string SettingVipSpawnMsg2;
private string SettingVipSpawnMsg3;
private string SettingVipSpawnYell;
private enumBoolYesNo SettingVipSpawnMsgSay;
private enumBoolYesNo SettingVipSpawnMsg2Say;
private enumBoolYesNo SettingVipSpawnMsg3Say;
private enumBoolYesNo SettingVipSpawnYellAll;
private int SettingVipSpawnMsgDelay;
private enumBoolYesNo SettingVipExp;
private string SettingVipExpMsg;
private string SettingVipExpMsg2;
private string SettingVipExpYell;
private int SettingVipExpDelay;
private enumBoolYesNo SettingForceSync;
private enumBoolYesNo SettingMiniManager;
private int SettingYellDuring;
private int SettingDBCleaner;
private int SettingAggressiveJoinKickAbuseMax;
private enumBoolYesNo EAGuidTracking;
private enumBoolYesNo SettingAggressiveJoin;
private enumBoolYesNo SettingAdkatsLog;
private enumBoolYesNo SettingAdkatsLogNonVipKick;
private enumBoolYesNo SettingAdkatsLogVipChanged;
private enumBoolYesNo SettingAdkatsLogVipAggressiveJoinAbuse;
private enumBoolYesNo SettingAggressiveJoinKickAbuse;

private string SettingAggressiveJoinKick;
private string SettingAggressiveJoinMsg;
private string SettingAggressiveJoinMsgType;
private string SettingAggressiveJoinKickAbuseMsg;
private string SettingAggressiveJoinKickAbuseMsgType;

public VipSlotManager() {
	this.fIsEnabled = false;
	this.gotVipsGS = false;
	this.firstCheck = false;
	this.AggressiveJoin = false;
	this.fDebugLevel = 3;
	this.SyncCounter = 999;
	this.ServerIP = String.Empty;
	this.ServerPort = String.Empty;
	this.isForceSync = false;
	this.SqlTableExist = false;
	this.AdkatsRunning = false;
	this.ticketLoserTeam = 0;
	this.CurrentGameMode = String.Empty;
	this.NewLiner = "";
	this.vipsCurrentlyOnline = 0;

	this.SettingStrSqlHostname = String.Empty;
	this.SettingStrSqlPort = "3306";
	this.SettingStrSqlDatabase = String.Empty;
	this.SettingStrSqlUsername = String.Empty;
	this.SettingStrSqlPassword = String.Empty;
	this.SettingGameType = "AUTOMATIC";
	this.SettingStrSqlServerGroup = "1";
	this.SettingSyncInterval = 5;
	this.SettingSyncGs2Sql = "yes  (30 days first Plugin installation only)";
	this.SettingProconRulzIni = enumBoolYesNo.No;
	this.SettingAdminCmd = enumBoolYesNo.Yes;
	this.SettingPluginCmd = enumBoolYesNo.Yes;
	this.SettingVipCmd = enumBoolYesNo.No;
	this.SettingLeadCmd = enumBoolYesNo.No;
	this.SettingIgnoreVipLeader = enumBoolYesNo.Yes;
	this.SettingLeadByCRose = enumBoolYesNo.No;
	this.SettingKillmeCmd = enumBoolYesNo.No;
	this.SettingSwitchmeCmd = enumBoolYesNo.No;
	this.SettingChatReq = enumBoolYesNo.Yes;
	this.SettingInfoCommands = "!vip,!slot,!reserved,!buy";	
	this.SettingInfoCmdRegMatch = "^!vip|^!slot|^!reserved|^!buy";
	this.SettingInfoVip1 = "Buy a !VIP SLOT for 4 Euro / Month";
	this.SettingInfoVip2 = "!VIP SLOT includes: Reserved Slot, Auto Balancer Whitelist, High Ping Whitelist";
	this.SettingInfoVip3 = "!VIP %player% valid for: %time%";
	this.SettingInfoSay = enumBoolYesNo.Yes;
	this.SettingVipJoin = enumBoolYesNo.No;
	this.SettingVipJoinMsg = "%player% with !VIP SLOT joined the server";
	this.SettingNonVipJoin = enumBoolYesNo.No;
	this.SettingNonVipJoinMsg = "%player% joined the server";
	this.SettingVipSpawn = enumBoolYesNo.Yes;
	this.SettingVipSpawnMsg = "%player% welcome! Enjoy your !VIP SLOT";
	this.SettingVipSpawnMsg2 = "!VIP SLOT valid for: %time%";
	this.SettingVipSpawnMsg3 = String.Empty;
	this.SettingVipSpawnYell = String.Empty;
	this.SettingVipSpawnMsgSay = enumBoolYesNo.Yes;
	this.SettingVipSpawnMsg2Say = enumBoolYesNo.No;
	this.SettingVipSpawnMsg3Say = enumBoolYesNo.No;
	this.SettingVipSpawnYellAll = enumBoolYesNo.No;
	this.SettingVipSpawnMsgDelay = 7;
	this.SettingVipExp = enumBoolYesNo.Yes;
	this.SettingVipExpMsg = "%player% your !VIP SLOT has expired";
	this.SettingVipExpMsg2 = "You can buy a !VIP SLOT on our website";
	this.SettingVipExpYell = "%player% your !VIP SLOT has expired";
	this.SettingVipExpDelay = 7;
	this.SettingForceSync = enumBoolYesNo.No;
	this.SettingMiniManager = enumBoolYesNo.No;
	this.SettingYellDuring = 15;
	this.SettingDBCleaner = 60;
	this.EAGuidTracking = enumBoolYesNo.No;
	this.SettingAggressiveJoin = enumBoolYesNo.No;
	this.SettingAdkatsLog = enumBoolYesNo.No;
	this.SettingAdkatsLogNonVipKick = enumBoolYesNo.No;
	this.SettingAdkatsLogVipChanged = enumBoolYesNo.No;
	this.SettingAdkatsLogVipAggressiveJoinAbuse = enumBoolYesNo.No;
	this.SettingAggressiveJoinKick = "%player% got disconnected to make room for !VIP on full server";
	this.SettingAggressiveJoinMsg = "%player% welcome back! " + System.Environment.NewLine + "You got KICKED randomly because a !VIP joined on full server :/";
	this.SettingAggressiveJoinMsgType = "Private Yell and Private Say";
	this.SettingAggressiveJoinKickAbuse = enumBoolYesNo.Yes;
	this.SettingAggressiveJoinKickAbuseMax = 3;
	this.SettingAggressiveJoinKickAbuseMsg = "%player% you can bypass the server queue on next round (too many rejoins per round).";
	this.SettingAggressiveJoinKickAbuseMsgType = "Private Say";
}

//////////////////////
// BasicPlugin.cs part by PapaCharlie9@gmail.com
//////////////////////

public enum MessageType { Warning, Error, Exception, Normal };

public String FormatMessage(String msg, MessageType type) {
	String prefix = "[^b" + GetPluginName() + "^n] ";

	if (type.Equals(MessageType.Warning))
		prefix += "^1^bWARNING^0^n: ";
	else if (type.Equals(MessageType.Error))
		prefix += "^1^bERROR^n^0: ";
	else if (type.Equals(MessageType.Exception))
		prefix += "^1^bEXCEPTION^0^n: ";

	return prefix + msg;
}

public void LogWrite(String msg) {
	this.ExecuteCommand("procon.protected.pluginconsole.write", msg);
}

public void ConsoleWrite(String msg, MessageType type) {
	LogWrite(FormatMessage(msg, type));
}

public void ConsoleWrite(String msg) {
	ConsoleWrite(msg, MessageType.Normal);
}

public void ConsoleWarn(String msg) {
	ConsoleWrite(msg, MessageType.Warning);
}

public void ConsoleError(String msg) {
	ConsoleWrite("^8" + msg + "^0", MessageType.Error);
}

public void ConsoleException(String msg) {
	ConsoleWrite(msg, MessageType.Exception);
}

public void DebugWrite(String msg, int level) {
	if (this.fDebugLevel >= level) ConsoleWrite(msg, MessageType.Normal);
}

public void OnPluginLoadingEnv(List<string> lstPluginEnv) {
	this.GetGameType = lstPluginEnv[1].ToUpper();
	if (this.SettingGameType == "AUTOMATIC") {
		if (this.GetGameType == "BF3") {this.NewLiner = "\n"; this.SettingGameType = "BF3";}
		if (this.GetGameType == "BF4") {this.NewLiner = "\n"; this.SettingGameType = "BF4";}
		if (this.GetGameType == "BFHL") {this.NewLiner = ""; this.SettingGameType = "BFH";}
		if (this.GetGameType == "BFBC2") {this.NewLiner = ""; this.SettingGameType = "BC2";}
	}
}

public String GetPluginName() {
	return "VIP Slot Manager";
}

public String GetPluginVersion() {
	return "1.0.0.6";
}

public String GetPluginAuthor() {
	return "maxdralle";
}

public String GetPluginWebsite() {
	return "github.com/procon-plugin/vip-slot-manager";
}

public String GetPluginDescription() {
	return @"
Credits:<br>
PapaCharlie9 (forum.myrcon.com) - Basic Plugin Template Part (BasicPlugin.cs)<br>
[GWC]XpKiller (forum.myrcon.com) - MySQL Functions (CChatGUIDStatsLogger.inc)<br>
[VdSk]LmaA-aD - Sponsoring BF3 & BFH Gameserver for testing Plugin
<p></p><p></p>
<h2>VIP Slot Manager [BF3,  BF4,  BFH,  BC2]</h2>
<p>If you find this plugin useful, please feel free to donante.</p>
<p><form action='https://www.paypal.com/cgi-bin/webscr' target='_blank' method='post'>
<input type='hidden' name='cmd' value='_s-xclick'>
<input type='hidden' name='hosted_button_id' value='M7UQ2566MNNE4'>
<input type='image' src='https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif' border='0' name='submit' alt='PayPal - The safer, easier way to pay online!'>
<img alt='' border='0' src='https://www.paypalobjects.com/de_DE/i/scr/pixel.gif' width='1' height='1'>
</form></p>
<h2>Description</h2>
<p>This FREE Plugin gives you full control over reserved VIP Slots, with many customizations and features. It includes a time management control for each VIP player. This means you can add VIP players for a custom time period, whether it be 30 days, or longer. Expired VIP Slots will be disabled automatically. It is also possible to manage multiple Gameservers with one global list of VIPs or alternatively each Gamerserver separately with his own list of VIPs.</p>
<p>The Plugin supports a web-based interface to manage a single Gameserver or many Gameservers with different VIP players. This means you can add, edit and remove VIP Slots via the admin website. It is highly recommended to use a website for administrative purposes! </p>
<p>In addition, you can customize any in-game message based on player events. For example, a welcome message for valid VIPs only, such as '%player% your VIP Slot is still valid for: %time%'.</p>
<p><u>NEW</u>: The Aggressive Join detection keeps you informed if a NON-VIP player got kicked to make room for a VIP on full server. If the kicked player rejoins, the Plugin sends him a customized message.</p>
<p></p>
<h2>Installation</h2>
<p><u>IMPORTANT</u>: This Plugin requires a MySQL database with INNODB support.</p>
<p><b>1.</b> Disable the Plugin VIP Slot Manager and open the Plugin settings.</p>
<p><b>2.</b> In the settings, you will find the section  <b>'1. MySQL Details'</b>. There simply enter your MySQL details (host IP, port, database name, username, password).</p>
<p><b>3.</b> In the section <b>'2. Main Settings'</b> you can choose your <b>'Gameserver Type'</b>.</p>
<p><b>4.</b> The <b>'Server Group'</b> is an important setting, for when you have more than one Gameserver. If two or more Gameservers use the same MySQL database, then the VIP players are valid for all these Gameservers with the same <b>'Gameserver Type'</b> and <b>'Server Group'</b> ID. You can change the ID in order to manage the VIPs for each Gameserver separately.</p>
<p><b>5.</b> Enable the Plugin.</p>
<p><b>6.</b> Install the website (optional): In the downloaded ZIP file you find a free website template for this job. Before you upload the website replace your SQL details (SQL Server IP, dbName, dbUser, dbPW) in the 'config.php' file. The default login (user, pw) after the installation: admin , admin</p>
<p>After the first start the Plugin will connect to the MySQL database to automatically create tables for the Plugin. After the table is created, it will sync all VIP players from the Gameserver to the MySQL database. All the imported VIP players will get a valid VIP Slot for 30 days by the default settings <b>'Import NEW VIPS from Gameserver to SQL' = yes (30 days first Plugin installation only)</b>. This means that all your VIPs will stay within the SQL database and on your Gameserver! This setting will be changed after the first Sync/Import is completed successfully.</p>
<p></p>
<h2>Website (highly recommended)</h2>
<p>The easiest way to manage reserved VIP Slots is a website with access to the MySQL database. In this way you can manage a single Gameserver or many Gameservers with different VIP players. It gives you full control. You can add, edit and remove VIP players via the website. After a few minutes, the Plugin on each Gameserver receives the updated information automatically and will do the rest.</p>
<p>It is highly recommended to use a website for administrative purposes! You can find a free website template for this purpose within the downloaded ZIP file from this Plugin. It requires a webspace with PHP support and access to the SQL database.</p>
<p></p>
<h2>Sync Settings</h2>
<p>All VIP informations are stored within the SQL database, in addition to the VIP Slot remaining time for each VIP player. The Plugin updates the Gameserver with the valid VIP Slots. Expired VIP Slots will be removed automatically.</p>
<p><b>Multiple Gameservers with one global list of VIPs (optional)</b><br>
The following Plugin settings are important to provide multiple Gameservers with one global list of VIPs. The settings <b>'MySQL details'</b>, <b>'Gameserver Type'</b>, <b>'Server Group'</b>, <b>'Import NEW VIPs from Gameserver to SQL'</b>, <b>'Notify Vip Slot Expiered'</b>, <b>'EA GUID Tracking'</b> and <b>'Aggressive Join Abuse Protection'</b> have to be exactly equal on all Gameservers. In addition, the setting <b>'Import NEW VIPs from Gameserver to SQL'</b> must be set to <b>'no (remove)'</b>. In this way the Plugin and the VIP Sync works perfect.</p>
<p><b>Server Groups</b><br>
Based on the Plugin settings <b>'Gameserver Type'</b> and <b>'Server Group'</b> the VIPs are valid for one or more Gameserver. If two Gameservers use the same <b>'Server Group'</b> ID, then the VIP players are valid for both Gameservers. You can change the <b>'Server Group'</b> ID in order to manage the VIPs for each Gameserver separately.</p>
<p><u>IMPORTANT</u>: If two or more Gameservers use the same <b>'Server Group'</b> than the Plugin setting <b>'Import NEW VIPs from Gameserver to SQL'</b> must be set to <b>no (remove)</b>.</p>
<p><b>Sync Update Interval</b><br>
The Sync between the MySQL database and the Gameserver starts automatically every few minutes. You can change the Sync interval in the Plugin setting <b>'Sync Interval between SQL and Gameserver'</b>.</p>
<p><b>Advanced Import</b><br>
This feature is important for the first Plugin start and the first Sync to the SQL database. If the Plugin is found on the reserved slot list on the Gameserver a NEW VIP without an entry into the SQL database (or with the VIP status 'inactive / expired'), then you can configurate what the Plugin have to do with this NEW VIP. Based on the Plugin settings <b>'Import NEW VIPs from Gameserver to SQL'</b> you can select the following options:</p>
<ul><li><b>yes (first Plugin installation)</b> - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days. This default setting will be changed to 'no (remove)' after the first Sync/Import is completed successfully. This setting is <u>recommended</u> for the first Plugin start and the first Sync to the SQL database.</li>
<li><b>no (ignore)</b> - The new VIP player will stay on the Gameserver without an entry in the SQL. It is not a valid VIP for the Plugin. The player will stay in the reserved slot list on the Gameserver. The player can not use VIP Commands.</li>
<li><b>no (remove)</b> - The new VIP player will be removed from the Gameserver reserved slot list. This default setting is <u>recommended</u> after the Plugin configuration and the first Sync to SQL is completed successfully. This default setting is also required to enable the function <b>'EA GUID Tracking'</b>, the function<b>'Aggressive Join Abuse Protection'</b> or if two or more Gameservers use the same <b>'Server Group'</b> ID.</li>
<li><b>yes (as inactive)</b> - The new VIP player will be added to the SQL database with the status 'inactive'. The player will be removed from the Gameserver. On the website with access to the SQL database, you can edit the VIP status to activate them.</li>
<li><b>yes (for 7/30/90/365 days)</b> - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 30 days.</li>
<li><b>yes (permanent)</b> - The new VIP player will be added to the SQL database. The new VIP will be activated and valid for the next 7 years (permanent).</li></ul>
<p><b>Manual Force Sync</b><br>
For a quick one time Sync you can use the <b>'Force Sync SQL and Gameserver NOW'</b> function in the settings. The proconrulz.ini file will also be updated (if this feature is enabled).</p>
<p></p>
<h2>Notify & In-Game Messages</h2>
<p>You can enable, disable and customize every single in-game message based on chat and player events. OnJoin, OnSpawn and OnChat are trigger events.</p>
<p>You do not have to use all the available textboxes for messages, leave it blank if you do not need it.</p>
<p>The 'Replacement Strings' below are available for any message:</p>
<table border ='1'>
	<tr><th>Replacement String</th><th>Effect</th></tr>
	<tr><td>%player%</td><td>Will be replaced by the playername</td></tr>
	<tr><td>%time%</td><td>Will be replaced by the VIP Slot remaining time (for valid VIPs only)</td></tr>
	<tr><td>%total%</td><td>Will be replaced by the total number of all valid VIPs on this server</td></tr>
	<tr><td>%online%</td><td>Will be replaced by the number of online VIPs</td></tr>
</table>
<blockquote><b>Sample Message:</b><br>
!VIP %player% valid for: %time%<br>
!VIPs online: %online%/%total%</blockquote>
<p></p><p></p>
<h2>In-Game VIP Commands</h2>
<p>These commands are for valid VIP Slot players only. Each command can be enabled or disabled separately.</p>
<blockquote><h4>!lead</h4>take squad leader position</blockquote>
<blockquote><h4>!killme</h4>admin kill without death in scoreboard</blockquote>
<blockquote><h4>!switchme</h4>switch between teams</blockquote>
<p></p><p></p>
<h2>In-Game Admin Commands</h2>
<p>These commands are for in-game admins only. Admins need the privilege 'Can Edit Reserved Slots List'. You can enable or disable this function in the setting <b>'Enable In-Game Admin Commands'</b>.</p>
<p><u>IMPORTANT</u>: Requires the &lt;full playername&gt; with case sensitive!</p>
<blockquote><h4>!addvip &lt;full playername&gt; &lt;days&gt;</h4>e.g. !addvip SniperBen 30<br>e.g. !addvip SniperBen +7</blockquote>
<blockquote><h4>!removevip &lt;full playername&gt;</h4>e.g. !removevip SniperBen</blockquote>
<blockquote><h4>!checkvip &lt;full playername&gt;</h4>e.g. !checkvip SniperBen</blockquote>
<blockquote><h4>!changevip &lt;old playername&gt; &lt;new playername&gt;</h4>e.g. !changevip SniperBen SniperBenni</blockquote>
<blockquote><h4>!addsemivip &lt;full playername&gt;</h4>e.g. !addsemivip SniperBen</blockquote>
<p></p>
<p>The command <b>!addvip</b> with <b>30</b> days adds and activates a player’s VIP status for the next 30 days. The time period of 30 days is fixed. When you enter this command more than once it has no effect on the time period.</p>
<p>The command <b>!addvip</b> with <b>+7</b> days checks the VIP player’s remaining time (e.g. the VIP Slot is still valid for 5 days). Then the Plugin ADDS 7 days to the 'old' time period. For example: old time period (5 days) + new time period (7 days) = total time period (12 days). Now the VIP Slot is valid for 12 days.</p>
<p>The command <b>!removevip</b> will remove the VIP from the Gameserver. The player will stay in the SQL database and be marked as 'status inactive'.</p>
<p>The command <b>!addsemivip</b> will add an Semi VIP Slot temporary (valid on current Gameserver till round end / player rejoin). The plugin setting <b>'Aggressive Join Abuse Protection'</b> must be enabled to handle Semi VIPs.</p>
<p></p>
<h2>Other Plugin Support</h2>
<p>Other Plugins such as ProconRulz/InsaneLimits can use special commands to remove or add a VIP Slot for a custom time period. Other Plugins can send commands as a 'hidden say' within the in-game chat. Other players will not see this in-game message, but the Plugin receives this information. In the Procon PC Tool you can enter the commands to (say, all players). You can enable or disable this function in the setting <b>'Enable Commands for other Plugins'</b>.</p>
<p>Commands (hidden admin say)</p>
<blockquote><h4>/vsm-addvip &lt;full playername&gt; &lt;days&gt;</h4>e.g. /vsm-addvip SniperBen 30<br>e.g. /vsm-addvip SniperBen +7</blockquote>
<blockquote><h4>/vsm-removevip &lt;full playername&gt;</h4>e.g. /vsm-removevip SniperBen</blockquote>
<blockquote><h4>/vsm-changevip &lt;old playername&gt; &lt;new playername&gt;</h4>e.g. /vsm-changevip SniperBen SniperBenni</blockquote>
<blockquote><h4>/vsm-addsemivip &lt;full playername&gt;</h4>e.g. /vsm-addsemivip SniperBen</blockquote>
<p></p>
<blockquote><b>Sample Code for ProconRulz (perform 5 knife kills =  VIP Slot for 7 days):</b><br>
On Kill; Damage Melee; if %c% == 5; Say /vsm-addvip %p% +7</blockquote>
<p></p><p></p>
<h2>Advanced ProconRulz Support</h2>
<p>The VIP Slot Manager Plugin can store a list of valid VIPs in the proconrulz.ini file. This file stays on your Procon Layer Server (path: CONFIGS/proconrulz_&lt;ip&gt_&lt;port&gt.ini). When you read this file within the Plugin ProconRulz (%ini_vipslotmanager_&lt;playername&gt%) you will get the VIP timestamp in seconds. This means you can check the VIP status without any player protection for weapon rule punishment (kill, kick, ban). You can enable or disable this function within the setting <b>'On Round End write VIPs in proconrulz.ini file'</b>.</p>
<p>For a quick one time update to the proconrulz.ini file you can use the <b>'Force Sync SQL and Gameserver NOW'</b> function in the settings.</p>
<p><u>IMPORTANT</u>: Requires Read+Write file permission in the directory /configs/ on your Procon Layer.</p>
<blockquote><b>Sample Code for ProconRulz (in-game command '!check' returns the VIP player status):</b><br>
On Say; Text !check; if %ini_vipslotmanager_%p%% != 0; Say Yes, you are a VIP<br>
On Say; Text !check; if %ini_vipslotmanager_%p%% == 0; Say No, you are NOT a VIP</blockquote>
<p></p><p></p>
<h2>Aggressive Join for VIPs</h2>
<p>The 'Aggressive Join' is a server setting that allows VIPs to join a full server without waiting. A random NON-VIP player will be kicked to make room for a VIP.</p>
<p>The Plugin can detect this kind of kick and keeps you informed if a NON-VIP player got kicked to make room for a VIP on full server. If the kicked player rejoins, the Plugin sends him a customized message. You can enable, disable and customize this feature in the setting <b>'Private Message after NON-VIP got kicked and rejoins'</b>.</p>
<p>In addition, the Plugin can disable the 'Aggressive Join' close on round end to keep as many players as possible on the server. On the next round it will be enabled automatically. This feature works for the following game modes: ConquestLarge, ConquestSmall, TDM and Chainlink. You can enable or disable this function in the setting <b>'Temporary disable the Aggressive Join close on round end'</b>.</p>
<p><b>Aggressive Join Abuse Protection</b><br>
The function <b>'Aggressive Join Abuse Protection'</b> is also helpful to track each VIP if he rejoins too many times with an 'Aggressive Join Kick' on full server. When a single VIP triggered is his max. threshold (custom setting value) of this kind of rejoins per round, then he can not rejoin again with 'Aggressive Join Kick' privilege till next round. In this case, if he rejoins again in the same round, then he can NOT bypass the server queue. He have to wait like normal players. While he is on the server, the Gameserver and all Plugins handles him as an valid VIP but without 'Aggressive Join Kick' privilege. All other VIPs can still join with 'Aggressive Join Kick' privilege. On next round he can join again with 'Aggressive Join Kick' privilege.</b></p>
<p></p>
<h2>Advanced Settings</h2>
<p><b>Debug Level</b><br>
1 - Errors will be displayed.<br>
2 - will also show log entries for added and removed VIP players.<br>
3 - will also show log entries when a VIP player joins the server.<br>
4 - will also show log entries when a player uses the in-game commands (e.g. !lead, !killme).<br>
5 - just for development and testing.</p>
<p><b>Manual Force Sync</b><br>
For a quick one time Sync you can use the <b>'Force Sync SQL and Gameserver NOW'</b> function in the settings. The proconrulz.ini file will also be updated (if this feature is enabled).</p>
<p><b>Auto Database Cleaner</b><br>
This feature reduces the Sync traffic between SQL and Gameserver. It is necessary because the Sync is limited (max. 800 active/expired VIPs for each Server Group). Each Sync includes a list of valid VIPs and expired VIPs. Expired VIPs will get a notify message on the next spawn event. But if the player does not join the server for long time period (60 days by default setting), then this feature will remove him from the Sync in order to reduce the traffic. It changes the player status from 'expired' to 'inactive' and the player will not recives the expired VIP Slot message.</p>
<p>In addition, old VIPs with the status 'inactive' will be deleted after 365 days automatically.</p>
<p><b>VIP EA GUID Tracking</b><br>
If a VIP changes his playername then his VIP Slot will be updated to the new playername automatically. After a VIP joins the server, the Plugin links his playername to his EA GUID. If he joins again with a new/changed playername then his VIP Slot will be updated to the new playername for all <b>Server Groups</b> on current <b>Gameserver Type</b> in SQL database (e.g. for all BF4 Groups 1-99). After the VIP Slot has expired the EA GUID will be unlinked.</p>
<p><u>IMPORTANT</u>: If the Plugins runs on two or more Gameservers with the same <b>'Gametype'</b>, then the Plugin setting <b>'Import NEW VIPs from GS to SQL'</b> must be set to <b>'no (remove)'</b> on ALL Gameservers to use this function. You can enable or disable the tracking function in the setting <b>'EA GUID Tracking'</b>.</p>
<p></p>
<h2>How to add, edit and remove VIPs</h2>
<p><b>Website (highly recommended)</b><br>
The easiest way to manage reserved VIP Slots is a website with access to the MySQL database. You can find a free website template in the downloaded ZIP file from this Plugin. It is highly recommended that you use it!</p>
<p><b>In-Game Admin Commands</b><br>
As an in-game admin you can use the commands: !addvip, !checkvip, and !removevip for the current Gameserver (Server Group).</p>
<p><b>Procon PC Tool</b><br>
In the Plugin settings you can use the <b>'Mini Manager - Print VIP list'</b> to display the current VIP list with players remaining time on your Procon PC Tool Chat tab.<br>
You can also use the commands from the 'Other Plugin Support' function to add and remove VIP Slots. You can enter the commands in the Procon PC Tool chat as a hidden admin say (e.g. /vsm-addvip SniperBen +7). Nobody will see the commands within the in-game chat.</p>
<p></p>
<h2>FAQ</h2>
<p><b>Do I need a new MySQL database?</b><br>
No. For this Plugin, it is NOT necessary to create a new MySQL database. You can use the same MySQL database as the Statslogger Plugin.</p>
<p><b>How to manage two or more Gameservers?</b><br>
With the web-based interface you can manage a single Gameserver or many Gameservers with different VIP players. If two Gameservers use the same <b>'Server Group'</b> ID (Plugin settings) (Plugin settings), then the VIP players are valid for both Gameservers. You can change the <b>'Server Group'</b> ID in order to manage the VIPs for each Gameserver separately. If two or more Gameservers use the same <b>'Server Group'</b> on the same SQL database then the Plugin setting <b>'Import NEW VIPs from Gameserver to SQL'</b> must be set to <b>no (remove)</b>.</p>
<p><b>AdKats Plugin</b><br>
It is possible to use the VIP Slot Manager and the AdKats Plugin. Please make sure that Adkats do not manage the reserved VIP Slots. This is disabled by default. Open the settings from Adkats Plugin, then go to Adkats &gt; A16. Orchestration Settings &gt; Feed Server Reserved Slots &gt; False</p>
<p><b>What is the diffenence between 'add 30' and 'add +30'?</b><br>
The command with + checks the VIP players remaining time (e.g. the VIP Slot is still valid for 5 days), then the Plugin ADDS 30 days to the 'old' time period. For example: old time period (5 days) + new time period (30 days) = total time period (35 days). Now the VIP Slot is valid for 35 days.</p>
<p><b>How to clean up the database?</b><br>
With the website you can clean up the database to remove all old VIPs with the status 'inactive'. Go to the website. Type 'inactive' into the search box. Mark all entires (click on the first VIP and then hold down the SHIFT key on your keyboard and click on the last VIP). Then open the drop down menu and click on 'DELETE' to delete the marked entries.</p>
<p><b>Witch Games are supported?</b><br>
The Plugin works fine for BF3, BF4, BFH and BFBC2. The support for other Games are still not tested.</p>
<p><b>Where can I find updates and support?</b><br>
Updates and support will be handled through the procon thread or on github website:<br>
https://github.com/procon-plugin/vip-slot-manager<br>
https://forum.myrcon.com/showthread.php?17050-VIP-Slot-Manager-Plugin</p>
<p></p>
<h2>Development</h2>
<p><b>Changelog</b></p>
<blockquote><h4>1.0.0.6 (23.06.2018)</h4>
- Add: Aggressive Join Abuse Protection (optional)<br/>
- Add: Command !addsemivip to add temporary VIP till round end / rejoin (optional)<br/>
- Add: Advanced Log to Adkats (optional)<br/>
- Add: Alternative Link to github<br/>
</blockquote><br>
<blockquote><h4>1.0.0.5 (26.01.2018)</h4>
- Add: VIP EA Guid Tracking to update playername changes automatically (optional)<br/>
- Add: Command !changevip to change VIP Slot playername<br/>
- Modification: In-Game VIP Command !lead (optional VIP protection)<br/>
- Modification: Small code improvements<br/>
- Fix: SQL Credentials after server restart<br/>
- Fix: BC2 Procon compatibility<br/>
- Fix: Website compatibility to php 5.6 / 7.0 / 7.1 / 7.2. New features and filters for better workflow<br/>
</blockquote><br>
<blockquote><h4>1.0.0.4 (04.10.2017)</h4>
- Add: Aggressive Join features<br/>
</blockquote><br>
<blockquote><h4>1.0.0.3 (12.08.2017)</h4>
- Add: Auto Correction for case sensitive difference in playername<br/>
- Add: Auto Database Cleaner<br/>
- Fix: Website (add days button)<br/>
</blockquote><br>
<blockquote><h4>1.0.0.2 (09.05.2017)</h4>
- Fix: Website blank site<br/>
</blockquote><br>
<blockquote><h4>1.0.0.1 (02.05.2017)</h4>
- Fix: In-Game VIP Commands<br/>
</blockquote><br>
<blockquote><h4>1.0.0.0 (20.01.2017)</h4>
- First Release<br/>
</blockquote><br>
";
}

public List<CPluginVariable> GetPluginVariables() {
	return GetDisplayPluginVariables();
}

public List<CPluginVariable> GetDisplayPluginVariables() {
	List<CPluginVariable> lstReturn = new List<CPluginVariable>();

	lstReturn.Add(new CPluginVariable("1. MySQL Details|Host", this.SettingStrSqlHostname.GetType(), this.SettingStrSqlHostname));
	lstReturn.Add(new CPluginVariable("1. MySQL Details|Port", this.SettingStrSqlPort.GetType(), this.SettingStrSqlPort));
	lstReturn.Add(new CPluginVariable("1. MySQL Details|Database", this.SettingStrSqlDatabase.GetType(), this.SettingStrSqlDatabase));
	lstReturn.Add(new CPluginVariable("1. MySQL Details|Username", this.SettingStrSqlUsername.GetType(), this.SettingStrSqlUsername));
	lstReturn.Add(new CPluginVariable("1. MySQL Details|Password", this.SettingStrSqlPassword.GetType(), this.SettingStrSqlPassword));
	lstReturn.Add(new CPluginVariable("2. Main Settings|Gameserver Type", "enum.SettingGameType(AUTOMATIC|BF3|BF4|BFH|BC2)", this.SettingGameType));
	lstReturn.Add(new CPluginVariable("2. Main Settings|Server Group (1-99)", this.SettingStrSqlServerGroup.GetType(), this.SettingStrSqlServerGroup));
	lstReturn.Add(new CPluginVariable("3. Sync Settings|Sync Interval between SQL and Gameserver. Minutes (2-60)", this.SettingSyncInterval.GetType(), this.SettingSyncInterval));
	lstReturn.Add(new CPluginVariable("3. Sync Settings|Import NEW VIPS from Gameserver to SQL", "enum.SettingSyncGs2Sql(yes  (30 days first Plugin installation only)|no  (ignore)|no  (remove from Gameserver)|yes  (as inactive)|yes  (for 7 days)|yes  (for 30 days)|yes  (for 90 days)|yes  (for 365 days)|yes  (permanent))", this.SettingSyncGs2Sql));
	lstReturn.Add(new CPluginVariable("4. Commands|Enable In-Game Admin Commands?", typeof(enumBoolYesNo), this.SettingAdminCmd));
	lstReturn.Add(new CPluginVariable("4. Commands|Enable Commands for other Plugins?", typeof(enumBoolYesNo), this.SettingPluginCmd));
	lstReturn.Add(new CPluginVariable("4. Commands|Enable In-Game VIP Commands?", typeof(enumBoolYesNo), this.SettingVipCmd));
	if (this.SettingVipCmd == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("4. Commands|  - Enable VIP Command !lead", typeof(enumBoolYesNo), this.SettingLeadCmd));
		if (this.SettingLeadCmd == enumBoolYesNo.Yes) {
			lstReturn.Add(new CPluginVariable("4. Commands|     - VIP can take from other VIP", typeof(enumBoolYesNo), this.SettingIgnoreVipLeader));
			lstReturn.Add(new CPluginVariable("4. Commands|     - Enforce Commo Rose Request from VIPs", typeof(enumBoolYesNo), this.SettingLeadByCRose));
		}
		lstReturn.Add(new CPluginVariable("4. Commands|  - Enable VIP Command !killme", typeof(enumBoolYesNo), this.SettingKillmeCmd));
		lstReturn.Add(new CPluginVariable("4. Commands|  - Enable VIP Command !switchme", typeof(enumBoolYesNo), this.SettingSwitchmeCmd));
	}
	lstReturn.Add(new CPluginVariable("3. Sync Settings|On Round End write VIPs in proconrulz.ini file?", typeof(enumBoolYesNo), this.SettingProconRulzIni));
	lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|Show VIP Slot Infos on Chat request?", typeof(enumBoolYesNo), this.SettingChatReq));
	if (this.SettingChatReq == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|Chat Commands", this.SettingInfoCommands.GetType(), this.SettingInfoCommands));
		lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|First Info Message", this.SettingInfoVip1.GetType(), this.SettingInfoVip1));
		lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|Second Info Message", this.SettingInfoVip2.GetType(), this.SettingInfoVip2));
		lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|Private Message to valid VIPs only", this.SettingInfoVip3.GetType(), this.SettingInfoVip3));
		lstReturn.Add(new CPluginVariable("5. Notify - Chat Request|Send to all Players?", typeof(enumBoolYesNo), this.SettingInfoSay));
	}
	lstReturn.Add(new CPluginVariable("6. Notify - Join Message|Show Message when VIP is joining?", typeof(enumBoolYesNo), this.SettingVipJoin));
	if (this.SettingVipJoin == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("6. Notify - Join Message|  - VIP Player Join Message", this.SettingVipJoinMsg.GetType(), this.SettingVipJoinMsg));
	}
	lstReturn.Add(new CPluginVariable("6. Notify - Join Message|Show Message when NON-VIP is joining?", typeof(enumBoolYesNo), this.SettingNonVipJoin));
	if (this.SettingNonVipJoin == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("6. Notify - Join Message|  - NON-VIP Join Message", this.SettingNonVipJoinMsg.GetType(), this.SettingNonVipJoinMsg));
	}
	lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Show Message when VIP is spawning (each round first spawn)?", typeof(enumBoolYesNo), this.SettingVipSpawn));
	if (this.SettingVipSpawn == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|First VIP Spawn Message", this.SettingVipSpawnMsg.GetType(), this.SettingVipSpawnMsg));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Second VIP Spawn Message", this.SettingVipSpawnMsg2.GetType(), this.SettingVipSpawnMsg2));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Third VIP Spawn Message", this.SettingVipSpawnMsg3.GetType(), this.SettingVipSpawnMsg3));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|VIP Spawn Yell", this.SettingVipSpawnYell.GetType(), this.SettingVipSpawnYell));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Send First Msg to all Players?", typeof(enumBoolYesNo), this.SettingVipSpawnMsgSay));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Send Second Msg to all Players?", typeof(enumBoolYesNo), this.SettingVipSpawnMsg2Say));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Send Third Msg to all Players?", typeof(enumBoolYesNo), this.SettingVipSpawnMsg3Say));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Send Yell to all Players?", typeof(enumBoolYesNo), this.SettingVipSpawnYellAll));
		lstReturn.Add(new CPluginVariable("7. Notify - Spawn Message|Send Message Delay in sec. (0-20)", this.SettingVipSpawnMsgDelay.GetType(), this.SettingVipSpawnMsgDelay));
	}
	lstReturn.Add(new CPluginVariable("8. Notify - VIP Slot Expired|Show Private Message when VIP Slot expired?", typeof(enumBoolYesNo), this.SettingVipExp));
	if (this.SettingVipExp == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("8. Notify - VIP Slot Expired|First Slot Expired Message", this.SettingVipExpMsg.GetType(), this.SettingVipExpMsg));
		lstReturn.Add(new CPluginVariable("8. Notify - VIP Slot Expired|Second Slot Expired Message", this.SettingVipExpMsg2.GetType(), this.SettingVipExpMsg2));
		lstReturn.Add(new CPluginVariable("8. Notify - VIP Slot Expired|Slot Expired Yell", this.SettingVipExpYell.GetType(), this.SettingVipExpYell));
		lstReturn.Add(new CPluginVariable("8. Notify - VIP Slot Expired|Delay for send Message in sec. (0-20)", this.SettingVipExpDelay.GetType(), this.SettingVipExpDelay));
	}
	lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|Temporary disable the 'Aggressive Join' close on round end (Conquest, TDM and Chainlink only)", typeof(enumBoolYesNo), this.SettingAggressiveJoin));
	lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|Public Say Message when NON-VIP got kicked for VIP", this.SettingAggressiveJoinKick.GetType(), this.SettingAggressiveJoinKick));
	lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|Private Message after NON-VIP got kicked and rejoins", this.SettingAggressiveJoinMsg.GetType(), this.SettingAggressiveJoinMsg));
	lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|  - Send Private Message as:", "enum.SettingAggressiveJoinMsgType(Private Yell and Private Say|Private Yell|Private Say|Say to all Players)", this.SettingAggressiveJoinMsgType));
	lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|Enable Aggressive Join Abuse Protection", typeof(enumBoolYesNo), this.SettingAggressiveJoinKickAbuse));
	if (this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|  - Each VIP can rejoin with an Aggressive Join Kick on full server maximal: (# per round)", this.SettingAggressiveJoinKickAbuseMax.GetType(), this.SettingAggressiveJoinKickAbuseMax));
		lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|  - Private Message to target VIP if he triggered his max. threshold per round", this.SettingAggressiveJoinKickAbuseMsg.GetType(), this.SettingAggressiveJoinKickAbuseMsg));
		lstReturn.Add(new CPluginVariable("8. Server - Aggressive Join for VIPs|  - Send Private Msg as:", "enum.SettingAggressiveJoinKickAbuseMsgType(Private Yell and Private Say|Private Yell|Private Say)", this.SettingAggressiveJoinKickAbuseMsgType));
	}
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Debug Level (1-5)", this.fDebugLevel.GetType(), this.fDebugLevel));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Force Sync SQL and Gameserver NOW", typeof(enumBoolYesNo), this.SettingForceSync));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Mini Manager - Print VIP list with time", typeof(enumBoolYesNo), this.SettingMiniManager));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|During for Yell and PYell in sec. (5-60)", this.SettingYellDuring.GetType(), this.SettingYellDuring));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Auto DB Cleaner (set expired VIPs without join event to inactive after # days)", this.SettingDBCleaner.GetType(), this.SettingDBCleaner));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Enable EA Guid Tracking and update playername changes", typeof(enumBoolYesNo), this.EAGuidTracking));
	lstReturn.Add(new CPluginVariable("9. Advanced Settings|Enable Advanced Log to Adkats", typeof(enumBoolYesNo), this.SettingAdkatsLog));
	if (this.SettingAdkatsLog == enumBoolYesNo.Yes) {
		lstReturn.Add(new CPluginVariable("9. Advanced Settings|  - NON-VIP got kicked for VIP", typeof(enumBoolYesNo), this.SettingAdkatsLogNonVipKick));
		lstReturn.Add(new CPluginVariable("9. Advanced Settings|  - VIP changed his playername", typeof(enumBoolYesNo), this.SettingAdkatsLogVipChanged));
		lstReturn.Add(new CPluginVariable("9. Advanced Settings|  - VIP triggered Aggressive Join Abuse Protection (min. threshold from settings: 2)", typeof(enumBoolYesNo), this.SettingAdkatsLogVipAggressiveJoinAbuse));
	}
	return lstReturn;
}

public void SetPluginVariable(String strVariable, String strValue) {
	bool layerReady = (((DateTime.UtcNow - this.LayerStartingTime).TotalSeconds) > 30);
	if (Regex.Match(strVariable, @"Debug Level").Success) {
		int tmp = 3;
		int.TryParse(strValue, out tmp);
		if (tmp >= 0 && tmp <= 5) {
			this.fDebugLevel = tmp;
		} else {
			ConsoleError("Invalid value for Debug Level: '" + strValue + "'. It must be a number between 1 and 5. (e.g.: 3)");
		}
	} else if (Regex.Match(strVariable, @"Host").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		if (strValue.Length <= 100) { this.SettingStrSqlHostname = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Port").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		int tmpport = 3306;
		int.TryParse(strValue, out tmpport);
		if (tmpport > 0 && tmpport < 65536) {
			this.SettingStrSqlPort = tmpport.ToString();
		} else {
			ConsoleError("Invalid value for MySQL Port: '" + strValue + "'. Port must be a number between 1 and 65535. (e.g.: 3306)");
		}
	} else if (Regex.Match(strVariable, @"Database").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		if (strValue.Length <= 100) { this.SettingStrSqlDatabase = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Username").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		if (strValue.Length <= 100) { this.SettingStrSqlUsername = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Password").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		if (strValue.Length <= 100) { this.SettingStrSqlPassword = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Gameserver Type").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("SQL Settings locked! Please disable the Plugin and try again..."); return; }
		this.SettingGameType = strValue;
		if (strValue == "AUTOMATIC") {
			if (this.GetGameType == "BF3") {this.NewLiner = "\n"; this.SettingGameType = "BF3";}
			if (this.GetGameType == "BF4") {this.NewLiner = "\n"; this.SettingGameType = "BF4";}
			if (this.GetGameType == "BFHL") {this.NewLiner = ""; this.SettingGameType = "BFH";}
			if (this.GetGameType == "BFBC2") {this.NewLiner = ""; this.SettingGameType = "BC2";}
		}
		if (strValue == "BF3") { this.NewLiner = "\n"; }
		if (strValue == "BF4") { this.NewLiner = "\n"; }
		if (strValue == "BFH") { this.NewLiner = ""; }
		if (strValue == "BC2") { this.NewLiner = ""; }
	} else if (Regex.Match(strVariable, @"Server Group").Success) {
		if ((this.fIsEnabled) && (layerReady) && (this.firstCheck)) { ConsoleError("Setting 'Server Group' locked! Please disable the Plugin and try again..."); return; }
		int tmpserverid = 1;
		int.TryParse(strValue, out tmpserverid);
		if (tmpserverid > 0 && tmpserverid < 100) {
			this.SettingStrSqlServerGroup = tmpserverid.ToString();
		} else {
			ConsoleError("Invalid value for MySQL Server Group. It must be a number between 1 and 99. (e.g.: 1)");
		}
	} else if (Regex.Match(strVariable, @"Delay for send Message").Success) {
		int tmpMsgDel2 = 0;
		int.TryParse(strValue, out tmpMsgDel2);
		if (tmpMsgDel2 >= 0 && tmpMsgDel2 < 21) {
			this.SettingVipExpDelay = tmpMsgDel2;
		} else {
			ConsoleError("Invalid value for Send Message Delay. It must be a number between 0 and 20. (e.g.: 0 for no delay)");
		}
	} else if (Regex.Match(strVariable, @"Sync Interval between SQL and Gameserver").Success) {
		int tmpIntr = 0;
		int.TryParse(strValue, out tmpIntr);
		if (tmpIntr >= 2 && tmpIntr < 61) {
			this.SettingSyncInterval = tmpIntr;
		} else {
			ConsoleError("Invalid value for Sync Interval. It must be a number between 2 and 60");
		}
	} else if (Regex.Match(strVariable, @"Import NEW VIPS from Gameserver to SQL").Success) {
		this.SettingSyncGs2Sql = strValue;
	} else if (Regex.Match(strVariable, @"On Round End write VIPs in proconrulz.ini file?").Success) {
		this.SettingProconRulzIni = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Game Admin Commands?").Success) {
		this.SettingAdminCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Enable Commands for other Plugins").Success) {
		this.SettingPluginCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Game VIP Commands?").Success) {
		this.SettingVipCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"lead").Success) {
		this.SettingLeadCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"VIP can take from other VIP").Success) {
		this.SettingIgnoreVipLeader = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Enforce Commo Rose Request from VIPs").Success) {
		this.SettingLeadByCRose = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"killme").Success) {
		this.SettingKillmeCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"switchme").Success) {
		this.SettingSwitchmeCmd = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Show VIP Slot Infos on Chat request?").Success) {
		this.SettingChatReq = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Chat Commands").Success) {
		if ((strValue.Length < 3) || (strValue.Length > 100)) {
			this.SettingInfoCommands = "!vip,!slot,!reserved,!buy";
			GenRegMatch();
		} else {
			this.SettingInfoCommands = strValue.Replace(System.Environment.NewLine, ",").Replace(" ", "").Replace(";", ",").Replace(",,", ",");
			GenRegMatch();
		}
	} else if (Regex.Match(strVariable, @"First Info Message").Success) {
		if ((strValue.Length >= 3) && (strValue.Length <= 100)) {
			this.SettingInfoVip1 = strValue.Replace(System.Environment.NewLine, "");
		} else {
			ConsoleError("Invalid value for First Info Message (min. 3 and max. 100 chararcters)");
		}
	} else if (Regex.Match(strVariable, @"Second Info Message").Success) {
		this.SettingInfoVip2 = strValue.Replace(System.Environment.NewLine, "");
	} else if (Regex.Match(strVariable, @"Private Message to valid VIPs only").Success) {
		this.SettingInfoVip3 = strValue;
	} else if (Regex.Match(strVariable, @"Send to all Players?").Success) {
		this.SettingInfoSay = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Show Message when VIP is joining?").Success) {
		this.SettingVipJoin = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"  - VIP Player Join Message").Success) {
		if ((strValue.Length < 3) || (strValue.Length > 100)) {
			this.SettingVipJoinMsg = "%player% with !VIP SLOT joined the server";
		} else {
			this.SettingVipJoinMsg = strValue.Replace(System.Environment.NewLine, "");
		}
	} else if (Regex.Match(strVariable, @"Show Message when NON-VIP is joining?").Success) {
		this.SettingNonVipJoin = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"  - NON-VIP Join Message").Success) {
		if ((strValue.Length < 3) || (strValue.Length > 100)) {
			this.SettingNonVipJoinMsg = "%player% joined the server";
		} else {
			this.SettingNonVipJoinMsg = strValue.Replace(System.Environment.NewLine, "");
		}
	} else if (Regex.Match(strVariable, @"Show Message when VIP is spawning").Success) {
		this.SettingVipSpawn = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"First VIP Spawn Message").Success) {
		if ((strValue.Length < 3) || (strValue.Length > 100)) {
			this.SettingVipSpawnMsg = String.Empty;
		} else {
			this.SettingVipSpawnMsg = strValue;
		}
	} else if (Regex.Match(strVariable, @"Second VIP Spawn Message").Success) {
		if (strValue.Length <= 100) { this.SettingVipSpawnMsg2 = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Third VIP Spawn Message").Success) {
		if (strValue.Length <= 100) { this.SettingVipSpawnMsg3 = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"VIP Spawn Yell").Success) {
		if (strValue.Length <= 100) { this.SettingVipSpawnYell = strValue; }
	} else if (Regex.Match(strVariable, @"Send First Msg to all Players?").Success) {
		this.SettingVipSpawnMsgSay = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Send Second Msg to all Players?").Success) {
		this.SettingVipSpawnMsg2Say = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Send Third Msg to all Players?").Success) {
		this.SettingVipSpawnMsg3Say = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Send Yell to all Players?").Success) {
		this.SettingVipSpawnYellAll = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Send Message Delay").Success) {
		int tmpMsgDel = 0;
		int.TryParse(strValue, out tmpMsgDel);
		if (tmpMsgDel >= 0 && tmpMsgDel < 21) {
			this.SettingVipSpawnMsgDelay = tmpMsgDel;
		} else {
			ConsoleError("Invalid value for Send Message Delay. It must be a number between 0 and 20. (e.g.: 0 for no delay)");
		}
	} else if (Regex.Match(strVariable, @"Show Private Message when VIP Slot expired?").Success) {
		if (strValue.Length <= 100) { this.SettingVipExp = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue); }
	} else if (Regex.Match(strVariable, @"First Slot Expired Message").Success) {
		if (strValue.Length > 100) {
			this.SettingVipExpMsg = "%player% your !VIP SLOT has expired";
		} else {
			this.SettingVipExpMsg = strValue.Replace(System.Environment.NewLine, "");
		}
	} else if (Regex.Match(strVariable, @"Second Slot Expired Message").Success) {
		if (strValue.Length <= 100) { this.SettingVipExpMsg2 = strValue.Replace(System.Environment.NewLine, ""); }
	} else if (Regex.Match(strVariable, @"Slot Expired Yell").Success) {
		if (strValue.Length <= 100) { this.SettingVipExpYell = strValue; }
	} else if (Regex.Match(strVariable, @"Temporary disable the").Success) {
		if ((enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue) == enumBoolYesNo.Yes) {
			ConsoleWrite("'Aggressive Join' server setting enabled. Close on round end it will be disabled to keep as many players as possible on the server. On next round it will be enabled automatically. This feature works for the following game modes: ConquestLarge, ConquestSmall, TDM and Chainlink.");
		}
		this.SettingAggressiveJoin = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"Public Say Message when NON-VIP got kicked for VIP").Success) {
		this.SettingAggressiveJoinKick = strValue;
	} else if (Regex.Match(strVariable, @"Private Message after NON-VIP got kicked and rejoins").Success) {
		if (strValue.Length <= 100) { this.SettingAggressiveJoinMsg = strValue; }
	} else if (Regex.Match(strVariable, @"Send Private Message as:").Success) {
		this.SettingAggressiveJoinMsgType = strValue;
	} else if (Regex.Match(strVariable, @"Enable Aggressive Join Abuse Protection").Success) {
		this.SettingAggressiveJoinKickAbuse = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
		if ((enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue) == enumBoolYesNo.Yes) {
			if ((layerReady) && (this.SettingSyncGs2Sql != "no  (remove from Gameserver)") && (this.SettingSyncGs2Sql != "no  (ignore)") && (this.SettingSyncGs2Sql != "yes  (30 days first Plugin installation only)")) {
				ConsoleWrite("ERROR: The setting 'Import NEW VIPS from Gameserver to SQL' must be set to 'NO (remove) to use 'Aggressive Join Abuse Protection'");
				this.SettingAggressiveJoinKickAbuse = enumBoolYesNo.No;
			}
		} else {
			if (layerReady) { this.AggressiveJoinAbuseCleaner(); }
		}
	} else if (Regex.Match(strVariable, @"Each VIP can rejoin with an Aggressive Join Kick on full server").Success) {
		int tmpMaxRejoin = 0;
		int.TryParse(strValue, out tmpMaxRejoin);
		if (tmpMaxRejoin >= 0 && tmpMaxRejoin < 10) {
			this.SettingAggressiveJoinKickAbuseMax = tmpMaxRejoin;
		} else {
			this.SettingAggressiveJoinKickAbuseMax = 3;
			ConsoleError("Invalid value. It must be a number between 0 and 10. (e.g.: 3 - for max. 3 rejoins on full server)");
		}
	} else if (Regex.Match(strVariable, @"Private Message to target VIP if he triggered his max").Success) {
		if (strValue.Length <= 100) { this.SettingAggressiveJoinKickAbuseMsg = strValue; }
	} else if (Regex.Match(strVariable, @"Send Private Msg as:").Success) {
		this.SettingAggressiveJoinKickAbuseMsgType = strValue;
	} else if (Regex.Match(strVariable, @"Force Sync SQL and Gameserver NOW").Success) {
		if ((enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue) == enumBoolYesNo.Yes) {
			if (this.fIsEnabled) {
				this.ProconVipList();
				if (this.firstCheck) {
					ConsoleWrite("[ForceSync] ^bForce Sync started...^n");
					DebugWrite("[ForceSync] Receive VIP players from Gameserver and Sync with SQL", 3);
					this.isForceSync = true;
					this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "2", "1", "1", "procon.protected.plugins.call", "VipSlotManager", "SyncVipList");
				} else {
					ConsoleError("Force Sync canceled! Still loading VIPs from Gameserver");
				}
			} else {
				ConsoleError("Force Sync canceled! Please enable the Plugin and try again...");
			}
		}
		this.SettingForceSync = enumBoolYesNo.No;
	} else if (Regex.Match(strVariable, @"Print VIP list with time").Success) {
		if ((enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue) == enumBoolYesNo.Yes) {
			if (this.fIsEnabled) {
				if (this.firstCheck) {
					this.DisplayVips();
				} else {
					ConsoleError("Mini Manager canceled! Still loading VIPs from Gameserver");
				}
			} else {
				ConsoleError("Mini Manager canceled! Please enable the Plugin and try again...");
			}
		}
		this.SettingMiniManager = enumBoolYesNo.No;
	} else if (Regex.Match(strVariable, @"During for Yell and PYell in sec").Success) {
		int tmpyelltime = 1;
		int.TryParse(strValue, out tmpyelltime);
		if (tmpyelltime >= 5 && tmpyelltime <= 60) {
			this.SettingYellDuring = tmpyelltime;
		} else {
			ConsoleError("Invalid value for Yell During. Time must be a number between 5 and 60. (e.g.: 15)");
		}
	} else if (Regex.Match(strVariable, @"Auto DB Cleaner").Success) {
		int tmpDBCleaner = 1;
		int.TryParse(strValue, out tmpDBCleaner);
		if (tmpDBCleaner >= 1 && tmpDBCleaner <= 999) {
			this.SettingDBCleaner = tmpDBCleaner;
			this.DBCleanerCounter = (new Random().Next(960, 990));
		} else {
			ConsoleError("Invalid value. It must be a number between 1 and 999. (e.g.: 90)");
		}
	} else if (Regex.Match(strVariable, @"Enable EA Guid Tracking and update playername changes").Success) {
		this.EAGuidTracking = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
		if (layerReady) {
			if (this.EAGuidTracking == enumBoolYesNo.Yes) {
				ConsoleWrite("EA GUID Tracking enabled.");
				ConsoleWrite("EA GUID Tracking > ^bIMPORTANT INFO:^n If the Plugins runs on two or more " + this.SettingGameType + " Gameservers, then the Plugin setting 'Import NEW VIPs from GS to SQL'^n must be set to 'no (remove)^n' on ALL " + this.SettingGameType + " Gameservers to enable the 'EA GUID Tracking'.");
				ConsoleWrite("EA GUID Tracking > INFO: If a valid VIP joins the server, then his playername will be linked to his EA GUID. If he joins again with a new/changed playername then his VIP Slot will be updated to the new playername for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database.");
			} else {
				this.Guid2Check.Clear();
				this.GetPlayerGuid.Clear();
			}
			if ((this.EAGuidTracking == enumBoolYesNo.Yes) && ((this.SettingGameType != this.GetGameType.Replace("BFHL","BFH").Replace("BFBC", "BC")) && (this.GetGameType != String.Empty) && (this.SettingGameType != "AUTOMATIC"))) { ConsoleWrite("WARNING: YOU selected " + this.strBlack(this.SettingGameType) + " as Gametype but Plugin detects " + this.strBlack(this.GetGameType) + ". EA GUID Tracking can NOT work correctly because the same player can have a diffrent EA GUID on a diffrent BF Version."); }
		}
	} else if (Regex.Match(strVariable, @"Enable Advanced Log to Adkats").Success) {
		this.SettingAdkatsLog = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"NON-VIP got kicked for VIP").Success) {
		this.SettingAdkatsLogNonVipKick = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"VIP changed his playername").Success) {
		this.SettingAdkatsLogVipChanged = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	} else if (Regex.Match(strVariable, @"VIP triggered Aggressive Join Abuse Protection").Success) {
		this.SettingAdkatsLogVipAggressiveJoinAbuse = (enumBoolYesNo)Enum.Parse(typeof(enumBoolYesNo), strValue);
	}
}


//////////////////////
// Server Events
//////////////////////

public void OnPluginLoaded(String strHostName, String strPort, String strPRoConVersion) {
	this.RegisterEvents(this.GetType().Name, "OnServerInfo", "OnPlayerJoin", "OnPlayerDisconnected", "OnPlayerSpawned", "OnGlobalChat", "OnTeamChat", "OnSquadChat", "OnRoundOver", "OnLevelLoaded", "OnLoadingLevel", "OnReservedSlotsList", "OnReservedSlotsListAggressiveJoin", "OnListPlayers", "OnSquadLeader");
	this.ServerIP = strHostName;
	this.ServerPort = strPort;

	if (this.SettingGameType == "AUTOMATIC") {
		if (this.GetGameType == "BF3") { this.SettingGameType = "BF3";}
		if (this.GetGameType == "BF4") { this.SettingGameType = "BF4";}
		if (this.GetGameType == "BFHL") { this.SettingGameType = "BFH";}
		if (this.GetGameType == "BFBC2") { this.SettingGameType = "BC2";}
	}

	if (this.GetGameType == "BF3") { this.NewLiner = "\n"; }
	if (this.GetGameType == "BF4") { this.NewLiner = "\n"; }
	if (this.GetGameType == "BFHL") { this.NewLiner = ""; }
	if (this.GetGameType == "BFBC2") { this.NewLiner = ""; }
}

public void OnPluginEnable() {
	this.fIsEnabled = true;
	this.gotVipsGS = false;
	this.firstCheck = false;
	this.SqlTableExist = false;
	this.SyncCounter = 999;
	this.DBCleanerCounter = (new Random().Next(960, 990));
	ConsoleWrite("^b^2Enabled!^0^n");

	if (this.SettingGameType == "AUTOMATIC") {
		ConsoleError("[Checkup] ERROR: Please go to the Plugin settings and select a valid 'GameType'. Shutdown Plugin");
		return;
	}
	if (this.SettingGameType != this.GetGameType.Replace("BFHL","BFH").Replace("BFBC", "BC")) { ConsoleWrite("Info about your current Plugin settings: YOU selected " + this.strBlack(this.SettingGameType) + " as Gametype but Plugin detects " + this.strBlack(this.GetGameType)); }
	ConsoleWrite("Main Settings loaded. ^nServer Group: ^b" + this.SettingGameType + " - " + this.SettingStrSqlServerGroup + "^n");
	if ((this.SettingSyncGs2Sql != "no  (remove from Gameserver)") && (this.SettingSyncGs2Sql != "yes  (30 days first Plugin installation only)")) {ConsoleWrite("Info about your current Plugin settings: If two or more Gameservers use the same 'Server Group' ID then the Plugin setting 'Import NEW VIPs from GS to SQL'^n must be set to 'no (remove)^n'. This setting is also required to use the 'Aggressive Join Kick Protection'.");}

	this.ProconVipList();
	this.GenRegMatch();
	this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "3", "9", "20", "procon.protected.plugins.call", "VipSlotManager", "PluginStarter");
	if (this.GetGameType == "BFBC2") { this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "5", "100", "-1", "procon.protected.send", "reservedSlots.list"); } //bfbc2
}

public void OnPluginDisable() {
	this.ExecuteCommand("procon.protected.tasks.remove", "VipSlotManager");
	this.AggressiveJoinAbuseCleaner();
	this.fIsEnabled = false;
	this.SqlVipsActive.Clear();
	this.SqlVipsActiveNamesOnly.Clear();
	this.onJoinSpammer.Clear();
	this.vipsExpired.Clear();
	this.kickedForVip.Clear();
	this.vipmsg.Clear();
	this.playerTeamID.Clear();
	this.playerSquadID.Clear();
	this.SquadLederList.Clear();
	this.Guid2Check.Clear();
	this.GetPlayerGuid.Clear();
	this.vipsGS.Clear();
	this.NameGuidList.Clear();
	this.SyncCounter = 999;
	this.SqlTableExist = false;
	this.gotVipsGS = false;
	this.firstCheck = false;
	ConsoleWrite("^b^8Disabled!^0^n");
}

public override void OnReservedSlotsList(List<string> soldierNames) {
	if (!this.fIsEnabled) { return; }
	DebugWrite("[OnReservedSlotsList] Receive VIP players from Gameserver (reservedSlotList)", 5);
	this.vipsGS = soldierNames;
	this.gotVipsGS = true;
}

public override void OnReservedSlotsListAggressiveJoin(bool isEnabled) {
	this.AggressiveJoin = isEnabled;
}

public override void OnSquadLeader(int teamId, int squadId, string soldierName) {
	if ((teamId > 0) && (squadId > 0)) {
		if (this.SquadLederList.ContainsKey(teamId.ToString() + squadId.ToString())) {
			this.SquadLederList[teamId.ToString() + squadId.ToString()] = soldierName;
		} else {
			this.SquadLederList.Add(teamId.ToString() + squadId.ToString(), soldierName);
		}
		if (this.playerTeamID.ContainsKey(soldierName)) {
			this.playerTeamID[soldierName] = teamId;
		} else {
			this.playerTeamID.Add(soldierName, teamId);
		}
		if (this.playerSquadID.ContainsKey(soldierName)) {
			this.playerSquadID[soldierName] = squadId;
		} else {
			this.playerSquadID.Add(soldierName, squadId);
		}
	}
}

public override void OnListPlayers(List<CPlayerInfo> players, CPlayerSubset subset) {
	if (!this.fIsEnabled) { return; }
	try {
		if (CPlayerSubset.PlayerSubsetType.All == subset.Subset) {
			DebugWrite("[OnListPlayers] Receive playerlist with TeamID and SquadID", 5);
			this.vipsCurrentlyOnline = 0;
			// this.NameGuidList.Clear();
			foreach (CPlayerInfo playerinfo in players) {
				if (this.SqlVipsActive.ContainsKey(playerinfo.SoldierName)) { this.vipsCurrentlyOnline++; }
				if (this.playerTeamID.ContainsKey(playerinfo.SoldierName)) {
					this.playerTeamID[playerinfo.SoldierName] = playerinfo.TeamID;
				} else {
					this.playerTeamID.Add(playerinfo.SoldierName, playerinfo.TeamID);
				}
				if (this.playerSquadID.ContainsKey(playerinfo.SoldierName)) {
					this.playerSquadID[playerinfo.SoldierName] = playerinfo.SquadID;
				} else {
					this.playerSquadID.Add(playerinfo.SoldierName, playerinfo.SquadID);
				}
				if (playerinfo.GUID.StartsWith("EA_")) {
					if (this.GetPlayerGuid.Contains(playerinfo.SoldierName)) {
						if (!this.Guid2Check.ContainsKey(playerinfo.GUID)) {
							this.Guid2Check.Add(playerinfo.GUID, playerinfo.SoldierName);
						}
						this.GetPlayerGuid.Remove(playerinfo.SoldierName);
					}
					if (this.NameGuidList.ContainsKey(playerinfo.SoldierName)) {
						if (this.NameGuidList[playerinfo.SoldierName] != playerinfo.GUID) { this.NameGuidList[playerinfo.SoldierName] = playerinfo.GUID; }
					} else {
						this.NameGuidList.Add(playerinfo.SoldierName, playerinfo.GUID);
					}
				}
			}
		}
	}
	catch (Exception ex) {
		DebugWrite("[OnListPlayers] ^bERROR:^n Can not receive playerlist with TeamID and SquadID. ERROR: " + ex, 5);
	}
}

public override void OnPlayerJoin(String soldierName) {
	string tmp_remaing = String.Empty;
	string tmp_msg = String.Empty;
	int tmp_playerTimestamp = -1;

	// NO SEEDERBOT JOIN MSG - XXXXXXXXXXXXXXXXXXXXXX
	if ((soldierName.StartsWith("Seed")) || (soldierName.StartsWith("seed"))) { return; }

	if ((this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) || (this.SettingNonVipJoin == enumBoolYesNo.Yes) || (this.SettingVipJoin == enumBoolYesNo.Yes)) { tmp_playerTimestamp = this.GetVipTimestamp(soldierName); }

	// aggressive join kick protection
	if (this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) {
		if (tmp_playerTimestamp != -1) {
			if (!this.AggressiveVips.ContainsKey(soldierName)) { this.AggressiveVips.Add(soldierName, 0); }
			if (this.RoundTempVips.Contains(soldierName)) {
				// vip rejoined without 'aggressive join kick' power
				DebugWrite("[OnJoin] [VIP] [AggressiveJoinAbuseBlocked] Valid VIP player " + this.strGreen(soldierName) + " rejoined as an valid VIP without 'Aggressive Join Kick' privilege (counter: " + this.AggressiveVips[soldierName].ToString() + "/" + this.SettingAggressiveJoinKickAbuseMax.ToString() + ") till next round.", 3);
				// add tmp vip slot
				DebugWrite("[OnJoin] [VIP] [AggressiveJoinAbuseBlocked] Add VIP Slot " + this.strGreen(soldierName) + " to Gameserver.", 4);
				this.ProconVipAdd(soldierName);
				this.ProconVipSave();
				this.ProconVipList();
			} else {
				if (((DateTime.UtcNow - this.LastAggressiveJoinKickTime).TotalSeconds) <= 3) {
					// vip joined with 'aggressive join kick', count vip kicks
					DebugWrite("[OnJoin] [VIP] [AggressiveJoin] Valid VIP player " + this.strGreen(soldierName) + " joined with 'Aggressive Join Kick' (counter: " + this.AggressiveVips[soldierName].ToString() + "/" + this.SettingAggressiveJoinKickAbuseMax.ToString() + ").", 5);
					this.LastAggressiveJoinKickTime = DateTime.UtcNow.AddMinutes(-5);
					this.AggressiveVips[soldierName]++;
				}
				if (this.AggressiveVips[soldierName] >= this.SettingAggressiveJoinKickAbuseMax) {
					// block vip to use 'aggressive join kick' till next round
					DebugWrite("[OnJoin] [VIP] [AggressiveJoin] Valid VIP player " + this.strGreen(soldierName) + " triggered his max. threshold (" + this.AggressiveVips[soldierName].ToString() + "/" + this.SettingAggressiveJoinKickAbuseMax.ToString() + ") to use 'Aggressive Join Kick' till next round.", 4);
					
					if ((this.AggressiveVips[soldierName] >= 2) && (this.SettingAdkatsLog == enumBoolYesNo.Yes) && (this.SettingAdkatsLogVipAggressiveJoinAbuse == enumBoolYesNo.Yes)) {
						if (this.NameGuidList.ContainsKey(soldierName)) {
							this.AdkatsPlayerLog(soldierName, "Aggressive Join Abuse " + this.AggressiveVips[soldierName].ToString() + "/" + this.SettingAggressiveJoinKickAbuseMax.ToString() + " per round (blocked to use Aggressive Join Kick till next round)");
						} else {
							// wait, retry. need ea guid for adkats
							Thread ThreadWorker1011 = new Thread(new ThreadStart(delegate() {
								int tmp_retry_counter = 0;
								while (tmp_retry_counter < 10) {
									Thread.Sleep(12000);
									tmp_retry_counter++;
									if (this.NameGuidList.ContainsKey(soldierName)) {
										tmp_retry_counter = 99;
										this.AdkatsPlayerLog(soldierName, "Aggressive Join Abuse " + this.AggressiveVips[soldierName].ToString() + "/" + this.SettingAggressiveJoinKickAbuseMax.ToString() + " per round (blocked to use Aggressive Join Kick till next round)");
									}
								}
							}));
							ThreadWorker1011.IsBackground = true;
							ThreadWorker1011.Name = "threadworker1011";
							ThreadWorker1011.Start();
						}
					}

					this.RoundTempVips.Add(soldierName);
					this.ProconVipAdd(soldierName);
					this.ProconVipSave();
					this.ProconVipList();
				}
			}
		}
	}

	// no multiple join spam
	if (this.onJoinSpammer.ContainsKey(soldierName)) {
		if (((DateTime.UtcNow - this.onJoinSpammer[soldierName]).TotalSeconds) < 30) {
			this.onJoinSpammer[soldierName] = DateTime.UtcNow;
			return;
		}
	} else {
		this.onJoinSpammer.Add(soldierName, DateTime.UtcNow);
	}
	
	// check case sensitive difference between real ingame playername and sql playername
	if (this.SqlVipsActiveNamesOnly.Contains(soldierName, StringComparer.CurrentCultureIgnoreCase)) {
		if (!this.SqlVipsActive.ContainsKey(soldierName)) {
			// case sensitive problem detected
			if (this.changevipname(soldierName, soldierName)) {
				DebugWrite("[OnJoin] [Auto-Correction] Change VIP Slot playername to " + this.strGreen(soldierName) + " for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database (case sensitive difference detected between real playername and SQL playername). Gameserver will be updated on next Sync.", 3);
				this.ProconChat("Auto-Correction > Change VIP Slot playername to " + this.strGreen(soldierName) + " (case sensitive difference detected between real playername and SQL playername).");
			}
		}
	}

	/////////////////////////
	// Notify - Join Message
	/////////////////////////
	if (this.SettingVipJoin == enumBoolYesNo.Yes) {
		if (tmp_playerTimestamp != -1) {
			DebugWrite("[OnJoin] [VIP] " + this.strGreen(soldierName) + " with !VIP Slot joined the server", 3);
			tmp_remaing = this.strGetRemTime(tmp_playerTimestamp);
			tmp_msg = this.SettingVipJoinMsg.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
			this.SayMsg(tmp_msg);
		}
	}
	if (this.SettingNonVipJoin == enumBoolYesNo.Yes) {
		if (tmp_playerTimestamp == -1) {
			tmp_msg = this.SettingNonVipJoinMsg.Replace("%player%", soldierName).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
			this.SayMsg(tmp_msg);
		}
	}

	if (this.vipmsg.Contains(soldierName)) { this.vipmsg.Remove(soldierName); }
	if ((this.EAGuidTracking == enumBoolYesNo.Yes) && (!this.GetPlayerGuid.Contains(soldierName))) { this.GetPlayerGuid.Add(soldierName); }

} 

public override void OnPlayerSpawned(String soldierName, Inventory spawnedInventory) {
	string tmp_remaing = String.Empty;
	string tmp_msg = String.Empty;
	bool tmp_firstspawn = false;

	// clean player join spamming
	if (this.onJoinSpammer.ContainsKey(soldierName)) { this.onJoinSpammer.Remove(soldierName); }

	/////////////////////////
	// Notify - Spawn Message
	/////////////////////////
	if (this.SettingVipSpawn == enumBoolYesNo.Yes) {
		int tmp_playerTimestamp = this.GetVipTimestamp(soldierName);
		if (tmp_playerTimestamp != -1) {
			if (!this.vipmsg.Contains(soldierName)) {
				this.vipmsg.Add(soldierName);
				tmp_firstspawn = true;
				if ((this.EAGuidTracking == enumBoolYesNo.Yes) && (!this.GetPlayerGuid.Contains(soldierName))) { this.GetPlayerGuid.Add(soldierName); }
				DebugWrite("[OnSpawn] [VIP] " + this.strGreen(soldierName) + " with !VIP Slot spawned (first time this round)", 5);
				tmp_remaing = this.strGetRemTime(tmp_playerTimestamp);
				// First Spawn Msg
				tmp_msg = this.SettingVipSpawnMsg.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				if (this.SettingVipSpawnMsgSay == enumBoolYesNo.Yes) {
					this.SayMsg(tmp_msg, this.SettingVipSpawnMsgDelay);
				} else {
					this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay);
				}
				// Second Spawn Msg
				tmp_msg = this.SettingVipSpawnMsg2.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				if (this.SettingVipSpawnMsg2Say == enumBoolYesNo.Yes) {
					this.SayMsg(tmp_msg, this.SettingVipSpawnMsgDelay);
				} else {
					this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay);
				}
				// Third Spawn Msg
				tmp_msg = this.SettingVipSpawnMsg3.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				if (this.SettingVipSpawnMsg3Say == enumBoolYesNo.Yes) {
					this.SayMsg(tmp_msg, this.SettingVipSpawnMsgDelay);
				} else {
					this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay);
				}
				// Spawn Yell
				tmp_msg = this.SettingVipSpawnYell.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				if (this.SettingVipSpawnYellAll == enumBoolYesNo.Yes){
					this.YellMsg(tmp_msg, this.SettingVipSpawnMsgDelay);
				} else {
					this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay);
				}
			}
		}
	}

	/////////////////////////
	// Notify - VIP Slot Expired Message
	/////////////////////////
	if (this.SettingVipExp == enumBoolYesNo.Yes) {
		if (this.vipsExpired.Contains(soldierName)) {
			if (!this.vipmsg.Contains(soldierName)) {
				this.vipmsg.Add(soldierName);
				DebugWrite("[OnSpawn] [VIP] Send VIP Expired Message to " + this.strGreen(soldierName) , 3);
				// First Spawn Msg
				tmp_msg = this.SettingVipExpMsg.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipExpDelay);
				// Second Spawn Msg
				tmp_msg = this.SettingVipExpMsg2.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipExpDelay);
				// Spawn Yell
				tmp_msg = this.SettingVipExpYell.Replace("%player%", this.strGreen(soldierName)).Replace("%time%", tmp_remaing).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
				this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipExpDelay);
				// SQL Set Status inactive
				if (this.SetSqlpStatus(soldierName, "inactive")) { 
					DebugWrite("[OnSpawn] [VIP Expired] " + this.strGreen(soldierName) + " set vip status to 'inactive' in SQL database", 4);
				}
			}
		}
	}
	
	/////////////////////////
	// Notify - NON-VIP got kicked for VIP
	/////////////////////////
	if (this.SettingAggressiveJoinMsg.Length > 2) {
		if (this.kickedForVip.Contains(soldierName)) {
			DebugWrite("[OnSpawn] " + this.strBlack(soldierName) + " rejoined as kicked NON-VIP" , 4);
			tmp_msg = this.SettingAggressiveJoinMsg.Replace("%player%", soldierName).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
			// Send Msg
			if (this.SettingAggressiveJoinMsgType == "Private Yell") {
				this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay); 
			} else 	if (this.SettingAggressiveJoinMsgType == "Private Say") {
				this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay); 
			} else if (this.SettingAggressiveJoinMsgType == "Private Yell and Private Say") {
				this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay);
				this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay); 
			} else if (this.SettingAggressiveJoinMsgType == "Say to all Players") {
				this.SayMsg(tmp_msg, this.SettingVipSpawnMsgDelay); 
			}
			this.kickedForVip.Remove(soldierName);
		}
	}
	
	/////////////////////////
	// Notify - VIP lost his 'Aggressive Join Kick' privilege till next round
	/////////////////////////
	if (this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) {
		if ((tmp_firstspawn) || (!this.vipmsg.Contains(soldierName))) {
			if (!this.vipmsg.Contains(soldierName)) { this.vipmsg.Add(soldierName); }
			if (this.SettingAggressiveJoinKickAbuseMsg.Length > 2) {
				if ((this.SqlVipsActive.ContainsKey(soldierName)) && (this.RoundTempVips.Contains(soldierName))) {
					DebugWrite("[OnSpawn] [VIP] [AggressiveJoinAbuseBlocked]" + this.strGreen(soldierName) + " send message about 'Aggressive Join Abuse'." , 5);
					// Send Msg
					tmp_msg = this.SettingAggressiveJoinKickAbuseMsg.Replace("%player%", soldierName).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString());
					if (this.SettingAggressiveJoinKickAbuseMsgType == "Private Yell") {
						this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay + 1); 
					} else 	if (this.SettingAggressiveJoinKickAbuseMsgType == "Private Say") {
						this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay + 1); 
					} else if (this.SettingAggressiveJoinKickAbuseMsgType == "Private Yell and Private Say") {
						this.PlayerYellMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay + 1);
						this.PlayerSayMsg(soldierName, tmp_msg, this.SettingVipSpawnMsgDelay + 1); 
					}
				}
			}
		}
	}
}

public override void OnRoundOver(int winningTeamId) {
	this.RoundEndCleaner();
}

public override void OnLevelLoaded(String mapFileName, String Gamemode, int roundsPlayed, int roundsTotal) { // BF3 BF4 BFH ******
	this.RoundEndCleaner();
}

public override void OnLoadingLevel(string strMapFileName, int roundsPlayed, int roundsTotal) { // BFBC2 *******
	this.RoundEndCleaner();
}

public override void OnServerInfo(CServerInfo serverInfo) {
	this.CurrentGameMode = serverInfo.GameMode;
	if (this.SettingAggressiveJoin == enumBoolYesNo.Yes) {
		if ((this.CurrentGameMode == "ConquestLarge0") || (this.CurrentGameMode == "ConquestSmall0") || (this.CurrentGameMode == "TeamDeathMatch0") || (this.CurrentGameMode == "Chainlink0")) {
			double tmp_scoreLoser = 999999;
			foreach (TeamScore score in serverInfo.TeamScores) {
				if (score.TeamID != null && score.Score != null) {
					double tmp_remscore = score.WinningScore - score.Score;
					if (this.CurrentGameMode != "TeamDeathMatch0") { tmp_remscore = score.Score - score.WinningScore; }
					if (tmp_remscore <= tmp_scoreLoser) { tmp_scoreLoser = tmp_remscore; }
				}
			}
			this.ticketLoserTeam = tmp_scoreLoser;
		}
	}
}

public override void OnPlayerDisconnected(String playerName, String reason) {
	if ((this.AggressiveJoin) && (reason == "PLAYER_KICKED")) {
		DebugWrite(this.strBlack(playerName) + " got KICKED to make room for VIP", 3);
		if (this.SettingAggressiveJoinKick == String.Empty) { this.ProconChat(playerName + " got KICKED to make room for VIP"); }
		if ((this.SettingAdkatsLog == enumBoolYesNo.Yes) && (this.SettingAdkatsLogNonVipKick == enumBoolYesNo.Yes)) { this.AdkatsPlayerLog(playerName, "got kicked to make room for VIP"); }
		this.SayMsg(this.SettingAggressiveJoinKick.Replace("%player%", playerName).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()), this.SettingVipSpawnMsgDelay); 
		if (!this.kickedForVip.Contains(playerName)) {this.kickedForVip.Add(playerName); }
		this.LastAggressiveJoinKickTime = DateTime.UtcNow;
	}

	if (this.RoundTempVips.Contains(playerName)) {
		// remove vip
		if (this.SqlVipsActive.ContainsKey(playerName)) {
			DebugWrite("[OnDisconnected] [AggressiveJoinAbuse] Valid VIP player " + this.strGreen(playerName) + " blocked to use 'Aggressive Join Kick' till next round.", 3);
			DebugWrite("[OnDisconnected] Remove VIP Slot " + this.strGreen(playerName) + " from Gameserver.", 4);
		} else {
			DebugWrite("[OnDisconnected] [SemiVIP] Remove Semi VIP Slot " + this.strBlack(playerName) + " from Gameserver.", 3);
		}
		this.ProconVipRemove(playerName);
		this.ProconVipSave();
		this.ProconVipList();
	}

	if (this.playerTeamID.ContainsKey(playerName)) { this.playerTeamID.Remove(playerName); }
	if (this.playerSquadID.ContainsKey(playerName)) { this.playerSquadID.Remove(playerName); }
	if (this.GetPlayerGuid.Contains(playerName)) { this.GetPlayerGuid.Remove(playerName); }
}

public override void OnTeamChat(String speaker, String message, int teamId) { this.vsChatEvent(speaker, message); }

public override void OnSquadChat(String speaker, String message, int teamId, int squadId) { this.vsChatEvent(speaker, message); }

public override void OnGlobalChat(string strSpeaker, string strMessage) {this.vsChatEvent(strSpeaker, strMessage); }

//////////////////////
// In-Game Chat Event
//////////////////////

private void vsChatEvent(string tmpPlayername, string Msg) {
	if ((!this.fIsEnabled) || (!this.firstCheck) || (Msg.Length < 4)) { return; }
	Match regexResult = null;
	string tmp_pname = String.Empty;
	string tmp_pname_old = String.Empty;
	bool CRoseLead = false;

	//////////////////////
	// COMMANDS FROM OTHER PLUGIN
	//////////////////////
	if (tmpPlayername == "Server") {
		if (this.SettingPluginCmd == enumBoolYesNo.Yes) {
			if (Regex.Match(Msg, @"^/vsm-addvip", RegexOptions.IgnoreCase).Success) {
				// remote command from other plugin via hidden admin say  ( e.g. /vsm-addvip <playername> <days> )
				Match regexMatch = Regex.Match(Msg, @"/vsm-addvip\s+([^\s]+)\s+([^\s][0-9]*)$", RegexOptions.IgnoreCase);
				if (regexMatch.Success) {
					regexResult = regexMatch;
					if ((regexResult.Groups[1].Value.Length > 2) && (regexResult.Groups[2].Value.Length >= 1)) {
						int tmp_days = 0;
						string tmp_strdays = "1";
						int.TryParse(regexResult.Groups[2].Value, out tmp_days);
						if ((tmp_days > 0) && (tmp_days < 5555)) {
							if (regexResult.Groups[2].Value.Contains("+")) {
								tmp_strdays = "+" + tmp_days.ToString();
							} else {
								tmp_strdays = tmp_days.ToString();
							}
							tmp_pname = this.strSqlProtection(regexResult.Groups[1].Value);
							Thread SQLWorker1 = new Thread(new ThreadStart(delegate() {
								if (this.addvip(tmp_pname, tmp_strdays, "PluginCmd")) {
									DebugWrite("[OnChat] [PluginCmd] ^bAdd^n VIP Slot: " + this.strGreen(tmp_pname) + " for " + tmp_strdays + " days  (by command from other plugin)", 2);
									if (this.fDebugLevel >= 2) { this.ProconChat("PluginCmd > Add VIP Slot: " + this.strGreen(tmp_pname) + " for " + tmp_strdays + " days"); }
								}
							}));
							SQLWorker1.IsBackground = true;
							SQLWorker1.Name = "sqlworker1";
							SQLWorker1.Start();
						}
					} else {
						DebugWrite("[OnChat] [PluginCmd] ^bERROR^n in /vsm-addvip syntax. Command is NOT valid: " + Msg, 2);
					}
				} else {
					DebugWrite("[OnChat] [PluginCmd] ^bERROR^n in /vsm-addvip syntax. Command is NOT valid: " + Msg, 2);
				}
			} else if (Regex.Match(Msg, @"^/vsm-removevip", RegexOptions.IgnoreCase).Success) {
				// remote command from other plugin via hidden admin say  ( e.g. /vsm-removevip <playername> )
				Match regexMatch = Regex.Match(Msg, @"/vsm-removevip\s+([^\s]+)$", RegexOptions.IgnoreCase);
				if (regexMatch.Success) {
					regexResult = regexMatch;
					if (regexResult.Groups[1].Value.Length > 2) {
						DebugWrite("[OnChat] [PluginCmd] ^bRemove^n VIP Slot: " + this.strGreen(regexResult.Groups[1].Value.Replace(" ","")) + "  (by command from other plugin)", 4);
						tmp_pname = this.strSqlProtection(regexResult.Groups[1].Value);
						Thread SQLWorker2 = new Thread(new ThreadStart(delegate() {
							if (this.removevip(tmp_pname)) {
								DebugWrite("[OnChat] [PluginCmd] ^bRemove^n VIP Slot: " + this.strGreen(tmp_pname) + "  (by command from other plugin)", 2);
								if (this.fDebugLevel >= 2) { this.ProconChat("PluginCmd > Remove VIP Slot: " + this.strGreen(tmp_pname)); }
							}
						}));
						SQLWorker2.IsBackground = true;
						SQLWorker2.Name = "sqlworker2";
						SQLWorker2.Start();
					}
				}
			} else if (Regex.Match(Msg, @"^/vsm-changevip", RegexOptions.IgnoreCase).Success) {
				// remote command from other plugin via hidden admin say  ( e.g. /vsm-changevip <old playername> <new playername> )
				Match regexMatch = Regex.Match(Msg, @"/vsm-changevip\s+([^\s]+)\s+([^\s]*)$", RegexOptions.IgnoreCase);
				if (regexMatch.Success) {
					regexResult = regexMatch;
					DebugWrite("[OnChat] [PluginCmd] Command from other Plugin (/vsm-changevip)", 4);
					if ((regexResult.Groups[1].Value.Length > 2) && (regexResult.Groups[2].Value.Length > 2)) {
						tmp_pname_old = this.strSqlProtection(regexResult.Groups[1].Value);
						tmp_pname = this.strSqlProtection(regexResult.Groups[2].Value);
						Thread SQLWorker6 = new Thread(new ThreadStart(delegate() {
							if (this.changevipname(tmp_pname_old, tmp_pname)) {
								DebugWrite("[OnChat] [PluginCmd] Change VIP Slot playername from " + this.strGreen(tmp_pname_old) + " to " + this.strGreen(tmp_pname) + " for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database  (by command from other plugin)", 2);
								if (this.fDebugLevel >= 2) { this.ProconChat("PluginCmd > Change VIP Slot playername from " + this.strGreen(tmp_pname_old) + " to " + this.strGreen(tmp_pname) + " for all " + this.strBlack(this.SettingGameType) + " Server Groups"); }
							}
						}));
						SQLWorker6.IsBackground = true;
						SQLWorker6.Name = "sqlworker6";
						SQLWorker6.Start();
					}
				}
			} else if (Regex.Match(Msg, @"^/vsm-addsemivip", RegexOptions.IgnoreCase).Success) {
				// remote command from other plugin via hidden admin say  ( e.g. /vsm-addsemivip <playername> )
				Match regexMatch = Regex.Match(Msg, @"/vsm-addsemivip\s+([^\s]+)$", RegexOptions.IgnoreCase);
				if (regexMatch.Success) {
					regexResult = regexMatch;
					if (regexResult.Groups[1].Value.Length > 2) {
						if ((this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) && (this.SettingSyncGs2Sql == "no  (remove from Gameserver)")) {
							DebugWrite("[OnChat] [PluginCmd] ^Add^n Semi VIP: " + this.strBlack(regexResult.Groups[1].Value.Replace(" ","")) + " (valid for current round / rejoin) - (command from other plugin)", 3);
							if (this.fDebugLevel >= 2) { this.ProconChat("PluginCmd > Add Semi VIP Slot: " + this.strBlack(regexResult.Groups[1].Value.Replace(" ","")) + " (valid for current round / rejoin)"); }
							this.AddRoundSemiVip(regexResult.Groups[1].Value.Replace(" ",""));
						} else {
							DebugWrite("[OnChat] [PluginCmd] ^bERROR^n Semi VIPs are disabled by Plugin settings", 3);
						}
					}
				}
			}
		}
		return;
	}

	//////////////////////
	// Notify - Chat Request  (!vip, !slot, ...)
	//////////////////////
	if (this.SettingChatReq == enumBoolYesNo.Yes) {
		if (Regex.Match(Msg, this.SettingInfoCmdRegMatch, RegexOptions.IgnoreCase).Success) {
			DebugWrite("[OnChat] !VIP Slot Infos requested by ^b" + tmpPlayername + "^n", 3);
			if (this.SettingInfoSay == enumBoolYesNo.Yes) {
				this.SayMsg(this.SettingInfoVip1.Replace("%player%", tmpPlayername).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()));
				this.SayMsg(this.SettingInfoVip2.Replace("%player%", tmpPlayername).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()));
			} else {
				this.PlayerSayMsg(tmpPlayername, this.SettingInfoVip1.Replace("%player%", tmpPlayername).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()));
				this.PlayerSayMsg(tmpPlayername, this.SettingInfoVip2.Replace("%player%", tmpPlayername).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()));
			}
			if (this.SqlVipsActive.ContainsKey(tmpPlayername)) {
				this.PlayerSayMsg(tmpPlayername, this.SettingInfoVip3.Replace("%player%", tmpPlayername).Replace("%time%", this.strGetRemTime(this.GetVipTimestamp(tmpPlayername))).Replace("%online%", this.vipsCurrentlyOnline.ToString()).Replace("%total%", this.SqlVipsActive.Count.ToString()));
			} else if ((this.EAGuidTracking == enumBoolYesNo.Yes) && (!this.GetPlayerGuid.Contains(tmpPlayername))) {
				this.GetPlayerGuid.Add(tmpPlayername);
			}
			return;
		}
	}
	
	//////////////////////
	// In-Game VIP Commands  (!lead, !killme, !switchme)
	//////////////////////
	if (this.SettingVipCmd == enumBoolYesNo.Yes) {
		if ((this.SettingLeadByCRose == enumBoolYesNo.Yes) && (Msg == "ID_CHAT_REQUEST_ORDER")) { CRoseLead = true; }
		if ((Regex.Match(Msg, @"^/?[!|@|/]lead$", RegexOptions.IgnoreCase).Success) || (CRoseLead)) {
			if (this.SettingLeadCmd == enumBoolYesNo.Yes) {
				if ((this.SqlVipsActive.ContainsKey(tmpPlayername)) || (this.RoundTempVips.Contains(tmpPlayername))) {
					if ((this.playerTeamID[tmpPlayername] > 0) && (this.playerSquadID[tmpPlayername] > 0)) {
						if ((this.SettingIgnoreVipLeader == enumBoolYesNo.No) && (this.SquadLederList.ContainsKey(this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()))) {
							if (this.SqlVipsActive.ContainsKey(this.SquadLederList[this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()])) {
								// other vip is leader
								if (!CRoseLead) {
									DebugWrite("[OnChat] [IngameVipCmd] Igonore In-Game VIP Command !lead by ^b" + tmpPlayername + "^n. (Other VIP " + this.SquadLederList[this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()] + " is currently leader)", 4);
									this.PlayerSayMsg(tmpPlayername, "Sorry, you can NOT take the lead from other !VIP");
								}
							} else {
								DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !lead by ^b" + tmpPlayername + "^n (TeamID: " + this.playerTeamID[tmpPlayername].ToString() + " SquadID: " + this.playerSquadID[tmpPlayername].ToString() + ")", 4);
								this.PlayerSayMsg(tmpPlayername, "You are the leader now");
								this.ExecuteCommand("procon.protected.send", "squad.leader", this.playerTeamID[tmpPlayername].ToString(), this.playerSquadID[tmpPlayername].ToString(), tmpPlayername);
							}
						} else {
							DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !lead by ^b" + tmpPlayername + "^n (TeamID: " + this.playerTeamID[tmpPlayername].ToString() + " SquadID: " + this.playerSquadID[tmpPlayername].ToString() + ")", 4);
							this.PlayerSayMsg(tmpPlayername, "You are the leader now");
							this.ExecuteCommand("procon.protected.send", "squad.leader", this.playerTeamID[tmpPlayername].ToString(), this.playerSquadID[tmpPlayername].ToString(), tmpPlayername);
						}
					} else {
						// enforcing lead
						this.ExecuteCommand("procon.protected.send", "admin.listPlayers", "all");
						int tmp_enforcingcounter = 0;
						Thread ThreadWorker3 = new Thread(new ThreadStart(delegate() {
							while ((this.playerTeamID[tmpPlayername] > 0) && (this.playerSquadID[tmpPlayername] > 0)) {
								Thread.Sleep(1500);
								tmp_enforcingcounter++;
								if (tmp_enforcingcounter == 2) { this.PlayerSayMsg(tmpPlayername, "Enforcing lead..."); }
								if (tmp_enforcingcounter > 5) {
									if (!CRoseLead) {
										DebugWrite("[OnChat] [IngameVipCmd] [LeadEnforcer] ERROR: Can not execute In-Game VIP Command !lead by ^b" + tmpPlayername + "^n (TeamID: " + this.playerTeamID[tmpPlayername].ToString() + " SquadID: " + this.playerSquadID[tmpPlayername].ToString() + ")", 4);
										this.PlayerSayMsg(tmpPlayername, tmpPlayername + " please use the command !lead in few seconds again.");
									}
									return;
								}
							}
							// ready for check again
							if ((this.playerTeamID[tmpPlayername] > 0) && (this.playerSquadID[tmpPlayername] > 0)) {
								if ((this.SettingIgnoreVipLeader == enumBoolYesNo.No) && (this.SquadLederList.ContainsKey(this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()))) {
									if (this.SqlVipsActive.ContainsKey(this.SquadLederList[this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()])) {
										// other vip is leader
										if (!CRoseLead) {
											DebugWrite("[OnChat] [IngameVipCmd] Igonore In-Game VIP Command !lead by ^b" + tmpPlayername + "^n. (Other VIP " + this.SquadLederList[this.playerTeamID[tmpPlayername].ToString() + this.playerSquadID[tmpPlayername].ToString()] + " is currently leader)", 4);
											this.PlayerSayMsg(tmpPlayername, "Sorry, you can NOT take the lead from other !VIP");
										}
									} else {
										DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !lead by ^b" + tmpPlayername + "^n (TeamID: " + this.playerTeamID[tmpPlayername].ToString() + " SquadID: " + this.playerSquadID[tmpPlayername].ToString() + ")", 4);
										this.PlayerSayMsg(tmpPlayername, "You are the leader now");
										this.ExecuteCommand("procon.protected.send", "squad.leader", this.playerTeamID[tmpPlayername].ToString(), this.playerSquadID[tmpPlayername].ToString(), tmpPlayername);
									}
								} else {
									DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !lead by ^b" + tmpPlayername + "^n (TeamID: " + this.playerTeamID[tmpPlayername].ToString() + " SquadID: " + this.playerSquadID[tmpPlayername].ToString() + ")", 4);
									this.PlayerSayMsg(tmpPlayername, "You are the leader now");
									this.ExecuteCommand("procon.protected.send", "squad.leader", this.playerTeamID[tmpPlayername].ToString(), this.playerSquadID[tmpPlayername].ToString(), tmpPlayername);
								}
							}
						}));
						ThreadWorker3.IsBackground = true;
						ThreadWorker3.Name = "threadworker3";
						ThreadWorker3.Start();
					}
				} else if (!CRoseLead) {
					DebugWrite("[OnChat] [IngameVipCmd] ^b" + tmpPlayername + "^n have NO VIP privileges to use the command '!lead'", 4);
					this.PlayerSayMsg(tmpPlayername, "Sorry, this command is for !VIP SLOT players only");
				}
			}
		} else if (Regex.Match(Msg, @"^/?[!|@|/]killme$", RegexOptions.IgnoreCase).Success) {
			if (this.SettingKillmeCmd == enumBoolYesNo.Yes) {
				if ((this.SqlVipsActive.ContainsKey(tmpPlayername)) || (this.RoundTempVips.Contains(tmpPlayername))) {
					DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !killme by ^b" + tmpPlayername + "^n", 4);
					this.ExecuteCommand("procon.protected.send", "admin.killPlayer", tmpPlayername);
				} else {
					DebugWrite("[OnChat] [IngameVipCmd] ^b" + tmpPlayername + "^n have NO VIP privileges to use the command '!killme'", 4);
					this.PlayerSayMsg(tmpPlayername, "Sorry, this command is for !VIP SLOT players only");
				}
			}
		} else if (Regex.Match(Msg, @"^/?[!|@|/]switchme$", RegexOptions.IgnoreCase).Success) {
			if (this.SettingSwitchmeCmd == enumBoolYesNo.Yes) {
				if ((this.SqlVipsActive.ContainsKey(tmpPlayername)) || (this.RoundTempVips.Contains(tmpPlayername))) {
					DebugWrite("[OnChat] [IngameVipCmd] In-Game VIP Command !switchme by ^b" + tmpPlayername + "^n", 4);
					if (this.playerTeamID[tmpPlayername] > 0) {
						int tmp_newteam = 1;
						if (this.playerTeamID[tmpPlayername] == 1) { tmp_newteam = 2;}
						this.ExecuteCommand("procon.protected.send", "admin.movePlayer", tmpPlayername, tmp_newteam.ToString(), "0", "true");
					}
				} else {
					DebugWrite("[OnChat] [IngameVipCmd] ^b" + tmpPlayername + "^n have NO VIP privileges to use the command '!switchme'", 4);
					this.PlayerSayMsg(tmpPlayername, "Sorry, this command is for !VIP SLOT players only");
				}
			}
		}
	}

	//////////////////////
	// COMMANDS FROM ADMINS
	//////////////////////
	if (this.SettingAdminCmd == enumBoolYesNo.Yes) {
		if (Regex.Match(Msg, @"^/?[!|/|@]addvip|^/?[!|/|@]removevip|^/?[!|/|@]checkvip|^/?[!|/|@]changevip|^/?[!|/|@]addsemivip", RegexOptions.IgnoreCase).Success) {
			Msg = Msg.Replace("\"", "").Replace(";", "").Replace(",", "").Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("'", "").Replace("’", "").Replace("‘", "");
			Thread SQLWorker3 = new Thread(new ThreadStart(delegate() {
				if (Regex.Match(Msg, @"^/?[!|/|@]addvip", RegexOptions.IgnoreCase).Success) {
					if (this.isAdmin(tmpPlayername)) {
						DebugWrite("[OnChat] [IngameAdmin] Admin In-Game Command (!addvip) from " + tmpPlayername, 4);
						// command from In-Game chat  ( e.g. !addvip <playername> <days>)
						bool AddOk = false;
						Match regexMatch = Regex.Match(Msg, @"^/?[!|/|@]addvip\s+([^\s]+)\s+([^\s][0-9]*)$", RegexOptions.IgnoreCase);
						if (regexMatch.Success) {
							regexResult = regexMatch;
							if ((regexResult.Groups[1].Value.Length > 2) && (regexResult.Groups[2].Value.Length >= 1)) {
								int tmpx_days = 0;
								string tmpx_strdays = "1";
								int.TryParse(regexResult.Groups[2].Value, out tmpx_days);
								if ((tmpx_days > 0) && (tmpx_days < 5555)) {
									if (regexResult.Groups[2].Value.Contains("+")) {
										tmpx_strdays = "+" + tmpx_days.ToString();
									} else {
										tmpx_strdays = tmpx_days.ToString();
									}
									if (this.addvip(regexResult.Groups[1].Value.Replace(" ",""), tmpx_strdays, tmpPlayername)) {
										DebugWrite("[OnChat] [IngameAdmin] ^bAdd^n VIP Slot: " + this.strGreen(regexResult.Groups[1].Value.Replace(" ","")) + " for " + tmpx_strdays + " days  (ingame admin " + tmpPlayername + ")", 2);
										this.PlayerSayMsg(tmpPlayername, "!VIP SLOT added: " + regexResult.Groups[1].Value.Replace(" ",""));
										AddOk = true;
									}
								}
							}
						}
						if (!AddOk) {
							DebugWrite("[OnChat] [IngameAdmin] ^bERROR in !addvip syntax from " + tmpPlayername + ". Command is NOT valid: " + Msg + "^n", 4);
							this.PlayerSayMsg(tmpPlayername, "ERROR: Player NOT added as !VIP. Check your syntax");
							this.PlayerSayMsg(tmpPlayername, "TYPE: !addvip <full playername> <days>  (e.g. !addvip SniperBen +30)");
						}
					} else {
						DebugWrite("[OnChat] [IngameAdmin] ^b" + tmpPlayername + "^n have NO admin privileges to use the command '!addvip'.  (Requires: Can Edit Reserved Slots List)", 3);
						this.PlayerSayMsg(tmpPlayername, "ERROR: You do NOT have admin privileges");
					}
				} else if (Regex.Match(Msg, @"^/?[!|/|@]removevip", RegexOptions.IgnoreCase).Success) {
					if (this.isAdmin(tmpPlayername)) {
						// command from In-Game chat  ( e.g. !removevip <playername> )
						bool AddOk = false;
						Match regexMatch = Regex.Match(Msg, @"^/?[!|/|@]removevip\s+([^\s]+)$", RegexOptions.IgnoreCase);
						if (regexMatch.Success) {
							regexResult = regexMatch;
							if (regexResult.Groups[1].Value.Length > 2) {
								DebugWrite("[OnChat] [IngameAdmin] Admin In-Game Command (!removevip) from " + tmpPlayername, 4);
								if (this.removevip(regexResult.Groups[1].Value.Replace(" ", ""))) {
									DebugWrite("[OnChat] [IngameAdmin] ^bRemove^n VIP Slot: " + this.strGreen(regexResult.Groups[1].Value.Replace(" ","")) + " (ingame admin " + tmpPlayername + ")", 2);
									this.PlayerSayMsg(tmpPlayername, "!VIP SLOT removed: " + regexResult.Groups[1].Value.Replace(" ",""));
									AddOk = true;
								}
							}
						}
						if (!AddOk) {
							DebugWrite("[OnChat] [IngameAdmin] ^bERROR in !remove syntax from " + tmpPlayername + ". Command is NOT valid: " + Msg + "^n", 4);
							this.PlayerSayMsg(tmpPlayername, "ERROR: Player NOT removed! Check your syntax");
							this.PlayerSayMsg(tmpPlayername, "TYPE: !removevip <full playername>  (e.g. !removevip SniperBen)");
						}
					} else {
						DebugWrite("[OnChat] [IngameAdmin] ^b" + tmpPlayername + "^n have NO admin privileges to use the command '!removevip'.  (Requires: Can Edit Reserved Slots List)", 3);
						this.PlayerSayMsg(tmpPlayername, "ERROR: You do NOT have admin privileges");
					}
				} else if (Regex.Match(Msg, @"^/?[!|/|@]checkvip", RegexOptions.IgnoreCase).Success) {
					if (this.isAdmin(tmpPlayername)) {
						// command from In-Game chat  ( e.g. !checkevip <playername> )
						Match regexMatch = Regex.Match(Msg, @"^/?[!|/|@]checkvip\s+([^\s]+)$", RegexOptions.IgnoreCase);
						if (regexMatch.Success) {
							DebugWrite("[OnChat] [IngameAdmin] Admin In-Game Command (!checkvip) from " + tmpPlayername, 4);
							regexResult = regexMatch;
							int tmp_playerTimestamp = this.checkvip(regexResult.Groups[1].Value);
							if (tmp_playerTimestamp != -1) {
								this.PlayerSayMsg(tmpPlayername, regexResult.Groups[1].Value + " valid !VIP SLOT: " + this.strGetRemTime(tmp_playerTimestamp));
								if (this.RoundTempVips.Contains(regexResult.Groups[1].Value)) { this.PlayerSayMsg(tmpPlayername, regexResult.Groups[1].Value + " without 'Aggressive Join Kick' privilege till next round."); }
							} else {
								if (this.RoundTempVips.Contains(regexResult.Groups[1].Value)) {
									this.PlayerSayMsg(tmpPlayername, regexResult.Groups[1].Value + " Semi !VIP SLOT: till round end / rejoin.");
								} else {
									this.PlayerSayMsg(tmpPlayername, regexResult.Groups[1].Value + " is NOT a !VIP");
									this.PlayerSayMsg(tmpPlayername, "Requires: <full playername> with case sensitive");
								}
							}
						}
					} else {
						DebugWrite("[OnChat] [IngameAdmin] ^b" + tmpPlayername + "^n have NO admin privileges to use the command '!removevip'.  (Requires: Can Edit Reserved Slots List)", 3);
						this.PlayerSayMsg(tmpPlayername, "ERROR: You do NOT have admin privileges");
					}
				} else if (Regex.Match(Msg, @"^/?[!|/|@]changevip", RegexOptions.IgnoreCase).Success) {
					if (this.isAdmin(tmpPlayername)) {
						DebugWrite("[OnChat] [IngameAdmin] Admin In-Game Command (!changevip) from " + tmpPlayername, 4);
						// command from In-Game chat  ( e.g. !changevip <old playername> <new playername> )
						bool changeOk = false;
						Match regexMatch = Regex.Match(Msg, @"^/?[!|/|@]changevip\s+([^\s]+)\s+([^\s]*)$", RegexOptions.IgnoreCase);
						if (regexMatch.Success) {
							regexResult = regexMatch;
							if ((regexResult.Groups[1].Value.Length > 2) && (regexResult.Groups[2].Value.Length > 2)) {
								tmp_pname_old = this.strSqlProtection(regexResult.Groups[1].Value);
								tmp_pname = this.strSqlProtection(regexResult.Groups[2].Value);
								if (this.changevipname(tmp_pname_old, tmp_pname)) {
									DebugWrite("[OnChat] [IngameAdmin] Change VIP Slot playername from " + this.strGreen(tmp_pname_old) + " to " + this.strGreen(tmp_pname) + " for all " + this.SettingGameType + " Server Groups in SQL database  (ingame admin " + tmpPlayername + ")", 2);
									this.PlayerSayMsg(tmpPlayername, "VIP changed from " + tmp_pname_old + " to " + tmp_pname + " for all " + this.SettingGameType + " Server Groups");
									changeOk = true;
								}
							}
						}
						if (!changeOk) {
							DebugWrite("[OnChat] [IngameAdmin] ^bERROR in !changevip syntax from " + tmpPlayername + ". Command is NOT valid: " + Msg + "^n", 4);
							this.PlayerSayMsg(tmpPlayername, "ERROR: Playername NOT changed. Check your syntax");
							this.PlayerSayMsg(tmpPlayername, "TYPE: !changevip <old playername> <new playername>  (e.g. !changevip SniperBen SniperBenni)");
						}
					} else {
						DebugWrite("[OnChat] [IngameAdmin] ^b" + tmpPlayername + "^n have NO admin privileges to use the command '!changevip'.  (Requires: Can Edit Reserved Slots List)", 3);
						this.PlayerSayMsg(tmpPlayername, "ERROR: You do NOT have admin privileges");
					}
				} else if (Regex.Match(Msg, @"^/vsm-addsemivip", RegexOptions.IgnoreCase).Success) {
					// remote command from other plugin via hidden admin say  ( e.g. /vsm-addsemivip <playername> )
					Match regexMatch = Regex.Match(Msg, @"/vsm-addsemivip\s+([^\s]+)$", RegexOptions.IgnoreCase);
					if (this.isAdmin(tmpPlayername)) {
						if (regexMatch.Success) {
							regexResult = regexMatch;
							if (regexResult.Groups[1].Value.Length > 2) {
								if ((this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) && (this.SettingSyncGs2Sql == "no  (remove from Gameserver)")) {
									DebugWrite("[OnChat] [IngameAdmin] ^Add^n Semi VIP: " + this.strBlack(regexResult.Groups[1].Value.Replace(" ","")) + ". Valid for current round / rejoin.  (ingame admin " + tmpPlayername + ")", 2);
									this.PlayerSayMsg(tmpPlayername, "Semi VIP: " + regexResult.Groups[1].Value.Replace(" ","") + " added (valid for current round / rejoin).");
									this.AddRoundSemiVip(regexResult.Groups[1].Value.Replace(" ",""));
								} else {
									this.PlayerSayMsg(tmpPlayername, "ERROR: Semi VIPs are disabled by Plugin settings.");
								}
							}
						} else {
							this.PlayerSayMsg(tmpPlayername, "ERROR: Check your syntax. Type: !addsemivip <playername>");
						}
					} else {
						DebugWrite("[OnChat] [IngameAdmin] ^b" + tmpPlayername + "^n have NO admin privileges to use the command '!addsemivip'.  (Requires: Can Edit Reserved Slots List)", 3);
						this.PlayerSayMsg(tmpPlayername, "ERROR: You do NOT have admin privileges");
					}
				}
			}));
			SQLWorker3.IsBackground = true;
			SQLWorker3.Name = "sqlworker3";
			SQLWorker3.Start();
		}
	}
}


//////////////////////
// SQL Functions
// CChatGUIDStatsLogger part by [GWC]XpKiller
//////////////////////

private void TableBuilder() {
	bool TableExist = false;
	bool ColumnGuidExist = false;
	bool TableCreated = false;
	bool TableUpdated = false;
	
	if (!this.SqlTableExist) {
		if (this.SqlLoginsOk()) {
			DebugWrite("[SQL-TableBuilder] Connecting to SQL and check database", 4);
			Thread SQLWorker4 = new Thread(new ThreadStart(delegate() {
				try {
					using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
						Con.Open();
						try {
							// check if table exist in SQL database
							string SQL = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='vsm_vips' AND table_schema='" + this.SettingStrSqlDatabase + "'";
							DebugWrite("[SQL-TableBuilder] [CheckExist] Connected to SQL. Check if table exist or not in SQL database. SQL COMMAND (MyCommand): " + SQL, 5);
							using (MySqlCommand MyCommand = new MySqlCommand(SQL)) {
								DataTable resultTable = this.SQLquery(MyCommand);
								if (resultTable.Rows != null) {
									DebugWrite("[SQL-TableBuilder] [CheckExist] Receive informations from SQL", 5);
									foreach (DataRow row in resultTable.Rows) {
										// reading sql
										if (row["COLUMN_NAME"].ToString() == "servergroup") {
											// yes, table 'vsm_vips' exist in SQL DB!!
											DebugWrite("[SQL-TableBuilder] [CheckExist] Table 'vsm_vips' exist in SQL database", 5);
											TableExist = true;
										} else if (row["COLUMN_NAME"].ToString() == "guid") {
											DebugWrite("[SQL-TableBuilder] [CheckExist] Column 'guid' exist in table 'vsm_vips' in SQL database", 5);
											ColumnGuidExist = true;
										}
									}
								} else {
									ConsoleError("[SQL-TableBuilder] [CheckExist] Table 'vsm_vips' NOT exist on your SQL Server");
								}
							}
						}
						catch (Exception c) {
							ConsoleError("[SQL-TableBuilder] [CheckExist] SQL Error (MyCommand): " + c);
							TableExist = false;
						}

						// create NEW table in SQL if not exist (first plugin start after installation)
						if ((!TableExist) || (!ColumnGuidExist)) {
							////////////////////////////////
							// start table bulider
							////////////////////////////////
							try {
								string SqlTableBuild = String.Empty;
								if (!TableExist) {
									SqlTableBuild = "CREATE TABLE IF NOT EXISTS `vsm_tBrowserSessions` (id INT NOT NULL auto_increment,sessionID VARCHAR(250) NOT NULL,time INT NOT NULL,lockedUntil INT NOT NULL DEFAULT 0,error VARCHAR(300),userID INT,tSessionID INT,PRIMARY KEY (id))";
									ConsoleWrite("[SQL-TableBuilder] [CreateTable] Plugin create NEW table 'vsm_tBrowserSessions' SQL database");
									DebugWrite("^b[SQL-TableBuilder] [CreateTable] Connected to SQL.^n SQL COMMAND (MyCom): " + SqlTableBuild, 4);
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
									}
									SqlTableBuild = "CREATE TABLE IF NOT EXISTS `vsm_tUser` (id int NOT NULL auto_increment,sessionID varchar(250),email varchar(100),password varchar(40),passwordDummy varchar(20),salt VARCHAR(5),rights INT(0),PRIMARY KEY (id))";
									ConsoleWrite("[SQL-TableBuilder] [CreateTable] Plugin create NEW table 'vsm_tUser' in SQL database");
									DebugWrite("^b[SQL-TableBuilder] [CreateTable] Connected to SQL.^n SQL COMMAND (MyCom): " + SqlTableBuild, 4);
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
									}
									SqlTableBuild = "CREATE TABLE IF NOT EXISTS `vsm_tFilter` (id int NOT NULL auto_increment,userID INT,server varchar(10),gruppe varchar(10),PRIMARY KEY (id))";
									ConsoleWrite("[SQL-TableBuilder] [CreateTable] Plugin create NEW table 'vsm_tFilter' in SQL database");
									DebugWrite("^b[SQL-TableBuilder] [CreateTable] Connected to SQL.^n SQL COMMAND (MyCom): " + SqlTableBuild, 4);
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
									}
									// SqlTableBuild = "INSERT INTO `vsm_tUser` (email, password, salt, rights) SELECT 'admin', '8a2c156a7d5c76b1f9e4c75353627a3a', '28g7d', 0 FROM DUAL WHERE 0 IN (SELECT COUNT(*) FROM `vsm_tUser`)";
									SqlTableBuild = "INSERT INTO vsm_tUser (email, password, salt, rights) SELECT * FROM (SELECT 'admin', '8a2c156a7d5c76b1f9e4c75353627a3a', '28g7d', 0) AS tmp WHERE NOT EXISTS ( SELECT email FROM vsm_tUser ) LIMIT 1";
									ConsoleWrite("[SQL-TableBuilder] [CreateWebAdmin] ^bLOGIN FOR FOR WEBSITE: user: admin , pw: admin^n");
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
									}
									SqlTableBuild = "CREATE TABLE IF NOT EXISTS `vsm_vips` (`ID` INT NOT NULL AUTO_INCREMENT ,`gametype` varchar(3) NOT NULL,`servergroup` varchar(2) NOT NULL,`playername` varchar(35) NULL DEFAULT NULL ,`timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,`status` varchar(8) NOT NULL,`admin` varchar(35) NULL DEFAULT NULL ,`comment` text NULL DEFAULT NULL, `guid` varchar(35) NULL DEFAULT NULL ,PRIMARY KEY (`ID`),UNIQUE KEY `servergroup` (`servergroup`,`playername`,`gametype`))ENGINE = InnoDB";
									ConsoleWrite("[SQL-TableBuilder] [CreateTable] Plugin create NEW table 'vsm_vips' in SQL database");
									DebugWrite("^b[SQL-TableBuilder] [CreateTable] Connected to SQL.^n SQL COMMAND (MyCom): " + SqlTableBuild, 4);
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
										MyCom.Connection.Close();
										TableCreated = true;
									}
								} else if (!ColumnGuidExist) {
									// update sql, add column 'guid' to table 'vsm_vips'
									SqlTableBuild = "ALTER TABLE `vsm_vips` ADD `guid` varchar(35) NULL DEFAULT NULL";
									ConsoleWrite("[SQL-TableBuilder] [UpdateTable] Add new column into table 'vsm_vips' in SQL database");
									DebugWrite("^b[SQL-TableBuilder] [UpdateTable] Connected to SQL.^n SQL COMMAND (MyCom): " + SqlTableBuild, 4);
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
									}
									SqlTableBuild = "ALTER TABLE `vsm_vips` CHANGE `timestamp` `timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP";
									using (MySqlCommand MyCom = new MySqlCommand(SqlTableBuild, Con)) {
										MyCom.ExecuteNonQuery();
										TableUpdated = true;
										MyCom.Connection.Close();
									}
								}
							}
							catch (MySqlException oe) {
								ConsoleError("[SQL-TableBuilder] [CreateTable] Error in Tablebuilder:");
								this.DisplayMySqlErrorCollection(oe);
								TableCreated = false;
							}
							catch (Exception c) {
								ConsoleError("[SQL-TableBuilder] [CreateTable] SQL Error (MyCom): " + c );
								TableCreated = false;
							}
							finally {
								DebugWrite("[[SQL-TableBuilder] Close SQL Connection (Con)", 5);
								Con.Close();
								if (TableCreated) {
									this.SqlTableExist = true;
									ConsoleWrite("[SQL-TableBuilder] ^b^2NEW table created successfully^0^n");
								} else if (TableCreated) {
									this.SqlTableExist = true;
									ConsoleWrite("[SQL-TableBuilder] [UpdateTable] ^b^2NEW column in table 'vsm_vips' created successfully^0^n");
								}
							}
						} else {
							if (Con.State == ConnectionState.Open) {
								DebugWrite("[[SQL-TableBuilder] Close SQL Connection (Con)", 5);
								Con.Close(); 
							}
							this.SqlTableExist = true;
						}
					}
				}
				catch (Exception c) {
					ConsoleError("[SQL-TableBuilder] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c );
				}
			}));
			SQLWorker4.IsBackground = true;
			SQLWorker4.Name = "sqlworker4";
			SQLWorker4.Start();
		}
	}
}

public void SyncVipList() {
	if (!this.fIsEnabled) { return; }
	bool SyncGameserver = false;
	bool SqlConOK = false;
	bool tmp_gotSql = false;
	Dictionary<String, int> tmp_sql_vips_active = new Dictionary<string, int>();
	Dictionary<String, String> tmp_sqlguid = new Dictionary<string, string>();
	Dictionary<String, String> tmp_GuidsActive = new Dictionary<string, string>();
	List<string> tmp_sql_vips2add = new List<string>();
	List<string> tmp_sql_vips2del = new List<string>();
	List<string> tmp_sql_vips2inactive = new List<string>();
	List<string> tmp_sql_vips_INVALID = new List<string>();
	List<string> tmp_gs_vips = this.vipsGS;
	int tmp_intpTimestamp = 0;
	int tmp_intServerTimestamp = intUtcTimestamp(); // utc time
	int tmp_adders = 0;
	string SQL = String.Empty;
	string tmp_newStatus = String.Empty;
	string tmp_pName = String.Empty;
	string tmp_pStatus = String.Empty;
	string tmp_pGuid = String.Empty;
	string tmp_queryGuid = String.Empty;

	this.AdkatsRunning = GetRegisteredCommands().Any(command => command.RegisteredClassname == "AdKats" && command.RegisteredMethodName == "PluginEnabled");

	if (((DateTime.UtcNow - this.LastSync).TotalSeconds) < 10) { return; }
	DebugWrite("[SyncVipList] ^bSync started^n", 5);
	if ((!this.gotVipsGS) || (!this.firstCheck)) {
		DebugWrite("[SyncVipList] ^bSync canceled!^n Still loading VIPs from Gameserver", 3);
		return;
	}
	this.LastSync = DateTime.UtcNow;

	Thread SQLWorker = new Thread(new ThreadStart(delegate() {
		/////////////////////
		// Connect to SQL 
		/////////////////////
		DebugWrite("[SyncVipList] [SqlConnection] Try to sync VIP players from SQL database and Gameserver", 5);
		this.TableBuilder();
		if (!this.SqlTableExist) {
			ConsoleError("[SyncVipList] Sync canceled! (SQL Error)");
			return;
		}

		if (this.SqlLoginsOk()) {
			try {
				 using (MySqlConnection Connection = new MySqlConnection(this.SqlLogin())) {
					Connection.Open();
					try {
						if (Connection.State == ConnectionState.Open) {
							try {
								SqlConOK = true;
								if (this.EAGuidTracking == enumBoolYesNo.Yes) { tmp_queryGuid = "`guid`, "; }
								SQL = "SELECT `playername`, `status`, " + tmp_queryGuid + "TIMESTAMPDIFF(SECOND,'1970-01-01',timestamp) AS timestamp FROM `vsm_vips` WHERE gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "' AND status in ('active', 'adding', 'expired', 'deleting', 'removing') LIMIT 800";
								using (MySqlCommand MyCommand = new MySqlCommand(SQL)) {
									DataTable resultTable = this.SQLquery(MyCommand);
									if (resultTable.Rows != null) {
										DebugWrite("[SyncVipList] [SqlConnection] OK! Connected to SQL database. Read playerlist with [gametype]: " + this.SettingGameType + " and [servergroup]: " + this.SettingStrSqlServerGroup + " and [status]: active, adding, expired, deleting, removing). SQL COMMAND (MyCommand): " + SQL, 5);
										if (resultTable.Rows.Count >= 750) { DebugWrite("^b^8WARNING^0^n^8 This Gameserver (Server Group) have more than 700 VIPs (status = 'active / expired')! ^b^8LIMIT:^n^8 Maximal 800 VIPs for each Server Group!  IMPORTANT: YOU have to change the setting 'Auto Database Cleaner' or remove some VIPs with current status 'active' or 'expired' for this Server Group manually (go to the website and change the status from 'expired' to 'inactive' for some players).^0", 2); }
										this.vipsExpired.Clear();
										tmp_gotSql = true;
										foreach (DataRow row in resultTable.Rows) {
											// reading sql, create tmp lists
											tmp_pName = row["playername"].ToString();
											tmp_pStatus = row["status"].ToString();
											tmp_intpTimestamp = Convert.ToInt32(row["timestamp"]);
											if (tmp_pStatus == "active") {
												tmp_sql_vips_active.Add(tmp_pName, tmp_intpTimestamp);
											} else if (tmp_pStatus == "deleting") {
												tmp_sql_vips2del.Add(tmp_pName);
											} else if (tmp_pStatus == "removing") {
												tmp_sql_vips2inactive.Add(tmp_pName);
											} else if (tmp_pStatus == "adding") {
												tmp_sql_vips_active.Add(tmp_pName, tmp_intpTimestamp);
												tmp_sql_vips2add.Add(tmp_pName);
											} else if (tmp_pStatus == "expired") {
												this.vipsExpired.Add(tmp_pName);
											}
											if (this.EAGuidTracking == enumBoolYesNo.Yes) {
												if (row["guid"].ToString().StartsWith("EA_")) { tmp_sqlguid.Add(tmp_pName, row["guid"].ToString()); }
											}
										}
									} else {
										DebugWrite("[SyncVipList] [SqlConnection] ERROR: Can not read SQL informations", 3);
										this.SqlTableExist = false;
									}
									MyCommand.Connection.Close();
									DebugWrite("[SyncVipList] [SqlConnection] Close SQL Connection (MyCommand)", 5);
								}
							}
							catch (Exception e) {
								ConsoleError("[SyncVipList] SQL Connection Error: " + e);
								tmp_gotSql = false;
							}

							/////////////////////
							// Check vip player from TMP-SQL list and Gamerserver list
							/////////////////////
							if (tmp_gotSql) {
								DebugWrite("[SyncVipList] [SqlListActive] Parse VIP player list from SQL. Check VIP players remaining time. (Current UTC Timestmp in seconds from Gameserver: " + tmp_intServerTimestamp.ToString() + ")", 5);
								this.SqlVipsActive.Clear();
								this.SqlVipsActiveNamesOnly.Clear();
								foreach (KeyValuePair<String, int> tmp_sqlvips in tmp_sql_vips_active) {
									DebugWrite("[SyncVipList] [SqlListActive] Checking VIP player " + this.strGreen(tmp_sqlvips.Key) + " from SQL database", 5);
									if (tmp_sqlvips.Value > tmp_intServerTimestamp) {
										// player timestamp ok
										this.SqlVipsActive.Add(tmp_sqlvips.Key, tmp_sqlvips.Value);
										this.SqlVipsActiveNamesOnly.Add(tmp_sqlvips.Key);
										if (tmp_sqlguid.ContainsKey(tmp_sqlvips.Key)) { tmp_GuidsActive.Add(tmp_sqlguid[tmp_sqlvips.Key], tmp_sqlvips.Key); }
										if (tmp_sql_vips2add.Contains(tmp_sqlvips.Key)) {
											DebugWrite("[SyncVipList] [SqlListActive] Receive NEW VIP player " + this.strGreen(tmp_sqlvips.Key) + " with valid VIP Slot time from SQL database", 3);
											tmp_adders++;
											if (this.vipsExpired.Contains(tmp_sqlvips.Key)) {
												this.vipsExpired.Remove(tmp_sqlvips.Key);
												this.vipmsg.Remove(tmp_sqlvips.Key);
											}
											// set vip player status to "active" in SQL
											SQL = "UPDATE `vsm_vips` SET status='active' WHERE playername='" + tmp_sqlvips.Key + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
											using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
												DebugWrite("[SyncVipList] [SqlListActive] Set VIP player status to 'active' in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
												MyCom.ExecuteNonQuery();
											}
										}
										if (!tmp_gs_vips.Contains(tmp_sqlvips.Key)) {
											if (this.RoundTempVips.Contains(tmp_sqlvips.Key)) {
												DebugWrite("[SyncVipList] [SqlListActive] [AggressiveJoinAbuse] Valid VIP player " + this.strGreen(tmp_sqlvips.Key) + " blocked to use 'Aggressive Join Kick' till next round.", 5);
											} else {
												if (tmp_gs_vips.Contains(tmp_sqlvips.Key, StringComparer.CurrentCultureIgnoreCase)) {
													// remove from gs, case sensitive problem detected
													tmp_gs_vips.Remove(tmp_sqlvips.Key);
													this.ProconVipRemove(tmp_sqlvips.Key);
													this.ProconVipSave();
													this.ProconVipList();
													DebugWrite("[SyncVipList] [SqlListActive] Change VIP Slot playername to " + this.strGreen(tmp_sqlvips.Key) + " on Gameserver (case sensitive).", 3);
												}
												// add player to gameserver
												DebugWrite("[SyncVipList] [SqlListActive] ^bAdd^n VIP Slot: " + this.strGreen(tmp_sqlvips.Key) + " from SQL database to Gameserver", 2);
												this.ProconVipAdd(tmp_sqlvips.Key);
												tmp_gs_vips.Add(tmp_sqlvips.Key);
												SyncGameserver = true;
											}
										} else {
											DebugWrite("[SyncVipList] [SqlListActive] Valid VIP player " + this.strGreen(tmp_sqlvips.Key) + " from SQL database already on Gameserver", 5);
										}
										
									} else {
										// vip expired, remove from gameserver
										DebugWrite("[SyncVipList] [SqlListExpired] ^bExpired^n VIP Slot: " + this.strGreen(tmp_sqlvips.Key) , 3);
										this.vipsExpired.Add(tmp_sqlvips.Key);
										this.vipmsg.Remove(tmp_sqlvips.Key);
										tmp_sql_vips_INVALID.Add(tmp_sqlvips.Key); // ????
										// set SQL status to "expired" or "inactive"
										tmp_newStatus = "inactive";
										if (this.SettingVipExp == enumBoolYesNo.Yes) {
											tmp_newStatus = "expired";
											this.vipsExpired.Remove(tmp_sqlvips.Key);
										}
										SQL = "UPDATE `vsm_vips` SET status='" + tmp_newStatus + "', guid=NULL WHERE playername='" + tmp_sqlvips.Key + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
										using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
											DebugWrite("[SyncVipList] [SqlListExpired] Set VIP player status to '" + tmp_newStatus + "' in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
											MyCom.ExecuteNonQuery();
										}
										if (tmp_gs_vips.Contains(tmp_sqlvips.Key)) {
											DebugWrite("[SyncVipList] [SqlListExpired] ^bRemove^n expired VIP Slot: " + this.strGreen(tmp_sqlvips.Key) + " from Gameserver", 2);
											this.ProconVipRemove(tmp_sqlvips.Key);
											tmp_gs_vips.Remove(tmp_sqlvips.Key);
											SyncGameserver = true;
										}
										if (this.RoundTempVips.Contains(tmp_sqlvips.Key)) { this.RoundTempVips.Remove(tmp_sqlvips.Key); }
									}
								}
								
								// check ea guids from vips
								if (this.EAGuidTracking == enumBoolYesNo.Yes) {
									foreach (KeyValuePair<String, String> tmp_guid in this.Guid2Check) {
										if (tmp_GuidsActive.ContainsKey(tmp_guid.Key)) {
											if (tmp_GuidsActive[tmp_guid.Key] == tmp_guid.Value) {
												DebugWrite("[SyncVipList] [EAGuidTracking] VIP " + this.strGreen(tmp_guid.Value) + " already linked to EA GUID: " + tmp_guid.Key , 5);
											} else {
												// vip changed his playername
												DebugWrite("[SyncVipList] [EAGuidTracking] VIP " + tmp_GuidsActive[tmp_guid.Key] + " (" + tmp_guid.Key + ") changed his playername to " + this.strGreen(tmp_guid.Value) + ". Updating new playername in SQL database." , 2);
												if ((this.SettingAdkatsLog == enumBoolYesNo.Yes) && (this.SettingAdkatsLogVipChanged == enumBoolYesNo.Yes)) { this.AdkatsPlayerLog(tmp_guid.Value, "VIP Slot updated to new playername " + tmp_guid.Value); }
												SQL = "DELETE FROM `vsm_vips` WHERE `vsm_vips`.playername='" + tmp_guid.Value + "' AND gametype = '" + this.SettingGameType + "' AND status != 'active'";
												using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
													DebugWrite("[SyncVipList] [EAGuidTracking] Remove duplicate entries in SQL. SQL COMMAND (MyCom): " + SQL, 5);
													MyCom.ExecuteNonQuery();
												}
												SQL = "UPDATE IGNORE `vsm_vips` SET playername='" + tmp_guid.Value + "' WHERE playername='" + tmp_GuidsActive[tmp_guid.Key] + "' AND gametype = '" + this.SettingGameType + "'";
												using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
													DebugWrite("[SyncVipList] [EAGuidTracking] Change VIP Slot playername from " + tmp_GuidsActive[tmp_guid.Key] + " to " + this.strGreen(tmp_guid.Value) + " for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
													MyCom.ExecuteNonQuery();
													this.PlayerSayMsg(tmp_guid.Value, tmp_guid.Value + " your VIP Slot will be changed from playername " + tmp_GuidsActive[tmp_guid.Key] + " to " + this.strGreen(tmp_guid.Value) + " in few minutes.");
													this.PlayerYellMsg(tmp_guid.Value, tmp_guid.Value + " your VIP Slot will be changed to playername " + this.strGreen(tmp_guid.Value));
													this.ProconVipRemove(tmp_GuidsActive[tmp_guid.Key]);
													SyncGameserver = true;
													if (tmp_gs_vips.Contains(tmp_GuidsActive[tmp_guid.Key])) { tmp_gs_vips.Remove(tmp_GuidsActive[tmp_guid.Key]); }
													if(!tmp_sql_vips_INVALID.Contains(tmp_GuidsActive[tmp_guid.Key])) { tmp_sql_vips_INVALID.Add(tmp_GuidsActive[tmp_guid.Key]); }
												}
											}
										} else if (this.SqlVipsActive.ContainsKey(tmp_guid.Value)) {
											// link guid to vip playername
											DebugWrite("[SyncVipList] [EAGuidTracking] VIP " + this.strGreen(tmp_guid.Value) + " is now linked to EA GUID: " + tmp_guid.Key, 4);
											SQL = "UPDATE `vsm_vips` SET guid='" + tmp_guid.Key + "' WHERE playername='" + tmp_guid.Value + "' AND gametype = '" + this.SettingGameType + "'";
											using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
												DebugWrite("[SyncVipList] [EAGuidTracking] VIP " + this.strGreen(tmp_guid.Value) + " linked to EA GUID " + tmp_guid.Key + " for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
												MyCom.ExecuteNonQuery();
											}
										} else {
											DebugWrite("[SyncVipList] [EAGuidTracking] EA GUID: " + tmp_guid.Key + " from " + tmp_guid.Value + " is not linked." , 5);
										}
									}
									this.Guid2Check.Clear();
								}

								// SQL set expired slots to inactive when "notify vip slot expired" is disabled in plugin settings
								if (this.SettingVipExp == enumBoolYesNo.No) {
									if (this.vipsExpired.Count > 0) {
										SQL = "UPDATE `vsm_vips` SET status='inactive', guid=NULL WHERE status = 'expired' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
										using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
											DebugWrite("[SyncVipList] [SqlListExpired] Set SQL database status to 'inactive' from all expired VIP players for this Server Group. SQL COMMAND (MyCom): " + SQL, 5);
											MyCom.ExecuteNonQuery();
											MyCom.Connection.Close();
										}
										this.vipsExpired.Clear();
									}
								}

								// list to remove from gameserver, del from sql
								foreach (string vipplayer in tmp_sql_vips2del) {
									// DEL from SQL
									SQL = "DELETE FROM `vsm_vips` WHERE `vsm_vips`.playername='" + vipplayer + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "' AND status = 'deleting'";
									using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
										DebugWrite("[SyncVipList] [SqlListDeleting] ^bDELETE^n player in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
										MyCom.ExecuteNonQuery();
										MyCom.Connection.Close();
									}
									if (tmp_gs_vips.Contains(vipplayer)) {
										this.ProconVipRemove(vipplayer);
										DebugWrite("[SyncVipList] [SqlListDeleting] ^bDELETE^n player " + this.strGreen(vipplayer) + " from Gameserver", 5);
										tmp_gs_vips.Remove(vipplayer);
										SyncGameserver = true;
									} 
									if (this.RoundTempVips.Contains(vipplayer)) { this.RoundTempVips.Remove(vipplayer); }
								}

								// list to remove from gameserver, set sql status to 'inactive'
								if (tmp_sql_vips2inactive.Count > 0) {
									// update SQL player status
									SQL = "UPDATE `vsm_vips` SET status='inactive', guid=NULL WHERE status = 'removing' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
									using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
										DebugWrite("[SyncVipList] [SqlListDeleting] Set players status from 'removing' to 'inactive' in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
										MyCom.ExecuteNonQuery();
										MyCom.Connection.Close();
									}
									foreach (string vipplayer in tmp_sql_vips2inactive) {
										if (tmp_gs_vips.Contains(vipplayer)) {
											this.ProconVipRemove(vipplayer);
											DebugWrite("[SyncVipList] [SqlListRemoving] ^bREMOVE^n player " + this.strGreen(vipplayer) + " from Gameserver", 4);
											tmp_gs_vips.Remove(vipplayer);
											SyncGameserver = true;
										}
										if (this.RoundTempVips.Contains(vipplayer)) { this.RoundTempVips.Remove(vipplayer); }
									}
								}

								// check vips from gameserver
								string tmp_imp_status = "inactive";
								string tmp_imp_days = "30";
								string SqlPartTimetamp = "NULL";
								string SqlPartTimetamp2 = "NULL";
								if (this.SettingSyncGs2Sql.Contains("inactive")) {
									tmp_imp_status = "inactive";
									SqlPartTimetamp = "NULL";
									SqlPartTimetamp2 = "NULL";
								} else if (this.SettingSyncGs2Sql.Contains("permanent")) {
									tmp_imp_status = "active";
									SqlPartTimetamp = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '7' YEAR)";
									SqlPartTimetamp2 = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '7' YEAR)";
								} else if (this.SettingSyncGs2Sql.Contains("for ")) {
									tmp_imp_days = this.SettingSyncGs2Sql.Replace("yes  (for ", "").Replace(" days)", "").Replace(" ", "");
									tmp_imp_status = "active";
									SqlPartTimetamp = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + tmp_imp_days + "' DAY)";
									SqlPartTimetamp2 = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + tmp_imp_days + "' DAY)";
								} else if (this.SettingSyncGs2Sql.Contains("Plugin installation")) {
									tmp_imp_days = "30";
									tmp_imp_status = "active";
									SqlPartTimetamp = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + tmp_imp_days + "' DAY)";
									SqlPartTimetamp2 = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + tmp_imp_days + "' DAY)";
								}
								foreach (string vipplayer in tmp_gs_vips) {
									if ((!tmp_sql_vips_active.ContainsKey(vipplayer)) && (!tmp_sql_vips_INVALID.Contains(vipplayer)) &&  (!this.RoundTempVips.Contains(vipplayer))) {
										if (this.SettingSyncGs2Sql.Contains("remove")) {
											DebugWrite("[SyncVipList] [GameserverList] ^bRemove^n player " + this.strGreen(vipplayer) + " from Gameserver (Plugin setting: 'Import NEW VIPS from Gameserver to SQL: NO')", 4);
											this.ProconVipRemove(vipplayer);
											SyncGameserver = true;
										} else if (this.SettingSyncGs2Sql.Contains("ignore")) {
											DebugWrite("[SyncVipList] [GameserverList] " + this.strGreen(vipplayer) + " found on Gameserver but NOT in SQL database", 5);
										} else {
											DebugWrite("[SyncVipList] [GameserverList] ^bAdd^n NEW VIP: " + this.strGreen(vipplayer) + " from Gameserver to SQL database ^n" + this.SettingSyncGs2Sql.Replace("yes  ", "").Replace("no  ", "").Replace(" first Plugin installation only", "") + "^n", 2);
											// add new player from gameserver to SQL
											tmp_adders++;
											SQL = "INSERT INTO `vsm_vips` (`gametype`, `servergroup`, `playername`, `timestamp`, `status`, `admin`) VALUES ('" + this.SettingGameType + "', '" + this.SettingStrSqlServerGroup + "', '" + vipplayer + "', " + SqlPartTimetamp + ", '" + tmp_imp_status + "', 'Plugin') ON DUPLICATE KEY UPDATE status='" + tmp_imp_status + "', timestamp=" + SqlPartTimetamp2 + ", admin='Plugin'";
											using (MySqlCommand MyCom = new MySqlCommand(SQL, Connection)) {
												DebugWrite("[SyncVipList] [GameserverList] ^bAdd^n NEW VIP to SQL database. SQL COMMAND (MyCom): " + SQL, 5);
												MyCom.ExecuteNonQuery();
											}
											if (tmp_imp_status == "inactive") {
												// remove from gameserver
												DebugWrite("[SyncVipList] [GameserverList] ^bRemove^n player " + this.strGreen(vipplayer) + " from Gameserver (Status: inactive)", 4);
												tmp_adders--;
												this.ProconVipRemove(vipplayer);
												SyncGameserver = true;
											}
										}
									}
								}
							}
						} else {
							DebugWrite("[SyncVipList] Can NOT connect to SQL", 5);
							SqlConOK = false;
							this.SqlTableExist = false;
						}
					}
					catch (Exception e) {
						ConsoleError("[SyncVipList] SQL Connection Error: " + e);
						SqlConOK = false;
						this.SqlTableExist = false;
					}
					finally {
						Connection.Close();
						DebugWrite("[SyncVipList] Close SQL Connection (Connection)", 5);
					}
				}
			}
			catch (Exception e) {
				ConsoleError("[SyncVipList] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + e);
				SqlConOK = false;
				this.SqlTableExist = false;
			}
		} else {
			SqlConOK = false;
		}

		if (SyncGameserver) {
			DebugWrite("[SyncVipList] Refresh VIPs on Gameserver", 4);
			this.ProconVipSave();
			this.ProconVipList();
		}

		if (!SqlConOK) {
			DebugWrite("[SyncVipList] Sync stopped (SQL Error)", 2);
			this.SqlTableExist = false;
			return;
		} else {
			if (this.SettingSyncGs2Sql.Contains("Plugin installation")) {
				this.SettingSyncGs2Sql = "no  (remove from Gameserver)";
				this.ExecuteCommand("procon.protected.plugins.setVariable", "VipSlotManager", "Import NEW VIPS from Gameserver to SQL", "no  (remove from Gameserver)");
				ConsoleWrite("[SyncVipList] ^bInstallation finished:^n Plugin setting 'Import new VIP from GS to SQL' set to 'no (remove)");
			}
		}

		DebugWrite("[SyncVipList] ^bSync finished^n", 5);
		DebugWrite("[SyncVipList] Valid VIPs from SQL database: " + (this.SqlVipsActive.Count + tmp_adders).ToString() + "  ---  VIP Slot Expired: " + this.vipsExpired.Count.ToString(), 5);
		if (this.SqlVipsActive.Count >= 500) { DebugWrite("^b^8WARNING^0^n^8 This Gameserver (Server Group) have more than 500 valid VIPs! BF Gameservers can NOT handle more than 500 VIPs.", 2); }

		if (this.isForceSync) {
			this.isForceSync = false;
			if (this.SettingProconRulzIni == enumBoolYesNo.Yes) { this.FileWriteProconRulz(); }
			ConsoleWrite("[ForceSync] ^bForce Sync finished:^n Valid VIP players from SQL database: " + (this.SqlVipsActive.Count + tmp_adders).ToString() + "  ---  VIP Slot Expired: " + this.vipsExpired.Count.ToString());
			if ((this.SettingAggressiveJoinKickAbuseMax > 1) && (this.RoundTempVips.Count > 0)) { ConsoleWrite("[ForceSync] [AggressiveJoinAbuseProtection] Aggressive Join Kick on current round blocked for " + this.RoundTempVips.Count.ToString() + " valid VIPs: " + String.Join(", ",  this.RoundTempVips.ToArray())); }
		}
	}));
	SQLWorker.IsBackground = true;
	SQLWorker.Name = "sqlworker";
	SQLWorker.Start();
}

private bool addvip(String playername, String days, String admin) {
	bool SqlConOK = false;
	if ((playername.Length >= 3) && (days.Length >= 1) && (admin.Length >= 3)) {
		this.TableBuilder();
		if (this.SqlTableExist) {
			try {
				using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
					Con.Open();
					try {
						if (Con.State == ConnectionState.Open) {
							string SqlPartTimetamp = String.Empty;
							if (days.Contains("+")) {
								SqlPartTimetamp = "DATE_ADD(IF(TIMESTAMPDIFF(DAY,timestamp,UTC_TIMESTAMP()) >= 0, UTC_TIMESTAMP(), timestamp),INTERVAL '" + days.Replace("+", "") + "' DAY)";
							} else {
								SqlPartTimetamp = "DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + days.Replace("+", "") + "' DAY)";
							}

							string SQL = "INSERT INTO `vsm_vips` (`gametype`, `servergroup`, `playername`, `timestamp`, `status`, `admin`) VALUES ('" + this.SettingGameType + "', '" + this.SettingStrSqlServerGroup + "', '" + playername + "', DATE_ADD(UTC_TIMESTAMP(),INTERVAL '" + days.Replace("+", "") + "' DAY), 'active', '" + admin + "') ON DUPLICATE KEY UPDATE status='active', timestamp=" + SqlPartTimetamp + ", admin='" + admin + "'";
							using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
								MyCom.ExecuteNonQuery();
								SqlConOK = true;
								MyCom.Connection.Close();
								this.SyncCounter = 999;
							}
						} else {
							DebugWrite("[SQL-addvip] Can NOT connect to SQL", 5);
						}
					}
					catch (Exception c) {
						ConsoleError("[SQL-addvip] SQL Error (Con): " + c);
						SqlConOK = false;
						this.SqlTableExist = false;
					}
					finally {
						Con.Close();
						DebugWrite("[SQL-addvip] Close SQL Connection (Con)", 5);
					}
				}
			}
			catch (Exception c) {
				ConsoleError("[SQL-addvip] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c);
				SqlConOK = false;
				this.SqlTableExist = false;
			}
		}
	}
	if (!SqlConOK) {
		DebugWrite("[SQL-addvip] SQL Connection Error. Can not write in SQL", 2);
		this.SqlTableExist = false;
		return false;
	} else {
		if (this.vipmsg.Contains(playername)) { this.vipmsg.Remove(playername);}
		if (this.vipsExpired.Contains(playername)) { this.vipsExpired.Remove(playername);}
		return true;
	}
}

private bool removevip(String playername) {
	bool SqlConOK = false;
	if (playername.Length >= 3) {
		this.TableBuilder();
		if (this.SqlTableExist) {
			try {
				using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
					Con.Open();
					try {
						if (Con.State == ConnectionState.Open) {
							string SQL = "UPDATE `vsm_vips` SET status='inactive', guid=NULL WHERE playername='" + playername + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
							DebugWrite("[SQL-removevip] Set player status to 'inactive' in SQL database. SQL COMMAND (MyCom): " + SQL, 4);
							using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
								MyCom.ExecuteNonQuery();
								SqlConOK = true;
								MyCom.Connection.Close();
								this.SyncCounter = 999;
							}
						} else {
							DebugWrite("[SQL-removevip] Can NOT connect to SQL", 5);
							this.SqlTableExist = false;
						}
					}
					catch (Exception c) {
						ConsoleError("[SQL-removevip] SQL Error (Con): " + c);
						SqlConOK = false;
						this.SqlTableExist = false;
					}
					finally {
						Con.Close();
						DebugWrite("[SQL-removevip] Close SQL Connection (Con)", 5);
					}
				}
			}
			catch (Exception c) {
				ConsoleError("[removevip] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c);
				SqlConOK = false;
				this.SqlTableExist = false;
			}
		}
	}
	if (!SqlConOK) {
		DebugWrite("[SQL-removevip] SQL Connection Error. Can not write in SQL", 2);
		return false;
		this.SqlTableExist = false;
	} else {
		// remove player from gameserver list
		DebugWrite("[SQL-removevip] ^bRemove^n" + this.strGreen(playername) + " from Gameserver", 4);
		if (this.vipmsg.Contains(playername)) { this.vipmsg.Remove(playername); }
		if (this.vipsExpired.Contains(playername)) { this.vipsExpired.Remove(playername); }
		this.ProconVipRemove(playername);
		this.ProconVipSave();
		this.ProconVipList();
		return true;
	}
}

private bool changevipname(String oldplayername, String playername) {
	bool SqlConOK = false;
	if ((playername.Length >= 3) && (oldplayername.Length >= 3)) {
		this.TableBuilder();
		if (this.SqlTableExist) {
			try {
				using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
					Con.Open();
					try {
						if (Con.State == ConnectionState.Open) {
							string SQL = "DELETE FROM `vsm_vips` WHERE `vsm_vips`.playername='" + playername + "' AND gametype = '" + this.SettingGameType + "' AND status != 'active'";
							using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
								DebugWrite("[SQL-changevip] Remove duplicate entries in SQL. SQL COMMAND (MyCom): " + SQL, 5);
								MyCom.ExecuteNonQuery();
							}
							DebugWrite("[SQL-changevip] Change VIP Slot playername from " + this.strGreen(oldplayername) + " to " + this.strGreen(playername) + " for all " + this.strBlack(this.SettingGameType) + " Server Groups in SQL database.", 4);
							SQL = "UPDATE IGNORE `vsm_vips` SET playername='" + playername + "', guid=NULL WHERE playername='" + oldplayername + "' AND gametype = '" + this.SettingGameType + "'";
							using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
								MyCom.ExecuteNonQuery();
								SqlConOK = true;
								MyCom.Connection.Close();
								this.SyncCounter = 999;
							}
						} else {
							DebugWrite("[SQL-changevip] Can NOT connect to SQL", 5);
							this.SqlTableExist = false;
						}
					}
					catch (Exception c) {
						ConsoleError("[SQL-changevip] SQL Error (Con): " + c);
						SqlConOK = false;
						this.SqlTableExist = false;
					}
					finally {
						Con.Close();
						DebugWrite("[SQL-changevip] Close SQL Connection (Con)", 5);
					}
				}
			}
			catch (Exception c) {
				ConsoleError("[changevip] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c);
				SqlConOK = false;
				this.SqlTableExist = false;
			}
		}
	}
	if (!SqlConOK) {
		DebugWrite("[SQL-changevip] SQL Connection Error. Can not write in SQL", 2);
		return false;
		this.SqlTableExist = false;
	} else {
		DebugWrite("[SQL-changevip] ^bRemove^n VIP Slot: " + oldplayername + " from Gameserver", 5);
		this.ProconVipRemove(oldplayername);
		this.ProconVipSave();
		this.ProconVipList();
		return true;
	}
}

private int checkvip(String playername) {
	int erg = -1;
	if (playername.Length >= 3) {
		this.TableBuilder();
		if (this.SqlTableExist) {
			try {
				using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
					Con.Open();
					try {
						string SQL = "SELECT TIMESTAMPDIFF(SECOND,'1970-01-01',timestamp) AS timestamp FROM `vsm_vips` WHERE playername = '" + playername + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
						DebugWrite("[CheckVIP] Connected to SQL. SQL COMMAND (MyCommand): " + SQL, 5);
						using (MySqlCommand MyCommand = new MySqlCommand(SQL)) {
							DataTable resultTable = this.SQLquery(MyCommand);
							if (resultTable.Rows != null) {
								DebugWrite("[CheckVIP] Receive informations from SQL", 5);
								foreach (DataRow row in resultTable.Rows) {
									// reading sql
									erg = Convert.ToInt32(row["timestamp"].ToString());
								}
							} else {
								ConsoleError("[CheckVIP] ERROR: Can NOT receive informations from SQL.");
								if (Con.State == ConnectionState.Open) {
									DebugWrite("[CheckVIP] Close SQL Connection (Con)", 5);
									Con.Close();
								}
							}
						}
					}
					catch (Exception c) {
						ConsoleError("[CheckVIP] Error, can not read from SQL database (MyCommand): " + c);
					}
					if (Con.State == ConnectionState.Open) {
						DebugWrite("[CheckVIP] Close SQL Connection (Con)", 5);
						Con.Close();
					}
				}
			}
			catch (Exception c) {
				ConsoleError("[CheckVIP] Error (Con): " + c );
			}
		}
	}
	return erg;
}

private bool SetSqlpStatus(String playername, String newstatus) {
	bool SqlConOK = false;
	this.TableBuilder();
	if (this.SqlTableExist) {
		try {
			using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
				Con.Open();
				try {
					if (Con.State == ConnectionState.Open) {
						string SQL = "UPDATE `vsm_vips` SET status='" + newstatus + "' WHERE playername='" + playername + "' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
						using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
							MyCom.ExecuteNonQuery();
							SqlConOK = true;
							MyCom.Connection.Close();
						}
					} else {
						DebugWrite("[SetSqlStatus] Can NOT connect to SQL", 5);
					}
				}
				catch (Exception c) {
					ConsoleError("[SetSqlStatus] SQL Error (Con): " + c);
					SqlConOK = false;
					this.SqlTableExist = false;
				}
				finally {
					Con.Close();
					DebugWrite("[SetSqlStatus] Close SQL Connection (Con)", 5);
				}
			}
		}
		catch (Exception c) {
			ConsoleError("[SetSqlStatus] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c);
			SqlConOK = false;
			this.SqlTableExist = false;
		}
	}
	if (!SqlConOK) {
		DebugWrite("[SetSqlStatus] SQL Connection Error. Can not write in SQL", 2);
		return false;
		this.SqlTableExist = false;
	} else {
		return true;
	}
}

public void DatabaseCleaner() {
	if ((!this.fIsEnabled) || (!this.firstCheck)) { return; }
	bool SqlConOK = false;
	this.TableBuilder();
	if (this.SqlTableExist) {
		try {
			using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
				Con.Open();
				try {
					if (Con.State == ConnectionState.Open) {
						string SQL = "DELETE FROM `vsm_vips` WHERE `vsm_vips`.status = 'inactive' AND (TIMESTAMPDIFF(DAY, timestamp, UTC_TIMESTAMP()) > 365) AND comment IS NULL AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
						DebugWrite("[SqlAutoDatabaseCleaner] Clean up database", 4);
						DebugWrite("[SqlAutoDatabaseCleaner] set 'expired' VIPs without join event to 'inactive' in SQL database. SQL COMMAND (MyCom): " + SQL, 5);
						using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
							MyCom.ExecuteNonQuery();
						}
						SQL = "UPDATE `vsm_vips` SET status='inactive', guid=NULL WHERE (TIMESTAMPDIFF(DAY, timestamp, UTC_TIMESTAMP()) >= " + this.SettingDBCleaner.ToString() + ") AND status = 'expired' AND gametype = '" + this.SettingGameType + "' AND servergroup = '" + this.SettingStrSqlServerGroup + "'";
						using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
							MyCom.ExecuteNonQuery();
						}
						SQL = "UPDATE `vsm_vips` SET timestamp=UTC_TIMESTAMP() WHERE STR_TO_DATE(`timestamp`, '%Y') = '0000-00-00' OR `timestamp` IS NULL";
						using (MySqlCommand MyCom = new MySqlCommand(SQL, Con)) {
							MyCom.ExecuteNonQuery();
							SqlConOK = true;
							this.SyncCounter = 999;
							MyCom.Connection.Close();
						}
					} else {
						DebugWrite("[SQL-Auto-Database-Cleaner] Can NOT connect to SQL", 5);
						this.SqlTableExist = false;
					}
				}
				catch (Exception c) {
					ConsoleError("[SqlAutoDatabaseCleaner] SQL Error (Con): " + c);
					SqlConOK = false;
					this.SqlTableExist = false;
				}
				finally {
					Con.Close();
					DebugWrite("[SqlAutoDatabaseCleaner] Close SQL Connection (Con)", 5);
				}
			}
		}
		catch (Exception c) {
			ConsoleError("[SQL-Auto-Database-Cleaner] ERROR: CAN NOT CONNECT TO SQL SERVER. Error (Con): " + c);
			SqlConOK = false;
			this.SqlTableExist = false;
		}
	}

	if (!SqlConOK) {
		DebugWrite("[SqlAutoDatabaseCleaner] SQL Connection Error. Can not write in SQL", 2);
		this.SqlTableExist = false;
	} 
}

private DataTable SQLquery(MySqlCommand selectQuery) {
	DataTable MyDataTable = new DataTable();
	try {
		if (selectQuery == null) {
			ConsoleWrite("SQLquery: selectQuery is null");
			return MyDataTable;
		} else if (selectQuery.CommandText.Equals(String.Empty) == true) {
			DebugWrite("[SQLquery] CommandText is empty", 4);
			return MyDataTable;
		}

		try {
			using (MySqlConnection Connection = new MySqlConnection(this.SqlLogin())) {
				selectQuery.Connection = Connection;
				using (MySqlDataAdapter MyAdapter = new MySqlDataAdapter(selectQuery)) {
					if (MyAdapter != null) {
						MyAdapter.Fill(MyDataTable);
					} else {
						DebugWrite("[SQLquery] MyAdapter is null", 4);
					}
				}
				Connection.Close();
			}
		}
		catch (MySqlException me) {
			ConsoleError("[SQLquery] Error in SQL.");
			this.DisplayMySqlErrorCollection(me);
			this.SqlTableExist = false;
		}
		catch (Exception c) {
			ConsoleError("[SQLquery] Error in SQL Query: " + c);
			this.SqlTableExist = false;
		}
	}
	catch (Exception c) {
		ConsoleError("[SQLquery] SQLQuery OuterException: " + c);
		this.SqlTableExist = false;
	}
	return MyDataTable;
}

public void DisplayMySqlErrorCollection(MySqlException myException) {
	this.ExecuteCommand("procon.protected.pluginconsole.write", "^1Message: " + myException.Message + "^0");
	this.ExecuteCommand("procon.protected.pluginconsole.write", "^1Native: " + myException.ErrorCode.ToString() + "^0");
	this.ExecuteCommand("procon.protected.pluginconsole.write", "^1Source: " + myException.Source.ToString() + "^0");
	this.ExecuteCommand("procon.protected.pluginconsole.write", "^1StackTrace: " + myException.StackTrace.ToString() + "^0");
	this.ExecuteCommand("procon.protected.pluginconsole.write", "^1InnerException: " + myException.InnerException.ToString() + "^0");
}

private bool SqlLoginsOk() {
	if ((this.SettingStrSqlHostname != String.Empty) && (this.SettingStrSqlPort != String.Empty) && (this.SettingStrSqlDatabase != String.Empty) && (this.SettingStrSqlUsername != String.Empty) && (this.SettingStrSqlPassword != String.Empty)) {
		return true;
	} else {
		ConsoleWrite("[SqlLoginDetails]^8^b SQL Server Details not completed (Host IP, Port, Database, Username, PW). Please check your Plugin settings.^0^n");
		DebugWrite("[SqlLoginDetails] SQL Details: Host=`" + this.SettingStrSqlHostname + "` ; Port=`" + this.SettingStrSqlPort + "` ; Database=`" + this.SettingStrSqlDatabase + "` ; Username=`" + this.SettingStrSqlUsername + "` ; Password=`" + this.SettingStrSqlPassword + "`" ,2);
		if (this.fIsEnabled) {
			this.ExecuteCommand("procon.protected.plugins.enable", "VipSlotManager", "False");
		}
		return false;
	}
}

//////////////////////
// Private & Public Functions
//////////////////////

private void DisplayVips() {
	int vipcount = 0;
	this.ProconChat("[VIP LIST] [" + this.SettingGameType + " - " + this.SettingStrSqlServerGroup + "] Display VIP list with players remaining time (data from last Sync):");
	foreach (KeyValuePair<String, int> tmp_sqlvips in this.SqlVipsActive) {
		vipcount++;
		this.ProconChat("[VIP LIST] [" + this.SettingGameType + " - " + this.SettingStrSqlServerGroup + "] " + vipcount.ToString() + ". " + this.strGreen(tmp_sqlvips.Key) + " valid for: " + this.strGetRemTime(tmp_sqlvips.Value));
	}
	ConsoleWrite("^bVIP Slot Manager^n > [VIP LIST] Display VIP list with players remaining time in CHAT tab");
	this.ProconChat("[VIP LIST] Command: ^b/vsm-addvip <full playername> <days>^n   ( e.g. /vsm-addvip SniperBen +30 )");
	this.ProconChat("[VIP LIST] Command: ^b/vsm-removevip <full playername>^n   ( e.g. /vsm-removevip SniperBen )");
	this.ProconChat("[VIP LIST] Command: ^b/vsm-changevip <old playername> <new playername>^n   ( e.g. /vsm-changevip SniperBen SniperBenni )");
	this.ProconChat("[VIP LIST] You can enter these commands in the Procon PC Tool > Chat (say, all)");
	
	this.ProconChat("[VIP LIST] VIPs online: " + this.vipsCurrentlyOnline.ToString() + "/" + this.SqlVipsActive.Count.ToString());
	if (this.SettingVipExp == enumBoolYesNo.Yes) { this.ProconChat("[VIP LIST] VIPs expired: " + this.vipsExpired.Count.ToString() + " (Players will get a 'VIP Slot Expired' Message on next spawn/join event)"); } 
	if ((this.SettingAggressiveJoinKickAbuseMax > 1) && (this.RoundTempVips.Count > 0)) { ConsoleWrite("[VIP LIST] Aggressive Join Abuse Protection > On current round blocked for " + this.RoundTempVips.Count.ToString() + " VIPs: " + String.Join(", ",  this.RoundTempVips.ToArray())); }
}

private int GetVipTimestamp(String playername) {
	if (playername.Length >= 3) {
		if (this.SqlVipsActive.ContainsKey(playername)) {
			return this.SqlVipsActive[playername];
		}
	}
	return -1;
}

private void AdkatsPlayerLog(String playername, String msg) {
	if ((playername.Length >= 3) && (msg.Length >= 3) && (this.AdkatsRunning)) { 
		if (this.NameGuidList.ContainsKey(playername)) {
			this.ExecuteCommand("procon.protected.plugins.call", "AdKats", "IssueCommand", "VipSlotManager", JSON.JsonEncode(new Hashtable {{"caller_identity", "VipSlotManager"},{"response_requested", false},{"command_type", "player_log"},{"source_name", "VIP Slot Manager"},{"target_name", playername},{"target_guid", this.NameGuidList[playername]},{"record_message", msg}}));
		}
	}
}

public void PluginStarter() {
	if ((this.gotVipsGS) && (!this.firstCheck) && (this.fIsEnabled)) {
		// wait a little bit after layer restart
		DebugWrite("[Task] [PluginStarter] Layer restart detected. Warmup...", 5);
		if (((DateTime.UtcNow - this.LayerStartingTime).TotalSeconds) > this.start_delay) {
			Thread ThreadWorker2 = new Thread(new ThreadStart(delegate() {
				this.CheckSettingsSql();
				this.AdkatsRunning = GetRegisteredCommands().Any(command => command.RegisteredClassname == "AdKats" && command.RegisteredMethodName == "PluginEnabled");
			}));
			ThreadWorker2.IsBackground = true;
			ThreadWorker2.Name = "threadworker2";
			ThreadWorker2.Start();
		}
	}
}

public void CheckSettingsSql() {
	// basic checks after plugin enabled
	// check 1. sql logins
	
	DebugWrite("[Task] [Check] Checking SQL Settings", 5);
	if ((!this.fIsEnabled) || (!this.SqlLoginsOk())) { return; }
	if ((this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes) && (this.SettingSyncGs2Sql != "no  (remove from Gameserver)") && (this.SettingSyncGs2Sql != "no  (ignore)") && (this.SettingSyncGs2Sql != "yes  (30 days first Plugin installation only)")) {
		ConsoleWrite("ERROR: 'Aggressive Join Abuse Protection' disabled. To use this function the Plugin setting 'Import NEW VIPS from Gameserver to SQL' must be set to 'NO (remove)'.");
		this.SettingAggressiveJoinKickAbuse = enumBoolYesNo.No;
		this.ExecuteCommand("procon.protected.plugins.setVariable", "VipSlotManager", "Enable Aggressive Join Abuse Protection", "No");
	}

	// check 2. sql database connection
	DebugWrite("[Task] [Check] Try to connect to SQL Server", 5);
	try {
		using (MySqlConnection Con = new MySqlConnection(this.SqlLogin())) {
			Con.Open();
			DebugWrite("[Task] [Check] Test Connection to SQL was successfully", 5);
			List<MatchCommand> registered = this.GetRegisteredCommands();
			bool tmp_checkAdkatsSettings = false;
			foreach (MatchCommand command in registered) {
				if ((command.RegisteredClassname == "AdKats") && (command.RegisteredMethodName == "PluginEnabled")) { tmp_checkAdkatsSettings = true;}
			}
			if ((tmp_checkAdkatsSettings) && (this.ServerIP != String.Empty) && (this.ServerPort != String.Empty)) {
				DebugWrite("[Task] [Check] Adkats Plugin detected. Adkats is currently enabled", 5);
				try {
					//check adkats plugin conflict
					//check if table exist or not in SQL database
					bool AdkatsTableExist = false;
					string SQL = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='adkats_settings' AND table_schema='" + this.SettingStrSqlDatabase + "'";
					DebugWrite("[Task] [Check] Try to get Adkats settings. SQL COMMAND (MyCommand): " + SQL, 5);
					using (MySqlCommand MyCommand = new MySqlCommand(SQL)) {
						DataTable resultTable = this.SQLquery(MyCommand);
						if (resultTable.Rows != null) {
							string tmp_tablename = String.Empty;
							foreach (DataRow row in resultTable.Rows) {
								//reading sql
								tmp_tablename = row["TABLE_NAME"].ToString();
								if (tmp_tablename == "adkats_settings") {
									// yes, table 'adkats_settings' exist in SQL DB!!
									AdkatsTableExist = true;
								}
							}
						} else {
							ConsoleError("[Task] [Check] [AdkatsSettings] ERROR: Can NOT receive table informations from SQL. Shutdown Plugin");
							this.ExecuteCommand("procon.protected.plugins.enable", "VipSlotManager", "False");
							return;
						}
					}
					if (AdkatsTableExist) {
						//check 3. adkats plugin, "a16 Orchestration Settings, Feed Reserved Slots" = False
						SQL = "SELECT `setting_value` FROM `adkats_settings` WHERE setting_name = 'Feed Server Reserved Slots' AND server_id IN (SELECT ServerID as ServerID FROM `" + this.SettingStrSqlDatabase + "`.`tbl_server` WHERE `IP_Address` = '" + this.ServerIP + ":" + this.ServerPort + "')";
						DebugWrite("[Task] [Check] [AdkatsSettings] Try to get Adkats setting 'Feed Server Reserved Slots'. SQL COMMAND (MyCommand): " + SQL, 5);
						using (MySqlCommand MyCommand = new MySqlCommand(SQL)) {
							DataTable resultTable = this.SQLquery(MyCommand);
							if (resultTable.Rows != null) {
								string tmp_adkatsSetting = String.Empty;
								foreach (DataRow row in resultTable.Rows) {
									tmp_adkatsSetting = row["setting_value"].ToString();
									if (tmp_adkatsSetting == "True") {
										//problem found! adkats still manage the reserved slot list
										ConsoleError("[Task] [Check] [AdkatsSettings] ERROR: Plugin conflict with current settings from Adkats!");
										ConsoleError("[Task] [Check] [AdkatsSettings] ERROR: Adkats Plugin still manage the reserved VIP slot. IMPORTANT: You must disable this function in Adkats! Open the settings from Adkats Plugin. Then go to ^0^8^bAdkats  >  A16. Orchestration Settings  >  Feed Server Reserved Slots  > False^n^0");
										//shutdown plugin
										this.ExecuteCommand("procon.protected.plugins.enable", "VipSlotManager", "False");
										return;
									} else if (tmp_adkatsSetting == "False") {
										DebugWrite("[Task] [Check] [AdkatsSettings] Adkats Plugin do not manage the reserved VIP slot", 5);
									}
								}
							} else {
								DebugWrite("[Task] [Check] [AdkatsSettings] INFO: Please make sure that Adkats do not manage the reserved VIP Slots. Open the settings from Adkats Plugin. Then go to ^0^8^bAdkats  >  A16. Orchestration Settings  >  Feed Server Reserved Slots  > False^n^0", 3);
							}
						}
					}
				}
				catch (Exception c) {
					ConsoleError("[Task] [Check] Can not read the current settings from Adkats Plugin. SQL Error (MyCommand): " + c);
				}
			}

			if (Con.State == ConnectionState.Open) {
				DebugWrite("[Task] [Check] Close SQL Connection (Con)", 5);
				Con.Close();
			}
		}
		
		//startup test finished
		this.firstCheck = true;
		DebugWrite("[Task] Checkup completed.", 3);
		this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "5", "60", "-1", "procon.protected.plugins.call", "VipSlotManager", "updnow");
		this.TableBuilder();
	}
	catch (Exception c) {
		ConsoleError("[Task] [Check] FATAL ERROR: CAN NOT CONNECT TO SQL SERVER! Please check your Plugin settings 'SQL Server Details' (Host IP, Port, Database, Username, PW) and try again. Maybe the provider of your Procon Layer block the connection (server firewall settings)." );
		ConsoleError("[Task] [Check] Error (Con): " + c );
		ConsoleError("[Task] [Check] Error");
		ConsoleError("[Task] [Check] Shutdown Plugin...");
		this.ExecuteCommand("procon.protected.plugins.enable", "VipSlotManager", "False");
		return;
	}
}

public void updnow() {
	if (!this.fIsEnabled) { return; }
	this.SyncCounter++;
	if (this.SyncCounter >= this.SettingSyncInterval) {
		this.SyncCounter = 0;
		DebugWrite("[Task] Sync VIP players from SQL database with VIP players from Gameserver", 5);
		this.SyncVipList();
	} else {
		this.DBCleanerCounter++;
		if (this.DBCleanerCounter >= 1000) {
			this.DBCleanerCounter = 0;
			DebugWrite("[Task] Start Auto Database Cleaner", 5);
			this.DatabaseCleaner();
		}
	} 

	if (this.SettingAggressiveJoin == enumBoolYesNo.Yes) {
		// Enable / Disable Aggressive Join
		if (this.AggressiveJoin) {
			if ((this.CurrentGameMode == "ConquestLarge0") || (this.CurrentGameMode == "ConquestSmall0") || (this.CurrentGameMode == "Chainlink0")) {
				if (this.ticketLoserTeam <= 120) {
					DebugWrite("[Task] Aggressive Join for VIPs disabled (less than 120 tickets remaining).", 4);
					this.ProconChat("Aggressive Join for VIPs disabled (less than 120 tickets remaining).");
					this.ExecuteCommand("procon.protected.send", "reservedSlotsList.aggressiveJoin", "false");
				}
			} else if (this.CurrentGameMode == "TeamDeathMatch0") {
				if (this.ticketLoserTeam <= 30) {
					DebugWrite("[Task] Aggressive Join for VIPs disabled (less than 30 kills remaining).", 4);
					this.ProconChat("Aggressive Join for VIPs disabled (less than 30 kills remaining).");
					this.ExecuteCommand("procon.protected.send", "reservedSlotsList.aggressiveJoin", "false");
				}
			} 
		} else if (((DateTime.UtcNow - this.CurrentRoundEndTime).TotalSeconds) > 50) {
			if ((this.CurrentGameMode == "ConquestLarge0") || (this.CurrentGameMode == "ConquestSmall0")  || (this.CurrentGameMode == "Chainlink0")) {
				if (this.ticketLoserTeam > 120) {
					DebugWrite("[Task] Aggressive Join for VIPs enabled", 4);
					this.ProconChat("Aggressive Join for VIPs enabled");
					this.ExecuteCommand("procon.protected.send", "reservedSlotsList.aggressiveJoin", "true");
				}
			} else if (this.CurrentGameMode == "TeamDeathMatch0") {
				if (this.ticketLoserTeam > 30) {
					DebugWrite("[Task] Aggressive Join for VIPs enabled", 4);
					this.ProconChat("Aggressive Join for VIPs enabled");
					this.ExecuteCommand("procon.protected.send", "reservedSlotsList.aggressiveJoin", "true");
				}
			} else {
				DebugWrite("[Task] Aggressive Join for VIPs enabled", 4);
				this.ProconChat("Aggressive Join for VIPs enabled");
				this.ExecuteCommand("procon.protected.send", "reservedSlotsList.aggressiveJoin", "true");
			}
		}
	}
}

private void RoundEndCleaner() {
	if (((DateTime.UtcNow - this.CurrentRoundEndTime).TotalSeconds) > 120) {
		this.CurrentRoundEndTime = DateTime.UtcNow;
		if (this.SettingProconRulzIni == enumBoolYesNo.Yes) {
			DebugWrite("[OnRoundOver] Add Task to write proconrulz.ini in 15 secouds.", 5);
			this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "15", "1", "1", "procon.protected.plugins.call", "VipSlotManager", "FileWriteProconRulz");
		}
		DebugWrite("[OnRoundOver] VIPs online: " + this.vipsCurrentlyOnline.ToString() + "/" + this.SqlVipsActive.Count.ToString(), 4);
		this.ProconChat("VIPs online: " + this.vipsCurrentlyOnline.ToString() + "/" + this.SqlVipsActive.Count.ToString());
		if ((this.SettingAggressiveJoinKickAbuseMax > 1) && (this.RoundTempVips.Count > 0)) {
			DebugWrite("[OnRoundOver] [AggressiveJoinAbuseProtection] 'Aggressive Join Kick' was temporary blocked for " + this.RoundTempVips.Count.ToString() + " VIPs: " + String.Join(", ",  this.RoundTempVips.ToArray()), 3);			
			this.ProconChat("'Aggressive Join Kick' was temporary blocked for " + this.RoundTempVips.Count.ToString() + " VIPs: " + String.Join(", ",  this.RoundTempVips.ToArray()));
		}

		this.AggressiveJoinAbuseCleaner();
		this.vipmsg.Clear();
		this.playerTeamID.Clear();
		this.playerSquadID.Clear();
		this.GetPlayerGuid.Clear();
		this.Guid2Check.Clear();
		this.onJoinSpammer.Clear();
		this.SquadLederList.Clear();
		this.NameGuidList.Clear();
		this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", "80", "1", "1", "procon.protected.send", "admin.listPlayers", "all");
	}
}

private void AggressiveJoinAbuseCleaner() {
	bool tmp_upd_gs = false;
	if (this.RoundTempVips.Count > 0) {
		// switch blocked vips to full vips
		DebugWrite("[AggressiveJoinAbuseCleaner] Reactivate 'Aggressive Join Kick' privilege. for all vaild VIPs on Gameserver.", 4);
		foreach (string vipplayer in this.RoundTempVips) {
			 if (this.GetVipTimestamp(vipplayer) != -1) {
				// add vip slot
				DebugWrite("[AggressiveJoinAbuseCleaner] Add VIP Slot: " + this.strGreen(vipplayer) + " to Gameserver.", 5);
				tmp_upd_gs = true;
				this.ProconVipAdd(vipplayer);
			} else {
				// remove semi vip slot
				DebugWrite("[AggressiveJoinAbuseCleaner] Remove Semi VIP Slot: " + this.strBlack(vipplayer) + " from Gameserver.", 4);
				tmp_upd_gs = true;
				this.ProconVipRemove(vipplayer);
			}
		}
		if (tmp_upd_gs) {
			this.ProconVipSave();
			this.ProconVipList();
		}
	}
	this.RoundTempVips.Clear();
	this.AggressiveVips.Clear();
}

private void ProconVipRemove(String playername) {
	if (this.GetGameType == "BFBC2") {
		this.ExecuteCommand("procon.protected.send", "reservedSlots.removePlayer", playername);
	} else {
		this.ExecuteCommand("procon.protected.send", "reservedSlotsList.remove", playername);
	}
}

private void ProconVipAdd(String playername) {
	if (this.GetGameType == "BFBC2") {
		this.ExecuteCommand("procon.protected.send", "reservedSlots.addPlayer", playername);
	} else {
		this.ExecuteCommand("procon.protected.send", "reservedSlotsList.add", playername);
	}
}

private void ProconVipSave() {
	if (this.GetGameType == "BFBC2") {
		this.ExecuteCommand("procon.protected.send", "reservedSlots.save");
	} else {
		this.ExecuteCommand("procon.protected.send", "reservedSlotsList.save");
	}
}

private void ProconVipList() {
	if (this.GetGameType == "BFBC2") {
		this.ExecuteCommand("procon.protected.send", "reservedSlots.list");
	} else {
		this.ExecuteCommand("procon.protected.send", "reservedSlotsList.list");
	}
}

private void GenRegMatch() {
	int tmp_x = 0;
	string tmp_regmatch = "^/?!vip|^/?!slot|^/?!reserved|^/?!buy";
	foreach (string chatcommand in this.SettingInfoCommands.Split(',')) {
		if (chatcommand.Length >= 3) {
			tmp_x++;
			if (tmp_x == 1) {
				tmp_regmatch = "^/?" + chatcommand.ToLower();
			} else {
				tmp_regmatch = tmp_regmatch + "|^/?" + chatcommand.ToLower();
			}
		}
	}
	this.SettingInfoCmdRegMatch = tmp_regmatch;
}

public void FileWriteProconRulz() {
	if ((!this.fIsEnabled) || (!this.firstCheck)) { return; }
	Thread ThreadWorker1 = new Thread(new ThreadStart(delegate() {
		String tmp_currentvips = String.Empty;
		String tmp_tempData = String.Empty;
		bool tmp_parser = true;
		int tmp_line_counter = 0;
		String path  = "Configs\\" + this.ServerIP + "_" + this.ServerPort + "_proconrulz.ini";
		if ((this.ServerIP == String.Empty) || (this.ServerPort == String.Empty)) {
			ConsoleError("[FileWriteProconRulz] ^bERROR:^n Gameserver IP / Port is missing. Can not create filename");
			return;
		}
		DebugWrite("[FileWriteProconRulz] Write valid VIPs in proconrulz.ini file. Path: " + path , 5);
		// create list with valid vip player and proconrulz timestamp
		tmp_currentvips = "[vipslotmanager]" + Environment.NewLine;
		if (this.SqlVipsActive.Count > 0) {
			foreach (KeyValuePair<String, int> tmp_sqlvips in this.SqlVipsActive) {
				tmp_line_counter++;
				// proconrulz timestamp since 01.01.2012 in SECONDS
				tmp_currentvips += tmp_sqlvips.Key + "=" + (tmp_sqlvips.Value + Convert.ToInt32(((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds) - (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds) - 1325376000).ToString();
				if (tmp_line_counter < this.SqlVipsActive.Count) {
					tmp_currentvips += Environment.NewLine;
				}
			}
		} else {
			tmp_currentvips = String.Empty;
		}
		
		tmp_line_counter = 0;
		try {
			if(File.Exists(path)) {
				string[] readText = File.ReadAllLines(path);
				if (readText.Length != 0) {
					foreach (string s in readText) {
						// parse proconrulz.ini file
						tmp_line_counter++;
						if (s == "[vipslotmanager]") { tmp_parser = false; }
						if ((s.StartsWith("[")) && (s != "[vipslotmanager]")) {tmp_parser = true;}
						if (tmp_parser) {
							tmp_tempData += s;
							if (tmp_line_counter < readText.Length) {
								tmp_tempData += Environment.NewLine;
							}
						}
					}
				}
			}
			if (tmp_tempData.Length > 1) {
				File.WriteAllText(path, tmp_currentvips + Environment.NewLine + tmp_tempData);
			} else {
				File.WriteAllText(path, tmp_currentvips);
			}
		}
		catch (Exception e) {
			ConsoleError("[FileWriteProconRulz] Can NOT write proconrulz.ini file. Requires Read+Write file permission. Path: " + path + "    ERROR: " + e);
		}
	}));
	ThreadWorker1.IsBackground = true;
	ThreadWorker1.Name = "threadworker1";
	ThreadWorker1.Start();
}

public bool isAdmin(String playername) {
	try {
		CPrivileges AdminPrivis = this.GetAccountPrivileges(playername);
		if (AdminPrivis.CanEditReservedSlotsList) {
			return true;
		} 
	}
	catch { }
	return false;
}

private string SqlLogin() {	return "Server=" + this.SettingStrSqlHostname + ";" + "Port=" + this.SettingStrSqlPort + ";" + "Database=" + this.SettingStrSqlDatabase + ";" + "Uid=" + this.SettingStrSqlUsername + ";" + "Pwd=" + this.SettingStrSqlPassword + ";" + "Connection Timeout=5;"; }

private string strSqlProtection(String StrInp) {return StrInp.Replace("\"", "").Replace(";", "").Replace(",", "").Replace("(", "").Replace(")", "").Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace("'", "").Replace("’", "").Replace("‘", "").Replace(" ", "") ;}
	
private string strGreen(String StrInp) {return "^b^2" + StrInp + "^0^n";}

private string strNoGreen(String StrInp) {return StrInp.Replace("^b^2", "").Replace("^0^n", "");}

private string strBlack(String StrInp) {return "^b" + StrInp + "^n";}

private int intUtcTimestamp() {return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);}

private string strGetRemTime(int PlayerTimeStamp) {
	int tmp_days = ((PlayerTimeStamp - intUtcTimestamp()) / 86400);
	int tmp_hours = ((PlayerTimeStamp - intUtcTimestamp()) / 3600);
	string tmp_msg = String.Empty;
	if (tmp_days > 2000) {
		tmp_msg = "permanent";
	} else if (tmp_days > 1) {
		tmp_msg = tmp_days.ToString() + " days " + (tmp_hours - (tmp_days * 24)).ToString() + " hours";
	} else if (tmp_days == 1) {
		tmp_msg = tmp_days.ToString() + " day " + (tmp_hours - (tmp_days * 24)).ToString() + " hours";
	} else if (tmp_hours > 1) {
		tmp_msg = tmp_hours.ToString() + " hours";
	} else if (tmp_hours > 0) {
		tmp_msg = tmp_hours.ToString() + " hour";
	} else if (tmp_hours == 0) {
		tmp_msg = "> 1 hour";
	}
	return tmp_msg;
}

private string strYellDur() {
	if (this.GetGameType == "BFBC2") {
		return (this.SettingYellDuring * 1000).ToString();
	} else {
		return this.SettingYellDuring.ToString();
	}
}

public void AddRoundSemiVip(String soldierName) {
	if ((!this.fIsEnabled) || (!this.firstCheck) || (soldierName.Length < 4)) { return; }
	if ((!this.RoundTempVips.Contains(soldierName)) && (this.SettingAggressiveJoinKickAbuse == enumBoolYesNo.Yes)) {
		DebugWrite("[AddRoundSemiVip] Player " + this.strBlack(soldierName) + " added as Semi VIP (valid for current round / rejoin).", 4);
		this.RoundTempVips.Add(soldierName);
		this.ProconVipAdd(soldierName);
		this.ProconVipSave();
		this.ProconVipList();
	}
}

//////////////////////
// Send In-Game Messages (say,yell,...)
//////////////////////

public void PlayerSayMsg(String target, String message) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	this.ExecuteCommand("procon.protected.send", "admin.say", this.strNoGreen(message.Replace(System.Environment.NewLine, " ")), "player", target);
	this.ExecuteCommand("procon.protected.chat.write", "(PlayerSay " + target + ") " + message.Replace(System.Environment.NewLine, " "));
}

public void PlayerSayMsg(String target, String message, int msgdelay) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	if (msgdelay == 0) {
		PlayerSayMsg(target, message);
		return;
	}
	if (message.Length < 3) { return;}
	this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", msgdelay.ToString(), "1", "1", "procon.protected.send", "admin.say", this.strNoGreen(message.Replace(System.Environment.NewLine, " ")), "player", target);
	this.ExecuteCommand("procon.protected.chat.write", "(PlayerSay " + target + ") " + message.Replace(System.Environment.NewLine, " "));
}

public void SayMsg(String message) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	this.ExecuteCommand("procon.protected.send", "admin.say", this.strNoGreen(message.Replace(System.Environment.NewLine, " ")), "all");
	this.ExecuteCommand("procon.protected.chat.write", message.Replace(System.Environment.NewLine, " "));
}

public void SayMsg(String message, int msgdelay) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	if (msgdelay == 0) {
		SayMsg(message);
		return;
	}
	if (message.Length < 3) { return;}
	this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", msgdelay.ToString(), "1", "1", "procon.protected.send", "admin.say", this.strNoGreen(message.Replace(System.Environment.NewLine, " ")), "all");
	this.ExecuteCommand("procon.protected.chat.write", message.Replace(System.Environment.NewLine, " "));
}

public void PlayerYellMsg(String target, String message) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	this.ExecuteCommand("procon.protected.send", "admin.yell", "[VIP SLOT] " + this.NewLiner + this.strNoGreen(message.Replace(System.Environment.NewLine, this.NewLiner)), this.strYellDur(), "player", target);
	this.ExecuteCommand("procon.protected.chat.write", "(PlayerYell " + target + ") " + message.Replace(System.Environment.NewLine, "  -  "));
}

public void PlayerYellMsg(String target, String message, int msgdelay) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	if (msgdelay == 0) {
		PlayerYellMsg(target, message);
		return;
	}
	this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", msgdelay.ToString(), "1", "1", "procon.protected.send", "admin.yell", "[VIP SLOT] " + this.NewLiner + this.strNoGreen(message.Replace(System.Environment.NewLine, this.NewLiner)), this.strYellDur(), "player", target);
	this.ExecuteCommand("procon.protected.chat.write", "(PlayerYell " + target + ") " + message.Replace(System.Environment.NewLine, "  -  "));
}

public void YellMsg(String message) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	this.ExecuteCommand("procon.protected.send", "admin.yell", "[VIP SLOT] " + this.NewLiner + this.strNoGreen(message.Replace(System.Environment.NewLine, this.NewLiner)), this.strYellDur(), "all");
	this.ExecuteCommand("procon.protected.chat.write", "(Yell) " + message.Replace(System.Environment.NewLine, "  -  "));
}

public void YellMsg(String message, int msgdelay) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	if (msgdelay == 0) {
		YellMsg(message);
		return;
	}
	this.ExecuteCommand("procon.protected.tasks.add", "VipSlotManager", msgdelay.ToString(), "1", "1", "procon.protected.send", "admin.yell", "[VIP SLOT] " + this.NewLiner + this.strNoGreen(message.Replace(System.Environment.NewLine, this.NewLiner)), this.strYellDur(), "all");
	this.ExecuteCommand("procon.protected.chat.write", "(Yell) " + message.Replace(System.Environment.NewLine, "  -  "));
}

public void ProconChat(String message) {
	if ((!this.fIsEnabled) || (message.Length < 3)) { return;}
	this.ExecuteCommand("procon.protected.chat.write", "^bVIP Slot Manager^n > " + message.Replace(System.Environment.NewLine, " "));
}


}
} // end namespace PRoConEvents
