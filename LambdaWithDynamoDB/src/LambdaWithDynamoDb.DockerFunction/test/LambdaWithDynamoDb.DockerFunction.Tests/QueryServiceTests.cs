using FakeItEasy;
using FluentAssertions;
using LambdaWithDynamoDb.DockerFunction.Model;
using LambdaWithDynamoDb.DockerFunction.Repository;
using LambdaWithDynamoDb.DockerFunction.Service;
using Xunit;

namespace LambdaWithDynamoDb.DockerFunction.Tests;

public class QueryServiceTests
{
   [Fact]
   public async Task Should_QueryUserTable_ForStandardInput()
   {
      // arrange
      string userGuid = "e6f0ca6d-ba1c-4eec-9e2f-672e8f92447f";
      string tableName = "table";
      User user = new User()
      {
         Id = userGuid, 
         Name = "Test"
      };
      IUserRepository userRepository = A.Fake<IUserRepository>();
      A.CallTo(() => userRepository.GetUserForIdAsync(userGuid, tableName)).Returns(user);
      IQueryService queryService = new QueryService(userRepository);

      // act 
      User output = await queryService.QueryUserTableAsync(userGuid, tableName);

      // assert
      output.Should().BeEquivalentTo(user);
      A.CallTo(() => userRepository.GetUserForIdAsync(userGuid, tableName)).MustHaveHappened();
   }
}