namespace CitizenPanel.BL.Domain.Recruitment;

public class RecruitmentCalculator
{
    // Bevat de berekening voor het aantal te rekruteren panelleden.
    public RecruitmentResult CalculateRecruitment(RecruitmentCriteria criteria)
    {
        var TotalAvailablePotPanelmembers = 0.424 * double.Sqrt(criteria.TotalAvailablePotentialPanelmembers); //veranderen van plaats
        
        var result = new RecruitmentResult
        {
           
            MaleCount = (int)(TotalAvailablePotPanelmembers * (criteria.MalePercentage / 100)),
            FemaleCount = (int)(TotalAvailablePotPanelmembers * (criteria.FemalePercentage / 100)),
            Age18_25Count = (int)(TotalAvailablePotPanelmembers * (criteria.Age18_25Percentage / 100)),
            Age26_40Count = (int)(TotalAvailablePotPanelmembers * (criteria.Age26_40Percentage / 100)),
            Age41_60Count = (int)(TotalAvailablePotPanelmembers * (criteria.Age41_60Percentage / 100)),
            Age60PlusCount = (int)(TotalAvailablePotPanelmembers * (criteria.Age60PlusPercentage / 100)),
            ExtraCriteriaResults = new List<CriteriaResult>()
        };

        foreach (var extra in criteria.ExtraCriteria)
        {
            var criteriaResult = new CriteriaResult
            {
                Name = extra.Name,
                SubResults = extra.SubCriteria.Select(sub => new SubCriteriaResult
                {
                    Name = sub.Name,
                    Count = (int)(TotalAvailablePotPanelmembers * (sub.Percentage / 100))
                }).ToList()
            };
            result.ExtraCriteriaResults.Add(criteriaResult);
        }

        return result;
    }
}
