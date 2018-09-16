using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CommunityBot.Entities;
using Discord.WebSocket;
using Discord;

namespace CommunityBot.Helpers
{
    internal class RepeatedTaskFunctions
    {
        internal static Task InitRepeatedTasks()
        {
            // Look for expired reminders every 3 seconds
            Global.TaskHander.AddRepeatedTask("Reminders", 3000, new ElapsedEventHandler(CheckReminders));
            // Check if Snoops is alive every 10 seconds
            Global.TaskHander.AddRepeatedTask("Snoops", 10000, new ElapsedEventHandler(CheckSnoops));
            // Help Message every 2 hours
            Global.TaskHander.AddRepeatedTask("Help Message", 7200000, new ElapsedEventHandler(SendHelpMessage));
            return Task.CompletedTask;
        }

        private static void CheckSnoops(object sender, ElapsedEventArgs e)
        {
            Global.SnoopsHeartbeatCount++;
            Console.WriteLine($"Running Snoops heartbeat challenge #{Global.SnoopsHeartbeatCount}.");

            var snoops = Global.Client.GetUser(240905093929500674);
            if(snoops.Status == UserStatus.Offline)
            {
                if(Global.SnoopsLives)
                {
                    Console.WriteLine("Snoops stopped responding.");
                    Global.SnoopsLives = false;
                }
                Console.WriteLine($"Snoops heartbeat #{Global.SnoopsHeartbeatCount} failed.");
                return;
            }
            else
            {
                if(!Global.SnoopsLives)
                {
                    Global.SnoopsLives = true;
                    Console.WriteLine($"Snoops heartbeat challenge #{Global.SnoopsHeartbeatCount} succeeded!");
                    var spelos = Global.Client.GetUser(182941761801420802);
                    spelos.SendMessageAsync($"**YO! SNOOPS IS ALIVE.**\n\nOr at least his status changed. He's now: {snoops.Status.ToString()}");
                    if(spelos.Status != UserStatus.Online)
                    {
                        snoops.SendMessageAsync("Listen, Peter (Spelos) is not currently online (or he's DND or whatever), but you should probably go say hi. =)");
                    }
                }
            }
        }

        private static async void SendHelpMessage(object sender, ElapsedEventArgs e)
        {
            var general = Global.Client.GetChannel(403278466746810370) as SocketTextChannel;
            general?.SendMessageAsync("If you have any problems with your code, please follow the instructions in <#406360393489973248>!");
        }

        private static async void CheckReminders(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;
            // Get all accounts that have at least one reminder that needs to be sent out
            var accounts = Features.GlobalAccounts.GlobalUserAccounts.GetFilteredAccounts(acc => acc.Reminders.Any(rem => rem.DueDate < now));
            foreach (var account in accounts)
            {
                var guildUser = Global.Client.GetUser(account.Id);
                var dmChannel = await guildUser?.GetOrCreateDMChannelAsync();
                if (dmChannel == null) return;

                var toBeRemoved = new List<ReminderEntry>();

                foreach (var reminder in account.Reminders)
                {
                    if (reminder.DueDate >= now) continue;
                    dmChannel.SendMessageAsync(reminder.Description);
                    // Usage of a second list because trying to use 
                    // accountReminders.Remove(reminder) would break the foreach loop
                    toBeRemoved.Add(reminder);
                }
                // Remove all elements that needs to be removed
                toBeRemoved.ForEach(remRem => account.Reminders.Remove(remRem));
                Features.GlobalAccounts.GlobalUserAccounts.SaveAccounts(account.Id);
            }
        }
    }
}
