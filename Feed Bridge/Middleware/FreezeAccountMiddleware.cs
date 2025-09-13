using Feed_Bridge.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Feed_Bridge.Middleware
{
    public class FreezeAccountMiddleware
    {
        private readonly RequestDelegate _next;

        public FreezeAccountMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var user = await userManager.GetUserAsync(context.User);

                if (user != null)
                {
                    if (user.IsDeleted)
                    {
                        await signInManager.SignOutAsync();
                        context.Response.Redirect("/Account/Login?message=تم حذف هذا الحساب. برجاء التواصل مع الدعم.");
                        return;
                    }

                    if (user != null && user.IsFrozen)
                    {
                        await signInManager.SignOutAsync();
                        context.Response.Redirect("/Account/Login?message=تم تجميد حسابك من قبل الإدارة");
                        return;
                    }
                }
            }   

            await _next(context);
        }
    }
}

