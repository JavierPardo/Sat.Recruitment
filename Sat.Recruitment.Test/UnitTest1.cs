using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using Sat.Recruitment.Api.Controllers;

using Xunit;

namespace Sat.Recruitment.Test
{
    [CollectionDefinition("Tests", DisableParallelization = true)]
    public class UnitTest1
    {
        [Theory]
        [InlineData("mike@gmail.com", "+349 11223542115", "Av. Juan G2", "Mike2", "The user is duplicated")]//email 
        [InlineData("mike@gmail.comd", "+349 1122354215", "Av. Juan G3", "Mike2", "The user is duplicated")]//phone
        [InlineData("mike@gmail.com2", "+349 1122354215d", "Av. Juan G", "Mike", "The user is duplicated")]
        public void GivenUserCreation_WhenUserisDuplicated_ThenItWillGetAnErrorAndNoUserShouldBeAdded(string email, string phone, string address, string name, string expectedErrorMessage)
        {
            var originalUser = new User { Name = "Mike", Email = "mike@gmail.com", Address = "Av. Juan G", Phone = "+349 1122354215", UserType = "Normal", Money = 124 };
            var userController = new UsersForTestController(new List<User> {
                originalUser,
            });

            var result = userController.CreateUser(name, email, address, phone, "Normal", "124").Result;
            var lastUser = userController.Users.LastOrDefault();

            Assert.Equal(originalUser,lastUser);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.Errors);
        }

        [Theory]
        [InlineData(null, "+349 11223542115", "Av. Juan G", "Mike", " The email is required")]//email 
        [InlineData("mike@gmail.com", null, "Av. Juan G", "Mike", " The phone is required")]//phone
        [InlineData("mike@gmail.com", "+349 11223542115", null, "Mike", " The address is required")]//address
        [InlineData("mike@gmail.com", "+349 11223542115", "Av. Juan G", null, "The name is required")]//name
        public void GivenUserCreation_WhenUserHasInvalidData_ThenItWillGetAnError(string email, string phone, string address, string name, string expectedErrorMessage)
        {
            var userController = new UsersForTestController(new List<User> {
                new User{Name="Mike", Email="mike@gmail.com", Address="Av. Juan G", Phone="+349 1122354215", UserType="Normal", Money=124 },
            });

            var result = userController.CreateUser(name, email, address, phone, "Normal", "124").Result;


            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrorMessage, result.Errors);
        }


        [Fact]
        public void GivenUserCreation_WhenUserHasSameNameAnotherAddress_ThenWontBeADuplicatedUser()
        {
            var userToInsert = new User
            {
                Name = "Mike",
                Email = "mike2@gmail.com",
                Address = "Av. Juan G1",
                Phone = "+349 112323954215",
                UserType = "Normal",
                Money = 124
            };
            var userController = new UsersForTestController(new List<User> {
                new User{Name="Mike", Email="mike@gmail.com", Address="Av. Juan G", Phone="+349 1122354215", UserType="Normal", Money=124 },
            });

            var result = userController.CreateUser(userToInsert.Name, userToInsert.Email, userToInsert.Address, userToInsert.Phone, userToInsert.UserType, userToInsert.Money.ToString()).Result;
            var lastUsert = userController.Users.LastOrDefault();
            Assert.Equal(userToInsert.Name, lastUsert.Name);
            Assert.Equal(userToInsert.Email, lastUsert.Email);
            Assert.Equal(userToInsert.Phone, lastUsert.Phone);
            Assert.Equal(userToInsert.Address, lastUsert.Address);
            Assert.Equal(userToInsert.UserType, lastUsert.UserType);
            Assert.True(result.IsSuccess);
            Assert.Equal("User Created", result.Errors);
        }

        [Fact]
        public void GivenUserCreation_WhenUserEmailHasPlusChar_ThenUserEmailShouldBeStoredNormalize()
        {
            var userController = new UsersForTestController(new List<User> {
            });

            var result =userController.CreateUser("Mike", "mike+1@gmail.com", "Av. Juan G", "+3491122354215", "Normal", "124").Result;

            var lastUser = userController.Users.LastOrDefault();
            var expectedEmail = "mike@gmail.com";

            Assert.Equal(expectedEmail, lastUser.Email);
            Assert.True(result.IsSuccess);
        }



        [Theory]
        [InlineData("100", "Normal", 100)]
        [InlineData("101", "Normal", 113.12)]
        [InlineData("99", "Normal", 178.2)]

        [InlineData("100", "SuperUser", 100)]
        [InlineData("101", "SuperUser", 121.2)]
        [InlineData("99", "SuperUser", 99)]

        [InlineData("100", "Premium", 100)]
        [InlineData("101", "Premium", 303)]
        [InlineData("99", "Premium", 99)]
        public void GivenUserCreation_WhenUserHAsDifferentUserType_ThenMoneyWillBeChanged(string money, string userType, decimal expectedMoney)
        {
            var userController = new UsersForTestController(new List<User> {
                new User{Name="Mike", Email="mike@gmail.com", Address="Av. Juan G", Phone="+349 1122354215", UserType="Normal", Money=0 },
            });

            var result = userController.CreateUser("Mike2", "mike2@gmail.com", "Av. Juan G2", "+349 11223954215", userType, money).Result;
            var lastUser = userController.Users.LastOrDefault();

            Assert.Equal("User Created", result.Errors);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedMoney, lastUser.Money);
        }


    }
}
