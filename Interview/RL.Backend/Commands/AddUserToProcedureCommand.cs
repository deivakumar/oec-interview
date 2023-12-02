using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands
{
    public class AddUserToProcedureCommand : IRequest<ApiResponse<Unit>>
    {
        public AddUserToProcedureCommand() => UserIds = Array.Empty<int>();
        public int PlanId { get; set; }
        public int[] UserIds { get; set; }
        public int ProcedureId { get; set; }
    }
}
