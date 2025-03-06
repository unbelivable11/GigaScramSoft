using FakeItEasy;
using GigaScramSoft.Controllers;
using GigaScramSoft.Model;
using GigaScramSoft.Services;
using GigaScramSoft.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace GigaScramSoft.Tests.Controller
{
    public class UserControllerTest
    {
        private readonly IUserService _userService;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _userService = A.Fake<IUserService>();
            _controller = new UserController(_userService);
        }

        [Theory]
        [InlineData("bob", "123123", "bob@example.com")]
        public async Task SignUp_ReturnsUserViewModel(string login, string password, string email)
        {
            // Arrange
            var fakeUser = new UserModel { Id = 1, Email = email, Login = login, PasswordHash = password, Role = new UserRoleModel { Id = 1, Name = "User" } };
            var fakeResponse = new ResponseModel<UserModel>(fakeUser, "OK", HttpStatusCode.OK);
            A.CallTo(() => _userService.CreateUser(A<UserModel>.Ignored, "User")).Returns(Task.FromResult(fakeResponse));

            // Act
            var result = await _controller.SignUp(login, password, email);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseModel = Assert.IsType<ResponseModel<UserViewModel>>(objectResult.Value);
            Assert.NotNull(responseModel.Data);
            Assert.Equal(fakeUser.Email, responseModel.Data.Email);
            Assert.Equal(fakeUser.Login, responseModel.Data.Login);
            Assert.Equal("User", responseModel.Data.RoleName);
        }

        [Theory]
        [InlineData("bob", "123123")]
        public async Task Login_ReturnsToken(string login, string password)
        {
            // Arrange
            var fakeResponse = new ResponseModel<string>("fake-jwt-token", "OK", HttpStatusCode.OK);
            A.CallTo(() => _userService.Login(login, password)).Returns(Task.FromResult(fakeResponse));

            // Act
            var result = await _controller.Login(login, password);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseModel = Assert.IsType<ResponseModel<string>>(objectResult.Value);
            Assert.NotNull(responseModel.Data);
            Assert.Equal("fake-jwt-token", responseModel.Data);
        }

        [Fact]
        public async Task GetProfile_ReturnsUserProfile()
        {
            // Arrange
            var fakeUser = new UserModel { Id = 1, Email = "bob@example.com", Login = "bob", Role = new UserRoleModel { Id = 1, Name = "User" } };
            var fakeResponse = new ResponseModel<UserModel>(fakeUser, "OK", HttpStatusCode.OK);
            A.CallTo(() => _userService.GetUserByUsername("bob")).Returns(Task.FromResult(fakeResponse));

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("Login", "bob") }));
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = claims };

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var userViewModel = Assert.IsType<UserViewModel>(objectResult.Value);
            Assert.Equal(fakeUser.Email, userViewModel.Email);
            Assert.Equal(fakeUser.Login, userViewModel.Login);
            Assert.Equal("User", userViewModel.RoleName);
        }

        [Theory]
        [InlineData("oldPassword123", "newPassword456")]
        public async Task UpdatePassword_ReturnsSuccess(string oldPassword, string newPassword)
        {
            // Arrange
            var fakeResponse = new ResponseModel<bool>(true, "OK", HttpStatusCode.OK);
            A.CallTo(() => _userService.UpdatePassword("bob", oldPassword, newPassword)).Returns(Task.FromResult(fakeResponse));

            var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("Login", "bob") }));
            _controller.ControllerContext.HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = claims };

            // Act
            var result = await _controller.UpdatePassword(oldPassword, newPassword);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseModel = Assert.IsType<ResponseModel<bool>>(objectResult.Value);
            Assert.True(responseModel.Data);
        }
    }
}