namespace Evently.Modules.Users.Domain.Users;

#pragma warning disable S3453
public sealed class Role
#pragma warning restore S3453
{
    public string Name { get; private set; }
    public static readonly Role Administrator = new("Administrator");
    public static readonly Role Member = new("Member");
    
    private Role(){} // for entity framework

    private Role(string name)
    {
        Name = name;
    }
}
