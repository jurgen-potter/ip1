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
    IUserProfileManager userProfileManager,
    IUtilityManager utilityManager) : IRegistrationManager
{
    private readonly IUserProfileManager _userProfileManager = userProfileManager;
    
    private const string GenderMaleCriterion = "Man";
    private const string GenderFemaleCriterion = "Vrouw";
    
    private Dictionary<int, string> _criteriaNameCache = new();

    public List<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<Invitation> invitations)
    {
        // Als er invitations zijn, haal alle criteria op voor het panel van de eerste invitation
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
        // Check Gender
        bool requiresMale = bucket.SubCriteriaNames.Any(s => s.Equals(GenderMaleCriterion, StringComparison.OrdinalIgnoreCase));
        bool requiresFemale = bucket.SubCriteriaNames.Any(s => s.Equals(GenderFemaleCriterion, StringComparison.OrdinalIgnoreCase));
        
        if (requiresMale && invitation.Gender != Gender.Male) return false;
        if (requiresFemale && invitation.Gender != Gender.Female) return false;

        // Check Age
        string requiredAgeGroup = bucket.SubCriteriaNames.FirstOrDefault(IsAgeGroup);
        if (requiredAgeGroup != null && !IsInAgeGroup(invitation.Age, requiredAgeGroup)) return false;

        // Check Other Criteria
        var otherCriteria = bucket.SubCriteriaNames
            .Where(s => !s.Equals(GenderMaleCriterion, StringComparison.OrdinalIgnoreCase) &&
                        !s.Equals(GenderFemaleCriterion, StringComparison.OrdinalIgnoreCase) &&
                        !IsAgeGroup(s))
            .ToList();

        if (otherCriteria.Any())
        {
            if (invitation.SelectedCriteria == null || !invitation.SelectedCriteria.Any())
                return false; // No criteria selected, can't match

            // Get names from the ID's using our cache
            var invitationCriteriaNames = GetCriteriaNames(invitation.SelectedCriteria);
            
            // Check if all required criteria are matched
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

    // Helper om criteria namen op te halen uit de cache
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

    // Voor de StartFinalDraw methode zou je dezelfde aanpak moeten gebruiken
    public void StartFinalDraw(Panel panel)
    {
        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panel.Id);
        var recruitmentPlan = utilityManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);

        // Load criteria names to cache
        LoadCriteriaCache(panel.Id);

        // Ophalen van alle geregistreerde uitnodigingen voor dit panel
        var registeredInvitations = drawManager.GetRegisteredInvitationsByPanelId(panel.Id).ToList();
        
        // Map invitations to eligible buckets
        var invitationsByBucket = MapInvitationsToEligibleBuckets(recruitmentPlan.Buckets, registeredInvitations);

        // Update bucket actual counts
        UpdateBucketActualCounts(recruitmentPlan.Buckets, invitationsByBucket);

        // Perform the selection
        var drawSelectionResult = PerformSelection(recruitmentPlan, invitationsByBucket);

        // Update panel draw status and results
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
            ReservePanelmembers = recruitmentPlan.ReservePotPanelmembers,
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
        var potentialReserveInvitations = new List<Invitation>();
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
                    selectedInvitations.Add(invitation);
                    selectedCount++;
                }
                else if (!selectedOrReservedInvitationIds.Contains(invitation.Code))
                {
                     potentialReserveInvitations.Add(invitation);
                }
            }
        }

        potentialReserveInvitations = potentialReserveInvitations.DistinctBy(inv => inv.Code).ToList();

        var reserveInvitations = new List<Invitation>();
        var shuffledReservePool = potentialReserveInvitations
                                    .Where(inv => !selectedOrReservedInvitationIds.Contains(inv.Code))
                                    .OrderBy(_ => random.Next())
                                    .ToList();

        int reserveTarget = recruitmentPlan.ReservePotPanelmembers;
        int reserveSelectedCount = 0;

        foreach (var invitation in shuffledReservePool)
        {
            if (reserveSelectedCount < reserveTarget && selectedOrReservedInvitationIds.Add(invitation.Code))
            {
                reserveInvitations.Add(invitation);
                reserveSelectedCount++;
            }
            else if (reserveSelectedCount >= reserveTarget) break;
        }

        return new DrawSelectionInternalResult
        {
            SelectedInvitations = selectedInvitations,
            ReserveInvitations = reserveInvitations
        };
    }
    
    // Internal helper class to return selections from PerformSelection
    private class DrawSelectionInternalResult
    {
        public List<Invitation> SelectedInvitations { get; set; }
        public List<Invitation> ReserveInvitations { get; set; }
    }
}