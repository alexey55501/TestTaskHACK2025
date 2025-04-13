namespace Lapka.SharedModels.Routes
{
    public static class APIRoutes
    {
        public const string API_ROUTE = "/api";

        public static class V1
        {
            public const string V1Base = API_ROUTE + "/v1/";

            public static class Auth
            {
                public const string Base = V1Base + "auth/";

                public const string Login = Base + "login";
                public const string Register = Base + "register";
                public const string RegisterModerator = Base + "registerModerator";
                public const string RestorePassword = Base + "restorePassword";
                public const string ResetPassword = Base + "resetPassword";
            }

            public static class Admin
            {
                public const string Base = V1Base + "admin/";

                public const string GetUsers = Base + "users";
                public const string GetStrangeActivityUsers = Base + "strangeActivity";
                public const string GetUserInfo = Base + "users/{id}";
                public const string UpdateUser = Base + "users/{id}/update";
                public const string DeleteUser = Base + "users/{id}/delete";
                public const string BanUser = Base + "users/{id}/ban";
                public const string UnbanUser = Base + "users/{id}/unban";
                public const string ModerationHistory = Base + "moderation/history";

            }
            public static class User
            {
                public const string Base = V1Base + "user/";

                public const string GetId = Base + "id";
                public const string GetInfo = Base + "info";
                public const string GetSettings = Base + "settings";
                public const string Update = Base + "update";
                public const string UpdatePassword = Base + "update/password";
            }

            public static class Animals
            {
                public const string Base = V1Base + "animals/";

                // CRUD
                public const string Create = Base + "create";
                public const string GetPaginated = Base + "";
                public const string GetAll = Base + "all";
                public const string Get = Base + "{id}";
                public const string Update = Base + "{id}/update";
                public const string Delete = Base + "{id}/delete";

                public const string AddToFavorites = Base + "{id}/favorite";
                public const string RemoveFromFavorites = Base + "{id}/unfavorite";
            }

            public static class Shelters
            {
                public const string Base = V1Base + "shelters/";
                

                public const string CreateShelterRequest = Base + "request";
                public const string GetShelterInfo = Base + "{id}/info";
            }
        }
    }
}

