using MediatR;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data.DataModels;
using RL.Data;
using Microsoft.EntityFrameworkCore;

namespace RL.Backend.Commands.Handlers.Procedures
{
    public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AddUserToProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //Validate request
                if (request.PlanId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
                if (request.ProcedureId < 1)
                    return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

                var planProcedure = await _context.PlanProcedures
                    .Include(p => p.PlanProcedureUsers)
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId && p.ProcedureId == request.ProcedureId);

                if (planProcedure is null)
                    return ApiResponse<Unit>.Fail(new NotFoundException($"PlanId: {request.PlanId} and ProcedureId: {request.ProcedureId} mapping not found"));

                foreach (var userId in request.UserIds)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == userId);

                    if (user is null || planProcedure.PlanProcedureUsers.Any(p => p.UserId == user.UserId))
                        continue;

                    planProcedure.PlanProcedureUsers.Add(new PlanProcedureUser
                    {
                        UserId = userId,
                        CreateDate = DateTime.UtcNow
                    });
                }

                //Removed users
                var removedUserIds = planProcedure.PlanProcedureUsers.Select(p => p.UserId).Except(request.UserIds).ToList();

                foreach (var userId in removedUserIds)
                {
                    var removedUser = planProcedure.PlanProcedureUsers.FirstOrDefault(p => p.UserId == userId);
                    if (removedUser != null)
                        planProcedure.PlanProcedureUsers.Remove(removedUser);
                }


                await _context.SaveChangesAsync();

                return ApiResponse<Unit>.Succeed(new Unit());
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }
    }
}
