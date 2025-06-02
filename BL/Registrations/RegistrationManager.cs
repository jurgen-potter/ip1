using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;


namespace CitizenPanel.BL.Registrations;

public class RegistrationManager(
    IPanelManager panelManager,
    IDrawManager drawManager,
    IUtilityManager utilityManager) : IRegistrationManager
{
    private const string GenderMaleCriterion = "Man";
    private const string GenderFemaleCriterion = "Vrouw";
    
    private Dictionary<int, string> _criteriaNameCache = new();

    public List<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<Invitation> invitations)
    {
        if (invitations.Any())
        {
            int panelId = invitations.First().PanelId;
            LoadCriteriaCache(panelId);
        }

        foreach (var bucket in buckets)
        {
            int matchCount = 0;
            foreach (var invitation in invitations)
            {
                if (DoesInvitationMatchBucket(invitation, bucket))
                {
                    matchCount++;
                }
            }
            bucket.ActualCount = matchCount;
        }

        return buckets;
    }

    private void LoadCriteriaCache(int panelId)
    {
        _criteriaNameCache.Clear();
        var allCriteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panelId);
        
        foreach (var criterion in allCriteria)
        {
            foreach (var subcriterion in criterion.SubCriteria)
            {
                if (!_criteriaNameCache.ContainsKey(subcriterion.Id))
                {
                    _criteriaNameCache[subcriterion.Id] = subcriterion.Name;
                }
            }
        }
    }

    private bool DoesInvitationMatchBucket(Invitation invitation, RecruitmentBucket bucket)
    {
        bool requiresMale = bucket.SubCriteriaNames.Any(s => s.Equals(GenderMaleCriterion, StringComparison.OrdinalIgnoreCase));
        bool requiresFemale = bucket.SubCriteriaNames.Any(s => s.Equals(GenderFemaleCriterion, StringComparison.OrdinalIgnoreCase));
        
        if (requiresMale && invitation.Gender != Gender.Male) return false;
        if (requiresFemale && invitation.Gender != Gender.Female) return false;

        string requiredAgeGroup = bucket.SubCriteriaNames.FirstOrDefault(IsAgeGroup);
        if (requiredAgeGroup != null && !IsInAgeGroup(invitation.Age, requiredAgeGroup)) return false;

        var otherCriteria = bucket.SubCriteriaNames
            .Where(s => !s.Equals(GenderMaleCriterion, StringComparison.OrdinalIgnoreCase) &&
                        !s.Equals(GenderFemaleCriterion, StringComparison.OrdinalIgnoreCase) &&
                        !IsAgeGroup(s))
            .ToList();

        if (otherCriteria.Any())
        {
            if (invitation.SelectedCriteria == null || !invitation.SelectedCriteria.Any())
                return false;

            var invitationCriteriaNames = GetCriteriaNames(invitation.SelectedCriteria);
            
            foreach (var requiredCriterion in otherCriteria)
            {
                bool matchFound = false;
                foreach (var criterionName in invitationCriteriaNames)
                {
                    if (criterionName.Equals(requiredCriterion, StringComparison.OrdinalIgnoreCase) ||
                        criterionName.Contains(requiredCriterion, StringComparison.OrdinalIgnoreCase) ||
                        requiredCriterion.Contains(criterionName, StringComparison.OrdinalIgnoreCase))
                    {
                        matchFound = true;
                        break;
                    }
                }
                
                if (!matchFound)
                    return false;
            }
        }

        return true;
    }

    private IEnumerable<string> GetCriteriaNames(List<int> criteriaIds)
    {
        if (criteriaIds == null) 
            return Enumerable.Empty<string>();
        
        List<string> names = new List<string>();
        foreach (var id in criteriaIds)
        {
            if (_criteriaNameCache.TryGetValue(id, out string name))
            {
                names.Add(name);
            }
        }
        
        return names;
    }

    private static bool IsAgeGroup(string group)
    {
        group = group?.Trim() ?? string.Empty;
        return group.EndsWith("+") || (group.Contains('-') && group.Replace("-", "").All(char.IsDigit));
    }

    private bool IsInAgeGroup(int age, string group)
    {
        group = group?.Trim() ?? string.Empty;

        if (group.EndsWith("+"))
        {
            if (int.TryParse(group.TrimEnd('+'), out var lower))
                return age >= lower;
        }
        else if (group.Contains('-'))
        {
            var parts = group.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2
                && int.TryParse(parts[0].Trim(), out var lo)
                && int.TryParse(parts[1].Trim(), out var hi))
            {
                return age >= lo && age <= hi;
            }
        }

        return false;
    }

    public void StartFinalDraw(Panel panel)
    {
        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panel.Id);
        var recruitmentPlan = utilityManager.CalculateRecruitment(panel.TotalNeededPanelmembers, criteria);

        LoadCriteriaCache(panel.Id);

        var registeredInvitations = drawManager.GetRegisteredInvitationsByPanelId(panel.Id).ToList();
        
        var invitationsByBucket = MapInvitationsToEligibleBuckets(recruitmentPlan.Buckets, registeredInvitations);

        UpdateBucketActualCounts(recruitmentPlan.Buckets, invitationsByBucket);

        var drawSelectionResult = PerformSelection(recruitmentPlan, invitationsByBucket);

        panel.DrawStatus = DrawStatus.Complete;
        panel.DrawResult = new DrawResult
        {
            SelectedInvitations = drawSelectionResult.SelectedInvitations,
            ReserveInvitations = drawSelectionResult.ReserveInvitations,
            NotSelectedInvitations = registeredInvitations
                                        .Except(drawSelectionResult.SelectedInvitations)
                                        .Except(drawSelectionResult.ReserveInvitations)
                                        .ToList(),
            TotalNeededPanelmembers = recruitmentPlan.TotalNeededPanelmembers,
            ReservePanelmembers = recruitmentPlan.TotalNeededInvitations,
            TenantId = panel.TenantId
        };
        panelManager.EditPanel(panel);
    }
    
    private Dictionary<RecruitmentBucket, List<Invitation>> MapInvitationsToEligibleBuckets(
        List<RecruitmentBucket> buckets, List<Invitation> invitations)
    {
        var invitationsByBucket = buckets.ToDictionary(b => b, b => new List<Invitation>());

        foreach (var invitation in invitations)
        {
            foreach (var bucket in buckets)
            {
                if (DoesInvitationMatchBucket(invitation, bucket))
                {
                    invitationsByBucket[bucket].Add(invitation);
                }
            }
        }
        return invitationsByBucket;
    }
    
    private void UpdateBucketActualCounts(
        List<RecruitmentBucket> buckets,
        Dictionary<RecruitmentBucket, List<Invitation>> invitationsByBucket)
    {
        foreach (var bucket in buckets)
        {
            bucket.ActualCount = invitationsByBucket.TryGetValue(bucket, out var eligibleInvitations) ? eligibleInvitations.Count : 0;
        }
    }
    
    private DrawSelectionInternalResult PerformSelection(
        RecruitmentResult recruitmentPlan,
        Dictionary<RecruitmentBucket, List<Invitation>> invitationsByBucket)
    {
        var selectedInvitations = new List<Invitation>();
        var reserveInvitations = new List<Invitation>();
        var selectedOrReservedInvitationIds = new HashSet<string>();
        var random = new Random();

        foreach (var bucket in recruitmentPlan.Buckets)
        {
            int targetCount = bucket.Count;
            if (!invitationsByBucket.TryGetValue(bucket, out var eligibleInvitations) || !eligibleInvitations.Any())
            {
                continue;
            }

            var shuffledEligibleInvitations = eligibleInvitations.OrderBy(_ => random.Next()).ToList();

            int selectedCount = 0;
            foreach (var invitation in shuffledEligibleInvitations)
            {
                if (selectedCount < targetCount && selectedOrReservedInvitationIds.Add(invitation.Code))
                {
                    invitation.IsDrawn = true;
                    selectedInvitations.Add(invitation);
                    selectedCount++;
                }
                else
                {
                    reserveInvitations.Add(invitation);
                    break;
                }
            }
        }

        return new DrawSelectionInternalResult
        {
            SelectedInvitations = selectedInvitations,
            ReserveInvitations = reserveInvitations
        };
    }
    
    private class DrawSelectionInternalResult
    {
        public List<Invitation> SelectedInvitations { get; set; }
        public List<Invitation> ReserveInvitations { get; set; }
    }
}