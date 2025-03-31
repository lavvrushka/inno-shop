using AutoMapper;
using MediatR;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Application.DTOs.User.Responses;
using UserManagement.Domain.Interfaces.IRepositories;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, List<UserResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllUsersHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserResponse>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }

            var userResponse = _mapper.Map<List<UserResponse>>(users);
            return userResponse;
        }
    }
}
