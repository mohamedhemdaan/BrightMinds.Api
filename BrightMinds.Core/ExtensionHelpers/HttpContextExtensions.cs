using BrightMinds.Core.Models;
using Microsoft.AspNetCore.Http;

namespace BrightMinds.Core.ExtensionHelpers
{
    public static class HttpContextExtension
    {
        public static Shift? GetActiveShift(this HttpContext context)
        {
            return context.Items["ActiveShift"] as Shift;
        }

        public static Shift GetRequiredActiveShift(this HttpContext context)
        {
            var shft = context.Items["ActiveShift"] as Shift;

            if (shft == null)
                throw new InvalidOperationException("مفيش شيفتات مفتوحة ليك حاليا، لازم تبدأ شيفت عشان تكمل العملية، لو بتواجهك اى مشكلة كلم مدير النظام");

            return shft;
        }
    }
}
