using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Application.DTOs.User.Requests;
using UserManagement.Domain.Interfaces.IRepositories;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetByEmailConfirmationTokenAsync(request.Token);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid or expired confirmation token.");
            }
            if (user.EmailVerifiedAt != null)
            {
                return Unit.Value;
            }
            await _unitOfWork.Users.ConfirmEmailAsync(user.Id);
            await _unitOfWork.Users.UpdateUserAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value; 
        }
    }
}
