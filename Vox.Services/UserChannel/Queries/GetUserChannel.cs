using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vox.Data;
using Vox.Services.UserChannel.Commands;
using Vox.Services.UserChannel.Models;

namespace Vox.Services.UserChannel.Queries;

public record GetUserChannel(SocketUser SocketUser) : IRequest<Models.UserChannel>;

public class GetUserChannelHandler : IRequestHandler<GetUserChannel, Models.UserChannel>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMediator _mediator;
    private readonly VoxOverwritePermissions _defaultPermissions = new(285212688, 0);

    public GetUserChannelHandler(
        IServiceScopeFactory scopeFactory,
        IMediator mediator)
    {
        _scopeFactory = scopeFactory;
        _mediator = mediator;
    }

    /// <inheritdoc />
    public async Task<Models.UserChannel> Handle(GetUserChannel request, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.UserChannels
            .SingleOrDefaultAsync(x => x.UserId == (long) request.SocketUser.Id);

        if (entity is not null)
        {
            return new Models.UserChannel(
                entity.Id,
                entity.UserId,
                entity.ChannelName,
                entity.ChannelLimit,
                JsonSerializer.Deserialize<IReadOnlyCollection<VoxOverwrite>>(entity.OverwritesData) ??
                new List<VoxOverwrite>());
        }

        var userChannel = new Models.UserChannel(
            Guid.NewGuid(),
            (long) request.SocketUser.Id,
            request.SocketUser.Username,
            5,
            new List<VoxOverwrite> { new(request.SocketUser.Id, PermissionTarget.User, _defaultPermissions) });

        await _mediator.Send(new SaveUserChannel(userChannel));
        return userChannel;
    }
};
