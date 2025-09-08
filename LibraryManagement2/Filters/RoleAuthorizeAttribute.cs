using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string _role;

    public RoleAuthorizeAttribute(string role)
    {
        _role = role;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userRole = context.HttpContext.Session.GetString("UserRole");
        if (userRole == null || userRole != _role)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        base.OnActionExecuting(context);
    }

    [RoleAuthorize("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    [RoleAuthorize("Librarian")]
    public class LibrarianController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

    [RoleAuthorize("Student")]
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }

}
