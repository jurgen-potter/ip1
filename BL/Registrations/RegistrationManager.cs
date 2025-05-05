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
   private const string GenderMaleCriterion = "Man";
    private const string GenderFemaleCriterion = "Vrouw";

    private Dictionary<int, string> _criteriaNameCache = new();


    public List<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<Invitation> registeredInvitations)
    {
        foreach (var bucket in buckets)
        {
            int matchCount = 0;
            foreach (var invitation in registeredInvitations)
            {
                // Use matching logic adapted for Invitation
                if (DoesInvitationMatchBucket(invitation, bucket))
                {
                    matchCount++;
                }
            }
            bucket.ActualCount = matchCount; // Assuming RecruitmentBucket has ActualCount property
        }

        return buckets; // Return the updated list
    }



    public void StartFinalDraw(Panel panel)
    {

        var recruitmentPlan = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, panel.Criteria); // Assuming Panel has Criteria

        var registeredInvitations = memberManager.GetInvitationsByPanelId(panel.Id).ToList();
        
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
            ReservePanelmembers = recruitmentPlan.ReservePotPanelmembers,
            TenantId = panel.TenantId // Assuming Panel has TenantId
        };

        // TODO: Optionally, mark selected/reserved invitations as IsDrawn = true
        // MarkInvitationsAsDrawn(drawSelectionResult.SelectedInvitations, drawSelectionResult.ReserveInvitations);

        panelManager.ChangePanel(panel); 
    }

    

     /// <summary>
    /// Creates a mapping from each bucket to the list of invitations eligible for it.
    /// </summary>
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

    /// <summary>
    /// Updates the ActualCount property of each bucket based on the invitation mapping.
    /// </summary>
     private void UpdateBucketActualCounts(
        List<RecruitmentBucket> buckets,
        Dictionary<RecruitmentBucket, List<Invitation>> invitationsByBucket)
    {
        foreach (var bucket in buckets)
        {
             // Assuming RecruitmentBucket has ActualCount
            bucket.ActualCount = invitationsByBucket.TryGetValue(bucket, out var eligibleInvitations) ? eligibleInvitations.Count : 0;
        }
    }

    
    private DrawSelectionInternalResult PerformSelection(
        RecruitmentResult recruitmentPlan,
        Dictionary<RecruitmentBucket, List<Invitation>> invitationsByBucket)
    {
        var selectedInvitations = new List<Invitation>();
        var potentialReserveInvitations = new List<Invitation>();
        // Use Invitation Code or ID for uniqueness tracking
        var selectedOrReservedInvitationIds = new HashSet<string>(); // Assuming Invitation.Code is unique identifier
        var random = new Random();

        // --- Select Main Invitations per Bucket ---
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
                // Use a unique identifier from Invitation (e.g., Code or Id)
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

        // Remove duplicates from potential reserves
        potentialReserveInvitations = potentialReserveInvitations.DistinctBy(inv => inv.Code).ToList();

        // --- Select Reserve Invitations from the Pool ---
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

        return new DrawSelectionInternalResult // Use a temporary internal result class
        {
            SelectedInvitations = selectedInvitations,
            ReserveInvitations = reserveInvitations
        };
    }


    /// <summary>
    /// Checks if an invitation matches the criteria defined in a recruitment bucket.
    /// </summary>
    private bool DoesInvitationMatchBucket(Invitation invitation, RecruitmentBucket bucket)
    {
        bool requiresMale = false;
        bool requiresFemale = false;
        string requiredAgeGroup = null;
        var otherRequiredCriteriaNames = new List<string>();

        // Parse bucket criteria requirements ONCE per check
        foreach (var criterionName in bucket.SubCriteriaNames)
        {
            if (criterionName.Equals(GenderMaleCriterion, StringComparison.OrdinalIgnoreCase))
                requiresMale = true;
            else if (criterionName.Equals(GenderFemaleCriterion, StringComparison.OrdinalIgnoreCase))
                requiresFemale = true;
            else if (IsAgeGroup(criterionName))
                requiredAgeGroup = criterionName;
            else
                otherRequiredCriteriaNames.Add(criterionName);
        }

        // --- Perform Checks against Invitation ---

        // Check Gender
        if (requiresMale && invitation.Gender != Gender.Male) return false;
        if (requiresFemale && invitation.Gender != Gender.Female) return false;

        // Check Age
        if (requiredAgeGroup != null && !IsInAgeGroup(invitation.Age, requiredAgeGroup)) return false;

        // Check Other Criteria (Names)
        if (otherRequiredCriteriaNames.Any())
        {
            // Get the names of the criteria selected in the invitation
            var invitationCriteriaNames = GetInvitationCriteriaNames(invitation.SelectedCriteria);

            // Check if *all* required 'other' criteria names are present in the invitation's criteria names
            foreach (var reqCriterionName in otherRequiredCriteriaNames)
            {
                // Use OrdinalIgnoreCase for robust comparison
                if (!invitationCriteriaNames.Contains(reqCriterionName, StringComparer.OrdinalIgnoreCase))
                {
                    return false; // Invitation is missing a required criterion
                }
            }
        }

        return true; // All checks passed
    }

     /// <summary>
    /// Helper to get criteria names from IDs using the cache.
    /// </summary>
    private IEnumerable<string> GetInvitationCriteriaNames(List<int> criteriaIds)
    {
        if (criteriaIds == null) yield break; // Return empty if null

        foreach (var id in criteriaIds)
        {
            if (_criteriaNameCache.TryGetValue(id, out var name))
            {
                yield return name;
            }
            // Else: Log warning? Criterion ID from invitation not found in cache/system.
        }
    }

    // --- Age Group Helpers (Unchanged) ---
    private static bool IsAgeGroup(string group)
    {
        group = group.Trim();
        return group.EndsWith("+") || (group.Contains('-') && group.Replace("-","").All(char.IsDigit));
    }

    private bool IsInAgeGroup(int age, string group)
    {
        group = group.Trim();

        if (group.EndsWith("+"))
        {
            if (int.TryParse(group.TrimEnd('+'), out var lowerBound))
                return age >= lowerBound;
        }
        else if (group.Contains('-'))
        {
            var parts = group.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2
                && int.TryParse(parts[0].Trim(), out var lowerBound)
                && int.TryParse(parts[1].Trim(), out var upperBound))
            {
                return age >= lowerBound && age <= upperBound;
            }
        }
        return false;
    }

     // Optional: Method to update Invitation status after draw
    // private void MarkInvitationsAsDrawn(IEnumerable<Invitation> selected, IEnumerable<Invitation> reserved)
    // {
    //     var drawnInvitations = selected.Concat(reserved);
    //     foreach (var inv in drawnInvitations)
    //     {
    //         inv.IsDrawn = true;
    //         // Consider saving changes to invitations if needed immediately
    //         // invitationManager.UpdateInvitation(inv); // Requires method in IInvitationManager
    //     }
    //     // Or maybe batch update: invitationManager.UpdateInvitations(drawnInvitations);
    // }

    // Internal helper class to return selections from PerformSelection
    private class DrawSelectionInternalResult
    {
        public List<Invitation> SelectedInvitations { get; set; }
        public List<Invitation> ReserveInvitations { get; set; }
    }
}

