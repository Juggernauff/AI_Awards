namespace AI_Awards.Models
{
    public class TeamMember
    {
        public string ImagePath { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }

        public TeamMember() { }

        public TeamMember(string imagePath, string fullName, string role)
        {
            ImagePath = imagePath;
            FullName = fullName;
            Role = role;
        }
    }
}
