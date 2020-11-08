using System;
using JPlayer.Data.Dao.Model;

namespace JPlayer.Data.Dao
{
    internal static class ApplicationFactoryData
    {
        public static UsrFunctionDao[] Functions()
        {
            return new[]
            {
                new UsrFunctionDao
                {
                    Id = 1,
                    FunctionCode = "USR_USER_READ",
                    Name = "Ability to get users information"
                },
                new UsrFunctionDao
                {
                    Id = 2,
                    FunctionCode = "USR_USER_WRITE",
                    Name = "Ability to create or modify users"
                },
                new UsrFunctionDao
                {
                    Id = 3,
                    FunctionCode = "USR_PROFILE_READ",
                    Name = "Ability to get profiles information"
                },
                new UsrFunctionDao
                {
                    Id = 4,
                    FunctionCode = "USR_PROFILE_WRITE",
                    Name = "Ability to create or modify profiles"
                }
            };
        }

        public static UsrProfileDao[] Profiles()
        {
            return new[]
            {
                new UsrProfileDao
                {
                    Id = 1,
                    Name = "Administrator",
                    ReadOnly = true
                }
            };
        }

        public static UsrUserDao[] Users()
        {
            return new[]
            {
                new UsrUserDao
                {
                    Id = 1,
                    Login = "UserAdmin",
                    Deactivated = false,
                    CreationDate = DateTime.Now,
                    ReadOnly = true
                }
            };
        }

        public static UsrProfileFunctionDao[] ProfileFunctions()
        {
            return new[]
            {
                new UsrProfileFunctionDao
                {
                    Id = 1,
                    FunctionId = 1,
                    ProfileId = 1
                },
                new UsrProfileFunctionDao
                {
                    Id = 2,
                    FunctionId = 2,
                    ProfileId = 1
                },
                new UsrProfileFunctionDao
                {
                    Id = 3,
                    FunctionId = 3,
                    ProfileId = 1
                },
                new UsrProfileFunctionDao
                {
                    Id = 4,
                    FunctionId = 4,
                    ProfileId = 1
                }
            };
        }

        public static UsrUserProfileDao[] UserProfiles()
        {
            return new[]
            {
                new UsrUserProfileDao
                {
                    Id = 1,
                    ProfileId = 1,
                    UserId = 1
                }
            };
        }
    }
}