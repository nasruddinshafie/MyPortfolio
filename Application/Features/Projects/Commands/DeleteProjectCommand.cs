using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Projects.Commands;

public record DeleteProjectCommand(int Id) : IRequest;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        await _projectRepository.DeleteAsync(request.Id);
    }
}
