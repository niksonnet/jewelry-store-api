﻿using API.Services.Services;
using AutoFixture;
using AutoMapper;
using JewelryStoreAPI.Controllers;
using JewelryStoreAPI.DTO;
using JewelryStoreAPI.Helper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace APITests
{
    public class UserControllerTest
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        //IOptions<Setting> appSettings)
        public UserControllerTest(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Fact]
        public void Authenticate_Should_Return_OK()
        {
            var httpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var fixture = new Fixture();

            mockHttpMessageHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent(fixture.Create<String>()),
                    });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            client.BaseAddress = fixture.Create<Uri>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);
            
            var controller = new UserController(_userService, _mapper, _configuration);
            var result = controller.Authenticate(new UserModel { Username = "Regular", Password = "Test@123" });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.IsAssignableFrom<OkResult>((result as OkObjectResult)?.Value);
        }
    }

}
