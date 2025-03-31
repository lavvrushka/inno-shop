using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;
using AutoMapper;
using UserManagement.Domain.Interfaces.IServices;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConnectionService _connectionService;

        public DeleteUserHandler(IUnitOfWork unitOfWork, IMapper mapper, IConnectionService connectionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _connectionService = connectionService;
        }

        public async Task<Unit> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            
            var user = await _unitOfWork.Users.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new InvalidOperationException("Пользователь не найден.");
            }

            
            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
            await _connectionService.DeleteAllProductsByUserAsync(request.Id);
            return Unit.Value;
        }
    }

}
