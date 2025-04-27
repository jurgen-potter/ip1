using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CitizenPanel.UI.MVC.Identities;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    private readonly PanelDbContext _context;

    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger,
        PanelDbContext context
    ) : base(store, optionsAccessor, passwordHasher, userValidators,
        passwordValidators, keyNormalizer, errors, services, logger)
    {
        _context = context;
    }

    public async Task<ApplicationUser> GetUserWithProfilesAsync(string userId)
    {
        var user = await _context.Users
            .Include(u => u.MemberProfile)
            .Include(u => u.OrganizationProfile)
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(u => u.Id == userId);
        return user;
    }
}