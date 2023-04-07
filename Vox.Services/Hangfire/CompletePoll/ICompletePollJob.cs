using System;
using System.Threading.Tasks;

namespace Vox.Services.Hangfire.CompletePoll;

public interface ICompletePollJob
{
    Task Execute(Guid pollId, ulong guildId, ulong channelId, ulong messageId, string question, string avatarUrl);
}