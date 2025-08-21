using Microsoft.EntityFrameworkCore;
using NetworkAnalytics.Models.Entities;

namespace NetworkAnalytics.Services.Entities;

public class ContextDB(DbContextOptions<ContextDB> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<VKUser> VKUsers => Set<VKUser>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<CommonWord> CommonWords => Set<CommonWord>();
    public DbSet<Them> Thems => Set<Them>();
    public DbSet<Tonality> Tonalitys => Set<Tonality>();
    public DbSet<PartsSpeech> PartsSpeechs => Set<PartsSpeech>();
}
