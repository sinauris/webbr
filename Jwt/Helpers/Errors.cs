using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Webbr.Jwt.Helpers
{
    public static class Errors
    {
        public static ModelStateDictionary AddErrorToModelState(string title, string description, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(title, description);
            return modelState;
        }
    }
}