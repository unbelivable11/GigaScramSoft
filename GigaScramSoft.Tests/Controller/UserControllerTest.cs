using FakeItEasy;
using GigaScramSoft.Controllers;
using GigaScramSoft.Model;
using GigaScramSoft.Services;
using GigaScramSoft.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GigaScramSoft.Tests.Controller
{
    public class UserControllerTest
    {
        private readonly IUserService _userService;
        public UserControllerTest()
        {
            _userService = A.Fake<IUserService>();
        }

        [Theory]
        [InlineData("bob", "123123", "bob@example.com")]
        public async Task UserController_SignUp_ReturnUserViewModel(string login, string password, string email)
        {
            // Arrange
            var userService = A.Fake<IUserService>();
            var controller = new UserController(userService);
            var fakeUserModel = new UserModel
            {
                Id = 1,
                Email = email,
                Login = login,
                PasswordHash = password,
                Role = new UserRoleModel { Id = 1, Name = "User"}
            };

            var fakeResponse = new ResponseModel<UserModel>(fakeUserModel, "OK", System.Net.HttpStatusCode.OK);
            A.CallTo(() => userService.CreateUser(A<UserModel>.Ignored, "User")).Returns(Task.FromResult(fakeResponse));

            // Act
            var result = await controller.SignUp(login, password, email);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseModel = Assert.IsType<ResponseModel<UserViewModel>>(objectResult.Value);

            Assert.NotNull(responseModel.Data);
            Assert.Equal(fakeUserModel.Email, responseModel.Data.Email);
            Assert.Equal(fakeUserModel.Login, responseModel.Data.Login);
            Assert.Equal("User", responseModel.Data.RoleName);
        }
    }
}