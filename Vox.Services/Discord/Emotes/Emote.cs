namespace Vox.Services.Discord.Emotes
{
    public class Emote
    {
        public ulong Id { get; }
        public string Name { get; }
        public string Code { get; }

        public Emote(ulong id, string name, string code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
}
