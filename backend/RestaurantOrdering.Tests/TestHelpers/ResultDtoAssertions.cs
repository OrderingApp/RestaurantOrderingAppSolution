using Application.Dtos.Common;
using FluentAssertions;
using System.Net;

public static class ResultDtoAssertions
{
    public static void ShouldBeSuccessful<T>(this ResultDto<T> result, HttpStatusCode expectedStatusCode)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue("expected result to be successful but it was not");
        result.HttpStatusCode.Should().Be(expectedStatusCode);
        result.Data.Should().NotBeNull("expected result data to be present but it was null");
    }
}
