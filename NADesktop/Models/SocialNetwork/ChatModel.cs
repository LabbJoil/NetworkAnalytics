
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NADesktop.Models.SocialNetwork;

public class ChatModel
{
    public long Id { get; set; }
    public string? Title { get; set; }

    public ChatModel(long id, string? title)
    {
        Id = id;
        Title = title;
    }
}
