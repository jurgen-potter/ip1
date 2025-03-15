using CitizenPanel.BL.Domain.Recruitment;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public class RecruitmentManager
{
    public RecruitmentResult CalculateRecruitment(RecruitmentCriteria criteria)
    {
        var reservePerc = 0.08;
        var totalAvailablePotPanelmembers = 0.424 * Math.Sqrt(criteria.TotalAvailablePotentialPanelmembers);

        var result = new RecruitmentResult
        {
            MaleCount = (int)(totalAvailablePotPanelmembers * (criteria.MalePercentage / 100)),
            FemaleCount = (int)(totalAvailablePotPanelmembers * (criteria.FemalePercentage / 100)),
            Age18_25Count = (int)(totalAvailablePotPanelmembers * (criteria.Age18_25Percentage / 100)),
            Age26_40Count = (int)(totalAvailablePotPanelmembers * (criteria.Age26_40Percentage / 100)),
            Age41_60Count = (int)(totalAvailablePotPanelmembers * (criteria.Age41_60Percentage / 100)),
            Age60PlusCount = (int)(totalAvailablePotPanelmembers * (criteria.Age60PlusPercentage / 100)),
            ReservePotPanelmembers = (int)(totalAvailablePotPanelmembers / reservePerc),
            TotalNeededPanelmembers = (int)(totalAvailablePotPanelmembers),
            ExtraCriteriaResults = new System.Collections.Generic.List<CriteriaResult>(),
        };

        // Extra criteria (wordt overgenomen zoals eerder)
        foreach (var extra in criteria.ExtraCriteria)
        {
            var criteriaResult = new CriteriaResult
            {
                Name = extra.Name,
                SubResults = extra.SubCriteria.Select(sub => new SubCriteriaResult
                {
                    Name = sub.Name,
                    Count = (int)(totalAvailablePotPanelmembers * (sub.Percentage / 100))
                }).ToList()
            };
            result.ExtraCriteriaResults.Add(criteriaResult);
        }

        return result;
    }
}