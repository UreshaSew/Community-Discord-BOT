using System.Collections.Generic;

namespace CommunityBot.Features.RoleAssignment
{
    public class RoleByPhraseSettings
    {
        public List<ulong> RoleIds { get; set; } = new List<ulong>();
        public List<string> Phrases { get; set; } = new List<string>();
        public List<RoleByPhraseRelation> Relations { get; set; } = new List<RoleByPhraseRelation>();
    }
}
