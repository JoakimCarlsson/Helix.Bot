using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helix.Domain.Models;
using Helix.Services.Services;

namespace Helix.Services.Abstractions
{
    public interface IUserReminderService
    {
        Task<ServiceResponse<UserReminder>> GetReminderAsync(int id, CancellationToken cancellationToken = default);
        Task<ServiceResponse<UserReminder>> GetUserReminderAsync(int id, ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllUserRemindersAsync(ulong userId, ulong guildId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllRemindersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<UserReminder>>> GetAllRemindersInGuildAsync(ulong guildId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<UserReminder>> AddUserReminderAsync(ulong userId, ulong guildId, ulong channelId, string content, TimeSpan remindAt, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteUserReminderAsync(ulong userId, ulong guildId, int reminderId, CancellationToken cancellationToken = default);
    }
}
