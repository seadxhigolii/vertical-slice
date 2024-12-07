using MediatR;
using System.Runtime.CompilerServices;
using vertical_slice.Database;
using vertical_slice.Entities;
using vertical_slice.Shared;
using Carter;
using FluentValidation;
using vertical_slice.Features.Articles;

namespace vertical_slice.Features.Articles
{
    public class DeleteArticle
    {
        public class Command: IRequest<Result<bool>>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(c => c.Id).NotEmpty();
            }
        }

        internal sealed class Handler : IRequestHandler<Command, Result<bool>>
        {

            private readonly ApplicationDbContext _dbContext;
            private readonly IValidator<Command> _validator;

            public Handler(ApplicationDbContext dbContext, IValidator<Command> validator)
            {
                _dbContext = dbContext;
                _validator = validator;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (validationResult != null && !validationResult.IsValid)
                {
                    return Result<bool>.Failure(new Error("DeleteArticle.Validation", validationResult.ToString()));
                }

                var article = await _dbContext.Articles.FindAsync(new object[] { request.Id }, cancellationToken);
                if (article == null)
                {
                    return Result<bool>.Failure(new Error("DeleteArticle.NotFound", "Article not found."));
                }

                _dbContext.Articles.Remove(article);

                await _dbContext.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
        }
    }
}

public class DeleteArticleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Articles/{id:guid}", async (Guid id, ISender sender) =>
        {
            var command = new DeleteArticle.Command { Id = id };
            var result = await sender.Send(command);

            if (result.IsFailure)
            {
                return Results.BadRequest(result.Error);
            }

            return Results.Ok(result.Value);
        });
    }
}
