﻿using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> _commandHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> commandHandler)
    {
        _commandHandler = commandHandler;
    }

    public async Task HandleAsync(NewPostCommand command)
    {
        var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.EditMessage(command.Message);

        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.LikePost();

        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);

        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.EditComment(command.CommentId, command.Comment, command.Username);
        
        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);
        
        await _commandHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await _commandHandler.GetByIdAsync(command.Id);
        aggregate.DeletePost(command.Username);
        
        await _commandHandler.SaveAsync(aggregate);
    }
}