using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using Chat.Data;
using Chat.Models;

public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;

    public ChatController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int userId)
    {
        if (!HttpContext.Session.TryGetValue("UserId", out _))
        {
            return RedirectToAction("Login", "Account"); 
        }

        var username = HttpContext.Session.GetString("Username"); 

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account"); 
        }

        ViewBag.Username = username;

        var messages = _context.ChatMessages.OrderByDescending(m => m.Timestamp).ToList();
        return View(messages);
    }
    public IActionResult AdminPanel()
    {
        var users = _context.Users.ToList(); 
        return View(users);
    }

    public IActionResult ChatRoom(int? receiverId, string groupName = null)
    {
        if (!HttpContext.Session.TryGetValue("UserId", out _))
        {
            return RedirectToAction("Login", "Account");
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        var username = HttpContext.Session.GetString("Username");

        if (userId == null || string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.UserId = userId;
        ViewBag.Username = username;
        ViewBag.ReceiverId = receiverId;
        ViewBag.GroupName = groupName;

        IQueryable<ChatMessage> query = _context.ChatMessages;

        if (!string.IsNullOrEmpty(groupName))
        {
            query = query.Where(m => m.GroupName == groupName);
        }
        else if (receiverId != null)
        {
            query = query.Where(m =>
                (m.SenderId == userId && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == userId));
        }

        var messages = query.OrderBy(m => m.Timestamp).ToList();
        ViewBag.Messages = messages;

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> UploadMedia(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);


        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"/uploads/{fileName}"; 
        return Json(new { url = fileUrl });
    }

}
