﻿using MediatR;
using System.Runtime.CompilerServices;
using vertical_slice.Database;
using vertical_slice.Entities;
using vertical_slice.Shared;
using Carter;
using FluentValidation;
using vertical_slice.Features.Articles;

namespace vertical_slice.Features.Articles
{
    public static class CreateArticle
    {
        public class Command : IRequest<Result<Guid>>
        {
            public string Title { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Title).NotEmpty();
                RuleFor(c => c.Content).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, Result<Guid>>
        {

            private readonly ApplicationDbContext _dbContext;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (validationResult != null && !validationResult.IsValid)
                {
                    return Result<Guid>.Failure(new Error("CreateArticle.Validation", validationResult.ToString()));
                }

                var article = new Article
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _dbContext.AddAsync(article, cancellationToken);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(article.Id);
            }

        }
    }
}

public class CreateArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Articles", async (CreateArticle.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
