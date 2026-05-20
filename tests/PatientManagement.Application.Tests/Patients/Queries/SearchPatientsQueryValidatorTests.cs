using FluentAssertions;
using PatientManagement.Application.Patients.Queries.SearchPatients;
using Xunit;

namespace PatientManagement.Application.Tests.Patients.Queries;

public class SearchPatientsQueryValidatorTests
{
    private readonly SearchPatientsQueryValidator _validator;

    public SearchPatientsQueryValidatorTests()
    {
        _validator = new SearchPatientsQueryValidator();
    }

    [Fact]
    public void Validator_ShouldPass_WhenValidParametersProvided()
    {
        // Arrange
        var query = new SearchPatientsQuery
        {
            SearchTerm = "John",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validator_ShouldFail_WhenSearchTermIsTooShort()
    {
        // Arrange
        var query = new SearchPatientsQuery
        {
            SearchTerm = "A",
            PageNumber = 1,
            PageSize = 20
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SearchTerm");
    }

    [Fact]
    public void Validator_ShouldFail_WhenPageNumberIsZeroOrNegative()
    {
        // Arrange
        var query = new SearchPatientsQuery
        {
            SearchTerm = "John",
            PageNumber = 0,
            PageSize = 20
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void Validator_ShouldFail_WhenPageSizeExceedsMaximum()
    {
        // Arrange
        var query = new SearchPatientsQuery
        {
            SearchTerm = "John",
            PageNumber = 1,
            PageSize = 101
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }
}
