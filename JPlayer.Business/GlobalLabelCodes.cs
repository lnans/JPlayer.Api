namespace JPlayer.Business
{
    public static class GlobalLabelCodes
    {
        public const string RequestValidationError = "request_validation_error";

        public const string AuthAuthenticationFailed = "auth_authentication_failed";
        public const string AuthWrongPassword = "auth_wrong_password";
        public const string AuthPasswordNotSame = "auth_password_not_same";
        public const string AuthNotAuthenticated = "auth_not_authenticated";
        public const string AuthNotAuthorized = "auth_not_authorized";

        public const string UserNotFound = "user_not_found";
        public const string UserAlreadyExist = "user_already_exist";
        public const string UserReadOnly = "user_read_only";
        public const string UserTileLabel = "user_tile_label";
        public const string UserTileIcon = "user";
        public const string UserTileLink = "user";

        public const string ProfileNotFound = "profile_not_nound";
        public const string ProfileAlreadyExist = "profile_already_exist";
        public const string ProfileReadOnly = "profile_read_only";
        public const string ProfileTileLabel = "profile_tile_label";
        public const string ProfileTileIcon = "detail";
        public const string ProfileTileLink = "profile";

        public const string FunctionNotFound = "function_not_found";
        public const string FunctionTileLabel = "function_tile_label";
        public const string FunctionTileIcon = "cog";
        public const string FunctionTileLink = "function";

        public const string DefaultMenu = "dashboard";
    }
}