namespace TaskManagerAPI.DTOs
{
    public class TeamMemberDTO
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<TaskDTO> Tasks { get; set; }
    }
}
