using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;

namespace CitizenPanel.BL.Registrations;

public class RegistrationManager(
    IPanelManager panelManager,
    IDrawManager drawManager,
    IMemberManager memberManager) : IRegistrationManager
{
    public IEnumerable<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<ApplicationUser> users)
    {
        foreach (var bucket in buckets)
        {
            int matchCount = 0;
            foreach (var user in users)
            {
                var mp = user.MemberProfile;
                if (mp == null) continue;

                bool matches = MatchesBucket(mp, bucket);
                if (matches)
                    matchCount++;
            }
            bucket.ActualCount = matchCount;
        }

        return buckets;
    }

    private bool MatchesBucket(MemberProfile mp, RecruitmentBucket bucket)
    {
        // Check Gender
        bool hasGenderCriteria = false;
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (sub.Equals("Man", StringComparison.OrdinalIgnoreCase))
            {
                hasGenderCriteria = true;
                if (mp.Gender != Gender.Male)
                    return false;
            }
            else if (sub.Equals("Vrouw", StringComparison.OrdinalIgnoreCase))
            {
                hasGenderCriteria = true;
                if (mp.Gender != Gender.Female)
                    return false;
            }
        }

        // Check Age
        bool hasAgeCriteria = false;
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (IsAgeGroup(sub))
            {
                hasAgeCriteria = true;
                if (!IsInAgeGroup(mp.Age, sub))
                    return false;
            }
        }

        // Check other criteria
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (hasGenderCriteria && (sub.Equals("Man", StringComparison.OrdinalIgnoreCase) ||
                                       sub.Equals("Vrouw", StringComparison.OrdinalIgnoreCase)))
                continue;

            if (hasAgeCriteria && IsAgeGroup(sub))
                continue;

            if (!mp.SelectedCriteria.Any(sc =>
                    sc.Name.Equals(sub, StringComparison.OrdinalIgnoreCase) ||
                    sub.Contains(sc.Name, StringComparison.OrdinalIgnoreCase) ||
                    sc.Name.Contains(sub, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAgeGroup(string group)
    {
        group = group.Trim();
        return group.EndsWith("+") || group.Contains("-");
    }

    private bool IsInAgeGroup(int age, string group)
    {
        group = group.Trim();

        if (group.EndsWith("+"))
        {
            if (int.TryParse(group.TrimEnd('+'), out var lower))
                return age >= lower;
        }
        else if (group.Contains('-'))
        {
            var parts = group.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2
                && int.TryParse(parts[0], out var lo)
                && int.TryParse(parts[1], out var hi))
            {
                return age >= lo && age <= hi;
            }
        }

        return false;
    }
    

    public void StartFinalDraw(Panel panel)
    {
        var criteria = panelManager.GetCriteriaAndSubcriteriaWithPanelId(panel.Id);
        var allUsers = memberManager.GetMembersOfPanelWithCriteria(panel.Id).ToList();
        
        var recruitmentResult = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);
        
        var bucketsWithTargetsAndActuals = AssignActualRegistrationsToBuckets(recruitmentResult.Buckets, allUsers);

        var drawResult = new DrawResult
        {
            SelectedMembers = new List<ApplicationUser>(),
            ReserveMembers = new List<ApplicationUser>(),
            NotSelectedMembers = new List<ApplicationUser>(),
            TotalNeededPanelmembers = recruitmentResult.TotalNeededPanelmembers,
            ReservePanelmembers = recruitmentResult.ReservePotPanelmembers
        };

        var selectedAndReservedUserIds = new HashSet<string>();
        var eligibleForReservePoolCandidates = new List<ApplicationUser>(); 

        var random = new Random();

        foreach (var bucket in bucketsWithTargetsAndActuals)
        {
            int targetCount = bucket.Count;

            var eligibleUsersInBucket = allUsers
                .Where(u => u.MemberProfile != null &&
                            !selectedAndReservedUserIds.Contains(u.Id) &&
                            MatchesBucket(u.MemberProfile, bucket))
                .ToList();

            int actualSelectedCount = Math.Min(targetCount, eligibleUsersInBucket.Count);

            var shuffledUsers = eligibleUsersInBucket.OrderBy(_ => random.Next()).ToList(); 

            for (int i = 0; i < actualSelectedCount; i++)
            {
                var userToSelect = shuffledUsers[i];
                drawResult.SelectedMembers.Add(userToSelect);
                selectedAndReservedUserIds.Add(userToSelect.Id); 
            }

            for (int i = actualSelectedCount; i < shuffledUsers.Count; i++)
            {
                 var userForReserveCandidate = shuffledUsers[i];
                 if (!selectedAndReservedUserIds.Contains(userForReserveCandidate.Id) && eligibleForReservePoolCandidates.All(u => u.Id != userForReserveCandidate.Id))
                 {
                     eligibleForReservePoolCandidates.Add(userForReserveCandidate);
                 }
            }
        }
        
        var shuffledReservePool = eligibleForReservePoolCandidates.OrderBy(_ => random.Next()).ToList(); 

        int actualReserveCount = Math.Min(recruitmentResult.ReservePotPanelmembers, shuffledReservePool.Count);

        for (int i = 0; i < actualReserveCount; i++)
        {
             var userToReserve = shuffledReservePool[i];
             if (!selectedAndReservedUserIds.Contains(userToReserve.Id))
             {
                drawResult.ReserveMembers.Add(userToReserve);
                selectedAndReservedUserIds.Add(userToReserve.Id); 
             }
        }

        drawResult.NotSelectedMembers = allUsers
            .Where(u => !selectedAndReservedUserIds.Contains(u.Id)) 
            .ToList();

        panel.DrawStatus = DrawStatus.Complete;
        panel.DrawResult = drawResult;

        panelManager.ChangePanel(panel);
    }
}