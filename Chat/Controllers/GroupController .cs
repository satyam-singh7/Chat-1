using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Chat.Data;
using Chat.Models;

public class GroupController : Controller
{
    private readonly ApplicationDbContext _context;

    public GroupController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
        {
            return RedirectToAction("Login", "Account");
        }

        //ViewBag.Username = HttpContext.Session.GetString("Username");
        //return View();
        var groups = _context.Groups?.ToList() ?? new List<Group>();
        return View(groups);
    }

    [HttpPost]
    public IActionResult CreateGroup(string groupName)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var group = new Group { Name = groupName, AdminId = userId.Value };
        _context.Groups.Add(group);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult JoinGroup(int groupId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToAction("Login", "Account");

        var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (group != null && user != null && !group.Members.Contains(user))
        {
            group.Members.Add(user);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult RemoveUser(int groupId, int userId)
    {
        int? adminId = HttpContext.Session.GetInt32("UserId");
        if (adminId == null) return RedirectToAction("Login", "Account");

        var group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
        if (group != null && group.AdminId == adminId.Value)
        {
            var user = group.Members.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                group.Members.Remove(user);
                _context.SaveChanges();
            }
        }

        return RedirectToAction("Index");
    }
}
