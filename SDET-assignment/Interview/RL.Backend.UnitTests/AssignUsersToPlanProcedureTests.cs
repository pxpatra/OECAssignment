using FluentAssertions;
using MediatR;
using Moq;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class AssignUsersToPlanProcedureTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AssignUsersToPlanProcedureTests_InvalidPlanId_ReturnsBadRequest(int planId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context.Object);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = 1
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AssignUsersToPlanProcedureTests_InvalidProcedureId_ReturnsBadRequest(int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context.Object);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = procedureId
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(new int[] {})]
        public async Task AssignUsersToPlanProcedureTests_NoUser_ReturnsBadRequest(int[] userIds)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context.Object);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserIds = userIds.ToList()
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1,19, new int[] { 1, 2})]
        public async Task AssignUsersToPlanProcedureTests_PlanWithProcedureIdNotFound_ReturnsNotFound(int planId, int procedureId, int[] userIds)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserIds = userIds.ToList(),
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
            {
                PlanId = planId + 1,
                ProcedureId = procedureId,
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }        

        [TestMethod]
        [DataRow(1, 2, new int[] { 1 })]
        public async Task AssignUsersToPlanProcedureTests_AssignUser_NoChange_ReturnsSuccess(int planId, int procedureId, int[] userIds)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserIds = userIds.ToList()
            };

            mockPlanProcedureData(context, planId, procedureId);

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 2, new int[] { 1, 2 })]
        public async Task AssignUsersToPlanProcedureTests_AssignUser_Add_ReturnsSuccess(int planId, int procedureId, int[] userIds)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserIds = userIds.ToList()
            };

            mockPlanProcedureData(context, planId, procedureId);

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 2, new int[] { 2 })]
        public async Task AssignUsersToPlanProcedureTests_AssignUser_Remove_ReturnsSuccess(int planId, int procedureId, int[] userIds)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AssignUsersToPlanProcedureCommandHandler(context);
            var request = new AssignUsersToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserIds = userIds.ToList()
            };

            mockPlanProcedureData(context, planId, procedureId);

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        public async void mockPlanProcedureData(RLContext context, int planId, int procedureId)
        {
            var user1 = new Data.DataModels.User()
            {
                UserId = 1,
                Name = "Nick"
            };
            var user2 = new Data.DataModels.User
            {
                UserId = 2,
                Name = "Mark"
            };
            var user3 = new Data.DataModels.User
            {
                UserId = 3,
                Name = "John"
            };

            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId
            });
            context.Procedures.Add(new Data.DataModels.Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.PlanProcedures.Add(new Data.DataModels.PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId,
                PlanProcedureUsers = new List<Data.DataModels.User>()
                {
                    user1,
                }
            });

            await context.SaveChangesAsync();
        }
    }
}
