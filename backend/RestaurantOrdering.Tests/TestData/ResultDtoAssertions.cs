using System.Net;
using Application.Dtos.Common;
using FluentAssertions;

public static class ResultDtoAssertions
{
    public static void ShouldBeSuccessful<T>(
        this ResultDto<T> result,
        HttpStatusCode expectedStatusCode,
        string because = ""
    )
    {
        result.Should().NotBeNull(because);
        result.IsSuccess.Should().BeTrue(because: because);
        result.HttpStatusCode.Should().Be(expectedStatusCode, because);
        result.Data.Should().NotBeNull(because);
    }

    public static void ShouldFailWith<T>(
        this ResultDto<T> result,
        HttpStatusCode expectedStatusCode,
        string expectedErrorMessage,
        string because = ""
    )
    {
        result.Should().NotBeNull(because);
        result.IsSuccess.Should().BeFalse(because: because);
        result.HttpStatusCode.Should().Be(expectedStatusCode, because);
        result.ErrorMessage.Should().Be(expectedErrorMessage, because);
        result.Data.Should().BeNull(because);
    }
}
