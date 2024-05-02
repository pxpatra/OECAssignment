using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class AssignUsersToPlanProcedureCommandHandler : IRequestHandler<AssignUsersToPlanProcedureCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AssignUsersToPlanProcedureCommandHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AssignUsersToPlanProcedureCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));
            if (request.UserIds.Count <= 0)
                return ApiResponse<Unit>.Fail(new BadRequestException("No user selected"));

            var planProcedure = await _context.PlanProcedures
               .Include(p => p.PlanProcedureUsers)
               .FirstOrDefaultAsync(p => p.PlanId == request.PlanId && p.ProcedureId == request.ProcedureId);            
            if (planProcedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"ProcedureId: {request.ProcedureId} not found for PlanId: {request.PlanId}"));
           
            // Add or remove user
            var usersToBeAdded = request.UserIds.Except(planProcedure.PlanProcedureUsers.ToList().Select(u => u.UserId));
            var usersToBeRemoved = planProcedure.PlanProcedureUsers.ToList().Select(u => u.UserId).Except(request.UserIds);
            if (usersToBeRemoved.Count() > 0)
            {
                foreach (var userId in usersToBeRemoved)
                {
                    var user = planProcedure.PlanProcedureUsers.FirstOrDefault(u => u.UserId == userId);
                    planProcedure.PlanProcedureUsers.Remove(user);
                }
            }
            if (usersToBeAdded.Count() > 0)
            {
                foreach (var userId in usersToBeAdded)
                {
                    var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                    planProcedure.PlanProcedureUsers.Add(user);
                }
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